using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class NativeScreenCapture : IScreenCaptureService
    {
        public void CaptureScreenShot(string name, int zoomX)
        {
            ScreenCapture.CaptureScreenshot(name,zoomX);
        }

        public void CaptureScreenShot(string name)
        {
            ScreenCapture.CaptureScreenshot(name);
        }

        public void CaptureShot(string name, int zoomX)
        {
            pathName = name;
            superSize = zoomX;
        }

        private string pathName = string.Empty;
        private int superSize = 1;
        private Texture2D screenTexture = null;

        public IEnumerator TakeScreenShot()
        {
            yield return new WaitForEndOfFrame();

            var texture = ScreenCapture.CaptureScreenshotAsTexture(superSize);

            screenTexture = texture;

            Object.Destroy(texture);
        }
    }
}

