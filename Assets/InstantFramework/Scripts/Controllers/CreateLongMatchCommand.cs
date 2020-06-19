/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class CreateLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }
        [Inject] public bool isRanked { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public FriendBarBusySignal friendBarBusySignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public override void Execute()
        {
            Retain();

            friendBarBusySignal.Dispatch(opponentId, true, CreateLongMatchAbortReason.Unassigned);
            matchInfoModel.createLongMatchAborted = false;
            matchInfoModel.createLongMatchAbortReason = CreateLongMatchAbortReason.Unassigned;
            backendService.CreateLongMatch(opponentId, isRanked).Then(OnCreateLongMatch);
        }

        private void OnCreateLongMatch(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                if (matchInfoModel.createLongMatchAborted)
                {
                    //loadLobbySignal.Dispatch();
                    friendBarBusySignal.Dispatch(opponentId, false, matchInfoModel.createLongMatchAbortReason);
                }
            }
            else if (result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
