/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LobbyMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadRoomsSignal loadRoomsSignal { get; set; }
        [Inject] public LoadPlayerProfileSignal loadPlayerProfileSignal { get; set; }
        [Inject] public LoadShopSignal loadShopSignal { get; set; }
        [Inject] public LoadInventorySignal loadInventorySignal { get; set; }
        [Inject] public LoadFreeCurrency1ModalSignal loadFreeCurrency1ModalSignal { get; set; }

        // Dispatch to game signals
        [Inject] public LoadCPUMenuSignal loadCPUSignal { get; set; }

        // View injection
        [Inject] public LobbyView view { get; set; }
        
        public override void OnRegister()
        {
            view.currency1BuyButtonClickedSignal.AddListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.AddListener(OnCurrency2BuyButtonClicked);
            view.settingButtonClickedSignal.AddListener(OnSettingsButtonClicked);
            view.playButtonClickedSignal.AddListener(OnPlayButtonClicked);
            view.CPUButtonClickedSignal.AddListener(OnCPUButtonClicked);
            view.learnButtonClickedSignal.AddListener(OnLearnButtonClicked);
            view.freeCurrency1ButtonClickedSignal.AddListener(OnFreeCurrency1ButtonClicked);
            view.feedbackButtonClickedSignal.AddListener(OnFeedbackButtonClicked);
            view.profileButtonClickedSignal.AddListener(OnPlayerProfileButtonClicked);
            view.shopButtonClickedSignal.AddListener(OnShopButtonClicked);
            view.Init();
        }

        public override void OnRemove()
        {
            view.currency1BuyButtonClickedSignal.RemoveListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.RemoveListener(OnCurrency2BuyButtonClicked);
            view.settingButtonClickedSignal.RemoveListener(OnSettingsButtonClicked);
            view.playButtonClickedSignal.RemoveListener(OnPlayButtonClicked);
            view.CPUButtonClickedSignal.RemoveListener(OnCPUButtonClicked);
            view.learnButtonClickedSignal.RemoveListener(OnLearnButtonClicked);
            view.freeCurrency1ButtonClickedSignal.RemoveListener(OnFreeCurrency1ButtonClicked);
            view.feedbackButtonClickedSignal.RemoveListener(OnFeedbackButtonClicked);
            view.profileButtonClickedSignal.RemoveListener(OnPlayerProfileButtonClicked);
            view.shopButtonClickedSignal.RemoveListener(OnShopButtonClicked);
        }

        [ListensTo(typeof(UpdateLobbyViewSignal))]
        public void OnUpdateView(LobbyVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdatePlayerProfilePictureSignal))]
        public void OnUpdateProfilePicture(Sprite sprite)
        {
            view.UpdateProfilePicture(sprite);
        }
        
        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOBBY)
            {
                view.Hide();
            }
        }

        private void OnCurrency1BuyButtonClicked()
        {
            loadShopSignal.Dispatch();
        }

        private void OnCurrency2BuyButtonClicked()
        {
            loadInventorySignal.Dispatch();
        }

        private void OnSettingsButtonClicked()
        {
            LogUtil.Log("##### OnSettingsButtonClicked()", "yellow");
        }

        private void OnPlayButtonClicked()
        {
            loadRoomsSignal.Dispatch();
        }

        private void OnCPUButtonClicked()
        {
            loadCPUSignal.Dispatch();
        }

        private void OnLearnButtonClicked()
        {
            LogUtil.Log("##### OnLearnButtonClicked()", "yellow");
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            loadFreeCurrency1ModalSignal.Dispatch();
        }

        private void OnFeedbackButtonClicked()
        {
            LogUtil.Log("##### OnFeedbackButtonClicked()", "yellow");
        }

        private void OnPlayerProfileButtonClicked()
        {
            loadPlayerProfileSignal.Dispatch();
        }

        private void OnShopButtonClicked()
        {
            loadShopSignal.Dispatch();
        }
    }
}
