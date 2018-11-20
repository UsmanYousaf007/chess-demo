/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using System;
using GameSparks.Api.Requests;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> SendChatMessage(string recipientId, string text, string guid)
        {
            return new GSSendChatMessageRequest().Send(recipientId, text, guid);
        }
    }


    #region REQUEST

    public class GSSendChatMessageRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "SendChatMessage";
        const string ATT_RECIPIENT_ID = "recipientId";
        const string ATT_TEXT = "text";
        const string ATT_GUID = "guid";

        public IPromise<BackendResult> Send(string recipientId, string text, string guid)
        {
            this.errorCode = BackendResult.SEND_CHAT_MESSAGE_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_RECIPIENT_ID, recipientId)
                .SetEventAttribute(ATT_TEXT, text)
                .SetEventAttribute(ATT_GUID, guid)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}
