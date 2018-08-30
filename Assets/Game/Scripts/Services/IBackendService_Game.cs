/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-17 08:48:24 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public partial interface IBackendService
    {
        IPromise<BackendResult> PlayerTurn(FileRank from,
            FileRank to,
            string promotion,
            bool claimFiftyMoveDraw,
            bool claimThreefoldRepeatDraw,
            bool rejectThreefoldRepeatDraw);
        IPromise<BackendResult> AiTurn(FileRank from,
            FileRank to,
            string promotion);
        IPromise<BackendResult> AiResign();
        IPromise<BackendResult> ClaimFiftyMoveDraw();
        IPromise<BackendResult> ClaimThreefoldRepeatDraw();
        IPromise<BackendResult> PlayerResign();
        IPromise<BackendResult> Decline();
    }
}
