/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using DanielLochner.Assets.SimpleScrollSnap;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public enum CardName
    {
        Computer,
        Career,
        Lesson
    }

    [System.CLSCompliant(false)]
    public partial class NewLobbyView : View
    {
        public Transform topNav;
        public Transform topBar;
        public Transform ticker;
        public Transform bottomNav;
        public SimpleScrollSnap carousel;
        public Image[] glow;
        public Canvas canvas;

        private Coroutine animationCR;
        private bool isInitialized;

        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public Signal rebuildLayoutSignal = new Signal();

        public void Init()
        {
            carousel.ForceInitialize();
            glow[1].DOFade(1, 1);
            carousel.onPanelChanged.AddListener(StartAnimation);
            //HandleNotch();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(CarouselInit());
            rebuildLayoutSignal.Dispatch();
        }

        public void Hide()
        {
            StartCoroutine(Reset());
        }

        private IEnumerator CarouselInit()
        {
            if (!isInitialized)
            {
                canvas.enabled = false;
                yield return new WaitForEndOfFrame();
                carousel.GoToPanel(1, true);
                yield return new WaitForEndOfFrame();
                isInitialized = true;
                canvas.enabled = true;
            }

            glow[carousel.PreviousPanel].DOFade(0, 1);
            glow[1].DOFade(1, 1);
        }

        private IEnumerator Reset()
        {
            carousel.GoToPanel(1, true);
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);
        }

        public void StartAnimation()
        {
            animationCR = StartCoroutine(Animate());
        }

        public void CancelAnimation()
        {
            if (animationCR != null)
            {
                StopCoroutine(animationCR);
                animationCR = null;
            }
            for (int i = 0; i < glow.Length; i++)
            {
                glow[i].DOKill();
            }
        }

        IEnumerator Animate()
        {
            yield return new WaitForSeconds(1);
            glow[carousel.PreviousPanel].DOFade(0, 1);
            glow[carousel.TargetPanel].DOFade(1, 1);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void HandleNotch()
        {
            if (NotchHandler.HasNotch())
            {
                topNav.localPosition = new Vector3(topNav.localPosition.x, topNav.localPosition.y - 80, topNav.localPosition.z);
                topBar.localPosition = new Vector3(topBar.localPosition.x, topBar.localPosition.y - 50, topBar.localPosition.z);
                ticker.localPosition = new Vector3(ticker.localPosition.x, ticker.localPosition.y + 88, ticker.localPosition.z);
                bottomNav.localPosition = new Vector3(bottomNav.localPosition.x, bottomNav.localPosition.y + 88, bottomNav.localPosition.z);
            }
        }
    }
}