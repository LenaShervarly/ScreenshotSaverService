using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenshotSaver.Api
{
    [JsonObject]
    public class ApiMultipleURLScreenShotRequestWrapper
    {
        [JsonProperty]
        public string[] URLs { get; set; }
    }
}
