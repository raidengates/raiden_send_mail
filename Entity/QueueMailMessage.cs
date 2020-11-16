using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace raiden_mail_reader.Entity
{
    public class QueueMailMessage
    {
        public static Queue<MailMessage> queueMailMessage = new Queue<MailMessage>();
    }
}
