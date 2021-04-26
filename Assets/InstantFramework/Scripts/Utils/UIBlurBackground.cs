/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TurboLabz.TLUtils;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    static public class UIBlurBackground
    {
        static private NormalRoutineRunner routineRunner = new NormalRoutineRunner();
        static Material blurImageEffectMaterial = Resources.Load("BlurImageEffectMaterial", typeof(Material)) as Material;
        static Coroutine colorUpdateCR;

        static public Material GetBlurBackgroundMaterial()
        {
            return blurImageEffectMaterial;
        }

        static public void Setup(Image destImage, float blurBrightnessVal = Colors.BLUR_BG_BRIGHTNESS_NORMAL)
        {
            destImage.material = new Material(UIBlurBackground.GetBlurBackgroundMaterial());
            destImage.material.SetColor("_TintColor", new Color(blurBrightnessVal, blurBrightnessVal, blurBrightnessVal, 1.0f));
        }

        static public void UseLastBlurBackground(Image destImage, float blurBrightnessVal)
        {
            destImage.material.SetTexture("_MainTex", blurImageEffectMaterial.GetTexture("_MainTex"));
            destImage.material.SetColor("_TintColor", new Color(blurBrightnessVal, blurBrightnessVal, blurBrightnessVal, 1.0f));
        }

        // Apply blur image to destImage. Optional: Enable toEnableObj after blur image applied
        // Preconditions: destImage must have Setup applied on it
        // Blur level: 0-7 (high blur)
        static public Promise BlurBackground(Image destImage, int blurLevel, float brightness, GameObject toEnableObj = null, Promise promise = null)
        {
            Material mat = destImage.material;
            mat.SetColor("_TintColor", new Color(brightness, brightness, brightness, 1.0f));

            if (promise == null)
            {
                promise = new Promise();
            }

            CaptureScreenSprite(mat, Screen.width >> blurLevel, Screen.height >> blurLevel, toEnableObj, promise);
            return promise;
        }

        static public void SetBrightness(Image destImage, float brightness, float alpha)
        {
            destImage.material.SetColor("_TintColor", new Color(brightness, brightness, brightness, alpha));
        }

        static public void AnimateBrightness(Image destImage, float brightness, float alpha, float dur)
        {
            float currBrightness = destImage.material.GetColor("_TintColor").r;
            float currAlpha = destImage.material.GetColor("_TintColor").a;
            long destTime = TimeUtil.unixTimestampMilliseconds + (long)(dur * 1000);
            colorUpdateCR = routineRunner.StartCoroutine(OnAnimateColorUpdateCR(destImage.material, currBrightness, brightness, currAlpha, alpha, dur, destTime));
        }

        static public void StopAnimateBrightness()
        {
            if (colorUpdateCR != null)
            {
               routineRunner.StopCoroutine(colorUpdateCR);
            }
        }

        static private IEnumerator OnAnimateColorUpdateCR(Material mat, float aBrightness, float bBrightness, float aAlpha, float bApha, float dur, long destTime)
        {
            long currTime = TimeUtil.unixTimestampMilliseconds;
            while (TimeUtil.unixTimestampMilliseconds <= destTime)
            {
                float tParam = 1.0f - ((destTime - TimeUtil.unixTimestampMilliseconds) / (dur * 1000));
                float brightness = Mathf.Lerp(aBrightness, bBrightness, tParam);
                float alpha = Mathf.Lerp(aAlpha, bApha, tParam);

                mat.SetColor("_TintColor", new Color(brightness, brightness, brightness, alpha));

                yield return null;
            }

            mat.SetColor("_TintColor", new Color(bBrightness, bBrightness, bBrightness, bApha));
        }

        static private void CaptureScreenSprite(Material mat, int destTextureWidth, int destTextureHeight, GameObject toEnableObj, Promise promise)
        {
            routineRunner.StartCoroutine(CaptureScreenSpriteCR(mat, destTextureWidth, destTextureHeight, toEnableObj, promise));
        }

        static private IEnumerator CaptureScreenSpriteCR(Material mat, int destTextureWidth, int destTextureHeight, GameObject toEnableObj, Promise promise)
        {
            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

            yield return new WaitForEndOfFrame();

            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            Texture2D nTex = ResizeTexture(screenImage, destTextureWidth, destTextureHeight);
            Texture2D.Destroy(screenImage);
            //Sprite sprite = Sprite.Create(nTex, new Rect(0, 0, nTex.width, nTex.height), new Vector2(0, 0));
            //dest.sprite = sprite;
            mat.SetTexture("_MainTex", nTex);
            blurImageEffectMaterial.SetTexture("_MainTex", nTex);

            yield return new WaitForEndOfFrame();

            if (toEnableObj != null) toEnableObj.SetActive(true);
            promise.Dispatch();
        }

        static private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
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