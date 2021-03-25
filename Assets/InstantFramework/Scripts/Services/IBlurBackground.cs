using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.promise.api;


namespace TurboLabz.InstantFramework
{
    public interface IBlurBackgroundService
    {
        IPromise BlurBackground(Image bgImage, int blurLevel, GameObject toEnableObj = null);
    }
}
