using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class OfferDrawCommand : Command
    {
        // Parameters
        //[Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        private string challengeId = null;

        public override void Execute()
        {
            Retain();

            challengeId = matchInfoModel.activeChallengeId;
            preferencesModel.resignCount++;
            backendService.PlayerOfferDraw(challengeId).Then(OnOfferDraw);
        }

        private void OnOfferDraw(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    //syncReconnectDataSignal.Dispatch(challengeId);
                }
            }

            Release();
        }
    }
}

