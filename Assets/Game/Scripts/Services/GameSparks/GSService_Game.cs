/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:32:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> TakeTurn(FileRank from,
                                                FileRank to,
                                                string promotion,
                                                bool claimFiftyMoveDraw,
                                                bool claimThreefoldRepeatDraw,
                                                bool rejectThreefoldRepeatDraw)
        {
            string fromStr = GSFileRank.GSFiles[from.file] + GSFileRank.GSRanks[from.rank];
            string toStr = GSFileRank.GSFiles[to.file] + GSFileRank.GSRanks[to.rank];

            return new GSTakeTurnRequest().Send(matchInfoModel.challengeId,
                                                fromStr,
                                                toStr,
                                                GSFormat.GetOptionalString(promotion),
                                                GSFormat.GetBool(claimFiftyMoveDraw),
                                                GSFormat.GetBool(claimThreefoldRepeatDraw),
                                                GSFormat.GetBool(rejectThreefoldRepeatDraw));
        }
            
        public IPromise<BackendResult> ClaimFiftyMoveDraw()
        {
            return new GSClaimFiftyMoveDrawRequest().Send(matchInfoModel.challengeId);
        }

        public IPromise<BackendResult> ClaimThreefoldRepeatDraw()
        {
            return new GSClaimThreefoldRepeatDrawRequest().Send(matchInfoModel.challengeId);
        }

        public IPromise<BackendResult> Resign()
        {
            return new GSResignRequest().Send(matchInfoModel.challengeId);
        }
    }
}
