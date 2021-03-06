using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dotNetPlayground
{
    static class Utils
    {
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

        public static void DateTimeToUnixI18N()
        {
            string time = "1970-01-01 00:00:00";
            DateTime dateTime = Convert.ToDateTime(time);
            // The key
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            DateTime zeroDataTime = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;
            Console.WriteLine(dateTime);
            Console.WriteLine(zeroDataTime);

            Console.WriteLine(new DateTimeOffset(dateTime).ToUnixTimeSeconds());
            Console.WriteLine(new DateTimeOffset(zeroDataTime).ToUnixTimeSeconds());
        }
    }
}
