using Newtonsoft.Json;
using NLog;
using ScreenshotSaver.Api;
using ScreenshotSaver.Model;
using Starcounter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;

namespace ScreenshotSaver.Handlers
{
    public class ScreenshotHandler
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static IScreenshotMaker ScreenshotMaker;

        public ScreenshotHandler(IScreenshotMaker screenshotMaker)
        {
            ScreenshotMaker = screenshotMaker;
        }

        public static Response PostScreenShotsAsync(Request request)
        {
            var urls = JsonConvert.DeserializeObject<ApiMultipleURLScreenShotRequestWrapper>(request.Body).URLs;
            var fileIds = new List<string>();

            foreach (var url in urls)
            {
                try
                {
                    //IScreenshotMaker.MakeAndSaveScreenShot(url) - should run in future as a separate process, for example as a Windows service. 
                    // with Rabbit MQ it will communicate to the Service 
                    var id = ScreenshotMaker.MakeAndSaveScreenShot(url);
                    fileIds.Add(id);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed navigate to URL and save a file");
                    var errorResponse = new Response
                    {
                        StatusCode = (ushort)HttpStatusCode.PreconditionFailed,
                        Body = $"Successfully made {fileIds.Count} screenshots. They are saved withs the following ids: {fileIds.ToString()}. " +
                            $"However there was a filure while processing url: { url}. Provided URL is not valid. " +
                            $"Please check your URL list. We expect to get URLs in the format: \"URLs\" : [ \"http://www.google.com\"]"
                    };
                    return errorResponse;
                }
            }

            var response = new Response
            {
                StatusCode = (ushort)HttpStatusCode.Created,
                Body = JsonConvert.SerializeObject(fileIds)
            };

            return response;
        }

        public static Response GetScreenShotByIdOrUrl(Request request, string idOrUrl)
        {
            var screenshot = GetScreenShotByUrl(idOrUrl);  
            if (screenshot == null)
            {
                screenshot = GetScreenShotById(idOrUrl);
            }

            var screenshotPath = screenshot?.Path;
            if (!string.IsNullOrEmpty(screenshotPath))
            { 
                var stream = File.OpenRead(screenshotPath);
                var response = new Response
                {
                    StatusCode = (ushort)HttpStatusCode.OK,
                    StreamedBody = stream,
                    ContentType = "image/png"
                };
                return response;
            }
            else
            {
                var responseMessage = $"For the provided id or Url: {idOrUrl}, The filepath was empty of doesn't exist: {screenshotPath}";
                Logger.Error(responseMessage);
                var errorResponse = new Response
                {
                    StatusCode = (ushort)HttpStatusCode.PreconditionFailed,
                    Body = responseMessage
                };
                return errorResponse;
            }
        }

        public static ScreenshotData GetScreenShotById(string id)
        {
            return Db.SQL<ScreenshotData>($"SELECT x FROM {typeof(ScreenshotData).Name} x WHERE x.{nameof(ScreenshotData.Id)} = ?", ulong.Parse(id)).FirstOrDefault();
        }

        public static ScreenshotData GetScreenShotByUrl(string url)
        {
            return Db.SQL<ScreenshotData>($"SELECT x FROM {typeof(ScreenshotData).Name} x WHERE x.{nameof(ScreenshotData.Url)} = ?", url).OrderBy(s => s.Updated).LastOrDefault();
        }
    }
}
