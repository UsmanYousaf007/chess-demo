/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:32:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using System;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;
using TurboLabz.Chess;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> AiTurn( FileRank from,
                                               FileRank to,
                                               string promotion)
        {
            return new GSAiTurnRequest().Send(matchInfoModel.challengeId,
                                              from,
                                              to,
                                              promotion);
        }
    }

    #region REQUEST

    public class GSAiTurnRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SetShardFlag";
        const string ATT_CHALLENGE_ID = "challengeId";
        const string ATT_FROM = "shard1";
        const string ATT_TO = "shard2";
        const string ATT_PROMOTION = "shard3";

        public IPromise<BackendResult> Send(string challengeId,
                                            FileRank from,
                                            FileRank to,
                                            string promotion)
        {
            this.errorCode = BackendResult.AI_TAKE_TURN_REQUEST_FAILED;

            string fromStr = GSFileRank.GSFiles[from.file] + GSFileRank.GSRanks[from.rank];
            string toStr = GSFileRank.GSFiles[to.file] + GSFileRank.GSRanks[to.rank];

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                .SetEventAttribute(ATT_FROM, fromStr)
                .SetEventAttribute(ATT_TO, toStr)
                .SetEventAttribute(ATT_PROMOTION, GSFormat.GetOptionalString(promotion))
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
