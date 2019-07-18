using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TurboLabz.TLUtils;


namespace TurboLabz.InstantFramework
{
    public class NativeScreenCapture : IScreenCaptureService
    {
        private NormalRoutineRunner routineRunner = new NormalRoutineRunner();

        public void CaptureScreenShot()
        {
            routineRunner.StartCoroutine(CaptureScreenshot());
        }

        IEnumerator CaptureScreenshot()
        {
            yield return new WaitForEndOfFrame();
            string path = ScreenShotPath.GetScreenCapturePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
            //Get Image from screen
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            //Convert to png
            byte[] imageBytes = screenImage.EncodeToPNG();

            //Save image to file
            File.WriteAllBytes(path, imageBytes);
        }
    }
}

