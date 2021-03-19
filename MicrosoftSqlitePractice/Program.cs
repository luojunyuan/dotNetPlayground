using System;

namespace MicrosoftSqlitePractice
{
    class Program
    {
        static void Main(string[] args)
        {
            var trans = new EhServerTransitionDateTime();
            trans.Comments2Subtitle();
            Console.WriteLine("Over");
        }
    }
}
