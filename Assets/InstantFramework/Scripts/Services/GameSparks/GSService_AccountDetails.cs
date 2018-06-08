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
        // Models
        [Inject] public ILevelSettingsModel levelSettingsModel { get; set; }

        public IPromise<BackendResult> GetAccountDetails()
        {
            return new GSAccountDetailsRequest().Send(OnAccountDetailsSuccess);
        }

        private void OnAccountDetailsSuccess(AccountDetailsResponse response)
        {
            GSData records = response.ScriptData.GetGSData(GSBackendKeys.ROOM_RECORDS);
            IDictionary<string, RoomRecord> roomRecords = GSParser.ParseRoomRecords(records, roomSettingsModel.settings);

            GSData externalIds = response.ExternalIds;
            IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);

            playerModel.id = response.UserId;
            playerModel.tag = response.ScriptData.GetString(GSBackendKeys.TAG);
            playerModel.name = response.DisplayName;
            playerModel.countryId = response.Location.Country;

            playerModel.currency1 = response.Currency1.Value;
            playerModel.bucks = response.Currency2.Value;
            playerModel.currency1Winnings = response.ScriptData.GetLong(GSBackendKeys.CURRENCY_1_WINNINGS).Value;
            playerModel.xp = response.ScriptData.GetInt(GSBackendKeys.XP).Value;
            playerModel.level = response.ScriptData.GetInt(GSBackendKeys.LEVEL).Value;
            playerModel.leagueId = response.ScriptData.GetString(GSBackendKeys.LEAGUE_ID);
            playerModel.roomRecords = roomRecords;
            playerModel.isSocialNameSet = response.ScriptData.GetBoolean(GSBackendKeys.IS_SOCIAL_NAME_SET).Value;
            playerModel.externalAuthentications = externalAuthentications;
            playerModel.eloDivision = response.ScriptData.GetString(GSBackendKeys.ELO_DIVISION);
            playerModel.eloScore = response.ScriptData.GetInt(GSBackendKeys.ELO_SCORE).Value;
            playerModel.eloTotalPlacementGames = response.ScriptData.GetInt(GSBackendKeys.ELO_TOTAL_PLACEMENT_GAMES).Value;
            playerModel.eloCompletedPlacementGames = response.ScriptData.GetInt(GSBackendKeys.ELO_COMPLETED_PLACEMENT_GAMES).Value;
            playerModel.league = response.ScriptData.GetString(GSBackendKeys.LEAGUE);
            playerModel.nextMedalAt = response.ScriptData.GetInt(GSBackendKeys.NEXT_MEDAL_AT).Value;
            playerModel.medals = response.ScriptData.GetInt(GSBackendKeys.MEDALS).Value;
            playerModel.adLifetimeImpressions = response.ScriptData.GetInt(GSBackendKeys.AD_LIFETIME_IMPRESSIONS).Value;

            // Player level data
            LevelInfo currentLevelInfo;
            GSData levelInfo = response.ScriptData.GetGSData(GSBackendKeys.LEVEL_INFO);
            currentLevelInfo.level = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_LEVEL).Value;
            currentLevelInfo.startXp = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_START_XP).Value;
            currentLevelInfo.endXp = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_END_XP).Value;
            currentLevelInfo.levelPromotionRewardBucks = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_LEVEL_PROMOTION_REWARD_BUCKS).Value;
            levelSettingsModel.currentLevelInfo = currentLevelInfo;

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
