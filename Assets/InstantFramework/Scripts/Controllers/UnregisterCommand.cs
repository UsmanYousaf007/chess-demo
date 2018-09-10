﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework 
{
    public class UnregisterCommand : Command
    {
        // Parameters
        [Inject] public string challengeId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        private string opponentId;

        public override void Execute()
        {
            opponentId = matchInfoModel.matches[challengeId].opponentPublicProfile.playerId;
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
                matchInfoModel.matches.Remove(challengeId);
                chessboardModel.chessboards.Remove(challengeId);
                matchInfoModel.activeChallengeId = null;
                matchInfoModel.activeLongMatchOpponentId = null;

                updateFriendBarSignal.Dispatch(opponentId);
            }

            Release();
        }
    }
}
