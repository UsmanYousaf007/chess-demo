/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.InstantFramework;

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

        // View signals
        public Signal<StoreItem> storeItemClickedSignal = new Signal<StoreItem>();

        public Text title;

        private StoreThumbsContainer thumbsContainer;

        public void Init()
        {
            thumbsContainer = StoreThumbsContainer.Load();
            title.text = localizationService.Get(LocalizationKey.CPU_STORE_HEADING);
        }

        public void UpdateView(StoreVO vo)
        {
            UpdateViewSkins(vo);
            UpdateViewPowerUps(vo);
            UpdateViewCoins(vo);
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
            storeItemClickedSignal.Dispatch(item);
        }

        public void ShowTab(StoreTabs tab)
        {
            scrollViewSkins.gameObject.SetActive(tab == StoreTabs.SKINS);
            scrollViewCoins.gameObject.SetActive(tab == StoreTabs.COINS);
            scrollViewPowerUps.gameObject.SetActive(tab == StoreTabs.POWERUPS);
        }
    }
}
