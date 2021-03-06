﻿using Microsoft.Office.Interop.Outlook;
using raiden_mail_reader.Common;
using raiden_mail_reader.Entity;
using raiden_mail_reader.Handle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// mail -f=net.pst -ce=graiden.gr@gmail.com -te=vuvh@qasol.net -pw=xxx
/// </summary>
namespace raiden_mail_reader
{
    public class Program
    {
        public static string prefix { get; set; }

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
            //var isConnect = ProgramHandle.Instance.ValidateCredentials(App.currentEmail, App.password, App.SmtpClient, App.SmtpPort, App.EnableSsl);

            //if (!isConnect)
            //{
            //    "Méo đăng nhập đực!".WriteMessage();
            //    return;
            //}
            ProgramHandle.Instance.GetSmtpClient();
            "Đăng nhập thành công!".WriteMessage();

            Console.Clear();
            "Bắt đầu giải nén file!".WriteMessage();
            EmailHandle.Instance.readPst(Path.Combine(App.path, "net.pst"), "net.pst");

            Console.Clear();
            var max = QueueMailMessage.queueMailMessage.Count();

            using (var progress = new ProgressBar())
            {
                do {
                    progress.Report((double)QueueMailMessage.queueMailMessage.Count() / max);
                    System.Exception o_ex = new System.Exception();
                    var isSend = EmailHandle.Instance.sendMail(ref o_ex);
                    if (!isSend)
                    {
                        exceptions.Add(o_ex);
                    }
                } while (QueueMailMessage.queueMailMessage.Count > 0);
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
