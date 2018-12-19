/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class CloseStripCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public UnregisterSignal unregisterSignal { get; set; }


        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }


        public override void Execute()
        {
            unregisterSignal.Dispatch(GetChallengeId());
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
