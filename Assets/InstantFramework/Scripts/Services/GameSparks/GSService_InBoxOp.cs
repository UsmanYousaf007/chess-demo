/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System.Collections.Generic;
using strange.extensions.promise.api;
using SimpleJson2;


namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }
        [Inject] public InboxRemoveMessagesSignal inboxRemoveMessagesSignal { get; set; }
        [Inject] public UpdateInboxMessageCountViewSignal updateInboxMessageCountViewSignal { get; set; }

        public IPromise<BackendResult> InBoxOpGet()
        {
            return new GSInBoxOpRequest(GetRequestContext()).Send("get", OnInBoxOpSuccess);
        }

        public IPromise<BackendResult> InBoxOpCollect(string messageId)
        {
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("messageId", messageId);

            return new GSInBoxOpRequest(GetRequestContext()).Send("collect", OnInBoxOpSuccess, jsonObj.ToString());
        }

        private void OnInBoxOpSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData == null)
            {
                return;
            }

            GSData error = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.ERROR);
            if (error != null)
            {
                // We can dispatch error signal here
                return;
            }

            GSData inBoxMessagesData = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.GET);
            if (inBoxMessagesData != null)
            {
                FillInbox(inboxModel.items, inBoxMessagesData);
                inboxAddMessagesSignal.Dispatch(inboxModel.items);
                updateInboxMessageCountViewSignal.Dispatch(inboxModel.inboxMessageCount);
                inboxModel.lastFetchedTime = DateTime.UtcNow;
            }

            GSData inBoxCollectData = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.COLLECT);
            if (inBoxCollectData != null)
            {
                string messageId = response.ScriptData.GetString("messageId");

                TLUtils.LogUtil.Log("+++++====> GSBackendKeys.InBoxOp.COLLECT");
                //int gems = inBoxCollectData.GetInt("gems").Value;
                //TLUtils.LogUtil.Log("+++++====> gems = " + gems);

                //GSData items = inBoxCollectData.GetGSData("items");
                foreach (KeyValuePair<string, Object> obj in inBoxCollectData.BaseData)
                {
                    string itemShortCode = obj.Key;
                    var qtyVar = obj.Value;
                    int qtyInt = Int32.Parse(qtyVar.ToString());
                    TLUtils.LogUtil.Log("+++++====>" + itemShortCode + " qty: " + qtyInt.ToString());
                }

                if (messageId != null)
                {
                    inboxModel.items.Remove(messageId);
                    inboxRemoveMessagesSignal.Dispatch(messageId);
                    //inboxAddMessagesSignal.Dispatch(inboxModel.items);
                }
                
            }
        }
    }

    #region REQUEST

    public class GSInBoxOpRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "InboxOp";
        const string ATT_OP = "op";
        const string ATT_JSON = "opJson";

        public GSInBoxOpRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string op, Action<object, Action<object>> onSuccess, string opJson = null)
        {
            this.errorCode = BackendResult.INBOX_OP_FAILED;
            this.onSuccess = onSuccess;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_OP, op)
                .SetEventAttribute(ATT_JSON, opJson)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

