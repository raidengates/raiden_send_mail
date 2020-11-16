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
  
        
        IEnumerable<MailItem> IEmailHandle.readPst(string pstFilePath, string pstName)
        {
            List<MailItem> mailItems = new List<MailItem>();
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

            foreach (Folder folder in subFolders)
            {
                ExtractItems(mailItems, folder);
            }
            // Remove PST file from Default Profile
            outlookNs.RemoveStore(rootFolder);
            return mailItems;
        }


        void ExtractItems(List<MailItem> mailItems, Folder folder)
        {
            Items items = folder.Items;

            int itemcount = items.Count;

            foreach (object item in items)
            {
                if (item is MailItem)
                {
                    MailItem mailItem = item as MailItem;
                    mailItems.Add(mailItem);
                }
            }

            foreach (Folder subfolder in folder.Folders)
            {
                ExtractItems(mailItems, subfolder);
            }
        }

        public bool sendMail(MailItem mailItem, ref System.Exception o_ex )
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(App.SmtpClient);
                mail.From = new MailAddress(App.currentEmail);
                mail.To.Add(App.tagetemail);
                mail.Subject = mailItem.TaskSubject.Replace('\r', ' ').Replace('\n', ' ');
                /*
                    olFormatUnspecified = 0,
                    olFormatPlain = 1,
                    olFormatHTML = 2,
                    olFormatRichText = 3
                 */
                mail.Body = mailItem.BodyFormat == OlBodyFormat.olFormatHTML ? mailItem.HTMLBody : mailItem.Body;
                mail.IsBodyHtml = true ;
                SmtpServer.Port = App.SmtpPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(App.currentEmail, App.password);
                SmtpServer.EnableSsl = App.EnableSsl;
                SmtpServer.Send(mail);
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
