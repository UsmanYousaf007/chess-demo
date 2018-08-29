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
    public class OpenLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public CreateLongMatchSignal createLongMatchSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            matchInfoModel.activeLongMatchOpponentId = opponentId;

            bool matchExists = GetMatchExists();

            if (!matchExists)
            {
                createLongMatchSignal.Dispatch(opponentId);
            }
        }

        private bool GetMatchExists()
        {
            bool matchExists = false;
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId == opponentId)
                {
                    if (!entry.Value.concluded)
                    {
                        matchExists = true;
                        break;
                    }
                }
            }

            return matchExists;
        }
    }
}
