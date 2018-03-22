/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 11:33:54 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class RoomsMediator : Mediator
    {
        // Dispatch signals
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public LoadShopSignal loadShopSignal { get; set; }
        [Inject] public LoadRoomsSignal loadRoomSignal { get; set; }
        [Inject] public LoadFreeCurrency1ModalSignal loadFreeCurrency1ModalSignal { get; set; }
        [Inject] public LoadOutOfCurrency1ModalSignal loadOutOfCurrency1ModalSignal { get; set; }

        // View injection
        [Inject] public RoomsView view { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        
        public override void OnRegister()
        {
            view.currency1BuyButtonClickedSignal.AddListener(OnCurrency1BuyButtonClicked);
            view.currency2BuyButtonClickedSignal.AddListener(OnCurrency2BuyButtonClicked);
            view.freeCurrency1ButtonClickedSignal.AddListener(OnFreeCurrency1ButtonClicked);
            view.roomButtonClickedSignal.AddListener(OnRoomButtonClicked);
            view.shopButtonClickedSignal.AddListener(OnShopButtonClicked);
            view.rotatingRoomTimeCompleteSignal.AddListener(OnRotatingRoomTimeCompleteSignal);
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);

            view.Init();
        }

        public override void OnRemove()
        {
            view.backButtonClickedSignal.RemoveListener(OnBackButtonClicked);
            view.shopButtonClickedSignal.RemoveListener(OnShopButtonClicked);
            view.roomButtonClickedSignal.RemoveListener(OnRoomButtonClicked);
            view.currency2BuyButtonClickedSignal.RemoveListener(OnCurrency2BuyButtonClicked);
            view.currency1BuyButtonClickedSignal.RemoveListener(OnCurrency1BuyButtonClicked);
            view.rotatingRoomTimeCompleteSignal.RemoveListener(OnRotatingRoomTimeCompleteSignal);
        }

        [ListensTo(typeof(UpdateRoomsViewSignal))]
        public void OnUpdateRoomsView(RoomsVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.ROOMS) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.ROOMS)
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
            loadShopSignal.Dispatch();
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            loadFreeCurrency1ModalSignal.Dispatch();
        }

        private void OnRoomButtonClicked(string roomId)
        {
            if (playerModel.currency1 < roomSettingsModel.settings[roomId].wager)
            {
                loadOutOfCurrency1ModalSignal.Dispatch();
            }
            else
            {
                // vo.roomId will provide the value for
                // GSEventData.FindMatch.ATT_VAL_MATCH_GROUP
                FindMatchVO vo;
                vo.findMatchInLastPlayedRoom = false;
                vo.roomId = roomId;

                findMatchSignal.Dispatch(vo);
            }
        }

        private void OnShopButtonClicked()
        {
            loadShopSignal.Dispatch();
        }

        private void OnRotatingRoomTimeCompleteSignal()
        {
            loadRoomSignal.Dispatch();
        }

        private void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }
    }
}
