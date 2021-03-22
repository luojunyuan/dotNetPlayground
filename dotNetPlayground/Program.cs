using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dotNetPlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetFileMd5(@"D:\makura(枕)\サクラノ詩\BGI.exe"));
        }

        public static string GetFileMd5(string filePath)
        {
            var file = File.OpenRead(filePath);
            var md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(file);
            file.Close();

            var sb = new StringBuilder();
            foreach (var byteItem in retVal)
            {
                sb.Append(byteItem.ToString("x2"));
            }
            return sb.ToString().ToUpper();
        }
    }
}
