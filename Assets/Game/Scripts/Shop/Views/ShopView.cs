using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class ShopView : View
    {
        public Text specialPacksHeading;
        public Text gemPacksHeading;
        public string welcomePackShortCode;
        public GameObject welcomePack;
        public GameObject elitePack;
        public Text subscriptionStripText;
        public Button subscriptionButton;
        public Text subscriptionButtonText;
        public GameObject owned;
        public Text ownedText;
        public GameObject loading;
        public RectTransform layout;

        [HideInInspector] public StoreIconsContainer iconsContainer;
        [HideInInspector] public StoreThumbsContainer thumbsContainer;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        //Dispatch Signals
        public Signal subscriptionButtonClickedSignal = new Signal();

        private bool isInitialised = false;

        public void Init()
        {
            specialPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_SPECIAL_PACKS);
            gemPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_GEM_PACKS);
            subscriptionButtonText.text = localizationService.Get(LocalizationKey.UPGRADE_TEXT);
            subscriptionStripText.text = localizationService.Get(LocalizationKey.SHOP_SUBSCRIPTION_STRIP);
            ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);

            subscriptionButton.onClick.AddListener(OnSubscirptionButtonClicked);

            iconsContainer = StoreIconsContainer.Load();
            thumbsContainer = StoreThumbsContainer.Load();
        }

        public void Show()
        {
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
            subscriptionButton.interactable = available;
            subscriptionButtonText.gameObject.SetActive(available);
            loading.SetActive(!available);
        }

        public void SetSubscriptionOwnedStatus()
        {
            var isSubscriber = playerModel.HasSubscription();
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
            subscriptionButtonClickedSignal.Dispatch();
        }
    }
}
