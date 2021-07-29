using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TurboLabz.TLUtils;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;


namespace TurboLabz.InstantFramework
{
    public class BlurBackgroundService : IBlurBackgroundService
    {
        private NormalRoutineRunner routineRunner = new NormalRoutineRunner();

        Material blurImageEffectMaterial = Resources.Load("BlurImageEffectMaterial", typeof(Material)) as Material;

        public Material GetBlurBackgroundMaterial()
        {
            return blurImageEffectMaterial;
        }

        // Apply blur image to destImage. Optional: Enable toEnableObj after blur image applied
        // Blur level: 0-7 (high blur)
        public IPromise BlurBackground (Image destImage, int blurLevel, float brightness, GameObject toEnableObj)
        {
            blurImageEffectMaterial.SetColor("_TintColor", new Color(brightness, brightness, brightness, 1.0f));
            if (destImage)
            {
                destImage.material = blurImageEffectMaterial;
            }

            Promise promise = new Promise();
            CaptureScreenSprite(Screen.width >> blurLevel, Screen.height >> blurLevel, toEnableObj, promise);
            return promise;
        }

        private void CaptureScreenSprite(int destTextureWidth, int destTextureHeight, GameObject toEnableObj, Promise promise)
        {
            routineRunner.StartCoroutine(CaptureScreenSpriteCR(destTextureWidth, destTextureHeight, toEnableObj, promise));
        }

        IEnumerator CaptureScreenSpriteCR(int destTextureWidth, int destTextureHeight, GameObject toEnableObj, Promise promise)
        {
            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

            yield return new WaitForEndOfFrame();

            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            Texture2D nTex = ResizeTexture(screenImage, destTextureWidth, destTextureHeight);
            Texture2D.Destroy(screenImage);
            //Sprite sprite = Sprite.Create(nTex, new Rect(0, 0, nTex.width, nTex.height), new Vector2(0, 0));
            //dest.sprite = sprite;
            blurImageEffectMaterial.SetTexture("_MainTex", nTex);

            yield return new WaitForEndOfFrame();

            if (toEnableObj != null) toEnableObj.SetActive(true);
            promise.Dispatch();
        }

        private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
        {
            source.filterMode = FilterMode.Trilinear;
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Trilinear;
            RenderTexture.active = rt;
            Graphics.Blit(source, rt);
            Texture2D nTex = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
            nTex.filterMode = FilterMode.Trilinear;
            nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            nTex.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            return nTex;
        }
    }
}

