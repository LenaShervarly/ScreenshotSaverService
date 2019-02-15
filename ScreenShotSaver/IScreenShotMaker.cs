using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenshotSaver
{
    public interface IScreenshotMaker
    {
        string MakeAndSaveScreenShot(string url);
    }
}
