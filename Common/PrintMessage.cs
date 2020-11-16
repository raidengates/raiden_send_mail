using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raiden_mail_reader.Common
{
    public static class PrintMessage
    {
        public static void compressorStart(this string Message) => Console.Write(string.Format("Performing compressor {0} task...", Message));

        public static void Done() => Console.Write(string.Format("Performing compressor complete!"));

        public static void Builder(this StringBuilder strBuilder) => Console.Write(strBuilder);


        /// <summary>
        /// for fun
        /// </summary>
        public static void help()
        {
            Console.WriteLine("");
            Console.WriteLine("     Usage: qac <command>");
            Console.WriteLine("         where <command> is one of:");
            Console.WriteLine("             combine, app, tools, elements, version, commit");
            Console.WriteLine("             but <commit> is coming soon! ^^");
            Console.WriteLine("     Good luck! <( _ _ )>");
        }


        public static void comingSoon()
        {
            Console.WriteLine("talked about coming soon");
        }

        public static void NotFound(this string fileName) => Console.WriteLine(string.Format("File '{0}' is not exists!", fileName));
    }
}
