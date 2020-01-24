/// @license Propriety <http://license.url>
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
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public UpdateSearchResultsSignal updateSearchResultsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private string opponentId;

        public override void Execute()
        {
            opponentId = matchInfoModel.matches[challengeId].opponentPublicProfile.playerId;
            matchInfoModel.unregisteredChallengeIds.Add(challengeId);

            LongPlayStatusVO vo;
            vo.playerId = opponentId;
            vo.lastActionTime = DateTime.UtcNow;
            vo.longPlayStatus = LongPlayStatus.DEFAULT;
            vo.isGameCanceled = false;
            vo.isPlayerTurn = false;
            vo.isRanked = false;
            updateFriendBarStatusSignal.Dispatch(vo);

            friendBarBusySignal.Dispatch(opponentId, true, CreateLongMatchAbortReason.Unassigned);
            backendService.Unregister(challengeId).Then(OnUnregister);
        }

        private void OnUnregister(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            friendBarBusySignal.Dispatch(opponentId, false, CreateLongMatchAbortReason.Unassigned);
            refreshFriendsSignal.Dispatch();

            // Todo: Community update may not be needed. Investigate especially because this is a backend server request.
            refreshCommunitySignal.Dispatch();

            // Refereshing search in case challenge is declined from searched results
            updateSearchResultsSignal.Dispatch();

            Release();
        }
    }
}
