﻿using raiden_mail_reader.Handle;
using raiden_mail_reader.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace raiden_mail_reader.Common
{
    public static class ArgsProviders
    {

        public static string[] args;

        public static string argsParams(this string val, bool isValue = true)
        {
            try
            {
                var valS = val.Split('=');
                if (valS == null) return string.Empty;
                if (valS.Length >= 2)
                {
                    return isValue ? valS.LastOrDefault() : valS.FirstOrDefault();
                }
                return valS.First();
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetReadLine(this string message)
        {
            Console.WriteLine("{0}: ", message.Trim());
            return Console.ReadLine();
        }


        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in </returns>
        public static string ReadPassword(char mask)
        {
            const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
            int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

            var pass = new Stack<char>();
            char chr = (char)0;

            while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
            {
                if (chr == BACKSP)
                {
                    if (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (chr == CTRLBACKSP)
                {
                    while (pass.Count > 0)
                    {
                        System.Console.Write("\b \b");
                        pass.Pop();
                    }
                }
                else if (FILTERED.Count(x => chr == x) > 0) { }
                else
                {
                    pass.Push((char)chr);
                    System.Console.Write(mask);
                }
            }

            System.Console.WriteLine();

            return new string(pass.Reverse().ToArray());
        }

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <returns>the string the user typed in </returns>
        public static string ReadPassword(this string message)
        {
            Console.WriteLine("{0}: ", message.Trim());
            return ReadPassword('*');
        }

        public static void WriteMessage(this string message)
        {
            Console.WriteLine("{0}: ", message.Trim());
        }


        public static void WaitingReadKey()
        {
            Console.ReadKey();
        }
    }
}
