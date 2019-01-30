using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public class SpotPurchaseView : View
    {
        public enum PowerUpSections
        {
            SAFEMOVES,
            HINTS,
            HINDSIGHTS
        };

        public Button closeButton;

        private StoreThumbsContainer thumbsContainer;

        public GameObject safeMovePowerUpSection;
        public GameObject hintPowerUpSection;
        public GameObject hindsightPowerUpSection;

        public GameObject[] galleryPowerUps;
        private IDictionary<string, PowerUpShopItemPrefab> prefabsPowerUps = null;


        public GameObject[] galleryCoins;
        private IDictionary<string, CoinShopItemPrefab> prefabsCoins = null;

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }

        // View signals
        public Signal closeClickedSignal = new Signal();
        public Signal<StoreItem> storeItemClickedSignal = new Signal<StoreItem>();

        public void Init()
        {
            thumbsContainer = StoreThumbsContainer.Load();
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        public void UpdateView(StoreVO vo, PowerUpSections activeSection)
        {
            UpdateViewPowerUps(vo);
            UpdateViewCoins(vo);
            UpdatePowerUpNotEnoughCoinsLabels(vo, galleryPowerUps);

            safeMovePowerUpSection.gameObject.SetActive(activeSection == PowerUpSections.SAFEMOVES);
            hintPowerUpSection.gameObject.SetActive(activeSection == PowerUpSections.HINTS);
            hindsightPowerUpSection.gameObject.SetActive(activeSection == PowerUpSections.HINDSIGHTS);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        void OnCloseClicked()
        {
            closeClickedSignal.Dispatch();
        }

        public void UpdateViewPowerUps(StoreVO vo)
        {
            if (prefabsPowerUps == null)
            {
                prefabsPowerUps = new Dictionary<string, PowerUpShopItemPrefab>();
                InitPrefabsPowerUps(vo, galleryPowerUps);
            }
        }

        private void InitPrefabsPowerUps(StoreVO vo, GameObject[] content)
        {
            foreach (GameObject child in content)
            {
                PowerUpShopItemPrefab powerUpPrefab = child.GetComponent<PowerUpShopItemPrefab>();
                prefabsPowerUps.Add(powerUpPrefab.key, powerUpPrefab);
                StoreItem storeItem = vo.storeSettingsModel.store.items[powerUpPrefab.key];
                powerUpPrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));

                Sprite sprite = thumbsContainer.GetSprite(powerUpPrefab.key);
                powerUpPrefab.Populate(vo, sprite);
            }
        }

        public void UpdateViewCoins(StoreVO vo)
        {
            if (prefabsCoins == null)
            {
                prefabsCoins = new Dictionary<string, CoinShopItemPrefab>();
                InitPrefabsCoins(vo, galleryCoins);
            }

            // Update prices
            foreach (GameObject child in galleryCoins)
            {
                CoinShopItemPrefab coinPrefab = child.GetComponent<CoinShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[coinPrefab.key];
                if (storeItem.remoteProductPrice == null)
                {
                    coinPrefab.price.text = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                }
                else
                {
                    coinPrefab.price.text = storeItem.remoteProductPrice;
                }
            }
        }

        private void InitPrefabsCoins(StoreVO vo, GameObject[] content)
        {
            foreach (GameObject child in content)
            {
                CoinShopItemPrefab coinPrefab = child.GetComponent<CoinShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[coinPrefab.key];

                prefabsCoins.Add(coinPrefab.key, coinPrefab);

                coinPrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));
                coinPrefab.displayName.text = storeItem.displayName;
                coinPrefab.thumbnail.sprite = thumbsContainer.GetSprite(coinPrefab.key);
            }
        }

        public void OnStoreItemClicked(StoreItem item)
        {
            storeItemClickedSignal.Dispatch(item);
        }

        private void UpdatePowerUpNotEnoughCoinsLabels(StoreVO vo, GameObject[] content)
        {
            long coins = vo.playerModel.bucks;

            foreach (GameObject child in content)
            {
                PowerUpShopItemPrefab powerUpPrefab = child.GetComponent<PowerUpShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[powerUpPrefab.key];

                GameObject notEnoughCoinsLabel = child.transform.Find("NotEnoughCoinsLabel").gameObject;
                notEnoughCoinsLabel.gameObject.SetActive(coins < storeItem.currency2Cost);
            }
        }

    }
}
