/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public partial interface IBackendService
    {
        IServerClock serverClock { get; }

        void AddMessageListeners();
        void StartPinger();
        void MonitorConnectivity();

        IPromise<BackendResult> GetInitData(int appVersion, string dataJson);
        IPromise<BackendResult> AuthFacebook(string accessToken);
        IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult, string> VerifyRemoteStorePurchase(string remoteProductId, string transactionID, string purchaseReceipt);
        IPromise<BackendResult> ClaimReward(string rewardType);
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId);
        IPromise<BackendResult> SetPlayerSocialName(string name);
        IPromise<BackendResult> FindMatch();
        IPromise<BackendResult> GetGameStartTime(string challengeId);
		IPromise<BackendResult> FriendsOp (string op, string friendId);
	}
}
