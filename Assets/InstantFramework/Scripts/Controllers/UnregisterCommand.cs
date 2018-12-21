﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;
using System;

namespace TurboLabz.InstantFramework 
{
    public class UnregisterCommand : Command
    {
        // Parameters
        [Inject] public string challengeId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public UpdateFriendBarStatusSignal updateFriendBarStatusSignal { get; set; }
        [Inject] public FriendBarBusySignal friendBarBusySignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private string opponentId;

        public override void Execute()
        {
            LogUtil.Log("UNREGISTER", "red");
            opponentId = matchInfoModel.matches[challengeId].opponentPublicProfile.playerId;
            matchInfoModel.unregisteredChallengeIds.Add(challengeId);

            LongPlayStatusVO vo;
            vo.playerId = opponentId;
            vo.lastActionTime = DateTime.UtcNow;
            vo.longPlayStatus = LongPlayStatus.DEFAULT;
            updateFriendBarStatusSignal.Dispatch(vo);

            friendBarBusySignal.Dispatch(opponentId, true);
            backendService.Unregister(challengeId).Then(OnUnregister);
        }

        private void OnUnregister(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }
                
            if (result == BackendResult.SUCCESS)
            {
                if (playerModel.friends.ContainsKey(opponentId))
                {
                    //TODO: The update friend bar signal is fired twice when canceling a match, investigate and optimize.
                    updateFriendBarSignal.Dispatch(playerModel.friends[opponentId], opponentId);
                }

                friendBarBusySignal.Dispatch(opponentId, false);
                sortFriendsSignal.Dispatch();
            }

            Release();
        }
    }
}
