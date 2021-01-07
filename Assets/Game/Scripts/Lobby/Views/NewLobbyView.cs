/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using FM.Legacy;
using System.Collections;
using DG.Tweening;

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

        public Scaler[] carouselItems;
        public Scroller carousel;
        public Image[] glow;
        private Coroutine animationCR;

        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void Init()
        {
            carousel.Setup();

            foreach (var item in carouselItems)
            {
                item.Setup();
            }

            Scroller.OnSettled += StartAnimation;
            Scroller.CancelAnimation += CancelAnimation;

            glow[carousel.GetCurrentItem()].DOFade(1, 1);

            //HandleNotch();
        }

        public void Show()
        {
            Reset();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Reset()
        {
            carousel.Reset();

            foreach (var item in carouselItems)
            {
                item.Reset();
            }
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
            glow[carousel.GetLastItem()].DOFade(0, 1);
            glow[carousel.GetCurrentItem()].DOFade(1, 1);
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