using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class InventoryView : View
    {
        public SkinItemView[] skinMenuItems;
        public Button themesBanner;
        public GameObject processing;
        public Text heading;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models 
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Signals
        public Signal applyThemeSignal = new Signal();
        public Signal unlockAllThemesSignal = new Signal();
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        [HideInInspector] public string originalSkinId;

        private bool isThemeBannerShown;

        public void Init()
        {
            heading.text = localizationService.Get(LocalizationKey.NAV_INVENTORY).ToUpper();
            themesBanner.onClick.AddListener(OnThemesBannerClicked);
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(true);
            gameObject.SetActive(true);
            originalSkinId = playerModel.activeSkinId;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            applyThemeSignal.Dispatch();
        }

        public void OnStoreAvailable(bool isAvailable)
        {
            if (!isAvailable)
            {
                SetupSkinItems();
            }
            else
            {
                isThemeBannerShown = !playerModel.OwnsAllThemes();
                ShowThemeBanner(isThemeBannerShown);
            }
        }

        public void ShowProcessing(bool show)
        {
            processing.SetActive(show);
        }

        private void SetupSkinItems()
        {
            foreach (var entry in storeSettingsModel.items)
            {
                if (entry.Value.kind.Equals(GSBackendKeys.ShopItem.SKIN_SHOP_TAG))
                {
                    skinMenuItems[entry.Value.skinIndex].Init(entry.Value.key);
                }
            }
        }

        public bool HasSkinChanged()
        {
            return originalSkinId != playerModel.activeSkinId;
        }

        private void OnThemesBannerClicked()
        {
            audioService.PlayStandardClick();
            unlockAllThemesSignal.Dispatch();
        }

        public void ShowThemeBanner(bool show)
        {
            themesBanner.gameObject.SetActive(show);
        }
    }
}
