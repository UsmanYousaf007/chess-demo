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
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ClearFriendSignal clearFriendSignal { get; set; }

        private string opponentId;

        public override void Execute()
        {
            opponentId = matchInfoModel.matches[challengeId].opponentPublicProfile.playerId;
            matchInfoModel.unregisteredChallengeIds.Add(challengeId);

            LongPlayStatusVO vo;
            vo.playerId = opponentId;
            vo.lastActionTime = DateTime.UtcNow;
            vo.longPlayStatus = LongPlayStatus.NONE;
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
                Friend friend = playerModel.friends[opponentId];
                if (friend.friendType == Friend.FRIEND_TYPE_COMMUNITY)
                {
                    if (friend.gamesWon == 0 &&
                        friend.gamesLost == 0 &&
                        friend.gamesDrawn == 0)
                    {
                        clearFriendSignal.Dispatch(opponentId);
                        playerModel.friends.Remove(opponentId);
                    }
                }

                matchInfoModel.matches.Remove(challengeId);
                chessboardModel.chessboards.Remove(challengeId);

                if (playerModel.friends.ContainsKey(opponentId))
                {
                    friendBarBusySignal.Dispatch(opponentId, false);
                    updateFriendBarSignal.Dispatch(playerModel.friends[opponentId], opponentId);
                    sortFriendsSignal.Dispatch();
                }
            }

            Release();
        }
    }
}
