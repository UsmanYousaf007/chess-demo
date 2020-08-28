﻿using strange.extensions.mediation.impl;
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
        public Button themesBanner;
        public GameObject themesAlert;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models 
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        //Signals
        public Signal applyThemeSignal = new Signal();
        public Signal unlockAllThemesSignal = new Signal();

        [HideInInspector] public string originalSkinId;

        private bool isThemeBannerShown;

        public void Init()
        {
            specialItem.title.text = localizationService.Get(LocalizationKey.INVENTORY_SPECIAL_ITEMS);
            themes.title.text = localizationService.Get(LocalizationKey.CPU_MENU_THEMES);
            specialItem.button.onClick.AddListener(OnClickSpecialItems);
            themes.button.onClick.AddListener(OnClickThemes);
            themesBanner.onClick.AddListener(OnThemesBannerClicked);
        }

        public void Show()
        {
            SetupTab(specialItem, themes);
            gameObject.SetActive(true);
            originalSkinId = playerModel.activeSkinId;
            themesAlert.SetActive(!preferencesModel.themesTabVisited);
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
            preferencesModel.themesTabVisited = true;
            themesAlert.SetActive(false);
            SetupTab(themes, specialItem);

            if (isThemeBannerShown)
            {
                analyticsService.Event(AnalyticsEventId.banner_shown, AnalyticsContext.unlock_all_themes);
            }
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