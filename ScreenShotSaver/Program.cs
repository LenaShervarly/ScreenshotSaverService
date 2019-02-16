using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using NLog;
using LogLevel = NLog.LogLevel;
using Starcounter;
using ScreenshotSaver.Handlers;
using ScreenShotSaver;
using System.IO;

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
            chromeOptions.AddArguments(new List<string>() { "headless", "--window-size=1920,1080" });
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Directory.GetCurrentDirectory());
            browser = new ChromeDriver(chromeDriverService, chromeOptions);
            browser.Manage().Window.Maximize();

            var screenshotHandler = new ScreenshotHandler(new ScreenshotMaker());

            Handle.POST<Request>(API_BASE_V1 + "screenshotfromurl", (req) => ScreenshotHandler.PostScreenShotsAsync(req), DefaultOption);
            Handle.GET<Request, string>(API_BASE_V1 + "screenshot/{?}", (req, idOrUrl) => ScreenshotHandler.GetScreenShotByIdOrUrl(req, idOrUrl), DefaultOption);
        }
    }
}

