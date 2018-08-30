/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;


namespace TurboLabz.InstantFramework
{
    public class TapLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public CreateLongMatchSignal createLongMatchSignal { get; set; }
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            matchInfoModel.activeLongMatchOpponentId = opponentId;

            string challengeId = GetMatchExists();

            if (challengeId == null)
            {
                createLongMatchSignal.Dispatch(opponentId);
            }
            else
            {
                startLongMatchSignal.Dispatch(challengeId);
            }
        }

        private string GetMatchExists()
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId == opponentId)
                {
                    if (!entry.Value.concluded)
                    {
                        return entry.Key;
                    }
                }
            }

            return null;
        }
    }
}
