/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        public void AddMessageListeners()
        {
            ScriptMessage.Listener += OnScriptMessage;
            ChallengeWonMessage.Listener += OnChallengeWonMessage;
            ChallengeLostMessage.Listener += OnChallengeLostMessage;
            ChallengeDrawnMessage.Listener += OnChallengeDrawnMessage;
            SessionTerminatedMessage.Listener += OnSessionTerminateMessage;

            // TODO: Eventually move to a game specific module
            // AddGameMessageListeners();
        }

        private void RemoveMessageListeners()
        {
            // TODO: Eventually move to a game specific module
            //RemoveGameMessageListeners();

            ChallengeDrawnMessage.Listener -= OnChallengeDrawnMessage;
            ChallengeLostMessage.Listener -= OnChallengeLostMessage;
            ChallengeWonMessage.Listener -= OnChallengeWonMessage;
            ScriptMessage.Listener -= OnScriptMessage;
            SessionTerminatedMessage.Listener -= OnSessionTerminateMessage;
        }

        private void OnScriptMessage(ScriptMessage message)
        {
            if (message.ExtCode == GSBackendKeys.OPPONENT_DISCONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                // TODO: Eventually move to a game specific module or convert to signal
                // OnOpponentDisconnect();
            }
            else if (message.ExtCode == GSBackendKeys.OPPONENT_RECONNECTED_MESSAGE)
            {
                if (!IsCurrentChallenge(message.Data.GetString(GSBackendKeys.CHALLENGE_ID)))
                {
                    return;
                }

                // TODO: Eventually move to a game specific module or convert to signal
                // OnOpponentReconnect();

            }
        }

        private bool IsCurrentChallenge(string challengeId)
        {
            return (challengeId == matchInfoModel.challengeId);
        }

        private void OnChallengeWonMessage(ChallengeWonMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeLostMessage(ChallengeLostMessage message)
        {
            GSData scriptData = message.ScriptData;
            EndGame(scriptData, matchInfoModel.opponentPublicProfile.id);
        }

        private void OnChallengeDrawnMessage(ChallengeDrawnMessage message)
        {
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

            promotionsModel.awardMedal = data.GetBoolean(GSBackendKeys.MatchData.AWARD_MEDAL).Value;
            promotionsModel.prizeBucks = data.GetInt(GSBackendKeys.MatchData.PRIZE_BUCKS).Value;

            // Opponent public profile elo update
            PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

            opponentPublicProfile.eloScore = data.GetInt(GSBackendKeys.MatchData.OPPONENT_ELO_SCORE).Value;
            opponentPublicProfile.eloDivision = data.GetString(GSBackendKeys.MatchData.OPPONENT_ELO_DIVISION);
            opponentPublicProfile.eloCompletedPlacementGames = data.GetInt(GSBackendKeys.MatchData.OPPONENT_ELO_COMPLETED_PLACEMENT_GAMES).Value;

            matchInfoModel.opponentPublicProfile = opponentPublicProfile;

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

                    levelPromotions.Add(levelPromotion);
                }

                promotionsModel.levelPromotions = levelPromotions;
            }

            // League promotion.
            GSData leaguePromotionData = data.GetGSData(GSBackendKeys.LeaguePromotion.KEY);

            if (leaguePromotionData != null)
            {
                LeaguePromotion leaguePromotion = new LeaguePromotion();
                leaguePromotion.state = leaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.STATE);
                leaguePromotion.from = leaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.FROM);
                leaguePromotion.to = leaguePromotionData.GetString(GSBackendKeys.LeaguePromotion.TO);

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

        private void OnSessionTerminateMessage(SessionTerminatedMessage message)
        {
            // Session terminated because this user authenticated on another device
            backendErrorSignal.Dispatch(BackendResult.SESSION_TERMINATED_ON_MULTIPLE_AUTH);
        }
    }
}
