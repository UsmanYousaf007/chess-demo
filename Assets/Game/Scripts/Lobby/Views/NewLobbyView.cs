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
        public Transform[] carouselItems;
        public RectTransform carousel;
        public Scroller scroller;

        private Vector3[] carouselItemsOriginalPositions;
        private Vector3 carouselOriginalPosition;
        private Vector3 carouselOriginalSize;

        public Image[] glow;

        public Sprite computerCardGlow;
        public Sprite careerCardGlow;
        public Sprite lessonCardGlow;

        private Coroutine animationCR;

        public void Init()
        {
            carouselOriginalPosition = carousel.localPosition;
            carouselOriginalSize = carousel.sizeDelta;
            carouselItemsOriginalPositions = new Vector3[carouselItems.Length];

            for (int i = 0; i < carouselItems.Length; i++)
            {
                carouselItemsOriginalPositions[i] = carouselItems[i].localPosition;
            }

            Scroller.OnSettled += StartAnimation;
            Scroller.CancelAnimation += CancelAnimation;

            glow[scroller.GetCurrentItem()].DOFade(1, 1);
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
            carousel.localPosition = carouselOriginalPosition;
            carousel.sizeDelta = carouselOriginalSize;

            for (int i = 0; i < carouselItems.Length; i++)
            {
                carouselItems[i].localPosition = carouselItemsOriginalPositions[i];
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
            glow[scroller.GetLastItem()].DOFade(0, 1);
            glow[scroller.GetCurrentItem()].DOFade(1, 1);
        }
    }
}