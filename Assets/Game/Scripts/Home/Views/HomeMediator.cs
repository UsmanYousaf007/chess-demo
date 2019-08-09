/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;
using System.Collections.Generic;
using TurboLabz.Multiplayer;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TurboLabz.CPU;

namespace TurboLabz.InstantFramework
{
    public class HomeMediator : Mediator
    {
        // View injection
        [Inject] public HomeView view { get; set; }

        // Dispatch signals
        [Inject] public AdjustStrengthSignal adjustStrengthSignal { get; set; }
        [Inject] public AuthFaceBookSignal authFacebookSignal { get; set; }
        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ShowProfileDialogSignal showProfileDialogSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public AcceptSignal acceptSignal { get; set; }
        [Inject] public DeclineSignal declineSignal { get; set; }
        [Inject] public CloseStripSignal closeStripSignal { get; set; }
        [Inject] public TurboLabz.Multiplayer.ResignSignal resignSignal { get; set; }
        [Inject] public RemoveCommunityFriendSignal removeCommunityFriendSignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        
        public override void OnRegister()
        {
            view.Init();

            view.playMultiplayerButtonClickedSignal.AddListener(OnQuickMatchBtnClicked);
            view.playCPUButtonClickedSignal.AddListener(OnPlayComputerMatchBtnClicked);
            view.actionCountUpdatedSignal.AddListener(OnActionCountUpdated);
            view.decStrengthButtonClickedSignal.AddListener(OnDecStrengthButtonClicked);
            view.incStrengthButtonClickedSignal.AddListener(OnIncStrengthButtonClicked);
        }

        private void OnDecStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(false);
        }

        private void OnIncStrengthButtonClicked()
        {
            adjustStrengthSignal.Dispatch(true);
        }

        [ListensTo(typeof(UpdateStrengthSignal))]
        public void OnUpdateStrength(HomeVO vo)
        {
            view.UpdateStrength(vo);
        }

        [ListensTo(typeof(UpdateHomeViewSignal))]
        public void OnUpdateView(HomeVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.HOME)
            {
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.home, facebookService.isLoggedIn());
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.HOME)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(PlayerProfilePicTappedSignal))]
        public void OnPlayerProfileButtonTapped()
        {
            if (gameObject.activeSelf)
            {
                loadStatsSignal.Dispatch();
            }
        }

        [ListensTo(typeof(ChessboardBlockerEnableSignal))]
        public void OnUIBlockerEnable(bool enable)
        {
            if (gameObject.activeSelf)
            {
                view.uiBlocker.SetActive(enable);
            }
        }

        private void OnActionCountUpdated(int count)
        {
            setActionCountSignal.Dispatch(count);
        }

        private void OnPlayComputerMatchBtnClicked()
        {
            startCPUGameSignal.Dispatch();
        }

        private void OnQuickMatchBtnClicked()
        {
            findMatchSignal.Dispatch();
        }
    }
}

