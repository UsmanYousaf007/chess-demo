/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using GameSparks.Api.Requests;
using System;
using GameSparks.Core;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> PlayerOfferDraw(string challengeId) { return new GSPlayerOfferDrawOpRequest(GetRequestContext()).Send("offered", challengeId); }//, OnOfferDrawOpSuccess); }
        public IPromise<BackendResult> PlayerOfferDrawRejected(string challengeId) { return new GSPlayerOfferDrawOpRequest(GetRequestContext()).Send("rejected", challengeId); }//, OnOfferDrawOpSuccess); }
        public IPromise<BackendResult> PlayerOfferDrawAccepted(string challengeId) { return new GSPlayerOfferDrawOpRequest(GetRequestContext()).Send("accepted", challengeId); }//, OnOfferDrawOpSuccess); }

        private void OnOfferDrawOpSuccess(object r)
        {

        }

        #region REQUEST

        public class GSPlayerOfferDrawOpRequest : GSFrameworkRequest
        {
            const string SHORT_CODE = "OfferDrawOp";
            //const string ATT_OPPONENT_ID = "opponentId";
            const string ATT_CHALLENGE_ID = "challengeId";
            const string ATT_OP = "op";

            public GSPlayerOfferDrawOpRequest(GSFrameworkRequestContext context) : base(context) { }

            public IPromise<BackendResult> Send(string op, string challengeId)
            {
                this.errorCode = BackendResult.OFFER_DRAW_OP_FAILED;
                //this.onSuccess = onSuccess; Action<object> onSuccess

                new LogEventRequest().SetEventKey(SHORT_CODE)
                    .SetEventAttribute(ATT_OP, op)
                    .SetEventAttribute(ATT_CHALLENGE_ID, challengeId)
                    .Send(OnRequestSuccess, OnRequestFailure);

                return promise;
            }
        }

        #endregion
    }
}