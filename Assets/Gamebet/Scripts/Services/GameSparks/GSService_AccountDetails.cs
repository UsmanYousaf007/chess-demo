/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 14:56:56 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
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

            Assertions.Assert(response.Location != null, "The location object for player must not be null!");
            playerModel.countryId = response.Location.Country;

            playerModel.currency1 = response.Currency1.Value;
            playerModel.currency2 = response.Currency2.Value;
            playerModel.currency1Winnings = response.ScriptData.GetLong(GSBackendKeys.CURRENCY_1_WINNINGS).Value;
            playerModel.xp = response.ScriptData.GetInt(GSBackendKeys.XP).Value;
            playerModel.level = response.ScriptData.GetInt(GSBackendKeys.LEVEL).Value;
            playerModel.leagueId = response.ScriptData.GetString(GSBackendKeys.LEAGUE_ID);
            playerModel.roomRecords = roomRecords;
            playerModel.isSocialNameSet = response.ScriptData.GetBoolean(GSBackendKeys.IS_SOCIAL_NAME_SET).Value;
            playerModel.externalAuthentications = externalAuthentications;

            GSData levelInfo = response.ScriptData.GetGSData(GSBackendKeys.LEVEL_INFO);

            LevelInfo currentLevelInfo;
            currentLevelInfo.level = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_LEVEL).Value;
            currentLevelInfo.startXp = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_START_XP).Value;
            currentLevelInfo.endXp = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_END_XP).Value;
            currentLevelInfo.levelPromotionRewardBucks = levelInfo.GetInt(GSBackendKeys.LEVEL_INFO_LEVEL_PROMOTION_REWARD_BUCKS).Value;

            levelSettingsModel.currentLevelInfo = currentLevelInfo;
        }
    }
}
