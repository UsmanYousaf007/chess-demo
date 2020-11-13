using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class ShopView : View
    {
        public Text bundleHeading;
        public Text specialPacksHeading;
        public Text gemPacksHeading;
        public string welcomePackShortCode;
        public GameObject welcomePack;
        public GameObject elitePack;
        public Button subscriptionStrip;
        public Text subscriptionStripText;
        public Button subscriptionButton;
        public Text subscriptionButtonText;
        public GameObject owned;
        public Text ownedText;
        public GameObject loading;
        public RectTransform layout;
        public GameObject uiBlocker;
        public GameObject processing;
        public string subscriptionKey;
        public string subscriptionSaleKey;
        public Text subscriptionOriginalPrice;
        public Text subscriptionNewPrice;
        public Text subscriptionRibbonText;
        public GameObject subscriptionRibbon;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Dispatch Signals
        public Signal subscriptionButtonClickedSignal = new Signal();
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        private bool isSubscriber;
        private bool isOnSale;

        public void Init()
        {
            bundleHeading.text = localizationService.Get(LocalizationKey.SHOP_SAVER_BUNDLES);
            specialPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_SPECIAL_PACKS);
            gemPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_GEM_PACKS);
            subscriptionButtonText.text = localizationService.Get(LocalizationKey.UPGRADE_TEXT);
            subscriptionStripText.text = localizationService.Get(LocalizationKey.SHOP_SUBSCRIPTION_STRIP);
            ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
            subscriptionButton.onClick.AddListener(OnSubscirptionButtonClicked);
            subscriptionStrip.onClick.AddListener(OnSubscirptionButtonClicked);
            ShowSaleItems(false);
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(true);
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnStoreAvailable(bool available)
        {
            SetBundle();
            SetSubscriptionOwnedStatus();
            subscriptionButtonText.gameObject.SetActive(available && !isOnSale);
            subscriptionOriginalPrice.gameObject.SetActive(available && isOnSale);
            subscriptionNewPrice.gameObject.SetActive(available && isOnSale);
            loading.SetActive(!available);

            if (available)
            {
                SetupSalePrice();
            }
        }

        public void SetSubscriptionOwnedStatus()
        {
            isSubscriber = playerModel.HasSubscription();
            owned.SetActive(isSubscriber);
            subscriptionButton.gameObject.SetActive(!isSubscriber);
        }

        public void SetBundle()
        {
            var isWelcomeBundlePurchased = playerModel.OwnsVGood(welcomePackShortCode);
            welcomePack.SetActive(!isWelcomeBundlePurchased);
            elitePack.SetActive(isWelcomeBundlePurchased);
        }

        private void OnSubscirptionButtonClicked()
        {
            audioService.PlayStandardClick();

            if (isSubscriber)
            {
                return;
            }

            subscriptionButtonClickedSignal.Dispatch();
        }

        public void ShowProcessing(bool showUiBlocked, bool showProcessing)
        {
            uiBlocker.SetActive(showUiBlocked);
            processing.SetActive(showProcessing);
        }

        private void ShowSaleItems(bool show)
        {
            subscriptionOriginalPrice.gameObject.SetActive(show);
            subscriptionNewPrice.gameObject.SetActive(show);
            subscriptionRibbonText.gameObject.SetActive(show);
            subscriptionButtonText.gameObject.SetActive(!show);
        }

        private void SetupSalePrice()
        {
            var saleItem = storeSettingsModel.items[subscriptionSaleKey];
            var storeItem = storeSettingsModel.items[subscriptionKey];

            if (saleItem == null || saleItem == null)
            {
                return;
            }

            var discount = 1 - (saleItem.productPrice / storeItem.productPrice);
            subscriptionOriginalPrice.text = storeItem.remoteProductPrice;
            subscriptionNewPrice.text = saleItem.remoteProductPrice;
            subscriptionRibbonText.text = $"MEGA SALE! {(int)discount * 100}% OFF";
        }

        public void SetupSale(string saleKey)
        {
            if (saleKey.Equals(subscriptionSaleKey))
            {
                isOnSale = true;
                ShowSaleItems(true);
            }
        }
    }
}
