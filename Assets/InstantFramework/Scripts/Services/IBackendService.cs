/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using System;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial interface IBackendService
    {
        IServerClock serverClock { get; }

        void AddChallengeListeners();
        void AddMessageListeners();
        void StartPinger();
        void MonitorConnectivity();

        IPromise<BackendResult> GetInitData(int appVersion, string dataJson);
        IPromise<BackendResult> AuthFacebook(string accessToken, bool isSync = false);
        IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionID, string purchaseReceipt);
        IPromise<BackendResult> ClaimReward(string rewardType);
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId);
        IPromise<BackendResult> SetPlayerSocialName(string name);
        IPromise<BackendResult> FindMatch();
        IPromise<BackendResult> CreateLongMatch(string opponentId);
        IPromise<BackendResult> GetGameStartTime(string challengeId);
        IPromise<BackendResult> PushNotificationRegistration(string token);
        IPromise<BackendResult> Accept(string challengeId);
        IPromise<BackendResult> Decline(string challengeId);
        IPromise<BackendResult> Unregister(string challengeId);
		
        IPromise<BackendResult> FriendsOpBlock(string friendId); // block a friend
        IPromise<BackendResult> FriendsOpFriends(); // get backend saved friends
        IPromise<BackendResult> FriendsOpRefresh(); // get backend fresh facebook friends
        IPromise<BackendResult> FriendsOpCommunity(); // get community suggested friends list
        IPromise<BackendResult> FriendsOpRegCommunity(); // registers player into community searches
        IPromise<BackendResult> FriendsOpAdd(string friendId); // get community suggested friends list
        IPromise<BackendResult> FriendsOpInitialize(); // initial setup after first facebook login

	}
}
