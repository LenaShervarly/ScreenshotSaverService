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

        public ScreenshotHandler(IScreenshotMaker _screenshotMaker)
        {
            ScreenshotMaker = _screenshotMaker;
        }

        public static Response PostScreenShotsAsync(Request request)
        {
            var urls = JsonConvert.DeserializeObject<ApiMultipleURLScreenShotRequestWrapper>(request.Body).URLs;
            var fileIds = new List<string>();

            foreach (var url in urls)
            {
                try
                {
                    //IScreenshotMaker.MakeAndSaveScreenShot(url) - runs as a separate process: Windows service. 
                    // with Rabbit MQ I'll communicate to the Service 
                    var id = ScreenshotMaker.MakeAndSaveScreenShot(url);
                    fileIds.Add(id);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Failed navigate to URL and save a file");
                    var errorResponse = new Response
                    {
                        StatusCode = (ushort)HttpStatusCode.PreconditionFailed,
                        Body = $"Provided URL ({url}) is not valid. Please check your URL list in the specified file"
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

        public static Response GetScreenShotById(Request request, string id)
        {
            var screenshotPath = GetScreenShotById(id)?.Path;
            if (!string.IsNullOrEmpty(screenshotPath))
            {
                var stream = File.OpenRead(screenshotPath);
                var response = new Response
                {
                    StatusCode = (ushort)HttpStatusCode.OK,
                    StreamedBody = stream,
                    ContentType = "image/jpg"
                };
                return response;
            }
            else
            {
                var responseMessage = $"For the provided id: {id}, The filepath was empty of doesn't exist: {screenshotPath}";
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
            return Db.SQL<ScreenshotData>($"SELECT x FROM {typeof(ScreenshotData).Name} x WHERE x.{nameof(ScreenshotData.Id)} = ?", id).FirstOrDefault();
        }

        public static ScreenshotData GetScreenShotByUrl(string url)
        {
            return Db.SQL<ScreenshotData>($"SELECT x FROM {typeof(ScreenshotData).Name} x WHERE x.{nameof(ScreenshotData.Url)} = ?", url).FirstOrDefault();
        }
    }
}
