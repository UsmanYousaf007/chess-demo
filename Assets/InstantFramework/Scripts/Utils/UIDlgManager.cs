/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TurboLabz.InstantGame;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    static public class UIDlgManager
    {
        static public void Setup(GameObject dlg)
        {
            // Add a full screen background rect for blur to the object
            Transform uiDlgContainer = new GameObject(dlg.name + "_UIDlgContainer", typeof(Image)).transform;
            uiDlgContainer.SetParent(dlg.transform.parent);
            uiDlgContainer.SetSiblingIndex(dlg.transform.GetSiblingIndex());

            RectTransform rt = uiDlgContainer.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
            rt.localScale = new Vector2(1.0f, 1.0f);
            rt.anchoredPosition = new Vector2(0.0f, 0.0f);

            uiDlgContainer.gameObject.SetActive(false);

            dlg.AddComponent<CanvasGroup>();
            dlg.transform.SetParent(uiDlgContainer.transform);
        }

        static public IPromise Show(GameObject dlg, float blurBrightnessVal = Colors.BLUR_BG_BRIGHTNESS_NORMAL, bool useLastBlurredBg = false)
        {
            // Blur background and enable this dialog
            Image BlurBg = dlg.transform.parent.GetComponent<Image>();

            Promise promise = new Promise();

            if (useLastBlurredBg)
            {
                BlurBg.material = UIBlurBackground.GetBlurBackgroundMaterial();
                BlurBg.gameObject.SetActive(true);
                promise.Dispatch();
            }
            else
            {
                UIBlurBackground.BlurBackground(BlurBg, 5, blurBrightnessVal, BlurBg.gameObject, promise);
            }

            UIBlurBackground.SetBrightness(blurBrightnessVal, 0.0f);
            UIBlurBackground.AnimateBrightness(blurBrightnessVal, 1.0f, 0.25f);

            AnimateDlg(dlg);
            return promise;
        }

        static public IPromise Hide(GameObject dlg)
        {
            Promise promise = new Promise();
            Image BlurBg = dlg.transform.parent.GetComponent<Image>();
            CanvasGroup canvasGroup = dlg.GetComponent<CanvasGroup>();

            canvasGroup.DOKill();
            canvasGroup.DOFade(0.0f, 0.25f).OnComplete(() => OnHide(promise,dlg));
            UIBlurBackground.AnimateBrightness(Colors.BLUR_BG_BRIGHTNESS_NORMAL, 0.0f, 0.25f);
            BlurBg.DOFade(0.0f, 0.25f).OnComplete(() => BlurBg.gameObject.SetActive(false));
            return promise;
        }

        static private void OnHide(IPromise promise, GameObject dlg)
        {
            promise.Dispatch();
            dlg.SetActive(false);
        }

        static public void AnimateDlg(GameObject dlg)
        {
            CanvasGroup canvasGroup = dlg.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
            canvasGroup.transform.localScale = new Vector3(0.8f, 0.8f, 0.0f);
            canvasGroup.DOKill();
            canvasGroup.DOFade(1.0f, 0.25f);
            canvasGroup.transform.DOScale(1.0f, 0.25f).SetEase(Ease.OutSine);

            dlg.SetActive(true);
        }

        static public void DisableBlurBlg(GameObject dlg)
        {
            dlg.transform.parent.GetComponent<Image>().enabled = false;
        }

        static public void EnableBlurBlg(GameObject dlg)
        {
            if (dlg.transform.parent.GetComponent<Image>().enabled == false)
            {
                UIBlurBackground.SetBrightness(Colors.BLUR_BG_BRIGHTNESS_NORMAL, 0.0f);
                dlg.transform.parent.GetComponent<Image>().enabled = true;
                UIBlurBackground.AnimateBrightness(Colors.BLUR_BG_BRIGHTNESS_NORMAL, 1.0f, 0.25f);
            }
        }

    }
}