using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class InventoryView : View
    {
        [Serializable]
        public class InventoryTab
        {
            public Button button;
            public Text title;
            public Image selected;
            public GameObject tab;
        }

        public InventoryTab specialItem;
        public InventoryTab themes;
        public SkinItemView[] skinMenuItems;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Models 
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Signals
        public Signal applyThemeSignal = new Signal();

        [HideInInspector] public string originalSkinId;

        public void Init()
        {
            specialItem.title.text = localizationService.Get(LocalizationKey.INVENTORY_SPECIAL_ITEMS);
            themes.title.text = localizationService.Get(LocalizationKey.CPU_MENU_THEMES);
            specialItem.button.onClick.AddListener(OnClickSpecialItems);
            themes.button.onClick.AddListener(OnClickThemes);
            SetupTab(specialItem, themes);
        }

        public void Show()
        {
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
        }

        private void OnClickSpecialItems()
        {
            audioService.PlayStandardClick();
            SetupTab(specialItem, themes);
            applyThemeSignal.Dispatch();
        }

        private void OnClickThemes()
        {
            audioService.PlayStandardClick();
            originalSkinId = playerModel.activeSkinId;
            SetupTab(themes, specialItem);
        }

        private void SetupTab(InventoryTab newTab, InventoryTab oldTab)
        {
            newTab.selected.enabled = true;
            oldTab.selected.enabled = false;
            newTab.tab.SetActive(true);
            oldTab.tab.SetActive(false);
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
    }
}
