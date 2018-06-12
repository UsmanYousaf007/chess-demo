/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public IPromise<BackendResult> GetAccountDetails()
        {
            return new GSAccountDetailsRequest().Send(OnAccountDetailsSuccess);
        }

        private void OnAccountDetailsSuccess(AccountDetailsResponse response)
        {
            GSData externalIds = response.ExternalIds;
            IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);

            playerModel.id = response.UserId;
            playerModel.tag = response.ScriptData.GetString(GSBackendKeys.TAG);
            playerModel.name = response.DisplayName;
            playerModel.countryId = response.Location.Country;

            playerModel.bucks = response.Currency2.Value;
            playerModel.isSocialNameSet = response.ScriptData.GetBoolean(GSBackendKeys.IS_SOCIAL_NAME_SET).Value;
            playerModel.externalAuthentications = externalAuthentications;
            playerModel.eloScore = response.ScriptData.GetInt(GSBackendKeys.ELO_SCORE).Value;
            playerModel.adLifetimeImpressions = response.ScriptData.GetInt(GSBackendKeys.AD_LIFETIME_IMPRESSIONS).Value;

            playerModel.totalGamesWon = response.ScriptData.GetInt(GSBackendKeys.GAMES_WON).Value;
            playerModel.totalGamesLost = response.ScriptData.GetInt(GSBackendKeys.GAMES_LOST).Value;
            playerModel.totalGamesDrawn = response.ScriptData.GetInt(GSBackendKeys.GAMES_DRAWN).Value;
            playerModel.totalGamesAbandoned = response.ScriptData.GetInt(GSBackendKeys.GAMES_ABANDONED).Value;
            playerModel.totalGamesPlayed = response.ScriptData.GetInt(GSBackendKeys.GAMES_PLAYED).Value;

            // Populate inventory data
            IList<GSData> playerActiveInventory = response.ScriptData.GetGSDataList(GSBackendKeys.PLAYER_ACTIVE_INVENTORY);
            IOrderedDictionary<string, int> inventory = new OrderedDictionary<string, int>(); 
            GSParser.PopulateInventory(inventory, response.VirtualGoods);
            playerModel.inventory = inventory;
            GSParser.PopulateActiveInventory(playerModel, playerActiveInventory);

            GSParser.LogPlayerInfo(playerModel);
        }
    }
}
