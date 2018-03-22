/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-12 06:55:00 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;

using TurboLabz.Patches;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        // Models
        [Inject] public IPromotionsModel promotionsModel { get; set; }

        private void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;

            AddGameMessageListeners();
        }

        private void RemoveMessageListeners()
        {
            RemoveGameMessageListeners();

            ChallengeDrawnMessage.Listener -= OnChallengeDrawnMessage;
            ChallengeLostMessage.Listener -= OnChallengeLostMessage;
            ChallengeWonMessage.Listener -= OnChallengeWonMessage;
            ScriptMessage.Listener -= OnScriptMessage;
        }

        private void OnScriptMessage(ScriptMessage message)
        {
            // Ignore the message if it is recurring.
            if (GSRecurringMessagePatch.isMessageRecurring(message, "GSService_Messages"))
            {
                return;
            }

            if (message.ExtCode == GSBackendKeys.OPPONENT_DISCONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                OnOpponentDisconnect();
            }
            else if (message.ExtCode == GSBackendKeys.OPPONENT_RECONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                OnOpponentReconnect();
            }
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            // Ignore the message if it is recurring.
            if (GSRecurringMessagePatch.isMessageRecurring(message, "GSService_Messages"))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            // Ignore the message if it is recurring.
            if (GSRecurringMessagePatch.isMessageRecurring(message, "GSService_Messages"))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
            // Ignore the message if it is recurring.
            if (GSRecurringMessagePatch.isMessageRecurring(message, "GSService_Messages"))
            {
                return;
            }

            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void EndGame(GSData data, string winnerId)
        {
            // Reset models first.
            promotionsModel.Reset();

            // Update player account details on game end.
            GSData accountDetailsData = data.GetGSData(GSBackendKeys.ACCOUNT_DETAILS);
            AccountDetailsResponse accountDetailsResponse = new AccountDetailsResponse(accountDetailsData);

            // Call the same method as for successful retrieval of account
            // details since we process the account details data in exactly the
            // same manner.
            OnAccountDetailsSuccess(accountDetailsResponse);

            // Level promotions.
            IList<GSData> levelPromotionsData = data.GetGSDataList(GSBackendKeys.LevelPromotions.KEY);

            if (levelPromotionsData != null)
            {
                IList<LevelPromotion> levelPromotions = new List<LevelPromotion>();

                foreach (GSData e in levelPromotionsData)
                {
                    LevelPromotion levelPromotion;
                    levelPromotion.from = e.GetInt(GSBackendKeys.LevelPromotions.FROM).Value;
                    levelPromotion.to = e.GetInt(GSBackendKeys.LevelPromotions.TO).Value;

                    GSData rewardData = e.GetGSData(GSBackendKeys.LevelPromotions.REWARD);

                    LevelPromotionReward levelPromotionReward;
                    levelPromotionReward.currency2 = rewardData.GetLong(GSBackendKeys.LevelPromotions.CURRENCY_2).Value;

                    levelPromotion.reward = levelPromotionReward;

                    GSData nextLeaguePromotionData = e.GetGSData(GSBackendKeys.LevelPromotions.NEXT_LEAGUE_PROMOTION);

                    NextLeaguePromotion nextLeaguePromotion = null;

                    if (nextLeaguePromotionData != null)
                    {
                        nextLeaguePromotion = new NextLeaguePromotion();
                        nextLeaguePromotion.id = nextLeaguePromotionData.GetString(GSBackendKeys.LevelPromotions.NEXT_LEAGUE_PROMOTION_ID);
                        nextLeaguePromotion.startLevel = nextLeaguePromotionData.GetInt(GSBackendKeys.LevelPromotions.NEXT_LEAGUE_PROMOTION_START_LEVEL).Value;
                    }

                    levelPromotion.nextLeaguePromotion = nextLeaguePromotion;

                    levelPromotions.Add(levelPromotion);
                }

                promotionsModel.levelPromotions = levelPromotions;
            }

            // League promotion.
            GSData leaguePromotionData = data.GetGSData(GSBackendKeys.LeaguePromotion.KEY);

            if (leaguePromotionData != null)
            {
                LeaguePromotion leaguePromotion = new LeaguePromotion();
                leaguePromotion.from = leaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.FROM);
                leaguePromotion.to = leaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.TO);

                GSData nextLeaguePromotionData = leaguePromotionData.GetGSData(GSBackendKeys.LeaguePromotion.NEXT_LEAGUE_PROMOTION);

                NextLeaguePromotion nextLeaguePromotion = null;

                if (nextLeaguePromotionData != null)
                {
                    nextLeaguePromotion = new NextLeaguePromotion();
                    nextLeaguePromotion.id = nextLeaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.NEXT_LEAGUE_PROMOTION_ID);
                    nextLeaguePromotion.startLevel = nextLeaguePromotionData.GetInt(GSBackendKeys.LeaguePromotion.NEXT_LEAGUE_PROMOTION_START_LEVEL).Value;
                }

                leaguePromotion.nextLeaguePromotion = nextLeaguePromotion;

                promotionsModel.leaguePromotion = leaguePromotion;
            }

            // Trophy promotion.
            GSData trophyPromotionData = data.GetGSData(GSBackendKeys.TrophyPromotion.KEY);

            if (trophyPromotionData != null)
            {
                TrophyPromotion trophyPromotion = new TrophyPromotion();
                trophyPromotion.from = trophyPromotionData.GetInt(GSBackendKeys.TrophyPromotion.FROM).Value;
                trophyPromotion.to = trophyPromotionData.GetInt(GSBackendKeys.TrophyPromotion.TO).Value;

                promotionsModel.trophyPromotion = trophyPromotion;
            }

            // Room title promotion.
            GSData roomTitlePromotionData = data.GetGSData(GSBackendKeys.RoomTitlePromotion.KEY);

            if (roomTitlePromotionData != null)
            {
                RoomTitlePromotion roomTitlePromotion = new RoomTitlePromotion();
                roomTitlePromotion.from = roomTitlePromotionData.GetString(GSBackendKeys.RoomTitlePromotion.FROM);
                roomTitlePromotion.to = roomTitlePromotionData.GetString(GSBackendKeys.RoomTitlePromotion.TO);

                GSData rewardData = roomTitlePromotionData.GetGSData(GSBackendKeys.RoomTitlePromotion.REWARD);

                RoomTitlePromotionReward roomTitlePromotionReward;
                roomTitlePromotionReward.currency2 = rewardData.GetLong(GSBackendKeys.RoomTitlePromotion.CURRENCY_2).Value;

                roomTitlePromotion.reward = roomTitlePromotionReward;

                promotionsModel.roomTitlePromotion = roomTitlePromotion;
            }
        }
    }
}
