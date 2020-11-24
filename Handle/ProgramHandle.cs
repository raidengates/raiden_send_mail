using raiden_mail_reader.Common;

using raiden_mail_reader.Entity;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace raiden_mail_reader.Handle
{
    class ProgramHandle : ServiceLocator<IProgramHandle, ProgramHandle>, IProgramHandle
    {
        void IProgramHandle.setupCurrentEmail()
        {
            string cEmail = ArgsProviders.args.Where(x => x.ToLower().Contains("-ce")).FirstOrDefault();
            if (!string.IsNullOrEmpty(cEmail))
            {
                cEmail = cEmail.argsParams();
            }
            else
            {
                cEmail = "Vui lòng nhập email của bạn".GetReadLine();
            }
            App.currentEmail = cEmail;
        }

        void IProgramHandle.setupFile()
        {
            string FileName = ArgsProviders.args.Where(x => x.ToLower().Contains("-f")).FirstOrDefault();
            if (!string.IsNullOrEmpty(FileName))
            {
                FileName = FileName.argsParams();
            }
            else
            {
                var totalFile = Directory.GetFiles(App.path, "*.pst", SearchOption.AllDirectories);

                if(totalFile.Length == 0)
                {
                    "Tại đây méo có file <*.pst> bạn nhé!".WriteMessage();
                    ArgsProviders.WaitingReadKey();
                    Environment.Exit(0);
                }
                else if(totalFile.Length > 1)
                {
                    string.Format("Tìm được {0} tệp tin. Không biết bạn thích tệp tin nào?", totalFile.Length).WriteMessage();
                    foreach (var item in totalFile)
                    {
                        string name =  Path.GetFileName(item);
                        string.Format("(★ ω ★) - {0}", name).WriteMessage();
                    }
                    "Thích tệp tin nào thì gõ đúng tên tệp đó nhé!".GetReadLine();
                    App.FileName = FileName;
                }
                else if(totalFile.Length ==1)
                {
                    string name = Path.GetFileName(totalFile[1]);
                    string.Format("Tìm được tệp tin 💌 tên {0}.", name).WriteMessage();
                    App.FileName = name;
                }
                
            }
            
        }

        void IProgramHandle.setupFuny()
        {
            throw new NotImplementedException();
        }

        void IProgramHandle.setupPassword()
        {
            string pw = ArgsProviders.args.Where(x => x.ToLower().Contains("-pw")).FirstOrDefault();
            if (!string.IsNullOrEmpty(pw))
            {
                pw = pw.argsParams();
            }
            else
            {
                pw = "Vui lòng nhập password email".ReadPassword();
            }
            App.password = pw;
        }

        void IProgramHandle.setupTagetEmail()
        {
            string cEmail = ArgsProviders.args.Where(x => x.ToLower().Contains("-te")).FirstOrDefault();
            if (!string.IsNullOrEmpty(cEmail))
            {
                cEmail = cEmail.argsParams();
            }
            else
            {
                cEmail = "Vui lòng nhập email bạn cần gửi".GetReadLine();
            }
            App.tagetemail = cEmail;
        }


        void IProgramHandle.setupSmtpConfig()
        {
            "Lựa chọn Smtp nào!".WriteMessage();
            "1 - Google".WriteMessage();
            "2 - Tự định nghĩa".WriteMessage();
            bool isChouse = true;
            int number_input = -1;
            do
            {
               var input =  "....".GetReadLine();
                if(int.TryParse(input, out  int o_value) && (o_value == 1 || o_value == 2))
                {
                    number_input = o_value;
                    isChouse = false;
                }
                else
                {
                    "Méo có lựa chọn dị vậy nhé.".WriteMessage();
                }

            } while (isChouse);

            if(number_input == 1)
            {
                App.SmtpClient = "smtp.gmail.com";
                //port
                bool isSmtpPort = true;
                do
                {
                    var input = "SmtpPort <587 || 993 || 465>".GetReadLine();
                    if (int.TryParse(input, out int o_value))
                    {
                        App.SmtpPort = o_value;
                        isSmtpPort = false;
                    }
                    else
                    {
                        "Méo có lựa chọn dị vậy nhé.".WriteMessage();
                    }
                } while (isSmtpPort);

                

                App.EnableSsl = true;
            }else if (number_input == 2)
            {
                "Định nghĩa smtp nào!".WriteMessage();
                App.SmtpClient = "SmtpClient".GetReadLine();
                //port
                bool isSmtpPort = true;
                do
                {
                    var input = "SmtpPort".GetReadLine();
                    if (int.TryParse(input, out int o_value))
                    {
                        App.SmtpPort = o_value;
                        isSmtpPort = false;
                    }
                    else
                    {
                        "Méo có lựa chọn dị vậy nhé.".WriteMessage();
                    }
                } while (isSmtpPort);

                App.EnableSsl = true;
            }
           
        }

        bool IProgramHandle.ValidateCredentials(string login, string password, string server, int port, bool enableSsl)
        {
            SmtpConnectorBase connector;
            if (enableSsl)
            {
                connector = new SmtpConnectorWithSsl(server, port);
            }
            else
            {
                connector = new SmtpConnectorWithoutSsl(server, port);
            }

            if (!connector.CheckResponse(220))
            {
                return false;
            }

            connector.SendData($"HELO {Dns.GetHostName()}{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(250))
            {
                return false;
            }

            connector.SendData($"AUTH LOGIN{SmtpConnectorBase.EOF}");
            if (!connector.CheckResponse(334))
            {
                return false;
            }

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(334))
            {
                return false;
            }

            connector.SendData(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{password}")) + SmtpConnectorBase.EOF);
            if (!connector.CheckResponse(235))
            {
                return false;
            }

            return true;
        }

        void IProgramHandle.GetSmtpClient()
        {
            SmtpClient SmtpServer = new SmtpClient(App.SmtpClient);
            SmtpServer.Port = App.SmtpPort;
            App.SmtpPortout = 25;
            SmtpServer.Credentials = new System.Net.NetworkCredential(App.currentEmail, App.password);
            //SmtpServer.EnableSsl = App.EnableSsl;
            SmtpServer.EnableSsl = false;



            App.smtpClient = SmtpServer;
            
        }


        protected override Func<IProgramHandle> GetFactory() => () => new ProgramHandle();
    }
}
