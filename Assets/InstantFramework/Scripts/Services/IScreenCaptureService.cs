using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TurboLabz.InstantFramework
{
    public interface IScreenCaptureService
    {
        void CaptureScreenShot(string name);
        void CaptureScreenShot(string name , int zoomX);
    }
}
