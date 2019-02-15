using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using NLog;
using LogLevel = NLog.LogLevel;
using Starcounter;
using ScreenshotSaver.Handlers;

namespace ScreenshotSaver
{
    class Program
    {
        public static ChromeDriver browser = null;
        public const string API_BASE_V1 = "/api/v1/";
        public static readonly HandlerOptions DefaultOption = new HandlerOptions() { SkipRequestFilters = true };

        static void Main()
        {
            var nLogconfig = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.log" };
            nLogconfig.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            LogManager.Configuration = nLogconfig;

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "headless" });
            var chromeDriverService = ChromeDriverService.CreateDefaultService(@"C:\Users\Lena\source\ScreenShotSaver\ScreenShotSaver");
            browser = new ChromeDriver(chromeDriverService, chromeOptions);

            Handle.POST<Request>(API_BASE_V1 + "screenshotfromurl", (req) => ScreenshotHandler.PostScreenShotsAsync(req), DefaultOption);
            Handle.GET<Request, string>(API_BASE_V1 + "screenshot/{?}", (req, id) => ScreenshotHandler.GetScreenShotById(req, id), DefaultOption);
        }
    }
}

