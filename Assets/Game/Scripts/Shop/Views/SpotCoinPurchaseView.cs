using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SpotCoinPurchaseView : View
    {
        public Button close;
        public Button showMoreButton;
        public Button showLessButton;
        public GameObject moreOffers;
        public RectTransform[] layouts;
        public Transform bgGlow;
        public Transform bgGlowPivot;
        public ShopCoinItemView[] packs;
        public GameObject normalDlg;
        public GameObject adDlg;
        public GameObject collectDlg;
        public Button watchAdButton;
        public Button buyCoinsButton;
        public Text coinsText;
        public Text gemsText;
        public Button closeButton2;
        public GameObject toolTip;
        public GameObject extraBadge;
        public Button collectButton;
        public Button closeButton3;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();
        public Signal watchAdButtonClickedSignal = new Signal();
        public Signal<StoreItem> buyButtonClickedSignal = new Signal<StoreItem>();
        public Signal collectButtonClickedSignal = new Signal();
        public Signal closeDlgWithAnalyticSignal = new Signal();

        private StoreItem storeItem;

        public void Init()
        {
            UIDlgManager.Setup(gameObject);

            close.onClick.AddListener(OnCloseButtonClicked);
            showMoreButton.onClick.AddListener(() => ButtonClicked(true));
            showLessButton.onClick.AddListener(() => ButtonClicked(false));
            closeButton2.onClick.AddListener(OnCloseButtonClickedWithAnalytic);
            buyCoinsButton.onClick.AddListener(OnBuyButtonClicked);
            watchAdButton.onClick.AddListener(OnWatchVideoButtonClicked);
            collectButton.onClick.AddListener(OnWatchVideoButtonClicked);
            closeButton3.onClick.AddListener(OnCloseButtonClickedWithAnalytic);
        }

        public void Show()
        {
            UIDlgManager.Show(gameObject).Then(RebuildLayouts);
        }

        public void Hide()
        {
            UIDlgManager.Hide(gameObject);
        }

        public void UpdateView(List<string> packsKeys)
        {
            normalDlg.SetActive(true);
            adDlg.SetActive(false);
            collectDlg.SetActive(false);
            buyCoinsButton.gameObject.SetActive(false);

            for (int i = 0; i < packs.Length; i++)
            {
                var showPack = i < packsKeys.Count;
                packs[i].gameObject.SetActive(showPack);

                if (showPack)
                {
                    packs[i].Setup(packsKeys[i]);
                }
            }

            SetupLayout(false);
        }

        public void UpdateAdDlg(StoreItem storeItem, bool hasConsent)
        {
            normalDlg.SetActive(false);
            adDlg.SetActive(hasConsent);
            collectDlg.SetActive(!hasConsent);
            buyCoinsButton.gameObject.SetActive(true);
            this.storeItem = storeItem;
            coinsText.text = storeItem.currency4Payout.ToString("N0");
            gemsText.text = storeItem.currency3Payout.ToString();
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeDlgSignal.Dispatch();
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

        private void OnWatchVideoButtonClicked()
        {
            audioService.PlayStandardClick();
            watchAdButtonClickedSignal.Dispatch();
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();
            buyButtonClickedSignal.Dispatch(storeItem);
        }

        private void OnCloseButtonClickedWithAnalytic()
        {
            audioService.PlayStandardClick();
            closeDlgWithAnalyticSignal.Dispatch();
        }
    }
}
