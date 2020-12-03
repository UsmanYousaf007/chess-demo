/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class NewLobbyView : View
    {
        public Transform[] carouselItems;
        public RectTransform carousel;

        private Vector3[] carouselItemsOriginalPositions;
        private Vector3 carouselOriginalPosition;
        private Vector3 carouselOriginalSize;

        public void Init()
        {
            carouselOriginalPosition = carousel.localPosition;
            carouselOriginalSize = carousel.sizeDelta;
            carouselItemsOriginalPositions = new Vector3[carouselItems.Length];

            for (int i = 0; i < carouselItems.Length; i++)
            {
                carouselItemsOriginalPositions[i] = carouselItems[i].localPosition;
            }
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
    }
}
