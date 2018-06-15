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

        void StartPinger();
        void MonitorConnectivity();

        IPromise<BackendResult> GetInitData(int clientVersion);
        IPromise<BackendResult> AuthFacebook(string accessToken);

		IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId, string activeAvatarsId, string activeAvatarsBorderId);
        IPromise<BackendResult> SetPlayerSocialName(string name);
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult, string> GooglePlayBuyGoods(string transactionID, string currencyCode, string signature, string signedData, int subUnitPrice);
        IPromise<BackendResult> ClaimReward(string rewardType);

		/*

        IPromise<BackendResult> TestRequest(string eventKey);

        IPromise<BackendResult> FindMatch(string groupId);
        IPromise<BackendResult> GetGameStartTime(string challengeId);
        IPromise<BackendResult> ConsumeVirtualGood(int quantity, string shortCode);


*/
	}
}
