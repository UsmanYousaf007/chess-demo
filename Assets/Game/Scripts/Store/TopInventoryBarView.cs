/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantGame
{
    public class TopInventoryBarView : View
    {
        [Header("TopInventoryBar")]
        public TopInventoryBarPrefab inventoryBar;

        [Inject] public ShowStoreTabSignal showStoreTabSignal { get; set; }

        public void InitTopInventoryBar()
        {
            inventoryBar.safeMoveButton.onClick.AddListener(OnSafeMoveButtonClicked);
            inventoryBar.hintButton.onClick.AddListener(OnHintButtonClicked);
            inventoryBar.hindsightButton.onClick.AddListener(OnHindsightButtonClicked);
            inventoryBar.addCoinsButton.onClick.AddListener(OnAddCoinsButtonClicked);
        }

        public void CleanupTopInventoryBar()
        {
            inventoryBar.safeMoveButton.onClick.RemoveAllListeners();
            inventoryBar.hintButton.onClick.RemoveAllListeners();
            inventoryBar.hindsightButton.onClick.RemoveAllListeners();
            inventoryBar.addCoinsButton.onClick.RemoveAllListeners();
        }

        public void UpdateTopInventoryBar(TopInventoryBarVO vo)
        {
            inventoryBar.safeMoveCountText.gameObject.SetActive(vo.safeMoveCount >= 0);
            inventoryBar.hintCountText.gameObject.SetActive(vo.hintCount >= 0); 
            inventoryBar.hindsightCountText.gameObject.SetActive(vo.hindsightCount >= 0);

            inventoryBar.safeMovePlus.gameObject.SetActive(vo.safeMoveCount <= 0);
            inventoryBar.hintPlus.gameObject.SetActive(vo.hintCount <= 0); 
            inventoryBar.hindsightPlus.gameObject.SetActive(vo.hindsightCount <= 0);

            inventoryBar.safeMoveCountText.text = vo.safeMoveCount.ToString();
            inventoryBar.hintCountText.text = vo.hintCount.ToString();
            inventoryBar.hindsightCountText.text = vo.hindsightCount.ToString();
            inventoryBar.coinsCountText.text = vo.coinCount.ToString();
        }

        public void OnSafeMoveButtonClicked()
        {
            showStoreTabSignal.Dispatch(StoreView.StoreTabs.POWERUPS);
        }

        public void OnHintButtonClicked()
        {
             showStoreTabSignal.Dispatch(StoreView.StoreTabs.POWERUPS);
        }

        public void OnHindsightButtonClicked()
        {
             showStoreTabSignal.Dispatch(StoreView.StoreTabs.POWERUPS);
        }

        public void OnAddCoinsButtonClicked()
        {
             showStoreTabSignal.Dispatch(StoreView.StoreTabs.COINS);
        }

    }
}
