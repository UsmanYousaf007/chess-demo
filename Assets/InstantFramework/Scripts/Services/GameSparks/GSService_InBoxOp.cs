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
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }
        [Inject] public InboxRemoveMessagesSignal inboxRemoveMessagesSignal { get; set; }
        [Inject] public UpdateInboxMessageCountViewSignal updateInboxMessageCountViewSignal { get; set; }
        [Inject] public InboxFetchingMessagesSignal inboxFetchingMessagesSignal { get; set; }
        [Inject] public InboxEmptySignal inboxEmptySignal { get; set; }

        public IPromise<BackendResult> InBoxOpGet()
        {
            inboxFetchingMessagesSignal.Dispatch(true);
            return new GSInBoxOpRequest(GetRequestContext()).Send("get", OnInBoxOpSuccess, OnInboxOpFailure);
        }

        public IPromise<BackendResult> InBoxOpCollect(string messageId)
        {
            inboxFetchingMessagesSignal.Dispatch(true);

            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("messageId", messageId);

            return new GSInBoxOpRequest(GetRequestContext()).Send("collect", OnInBoxOpSuccess, OnInboxOpFailure, jsonObj.ToString());
        }


        public List<InboxMessage> CheckForNewInboxMessages (Dictionary<string, InboxMessage> dict)
        {
            List<InboxMessage> newMsgs = new List<InboxMessage>();

            foreach (KeyValuePair<string, InboxMessage> item in dict)
            {
                if (inboxModel.items.ContainsKey(item.Key) == false)
                {
                    newMsgs.Add(item.Value);
                }
            }

            return newMsgs;
        }

        public void DispatchInboxNotifications(List<InboxMessage> inboxMsgIds)
        {
            for (int i = 0; i < inboxMsgIds.Count; i++)
            {
                InboxMessage msg = inboxMsgIds[i];
                if (msg.type == "RewardTournamentEnd")
                {
                    NotificationVO notificationVO;

                    notificationVO.isOpened = false;
                    notificationVO.title = localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_TITLE);
                    notificationVO.body = localizationService.Get(LocalizationKey.NOTIFICATION_TOURNAMENT_END_BODY);
                    notificationVO.senderPlayerId = msg.tournamentType;
                    notificationVO.challengeId = "undefined";
                    notificationVO.matchGroup = "undefined";
                    notificationVO.avatarId = "undefined";
                    notificationVO.avaterBgColorId = "undefined";
                    notificationVO.profilePicURL = "undefined";
                    notificationVO.isPremium = false;
                    notificationVO.timeSent = 0;
                    notificationVO.actionCode = "undefined";
                    notificationVO.league = -1;

                    notificationRecievedSignal.Dispatch(notificationVO);
                }
            }
        }

        private void OnInBoxOpSuccess(object r, Action<object> a)
        {
            LogEventResponse response = (LogEventResponse)r;

            if (response.ScriptData == null)
            {
                inboxFetchingMessagesSignal.Dispatch(false);
                return;
            }

            GSData error = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.ERROR);
            if (error != null)
            {
                inboxFetchingMessagesSignal.Dispatch(false);
                // We can dispatch error signal here
                return;
            }

            GSData inBoxCollectData = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.COLLECT);
            if (inBoxCollectData != null)
            {
                string messageId = response.ScriptData.GetString("messageId");

                TLUtils.LogUtil.Log("+++++====> GSBackendKeys.InBoxOp.COLLECT");

                foreach (KeyValuePair<string, Object> obj in inBoxCollectData.BaseData)
                {
                    string itemShortCode = obj.Key;
                    var qtyVar = obj.Value;
                    int qtyInt = Int32.Parse(qtyVar.ToString());
                    TLUtils.LogUtil.Log("+++++====>" + itemShortCode + " qty: " + qtyInt.ToString());

                    if (qtyInt > 0)
                    {
                        if (itemShortCode.Equals(GSBackendKeys.PlayerDetails.GEMS))
                        {
                            playerModel.gems += qtyInt;
                        }
                        else if (itemShortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
                        {
                            playerModel.coins += qtyInt;
                        }
                        else if (itemShortCode.Equals(GSBackendKeys.PlayerDetails.TROPHIES2))
                        {
                            playerModel.trophies2 += qtyInt;
                        }
                        else if (playerModel.inventory.ContainsKey(itemShortCode))
                        {
                            playerModel.inventory[itemShortCode] += qtyInt;
                        }
                        else
                        {
                            playerModel.inventory.Add(itemShortCode, qtyInt);
                        }

                        //Analytics
                        var leagueName = leaguesModel.GetCurrentLeagueInfo().name.Replace(" ", "_").Replace(".", string.Empty).ToLower();
                        var item = inboxModel.items[messageId];
                        var itemType = CollectionsUtil.GetContextFromState(item.type);
                        var itemId = string.Empty;
                        
                        switch (item.type)
                        {
                            case "RewardTournamentEnd":
                                itemId = $"rank{item.rankCount}_{leagueName}";
                                break;

                            case "RewardDailyLeague":
                                itemId = itemShortCode.Equals(GSBackendKeys.PlayerDetails.GEMS) ? $"{leagueName}_gems" : GSBackendKeys.PlayerDetails.COINS;
                                break;

                            case "RewardLeaguePromotion":
                                itemId = item.league.ToLower().Replace(" ", "_").Replace(".", string.Empty);
                                break;
                        }

                        analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, CollectionsUtil.GetContextFromString(itemShortCode).ToString(), qtyInt, itemType, itemId);
                        //Analyttics end
                    }
                }

                int updatedTrophies = GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.PlayerDetails.TROPHIES);
                if (updatedTrophies > playerModel.trophies)
                {
                    // Handle increase in trophies here
                }

                int promotedLeague = GSParser.GetSafeInt(response.ScriptData, GSBackendKeys.PlayerDetails.LEAGUE);
                if (promotedLeague > playerModel.league)
                {
                    // Handle league promotion here
                }

                if (messageId != null)
                {
                    inboxModel.items.Remove(messageId);
                    inboxRemoveMessagesSignal.Dispatch(messageId);
                }

                updatePlayerInventorySignal.Dispatch(playerModel.GetPlayerInventory());
            }

            GSData inBoxMessagesData = response.ScriptData.GetGSData(GSBackendKeys.InBoxOp.GET);
            if (inBoxMessagesData != null)
            {
                Dictionary<string, InboxMessage> dict = new Dictionary<string, InboxMessage>();
                FillInbox(dict, inBoxMessagesData);
                inboxModel.lastFetchedTime = DateTime.UtcNow;
                inboxModel.items = dict;
                inboxAddMessagesSignal.Dispatch();
            }
            else
            {
                inboxEmptySignal.Dispatch();
            }

            if (response.ScriptData.ContainsKey("count"))
            {
                int inboxMessageCount = response.ScriptData.GetInt("count").Value;
                inboxModel.inboxMessageCount = inboxMessageCount;
                updateInboxMessageCountViewSignal.Dispatch(inboxModel.inboxMessageCount);
            }

            inboxFetchingMessagesSignal.Dispatch(false);
        }

        private void OnInboxOpFailure(object r)
        {
            inboxFetchingMessagesSignal.Dispatch(false);
        }
    }

    #region REQUEST

    public class GSInBoxOpRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "InboxOp";
        const string ATT_OP = "op";
        const string ATT_JSON = "opJson";

        public GSInBoxOpRequest(GSFrameworkRequestContext context) : base(context) { }

        public IPromise<BackendResult> Send(string op, Action<object, Action<object>> onSuccess, Action<object> onFailure, string opJson = null)
        {
            this.errorCode = BackendResult.INBOX_OP_FAILED;
            this.onSuccess = onSuccess;
            this.onFailure = onFailure;

            new LogEventRequest().SetEventKey(SHORT_CODE)
                .SetEventAttribute(ATT_OP, op)
                .SetEventAttribute(ATT_JSON, opJson)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }
    }

    #endregion
}

