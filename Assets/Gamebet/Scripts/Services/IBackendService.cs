/// @license Propriety <http://license.url>
/// @copyright Copyright (C) DefaultCompany 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-10 11:52:44 UTC+05:00
/// 
/// @description
/// All interactions with the game server will happen here

using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial interface IBackendService
    {
        ConnectionState connectionState { get; }
        bool isAuthenticated { get; }
        IServerClock serverClock { get; }

        IPromise<BackendResult> Connect();
        void Disconnect();
        IPromise<BackendResult> AuthGuest();
        IPromise<BackendResult> AuthFacebook();
        IPromise<BackendResult> TestRequest(string eventKey);
        IPromise<BackendResult> CheckGameVersion();
        IPromise<BackendResult> GetInitData();
        IPromise<BackendResult> GetAccountDetails();
        IPromise<BackendResult> FindMatch(string groupId);
        IPromise<BackendResult> GetGameStartTime(string challengeId);
        IPromise<BackendResult> SetPlayerSocialName(string name);
        IPromise<BackendResult> ClaimAdReward();
        IPromise<BackendResult> BuyVirtualGoods(int currencyType, int quantity, string shortCode);
        IPromise<BackendResult> ConsumeVirtualGood(int quantity, string shortCode);
        IPromise<BackendResult> GooglePlayBuyVirtualGoods(string currencyCode, string signature, string signedData, int subUnitPrice);
        IPromise<BackendResult> IOSBuyVirtualGoods(string currencyCode, string receipt, bool sandbox, int subUnitPrice);
        IPromise<BackendResult> GrantForgedItem(string consumeVGoodShortCode);
        IPromise<BackendResult> SellForgeCards(string consumeVGoodShortCode, int consumeVGoodQuantity);
        IPromise<BackendResult> ClaimLoot(string key);
        IPromise<BackendResult> UpdateActiveInventory(string activeChessSkinsId, string activeAvatarsId, string activeAvatarsBorderId);
    }
}
