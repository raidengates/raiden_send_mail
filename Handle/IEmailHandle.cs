using Microsoft.Office.Interop.Outlook;
using System.Collections.Generic;

namespace raiden_mail_reader.Handle
{
    public interface IEmailHandle
    {
        /// <summary>
        /// Lấy nội dung email từ file *.pst
        /// </summary>
        /// <param name="pstFilePath">tên đường dẫn</param>
        /// <param name="pstName">tên file cần lấy </param>
        void readPst(string pstFilePath, string pstName);

        /// <summary>
        /// Gửi mail nè.
        /// </summary>
        /// <param name="mailItem"></param>
        /// <returns></returns>
        bool sendMail(ref System.Exception o_ex);
    }
}
