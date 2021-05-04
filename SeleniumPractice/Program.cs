using OpenQA.Selenium.Edge;
using System;
using System.Threading;

namespace SeleniumPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            //    Console.WriteLine("Hello World!");
            //    var options = new EdgeOptions();
            //    options.UseChromium = true;
            //    //options.BinaryLocation = @"C:\Program Files (x86)\Microsoft\Edge Beta\Application\msedge.exe";

            //    var driver = new EdgeDriver(options);

            //EdgeDriverService service = EdgeDriverService.CreateDefaultService(msedgedriverDir, msedgedriverExe, false);
            //EdgeDriver e = new EdgeDriver(service, edgeOptions)

            var deeplUrl = "https://www.deepl.com/translator#" + "jp" + "/zh/";


            EdgeOptions edgeOptions = new()
            {
                UseChromium = true,
            };
            edgeOptions.AddArgument("headless");
            edgeOptions.AddArgument("disable-gpu");

            var driver = new EdgeDriver(edgeOptions);
            driver.Navigate().GoToUrl(deeplUrl);

            var content = "ところで今日の最高気温、何度だと思う？37度だぜ、37度。夏にしても暑すぎる。これじゃオーブンだ。37度っていえば一人でじっとしてるより女の子と抱き合ってた方が涼しいくらいの温度だ。";

            try
            {

                //# 清空翻译框
                driver.FindElementByXPath(@"//*[@id=""dl_translator""]/div[5]/div[2]/div[1]/div[2]/div[1]/textarea").Clear();
                //# 输入要翻译的文本
                driver.FindElementByXPath(@"//*[@id=""dl_translator""]/div[5]/div[2]/div[1]/div[2]/div[1]/textarea").SendKeys(content);

                Thread.Sleep(5000);

                // # 提取翻译信息
                string outputText = driver.FindElementById("target-dummydiv").GetAttribute("textContent");
                if (!string.IsNullOrWhiteSpace(outputText) &&
                        outputText.Trim().Length > 1)
                {
                    Console.WriteLine(outputText);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


            driver.Close();
            driver.Quit();
        }
    }
}
