using Microsoft.Office.Interop.Outlook;
using raiden_mail_reader.Common;
using raiden_mail_reader.Entity;
using System;
using System.IO;
using System.Net.Mail;
namespace raiden_mail_reader.Handle
{
    class EmailHandle : ServiceLocator<IEmailHandle, EmailHandle>, IEmailHandle
    {

        protected override Func<IEmailHandle> GetFactory()
        {
            return () => new EmailHandle();
        }


        void IEmailHandle.readPst(string pstFilePath, string pstName)
        {
            Application app = new Application();
            NameSpace outlookNs = app.GetNamespace("MAPI");

            // Add PST file (Outlook Data File) to Default Profile
            outlookNs.AddStore(pstFilePath);

            string storeInfo = null;
            MAPIFolder rootFolder = null;
            foreach (Store store in outlookNs.Stores)
            {
                if (string.IsNullOrEmpty(storeInfo))
                {
                    var file = Path.GetFileName(store.FilePath);
                    if (file == pstName)
                    {
                        storeInfo = store.DisplayName;
                        storeInfo = store.FilePath;
                        storeInfo = store.StoreID;
                        rootFolder = store.GetRootFolder();
                    }
                }
            }

            //MAPIFolder rootFolder = outlookNs.Stores[pstName].GetRootFolder();

            // Traverse through all folders in the PST file
            Folders subFolders = rootFolder.Folders;
            using (var progress = new ProgressBar())
            {
                int count = 0;
                foreach (Folder folder in subFolders)
                {
                    progress.Report((double)count++ / subFolders.Count);
                    ExtractItems(folder);
                }
            }

            // Remove PST file from Default Profile
            outlookNs.RemoveStore(rootFolder);
        }


        void ExtractItems(Folder folder)
        {
            Items items = folder.Items;

            int itemcount = items.Count;
            foreach (object item in items)
            {
                if (item is MailItem)
                {
                    MailItem mailItem = item as MailItem;
                    PushQueueMailMessage(mailItem);
                }
            }
            foreach (Folder subfolder in folder.Folders)
            {
                ExtractItems(subfolder);
            }
        }


        void PushQueueMailMessage(MailItem mailItem)
        {
            QueueMailMessage.queueMailMessage.Enqueue(mailItem);
        }

        bool IEmailHandle.sendMail(ref System.Exception o_ex)
        {
            try
            {
                var mailItem = QueueMailMessage.queueMailMessage.Dequeue();

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(App.currentEmail);
                    mail.To.Add(App.tagetemail);
                    if (string.IsNullOrEmpty(mailItem.Subject))
                    {
                        mailItem.Subject = "";
                    }
                    else
                    {
                        mail.Subject = mailItem.Subject.Replace('\r', ' ').Replace('\n', ' ');
                    }
                    mail.Body = mailItem.BodyFormat == OlBodyFormat.olFormatHTML ? mailItem.HTMLBody : mailItem.Body;



                    for (int i = 1; i < mailItem.Attachments.Count; i++)
                    {
                        var att = mailItem.Attachments;
                        var FileAtachment = @"D:\TestFileSave\" + mailItem.Attachments[i].FileName;
                        mailItem.Attachments[i].SaveAsFile(FileAtachment);

                        if (File.Exists(FileAtachment))
                        {
                             mailItem.Attachments.Add(FileAtachment, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
                        }
                    }

                    mail.IsBodyHtml = true;

                    try
                    {
                        using (SmtpClient SmtpServer = new SmtpClient(App.SmtpClient))
                        {
                            SmtpServer.Port = App.SmtpPort;
                            SmtpServer.Credentials = new System.Net.NetworkCredential(App.currentEmail, App.password);
                            SmtpServer.EnableSsl = false;
                            SmtpServer.Send(mail);
                        }
                    }
                    catch (System.Exception e)
                    {
                        o_ex = e;
                        return false;
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                o_ex = ex;
                return false;
            }
        }
    }
}
