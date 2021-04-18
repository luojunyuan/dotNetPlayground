using RestSharp;
using System;
using System.Threading.Tasks;

namespace NetRequest404ThrowExceptions
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () => await TestRestSharpAsync());
            Console.ReadLine();
        }

        private static void TestRestSharpSync()
        {

        }


        private static async Task TestRestSharpAsync()
        {

            var client = new RestClient("http://www.baidu.com")
            {
                FailOnDeserializationError = false,
                ThrowOnAnyError = false,
                ThrowOnDeserializationError = false
            };

            var request = new RestRequest("api");
            var resp = await client.ExecutePostAsync(request);
            Console.WriteLine(resp.Content);
            Console.WriteLine("over");
            // RestSharp 
            /*
             * 引发的异常:“System.IO.IOException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.IO.IOException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.IO.IOException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.WebException”(位于 System.Net.Requests.dll 中)
             * 引发的异常:“System.Net.WebException”(位于 RestSharp.dll 中)
             */

            // Refit
            /*
             * 引发的异常:“System.IO.IOException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.IO.IOException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.IO.IOException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Net.Http.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             * 引发的异常:“System.Net.Http.HttpRequestException”(位于 System.Private.CoreLib.dll 中)
             */

            // 不论是RestSharp还是Refit使用异步方法网络404抛出异常时都会发生这么多异常
            // 并且EH中还会多三个IOException，一组HttpRequestException
            // 耗时20ms以内，调试模式下会感受到卡顿。Release没有问题。
            // 多数的翻译同时请求却没有互联网连接时能否保证界面不卡顿仍旧存疑、毕竟一个10多20毫秒，七八个就要超过100了。
            // 如果存在这个问题就需要考虑使用Net库判断链接情况，或者尝试使用RestSharp不抛出异常的Sync请求。
        }
    }
}
