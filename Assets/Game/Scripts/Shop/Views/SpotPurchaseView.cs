﻿using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SpotPurchaseView : View
    {
        public Text title;
        public Button close;
        public GameObject uiBlocker;
        public GameObject processing;
        public Button showMoreButton;
        public Button showLessButton;
        public GameObject moreOffers;
        public RectTransform[] layouts;
        public Transform bgGlow;
        public Transform bgGlowPivot;
        public GameObject extraBadge;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();

        public void Init()
        {
            title.text = localizationService.Get(LocalizationKey.SPOT_PURHCASE_TITLE);
            close.onClick.AddListener(OnCloseButtonClicked);
            showMoreButton.onClick.AddListener(() => ButtonClicked(true));
            showLessButton.onClick.AddListener(() => ButtonClicked(false));
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetupLayout(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeDlgSignal.Dispatch();
        }

        public void ShowProcessing(bool showUiBlocked, bool showProcessing)
        {
            uiBlocker.SetActive(showUiBlocked);
            processing.SetActive(showProcessing);
        }

        private void ButtonClicked(bool showMore)
        {
            audioService.PlayStandardClick();
            SetupLayout(showMore);
        }

        private void SetupLayout(bool showMore)
        {
            showMoreButton.gameObject.SetActive(!showMore);
            showLessButton.gameObject.SetActive(showMore);
            moreOffers.SetActive(showMore);
            extraBadge.SetActive(!showMore);
            RebuildLayouts();
        }

        private void RebuildLayouts()
        {
            foreach (var layout in layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }

            bgGlow.position = bgGlowPivot.position;
        }
    }
}
