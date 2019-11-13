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

        public void CaptureScreenShot(Texture2D logo)
        {
            routineRunner.StartCoroutine(CaptureScreenshot(logo));
        }

        IEnumerator CaptureScreenshot(Texture2D logo)
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

            //Add Logo
            AddLogo(screenImage, logo);

            //Convert to png
            byte[] imageBytes = screenImage.EncodeToPNG();

            //Save image to file
            File.WriteAllBytes(path, imageBytes);
        }

        private void AddLogo(Texture2D texture, Texture2D logo)
        {
            int startx = (texture.width - logo.width) / 2;
            int starty = (int)(texture.height - (logo.height * 1.5f));

            Color[] logoPixels = logo.GetPixels();
            Color[] originalPixels = texture.GetPixels(startx, starty, logo.width, logo.height);

            for (int i = 0; i < logoPixels.Length; i++)
            {
                originalPixels[i] = Color.Lerp(originalPixels[i], logoPixels[i], logoPixels[i].a);
            }

            texture.SetPixels(startx, starty, logo.width, logo.height, originalPixels);
            texture.Apply();
        }
    }
}

