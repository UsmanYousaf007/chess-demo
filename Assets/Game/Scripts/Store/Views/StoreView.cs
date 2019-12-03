/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public partial class StoreView : View
    {
        public enum StoreTabs
        {
            POWERUPS,
            SKINS,
            COINS
        };

        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        // View signals
        public Signal<StoreItem> storeItemClickedSignal = new Signal<StoreItem>();

        public TopInventoryBarPrefab topInventoryBar;

        public Text titleBundlesLabel;
        public Text tabPowerUpsLabel;
        public Text tabThemesLabel;
        public Text tabCoinsLabel;

        public Button tabButtonPowerUps;
        public Button tabButtonThemes;
        public Button tabButtonCoins;

        public GameObject tabUnderlinePowerUps;
        public GameObject tabUnderlineThemes;
        public GameObject tabUnderlineCoins;

        public GameObject processingUi;

        private StoreThumbsContainer thumbsContainer;
        private StoreIconsContainer iconsContainer;

        public void Init()
        {
            thumbsContainer = StoreThumbsContainer.Load();
            iconsContainer = StoreIconsContainer.Load();
            titleBundlesLabel.text = localizationService.Get(LocalizationKey.STORE_TITLE_BUNDLES);
            tabPowerUpsLabel.text = localizationService.Get(LocalizationKey.STORE_TAB_POWERUPS);
            tabThemesLabel.text = localizationService.Get(LocalizationKey.STORE_TAB_THEMES);
            tabCoinsLabel.text = localizationService.Get(LocalizationKey.STORE_TAB_COINS);

            tabButtonPowerUps.onClick.AddListener(OnTabPowerUpsClicked);
            tabButtonThemes.onClick.AddListener(OnTabThemesClicked);
            tabButtonCoins.onClick.AddListener(OnTabCoinsClicked);
        }

        public void UpdateView(StoreVO vo)
        {
            UpdateViewSkins(vo);
            UpdateViewPowerUps(vo);
            UpdateViewCoins(vo);
            UpdateViewBundles(vo);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ShowTab(StoreTabs.POWERUPS); // test
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        void OnStoreItemClicked(StoreItem item)
        {
            audioService.PlayStandardClick();
            storeItemClickedSignal.Dispatch(item);
        }

        public void ShowTab(StoreTabs tab)
        {
            scrollViewPowerUps.gameObject.SetActive(tab == StoreTabs.POWERUPS);
            scrollViewSkins.gameObject.SetActive(tab == StoreTabs.SKINS);
            scrollViewCoins.gameObject.SetActive(tab == StoreTabs.COINS);

            tabUnderlinePowerUps.gameObject.SetActive(tab == StoreTabs.POWERUPS);
            tabUnderlineThemes.gameObject.SetActive(tab == StoreTabs.SKINS);
            tabUnderlineCoins.gameObject.SetActive(tab == StoreTabs.COINS);
        }

        public void OnTabPowerUpsClicked()
        {
            ShowTab(StoreTabs.POWERUPS);
            analyticsService.ScreenVisit(AnalyticsScreen.powerups);
        }

        public void OnTabThemesClicked()
        {
            ShowTab(StoreTabs.SKINS);
            analyticsService.ScreenVisit(AnalyticsScreen.themes);
        }

        public void OnTabCoinsClicked()
        {
            ShowTab(StoreTabs.COINS);
            analyticsService.ScreenVisit(AnalyticsScreen.coins);
        }

        public void ShowProcessing(bool show)
        {
            processingUi.SetActive(show);
            uiBlocker.SetActive(show);
        }
    }
}
