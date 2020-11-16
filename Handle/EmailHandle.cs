using Microsoft.Office.Interop.Outlook;
using raiden_mail_reader.Common;
using raiden_mail_reader.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
#pragma warning disable CS0105 // The using directive for 'System' appeared previously in this namespace
using System;
#pragma warning restore CS0105 // The using directive for 'System' appeared previously in this namespace
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
            MailMessage mail = new MailMessage();
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
            mail.IsBodyHtml = true;
            QueueMailMessage.queueMailMessage.Enqueue(mail);
        }

        bool IEmailHandle.sendMail(ref System.Exception o_ex )
        {
            try
            {
                var mailMessage = QueueMailMessage.queueMailMessage.Dequeue();
                //App.smtpClient.Send(mailMessage);
                return true;
            }
            catch(System.Exception ex)
            {
                o_ex = ex;
                return false;
            }
        }
    }
}
