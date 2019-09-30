/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using TurboLabz.InstantFramework;
using strange.extensions.mediation.impl;
using UnityEngine.UI;
using TMPro;

namespace TurboLabz.InstantGame
{
    public class TopInventoryBarView : View
    {
        [Header("TopInventoryBar")]
        public Button safeMoveButton;
        public Button hintButton;
        public Button hindsightButton;
        public Button infoButton;
        public Button addCoinsButton;

        public Text safeMoveCountText;
        public Text hintCountText;
        public Text hindsightCountText;
        public Text coinsCountText;

        public Image safeMovePlus;
        public Image hintPlus;
        public Image hindsightPlus;

        [Inject] public ShowStoreTabSignal showStoreTabSignal { get; set; }



        public void InitTopInventoryBar()
        {
            safeMoveButton.onClick.AddListener(OnSafeMoveButtonClicked);
            hintButton.onClick.AddListener(OnHintButtonClicked);
            hindsightButton.onClick.AddListener(OnHindsightButtonClicked);
            addCoinsButton.onClick.AddListener(OnAddCoinsButtonClicked);
        }

        public void CleanupTopInventoryBar()
        {
            safeMoveButton.onClick.RemoveAllListeners();
            hintButton.onClick.RemoveAllListeners();
            hindsightButton.onClick.RemoveAllListeners();
            addCoinsButton.onClick.RemoveAllListeners();
        }

        public void UpdateTopInventoryBar(PlayerInventoryVO vo)
        {
     

            safeMoveCountText.gameObject.SetActive(true);
            hintCountText.gameObject.SetActive(true); 
            hindsightCountText.gameObject.SetActive(true);

            safeMovePlus.gameObject.SetActive(false);
            hintPlus.gameObject.SetActive(false); 
            hindsightPlus.gameObject.SetActive(false);

            safeMoveCountText.text = vo.safeMoveCount.ToString();
            hintCountText.text = vo.hintCount.ToString();
            hindsightCountText.text = vo.hindsightCount.ToString();
            coinsCountText.text = vo.coinCount.ToString();
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
