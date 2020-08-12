using strange.extensions.mediation.impl;
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

        [HideInInspector] public StoreIconsContainer iconsContainer;
        [HideInInspector] public StoreThumbsContainer thumbsContainer;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            specialPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_SPECIAL_PACKS);
            gemPacksHeading.text = localizationService.Get(LocalizationKey.SHOP_GEM_PACKS);
            subscriptionButtonText.text = localizationService.Get(LocalizationKey.UPGRADE_TEXT);
            subscriptionStripText.text = localizationService.Get(LocalizationKey.SHOP_SUBSCRIPTION_STRIP);
            ownedText.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);

            iconsContainer = StoreIconsContainer.Load();
            thumbsContainer = StoreThumbsContainer.Load();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
