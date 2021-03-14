using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Benchmark
{
    // XXX: Curious
    // https://www.c-sharpcorner.com/UploadFile/dacca2/5-tips-to-improve-performance-of-C-Sharp-code-part-3/
    // totally lie
    // 需要考虑预热反复实验
    // 1. == 看源码就能了解到是调用的 Equals()，> public static bool operator ==(string? a, string? b) => string.Equals(a, b);
    // 2. 有些时候这个方法快，有些时候那个方法快，完全是看编译器的优化
    // 3. String.Empty 是一个静态只读field，这也解释了为什么不能用来初始化const string，"" 是编译期常量，会创建一个object，而String.Empty只是一个引用（ref）
    // 4. 根据stackOverflow，`String.Length == 0` 单字符串判断empty这确实是最优越的方式，其实诸如IsNullOrEmpty() 里面的条件比较也是用了这句。记得lindexi某一篇博客也提到过，这样做确实、只是小心别被同事骂，因为可读性不好。所以该用什么用什么，还是以实际业务为主吧。
    public class StringCompare
    {
        /// <summary>
        /// String if empty
        /// </summary>
        public void CompareOne()
        {
            const string testStr = "The quick brown fox jumps over the lazy dog";
            var collect = new List<(string, long)>();

            var sw = new Stopwatch();
            sw.Start();
            _ = testStr.Equals(string.Empty);
            sw.Stop();
            collect.Add(("Equals() with string.Empty", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr.Equals("");
            sw.Stop();
            collect.Add((@"Equals() with """"", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr == string.Empty;
            sw.Stop();
            collect.Add(("== with string.Empty", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr == "";
            sw.Stop();
            collect.Add(("== with \"\"", sw.ElapsedTicks));

            sw.Restart();
            _ = string.IsNullOrEmpty(testStr);
            sw.Stop();
            collect.Add(("IsNullOrEmpty()", sw.ElapsedTicks));

            sw.Restart();
            _ = string.IsNullOrWhiteSpace(testStr);
            sw.Stop();
            collect.Add(("IsNullOrWhiteSpace()", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr.Length == 0;
            sw.Stop();
            collect.Add(("testStr.Length == 0", sw.ElapsedTicks));

            Console.WriteLine(testStr);
            foreach (var (info, time) in collect.OrderBy(o => o.Item2).ToList())
            {
                Console.WriteLine($"{info, -30} {time}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// String if null
        /// </summary>
        public void CompareTwo()
        {
            string? testStr = null;
            var collect = new List<(string, long)>();

            var sw = new Stopwatch();
            sw.Start();
            _ = testStr == string.Empty;
            sw.Stop();
            collect.Add(("== with string.Empty", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr == "";
            sw.Stop();
            collect.Add(("== with \"\"", sw.ElapsedTicks));

            sw.Restart();
            _ = string.IsNullOrEmpty(testStr);
            sw.Stop();
            collect.Add(("IsNullOrEmpty()", sw.ElapsedTicks));

            sw.Restart();
            _ = string.IsNullOrWhiteSpace(testStr);
            sw.Stop();
            collect.Add(("IsNullOrWhiteSpace()", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr is null;
            sw.Stop();
            collect.Add(("is null", sw.ElapsedTicks));

            Console.WriteLine("null");
            foreach (var (info, time) in collect.OrderBy(o => o.Item2).ToList())
            {
                Console.WriteLine($"{info, -30} {time}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// String with String
        /// </summary>
        public void CompareThree()
        {
            const string testStr = "The quick brown fox jumps over the lazy dog";
            const string stringTwo = "Free courses, tutorials, videos, and more for learning web development with ASP.NET.";
            var collect = new List<(string, long)>();

            var sw = new Stopwatch();
            sw.Start();
            _ = testStr.Equals(stringTwo);
            sw.Stop();
            collect.Add(("Equals()", sw.ElapsedTicks));

            sw.Restart();
            _ = testStr == stringTwo;
            sw.Stop();
            collect.Add(("==", sw.ElapsedTicks));

            Console.WriteLine("two string compare");
            foreach (var (info, time) in collect.OrderBy(o => o.Item2).ToList())
            {
                Console.WriteLine($"{info, -30} {time}");
            }
            Console.WriteLine();
        }

        private void OldOne()
        {
            var sw = new Stopwatch();

            const string testStr = "The quick brown fox jumps over the lazy dog";

            sw.Start();
            _ = string.Empty.Equals(testStr);
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = testStr.Equals(string.Empty); //
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = !testStr.Equals(string.Empty); //
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = testStr == string.Empty;
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = testStr != string.Empty;
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = string.IsNullOrWhiteSpace(testStr);
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);

            sw.Restart();
            _ = string.IsNullOrWhiteSpace(testStr); //
            sw.Stop();

            Console.WriteLine(sw.ElapsedTicks);
        }
    }
}
