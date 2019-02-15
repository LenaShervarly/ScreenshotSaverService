using System;
using System.Collections.Generic;
using Starcounter;

namespace ScreenshotSaver.Model
{
    [Database]
    public class ScreenshotData
    {
        public ulong Id => this.GetObjectNo();

        public string Path { get; set; }

        public string Url { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public bool Uploaded { get; set; }

        public ScreenshotData(string path, string url)
        {
            Path = path;
            Url = url;
            DateTime now = DateTime.UtcNow;
            Created = now;
            Updated = now;
            Uploaded = false;
        }

        public ScreenshotData() { }

        public void SetUpdated()
        {
            Updated = DateTime.UtcNow;
        }

        public void SetUploaded()
        {
            Uploaded = true;
        }

        public static IEnumerable<ScreenshotData> GetAll() 
        {
            return  Db.SQL<ScreenshotData>($"SELECT x FROM {typeof(ScreenshotData).Name} x");
        }
    }
}
