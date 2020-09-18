/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using System;
using GameSparks.Core;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public partial interface IBackendService
    {
        IServerClock serverClock { get; }
        string uploadUrl { get; set; }

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
        void ScheduleSwitchOffResumeGS();

        IPromise<BackendResult> GetInitData(int appVersion, string dataJson);
        IPromise<BackendResult> AuthFacebook(string accessToken, bool existingPlayer);
        IPromise<BackendResult> AuthSignInWithApple(string authorizationCode, bool existingPlayer);
        IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionID, string purchaseReceipt, long expiryTimeStamp, string subscriptionType);
        IPromise<BackendResult> ConsumeVirtualGood(GSRequestData jsonData);
        IPromise<BackendResult> ClaimReward(GSRequestData jsonData);
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId, string json = null);
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
        IPromise<BackendResult> SetLastWatchedVideo(string videoId);
        IPromise<BackendResult> VirtualGoodsTransaction(GSRequestData jsonData);
        IPromise<BackendResult> UpdatePlayerData();

        IPromise<BackendResult> FriendsOpBlock(string friendId); // block a friend
        IPromise<BackendResult> FriendsOpFriends(); // get backend saved friends
        IPromise<BackendResult> FriendsOpRefresh(); // get backend fresh facebook friends
        IPromise<BackendResult> FriendsOpCommunity(); // get community suggested friends list
        IPromise<BackendResult> FriendsOpRegCommunity(); // registers player into community searches
        IPromise<BackendResult> FriendsOpAdd(string friendId); // get community suggested friends list
        IPromise<BackendResult> FriendsOpAddFavourite(string friendId); // get community suggested friends list
        IPromise<BackendResult> FriendsOpInitialize(); // initial setup after first facebook login
        IPromise<BackendResult> FriendsOpRemove(string friendId, string opJson = null); // remove community friend
        IPromise<BackendResult> FriendsOpSearch(string matchString, int skip); // search players
        IPromise<BackendResult> FriendsOpStatus(string friendId); // retrieve status of friends
        IPromise<BackendResult> FriendsOpUnblock(string friendId); // unblock a friend
        IPromise<BackendResult> FriendsOpBlocked(string friendId); // get blocked friends

        IPromise<BackendResult> GetUploadUrl();
        IPromise<BackendResult> GetDownloadUrl(string id, Action<object> onSuccessExternal);
        //IPromise<BackendResult, Sprite, string> GetProfilePicture(string url, string playerId);
        IPromise<BackendResult> UploadProfilePic(string filename, byte[] stream, string mimeType);

        IPromise<BackendResult> TournamentsOpJoin(string tournamentShortCode, int score);
        IPromise<BackendResult> TournamentsOpGetJoinedTournaments();
        IPromise<BackendResult> TournamentsOpGetLiveTournaments();
        IPromise<BackendResult> TournamentsOpGetAllTournaments();
        IPromise<BackendResult> TournamentsOpGetLeaderboard(string tournamentId, bool update = true);
        IPromise<BackendResult> TournamentsOpGetLiveRewards(string tournamentShortCode);
        IPromise<BackendResult> TournamentsOpUpdateTournaments(string endedTournamentId = null);

        IPromise<BackendResult> InBoxOpGet();
        IPromise<BackendResult> InBoxOpCollect(string messageId);
        IPromise<BackendResult> GetDownloadableContentUrl(string shortCode, Action<object> onSuccessExternal);

    }
}
