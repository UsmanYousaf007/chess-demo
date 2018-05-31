


/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-14 11:36:45 UTC+05:00
/// 
/// @description
/// In the request classes for services the Send() method always returns a
/// promise with SomeResultType as a type parameter:
/// 
/// IPromise<BackendResult> Send()
/// IPromise<SocialServiceResult> Send()
/// 
/// We can return more data using more type parameters but if the returned type
/// is specific to the service itself then we need to shield the world outside
/// the service to not receive service specific type parameters. For that
/// purpose we use a callback as a parameter to the Send() method e.g.:
/// 
/// IPromise<BackendResult> Send(Action<SomeServiceSpecificType> callback)
/// 
/// instead of doing this
/// 
/// IPromise<BackendResult, SomeServiceSpecificType> Send()
/// 
/// However these would be valid:
/// 
/// IPromise<BackendResult, string> Send()
/// IPromise<BackendResult, SomeGenericType> Send()

using System;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace TurboLabz.InstantFramework
{
    public class GSUpdateActiveInventoryRequest : GSRequest
    {
        private IPromise<BackendResult> promise = new Promise<BackendResult>();
        private Action<LogEventResponse> successCallback;

        public IPromise<BackendResult> Send(
            string activeChessSkinsId,
            string activeAvatarsId,
            string activeAvatarsBorderId,
            Action<LogEventResponse> successCallback)
        {
            GSRequestSession.Instance.AddRequest(this);
            this.successCallback = successCallback;
            new LogEventRequest().SetEventKey(GSEventData.UpdateActiveInventory.EVT_KEY)
                .SetEventAttribute(GSEventData.UpdateActiveInventory.ATT_KEY_ACTIVE_CHESS_SKINS_ID, activeChessSkinsId)
                .SetEventAttribute(GSEventData.UpdateActiveInventory.ATT_KEY_ACTIVE_AVATARS_ID, activeAvatarsId)
                .SetEventAttribute(GSEventData.UpdateActiveInventory.ATT_KEY_ACTIVE_AVATARS_BORDER_ID, activeAvatarsBorderId)
                .Send(OnSuccess, OnFailure);

            return promise;
        }

        // This method is used only by GSRequestSession
        public override void Expire()
        {
            base.Expire();
            promise.Dispatch(BackendResult.EXPIRED_RESPONSE);
        }

        private void OnSuccess(LogEventResponse response)
        {
            if (!isExpired)
            {
                successCallback(response);
                DispatchResponse(BackendResult.SUCCESS); 
            }
        }

        private void OnFailure(LogEventResponse response)
        {
            if (!isExpired)
            {
                DispatchResponse(BackendResult.UPDATE_ACTIVE_INVENTORY_FAILED);
            }
        }

        private void DispatchResponse(BackendResult result)
        {  
            promise.Dispatch(result);
            GSRequestSession.Instance.RemoveRequest(this);
        }
    }
}

