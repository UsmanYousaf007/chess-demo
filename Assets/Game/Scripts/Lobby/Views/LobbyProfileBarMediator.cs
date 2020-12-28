/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LobbyProfileBarMediator : Mediator
    {
        // View injection
        [Inject] public LobbyProfileBarView view { get; set; }

        //Dispatch signals
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public LoadLeaderboardSignal loadLeaderboardSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.leaderboardButtonClickedSignal.AddListener(OnLeaderboardClicked);
        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }

        public void OnLeaderboardClicked()
        {
            loadLeaderboardSignal.Dispatch();
        }
    }
}
