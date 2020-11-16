using Microsoft.Office.Interop.Outlook;
using raiden_mail_reader.Common;
using raiden_mail_reader.Entity;
using raiden_mail_reader.Handle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


/// <summary>
/// mail -f=net.pst -ce=graiden.gr@gmail.com -te=vuvh@qasol.net -pw=xxx
/// </summary>
namespace raiden_mail_reader
{
    public class Program
    {
        public static string prefix { get; set; }
        static List<MailItem> MailItem = new List<MailItem>();

        static List<System.Exception> exceptions = new List<System.Exception>();
        static void Main(string[] args)
        {
            ArgsProviders.args = args;
            Console.OutputEncoding = Encoding.UTF8;
            ProgramHandle.Instance.setupFile();
            ProgramHandle.Instance.setupCurrentEmail();
            ProgramHandle.Instance.setupTagetEmail();
            ProgramHandle.Instance.setupPassword();
            ProgramHandle.Instance.setupSmtpConfig();

            Console.Clear();
            "Bắt đầu giải nén file!".WriteMessage();

            using (var progress = new ProgressBar())
            {
                progress.Report(1);
                MailItem = EmailHandle.Instance.readPst(Path.Combine(App.path, "net.pst"), "net.pst").ToList();
            }
            using (var progress = new ProgressBar())
            {
                foreach (var item in MailItem.Select((value, i) => new { i, value }))
                {
                    var ValueMail = item.value;
                    var index = item.i;
                    progress.Report((double)index / MailItem.Count());
                    System.Exception o_ex = new System.Exception();
                    var isSend = EmailHandle.Instance.sendMail(ValueMail, ref o_ex);
                    if (!isSend)
                    {
                        exceptions.Add(o_ex);
                    }
                }
            }
            Console.Clear();
            if (exceptions.Count > 0)
            {
                string.Format("Méo gửi đc {0} thư T_T", exceptions.Count).WriteMessage();

                foreach (var item in exceptions)
                {
                    item.Message.WriteMessage();
                }
            }
            else
            {
                string.Format("Ngon!!!!!!!!!!!!!!!!", exceptions.Count).WriteMessage();
            }

            Console.ReadKey();
        }
    }
}
