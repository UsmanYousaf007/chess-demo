/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-10 17:29:49 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

namespace TurboLabz.Gamebet
{
    public class EndGameMediator : Mediator
    {
        // Dispatch signals
        [Inject] public LoadRoomsSignal loadRoomsSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public LoadOutOfCurrency1ModalSignal loadOutOfCurrency1ModalSignal { get; set; }


        // View injection
        [Inject] public EndGameView view { get; set; }

        // Models 
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void OnRegister()
        {
            view.newMatchButtonClickedSignal.AddListener(OnNewMatchButtonClicked);
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);

            view.Init();
        }

        public override void OnRemove()
        {
            view.backButtonClickedSignal.RemoveListener(OnBackButtonClicked);
            view.newMatchButtonClickedSignal.RemoveListener(OnNewMatchButtonClicked);
        }

        // We don't need to do the same for the opponent profile picture because
        // the matchmaking command ensures that we only update the view after we
        // get the opponent's profile picture if there is any.
        [ListensTo(typeof(UpdatePlayerProfilePictureSignal))]
        public void OnUpdatePlayerProfilePicture(Sprite sprite)
        {
            view.UpdatePlayerProfilePicture(sprite);
        }

        [ListensTo(typeof(UpdateEndGameViewSignal))]
        public void OnUpdateView(EndGameVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.END_GAME) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.END_GAME)
            {
                view.Hide();
            }
        }

        private void OnNewMatchButtonClicked()
        {
            if (playerModel.currency1 < roomSettingsModel.settings[matchInfoModel.roomId].wager)
            {
                loadRoomsSignal.Dispatch();

                loadOutOfCurrency1ModalSignal.Dispatch();
            }
            else
            {
                // vo.roomId will provide the value for
                // GSEventData.FindMatch.ATT_VAL_MATCH_GROUP
                FindMatchVO vo;
                vo.findMatchInLastPlayedRoom = true;
                vo.roomId = null;
                findMatchSignal.Dispatch(vo);
            }
        }

        private void OnBackButtonClicked()
        {
            loadRoomsSignal.Dispatch();
        }
    }
}
