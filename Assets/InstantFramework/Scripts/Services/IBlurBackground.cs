using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.promise.api;


namespace TurboLabz.InstantFramework
{
    public interface IBlurBackgroundService
    {
        Material GetBlurBackgroundMaterial();
        IPromise BlurBackground(Image bgImage, int blurLevel, float brighness = 1.0f, GameObject toEnableObj = null);
    }
}
