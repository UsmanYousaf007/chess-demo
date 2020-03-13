/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using System;
using GameSparks.Core;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial interface IBackendService
    {
        IServerClock serverClock { get; }

        void AddChallengeListeners();
        void AddMessageListeners();
        void AddChatMessageListener();
        void StartPinger();
        void StopPinger();
        void MonitorConnectivity(bool enable);
        void OnlineCheckerStart();
        void OnlineCheckerStop();
        void OnlineCheckerAddListener(Action<bool, ConnectionSwitchType> listener);
        void OnlineCheckerRemoveListener(Action<bool, ConnectionSwitchType> listener);

        IPromise<BackendResult> GetInitData(int appVersion, string dataJson);
        IPromise<BackendResult> AuthFacebook(string accessToken, bool existingPlayer);
        IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionID, string purchaseReceipt, long expiryTimeStamp);
        IPromise<BackendResult> ConsumeVirtualGood(GSRequestData jsonData);
        IPromise<BackendResult> ClaimReward(GSRequestData jsonData);
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId);
        IPromise<BackendResult> SetPlayerSocialName(string name);
        IPromise<BackendResult> FindMatch(string opponentId);
        IPromise<BackendResult> CreateLongMatch(string opponentId, bool isRanked);
        IPromise<BackendResult> GetGameStartTime(string challengeId);
        IPromise<BackendResult> PushNotificationRegistration(string token);
        IPromise<BackendResult> Accept(string challengeId);
        IPromise<BackendResult> Decline(string challengeId);
        IPromise<BackendResult> Unregister(string challengeId);
        IPromise<BackendResult> SendChatMessage(string recipientId, string text, string guid);
        IPromise<BackendResult> ChangeUserDetails(string name);
        IPromise<BackendResult> MatchWatchdogPingAck(string currentTurnPlayerId, string challengerId, string challengedId, string challengeId, int moveCount);
        IPromise<BackendResult> SyncReconnectData(string challengeId);

        IPromise<BackendResult> UpdatePlayerData();

        IPromise<BackendResult> FriendsOpBlock(string friendId); // block a friend
        IPromise<BackendResult> FriendsOpFriends(); // get backend saved friends
        IPromise<BackendResult> FriendsOpRefresh(); // get backend fresh facebook friends
        IPromise<BackendResult> FriendsOpCommunity(); // get community suggested friends list
        IPromise<BackendResult> FriendsOpRegCommunity(); // registers player into community searches
        IPromise<BackendResult> FriendsOpAdd(string friendId); // get community suggested friends list
        IPromise<BackendResult> FriendsOpAddFavourite(string friendId); // get community suggested friends list
        IPromise<BackendResult> FriendsOpInitialize(); // initial setup after first facebook login
        IPromise<BackendResult> FriendsOpRemove(string friendId); // remove community friend
        IPromise<BackendResult> FriendsOpSearch(string matchString, int skip); // search players
        IPromise<BackendResult> FriendsOpStatus(string friendId); // retrieve status of friends
    }
}
