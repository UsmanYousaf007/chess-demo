﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework 
{
    public class AcceptCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        string challengeId;

        public override void Execute()
        {
            Retain();
            challengeId = GetChallengeId();
            backendService.Accept(challengeId).Then(OnAccept);
        }

        private void OnAccept(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (result == BackendResult.SUCCESS)
            {
                MatchInfo matchInfo = matchInfoModel.matches[challengeId];
                matchInfo.acceptStatus = GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED;
                updateFriendBarSignal.Dispatch(playerModel.friends[opponentId], opponentId);
                sortFriendsSignal.Dispatch();
                startLongMatchSignal.Dispatch(challengeId);
            }

            Release();
        }

        private string GetChallengeId()
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId == opponentId)
                {
                    return entry.Key;
                }
            }

            return null;
        }
    }
}
