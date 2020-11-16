namespace raiden_mail_reader.Handle
{
    public interface IProgramHandle
    {
        /// <summary>
        /// cài đặt file 
        /// </summary>
        void setupFile();
        /// <summary>
        /// cài đặt email cần gửi đi
        /// </summary>
        void setupTagetEmail();

        /// <summary>
        /// cài đặt email dùng để gửi
        /// </summary>
        void setupCurrentEmail();

        /// <summary>
        /// cài đặt password email
        /// </summary>
        void setupPassword();

        /// <summary>
        /// troll cho vui
        /// </summary>
        void setupFuny();

        void setupSmtpConfig();

    }
}
