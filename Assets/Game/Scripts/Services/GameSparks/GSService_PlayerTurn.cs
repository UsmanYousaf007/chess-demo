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
        public IPromise<BackendResult> PlayerTurn(FileRank from,
            FileRank to,
            string promotion,
            bool claimFiftyMoveDraw,
            bool claimThreefoldRepeatDraw,
            bool rejectThreefoldRepeatDraw)
        {
            string fromStr = GSFileRank.GSFiles[from.file] + GSFileRank.GSRanks[from.rank];
            string toStr = GSFileRank.GSFiles[to.file] + GSFileRank.GSRanks[to.rank];

            return new GSTakeTurnRequest().Send(matchInfoModel.activeMatch.challengeId,
                fromStr,
                toStr,
                GSFormat.GetOptionalString(promotion),
                GSFormat.GetBool(claimFiftyMoveDraw),
                GSFormat.GetBool(claimThreefoldRepeatDraw),
                GSFormat.GetBool(rejectThreefoldRepeatDraw));
        }
    }

    #region REQUEST

    public class GSTakeTurnRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "TakeTurn";
        const string ATT_FROM = "from";
        const string ATT_TO = "to";
        const string ATT_PROMOTION = "promotion";
        const string ATT_CLAIM_FIFTY_MOVE_DRAW = "claimFiftyMoveDraw";
        const string ATT_CLAIM_THREEFOLD_REPEAT_DRAW = "claimThreefoldRepeatDraw";
        const string ATT_REJECT_THREEFOLD_REPEAT_DRAW = "rejectThreefoldRepeatDraw";

        public IPromise<BackendResult> Send(string challengeId,
            string from,
            string to,
            string promotion,
            int claimFiftyMoveDraw,
            int claimThreefoldRepeatDraw,
            int rejectThreefoldRepeatDraw)
        {
            this.errorCode = BackendResult.TAKE_TURN_REQUEST_FAILED;

            new LogChallengeEventRequest().SetChallengeInstanceId(challengeId)
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_FROM, from)
                .SetEventAttribute(ATT_TO, to)
                .SetEventAttribute(ATT_PROMOTION, promotion)
                .SetEventAttribute(ATT_CLAIM_FIFTY_MOVE_DRAW, claimFiftyMoveDraw)
                .SetEventAttribute(ATT_CLAIM_THREEFOLD_REPEAT_DRAW, claimThreefoldRepeatDraw)
                .SetEventAttribute(ATT_REJECT_THREEFOLD_REPEAT_DRAW, rejectThreefoldRepeatDraw)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
