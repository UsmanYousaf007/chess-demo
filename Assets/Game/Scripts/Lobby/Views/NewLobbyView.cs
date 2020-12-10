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
    public class NewLobbyView : View
    {
        public Scaler[] carouselItems;
        public Scroller carousel;
        public Image[] glow;
        private Coroutine animationCR;

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
    }
}