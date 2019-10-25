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

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class OpponentProfileMediator : Mediator
    {
        // View injection
        [Inject] public OpponentProfileView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(UpdateOpponentProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(UpdateFriendOnlineStatusSignal))]
        public void OnUpdateFriendOnlineStatusSignal(ProfileVO vo)
        {
            view.UpdateFriendOnlineStatusSignal(vo.playerId, vo.isOnline, vo.isActive);
        }

        [ListensTo(typeof(UpdateEloScoresSignal))]
        public void OnUpdateEloScoresSignal(EloVO vo)
        {
            view.UpdateEloScores(vo);
        }


        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdateFriendPic(string playerId, Sprite sprite)
        {
            view.UpdateProfilePic(playerId, sprite);
        }
    }
}
