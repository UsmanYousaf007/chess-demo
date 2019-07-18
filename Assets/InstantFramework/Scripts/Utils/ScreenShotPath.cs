using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

namespace TurboLabz.TLUtils
{
    public static class ScreenShotPath
    {
        public static string GetScreenCapturePath()
        {
            return GetScreenCapturePath("ShareScreenShot.png");
        }

        public static string GetScreenCapturePath(string fileName)
        {
            string path = Application.persistentDataPath+"/"+fileName;
            return path;
        }
    }
}
