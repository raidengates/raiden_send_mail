using raiden_mail_reader.Handle;
using System;
using System.CodeDom;
using System.Net.Mail;
using System.Reflection;

namespace raiden_mail_reader.Entity
{

    public static class App 
    {
        private static string _prefix;
        private static string _path;
        public static string prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public static string path
        {
            //get { return string.IsNullOrWhiteSpace(prefix) ? _path = "C:\\Users\\graid\\Documents\\Outlook Files" : _path; }
            get { return string.IsNullOrWhiteSpace(prefix) ? _path = Environment.CurrentDirectory : _path; }
            set
            {
                _path = value;
            }
        }

        private static string _fileName;

        public static string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private static string _currentEmail;

        public static string currentEmail
        {
            get { return _currentEmail; }
            set { _currentEmail = value; }
        }

        private static string _tagetEmail;

        public static string tagetemail
        {
            get { return _tagetEmail; }
            set { _tagetEmail = value; }
        }

        private static string _password;

        public static string password
        {
            get { return _password; }
            set { _password = value; }
        }

        public static string SmtpClient { get; set; }

        public static int SmtpPort { get; set; }
        
        public static int SmtpPortout { get; set; }

        public static bool EnableSsl { get; set; }

        public static SmtpClient smtpClient { get; set; }
    }
}

