using OpenQA.Selenium;
using ScreenshotSaver;
using ScreenshotSaver.Model;
using Starcounter;
using System;
using System.IO;

namespace ScreenShotSaver
{
    public class ScreenshotMaker : IScreenshotMaker
    {
        public string MakeAndSaveScreenShot(string url)
        {
            var verifiedUrl = GetVerifiedUrl(url);
            Program.browser.Navigate().GoToUrl(verifiedUrl);

            var filepath = GenerateFileFullPath();

            var screenShotData = default(ScreenshotData);
            var id = default(string);
            Db.Transact(() =>
            {
                screenShotData = new ScreenshotData(filepath, verifiedUrl);
                id = screenShotData.Id.ToString();
            });

            Program.browser.GetScreenshot()
                .SaveAsFile(filepath, ScreenshotImageFormat.Png);

            Db.Transact(() =>
            {
                screenShotData.SetUploaded();
            });

            return id;
        }

        private static string GetVerifiedUrl(string url)
        {
            url = url.Trim();

            bool isUrlValid = Uri.TryCreate(url, UriKind.Absolute, out  Uri validUrl)
                && (validUrl.Scheme == Uri.UriSchemeHttp || validUrl.Scheme == Uri.UriSchemeHttps);

            if(!isUrlValid)
            {
                return $"http://{url}";
            }
            return url;
        }

        private static string GenerateFileFullPath()
        {
            return string.Format("{0}/{1:N}.{2}", Directory.GetCurrentDirectory(), Guid.NewGuid(), "png");
        }
    }
}
