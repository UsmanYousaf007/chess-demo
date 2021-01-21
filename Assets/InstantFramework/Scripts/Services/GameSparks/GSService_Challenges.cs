/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

using GameSparks.Api.Responses;
using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.TLUtils;
using System;
using GameSparks.Api.Requests;
using UnityEngine;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }
        [Inject] public UpdateEloScoresSignal updateEloScoresSignal { get; set; }
        [Inject] public UpdateOfferDrawSignal updateOfferDrawSignal { get; set; }


        // Called by get init data and facebook auth commands
        private void ParseActiveChallenges(GSData data)
        {
            GSData activeChallengesData = data.GetGSData(GSBackendKeys.Match.ACTIVE_CHALLENGES);
            if (activeChallengesData == null)
                return;

            Dictionary<string, object> activeChallenges = GSJson.From(activeChallengesData.JSON) as Dictionary<string, object>;

            foreach (KeyValuePair<string, object> entry in activeChallenges)
            {
                GSData challengeData = activeChallengesData.GetGSData(entry.Key);
                ParseChallengeData(entry.Key, challengeData);
            }
        }

        private void ParseChallengeDataOfferDraw(string challengeId, GSData challengeData, bool hasGameEnded = false)
        {

            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);
            GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);


            GSData offerDraw = gameData.GetGSData(GSBackendKeys.OFFER_DRAW);


            OfferDrawVO offerDrawVO = new OfferDrawVO();


            string challengedId = matchData.GetString(GSBackendKeys.Match.CHALLENGED_ID);
            string challengerId = matchData.GetString(GSBackendKeys.Match.CHALLENGER_ID);
            string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;

            offerDrawVO.status = offerDraw.GetString(GSBackendKeys.OFFER_DRAW_STATUS);
            offerDrawVO.offeredBy = offerDraw.GetString(GSBackendKeys.OFFER_DRAW_OFFERED_BY);
            offerDrawVO.opponentId = opponentId;
            offerDrawVO.challengeId = challengeId;

            matchInfoModel.matches[challengeId].drawOfferStatus = offerDrawVO.status;
            matchInfoModel.matches[challengeId].drawOfferedBy = offerDrawVO.offeredBy;

            if (offerDrawVO.status != null && matchInfoModel.activeChallengeId == offerDrawVO.challengeId)
            {
                updateOfferDrawSignal.Dispatch(offerDrawVO);
            }
        }


        private void ParseChallengeData(string challengeId, GSData challengeData, bool hasGameEnded = false)
        {
            if (challengeId != matchInfoModel.activeChallengeId)
            {
                foreach (string cid in matchInfoModel.unregisteredChallengeIds)
                {
                    if (cid == challengeId)
                        return;
                }
            }

            GSData matchData = challengeData.GetGSData(GSBackendKeys.ChallengeData.MATCH_DATA_KEY);
            GSData gameData = challengeData.GetGSData(GSBackendKeys.GAME_DATA);

            TLUtils.LogUtil.LogNullValidation(challengeId, "challengeId");

            bool isNewMatch = challengeId != null && !matchInfoModel.matches.ContainsKey(challengeId);

            if (isNewMatch)
            {
                ///////////////////////////////////////////////////////////////////////////////////////
                // Leave if this is a long match challenge by a blocked user
                string shortCode = matchData.GetString(GSBackendKeys.Match.SHORT_CODE);
                string challengedId = matchData.GetString(GSBackendKeys.Match.CHALLENGED_ID);
                string challengerId = matchData.GetString(GSBackendKeys.Match.CHALLENGER_ID);
                string opponentId = (playerModel.id == challengerId) ? challengedId : challengerId;

                TLUtils.LogUtil.LogNullValidation(opponentId, "opponentId");

                if (opponentId == null) return;

                if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE &&
                    playerModel.blocked.ContainsKey(opponentId))
                {
                    return;
                }

                SetupMatch(challengeId, matchData, gameData);
                SetupGame(challengeId, gameData);

                OfferDrawVO offerDrawVO = new OfferDrawVO();
                offerDrawVO.status = null;
                offerDrawVO.offeredBy = null;
                offerDrawVO.opponentId = opponentId;
                offerDrawVO.challengeId = challengeId;

                matchInfoModel.matches[challengeId].drawOfferStatus = offerDrawVO.status;
                matchInfoModel.matches[challengeId].drawOfferedBy = offerDrawVO.offeredBy;
                updateOfferDrawSignal.Dispatch(offerDrawVO);

                // Check and set if it's a tournament match
                GSData playerData = matchData.GetGSData(playerModel.id);
                if (playerData != null)
                {
                    string tournamentId = GSParser.GetSafeString(playerData, GSBackendKeys.Tournament.TOURNAMENT_ID, string.Empty);
                    if (string.IsNullOrEmpty(tournamentId) == false)
                    {
                        matchInfoModel.matches[challengeId].isTournamentMatch = true;
                    }

                    matchInfoModel.matches[challengeId].betValue = GSParser.GetSafeLong(playerData, GSBackendKeys.ChallengeData.BET_VALUE);
                    matchInfoModel.matches[challengeId].powerMode = GSParser.GetSafeBool(playerData, GSBackendKeys.ChallengeData.POWER_MODE);
                    matchInfoModel.matches[challengeId].freeHints = GSParser.GetSafeInt(playerData, GSBackendKeys.ChallengeData.FREE_HINTS);
                }

                preferencesModel.freeHint = FreePowerUpStatus.NOT_CONSUMED;
            }

            UpdateMatch(challengeId, matchData);
            UpdateGame(challengeId, gameData);

            // Update the bars
            if (matchInfoModel.matches.ContainsKey(challengeId) /*&& matchInfoModel.matches[challengeId].isLongPlay*/)
            {
                string opponentId = matchInfoModel.matches[challengeId].opponentPublicProfile.playerId;

                if (opponentId != null && playerModel.friends.ContainsKey(opponentId))
                {
                    updateFriendBarSignal.Dispatch(playerModel.friends[opponentId], opponentId);
                    sortFriendsSignal.Dispatch();
                }

                if (hasGameEnded)
                {
                    long elapsedTime = matchData.GetLong(GSBackendKeys.ChallengeData.GAME_END_TIME).Value - matchData.GetLong(GSBackendKeys.ChallengeData.GAME_START_TIME).Value;
                    matchInfoModel.matches[challengeId].gameDurationMs = elapsedTime;

                    // Parsing tournament match result
                    GSData playerData = matchData.GetGSData(playerModel.id);
                    if (playerData != null)
                    {
                        if (matchInfoModel.matches[challengeId].isTournamentMatch == true)
                        {
                            int score = GSParser.GetSafeInt(playerData, GSBackendKeys.Tournament.TOURNAMENT_MATCH_SCORE);
                            int winTimeBonus = GSParser.GetSafeInt(playerData, GSBackendKeys.Tournament.TOURNAMENT_MATCH_WIN_TIME_BONUS);
                            matchInfoModel.matches[challengeId].tournamentMatchScore = score;
                            matchInfoModel.matches[challengeId].tournamentMatchWinTimeBonus = winTimeBonus;
                        }
                    }
                }
            }
        }

        private void SetupMatch(string challengeId, GSData matchData, GSData gameData)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create new match
            MatchInfo matchInfo = matchInfoModel.CreateMatch(challengeId);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize fixed match data
            string shortCode = matchData.GetString(GSBackendKeys.Match.SHORT_CODE);
            var matchDuration = GSParser.GetSafeLong(matchData, GSBackendKeys.Match.DURATION);

            matchInfo.isLongPlay = (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE) ? true : false;
            if(matchDuration == 1 * 60 * 1000)
            {
                matchInfo.gameTimeMode = GameTimeMode.OneMin;
            }
            else if (matchDuration == 3 * 60 * 1000)
            {
                matchInfo.gameTimeMode = GameTimeMode.ThreeMin;
            }
            else if (matchDuration == 10 * 60 * 1000)
            {
                matchInfo.gameTimeMode = GameTimeMode.TenMin;
            }
            else if (matchDuration == 30 * 60 * 1000)
            {
                matchInfo.gameTimeMode = GameTimeMode.ThirtyMin;
            }

            //matchInfo.isTenMinGame = matchDuration == 10 * 60 * 1000;
            //matchInfo.isOneMinGame = matchDuration == 60 * 1000;
            //matchInfo.isThirtyMinGame = matchDuration == 30 * 60 * 1000;

            matchInfo.challengedId = matchData.GetString(GSBackendKeys.Match.CHALLENGED_ID);
            matchInfo.challengerId = matchData.GetString(GSBackendKeys.Match.CHALLENGER_ID);
            if (shortCode == GSBackendKeys.Match.LONG_MATCH_SHORT_CODE)
            {
                matchInfo.gameStartTimeMilliseconds = matchData.GetLong(GSBackendKeys.GAME_START_TIME).Value;

                if (matchData.ContainsKey(GSBackendKeys.Match.IS_RANKED))
                {
                    matchInfo.isRanked = matchData.GetBoolean(GSBackendKeys.Match.IS_RANKED).Value;
                }
                // else legacy long matches were always ranked
                else
                {
                    matchInfo.isRanked = true;
                }
            }
            // quick matches are always ranked (for now)
            else
            {
                matchInfo.isRanked = matchData.GetBoolean(GSBackendKeys.Match.IS_RANKED).Value;
            }

            if (matchData.ContainsKey(GSBackendKeys.GAME_START_TIME))
            {
                matchInfo.gameStartTimeMilliseconds = matchData.GetLong(GSBackendKeys.GAME_START_TIME).Value;
            }

            if (matchData.ContainsKey(GSBackendKeys.Match.CREATE_TIME))
            {
                matchInfo.createTimeMs = matchData.GetLong(GSBackendKeys.Match.CREATE_TIME).Value;
            }
            else
            {
                matchInfo.createTimeMs = TimeUtil.unixTimestampMilliseconds;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize opponent profile
            string opponentId = (playerModel.id == matchInfo.challengerId) ? matchInfo.challengedId : matchInfo.challengerId;
            GSData opponentData = matchData.GetGSData(opponentId);
            GSData opponentProfile = opponentData.GetGSData(GSBackendKeys.ChallengeData.PROFILE);
            PublicProfile opponentPublicProfile = new PublicProfile();

            //Populate Active Inventory
            IList<GSData> playerActiveInventoryData = opponentProfile.GetGSDataList(GSBackendKeys.PlayerDetails.PLAYER_ACTIVE_INVENTORY);
            GSParser.PopulateActiveInventory(opponentPublicProfile, playerActiveInventoryData);

            opponentPublicProfile.playerId = opponentId;
            opponentPublicProfile.name = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_NAME);
            opponentPublicProfile.countryId = opponentProfile.GetString(GSBackendKeys.ChallengeData.PROFILE_COUNTRY_ID);
            opponentPublicProfile.eloScore = opponentProfile.GetInt(GSBackendKeys.ChallengeData.PROFILE_ELO_SCORE).Value;
            opponentPublicProfile.totalGamesWon = opponentProfile.GetInt(GSBackendKeys.PublicProfile.TOTAL_GAMES_WON).Value;
            opponentPublicProfile.totalGamesLost = opponentProfile.GetInt(GSBackendKeys.PublicProfile.TOTAL_GAMES_LOST).Value;

            GSData externalIds = opponentProfile.GetGSData(GSBackendKeys.ChallengeData.PROFILE_EXTERNAL_IDS);
            IDictionary<ExternalAuthType, ExternalAuth> auths = GSBackendKeys.Auth.GetExternalAuthentications(externalIds);
            if (auths.ContainsKey(ExternalAuthType.FACEBOOK))
            {
                ExternalAuth facebookAuthData = auths[ExternalAuthType.FACEBOOK];
                opponentPublicProfile.facebookUserId = facebookAuthData.id;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Initialize bots
            matchInfo.botId = matchData.GetString(GSBackendKeys.ChallengeData.BOT_ID);
            if (opponentData.ContainsKey(GSBackendKeys.ChallengeData.BOT_DIFFICULTY))
            {
                matchInfo.botDifficulty = opponentData.GetFloat(GSBackendKeys.ChallengeData.BOT_DIFFICULTY).Value;

                // Assign a random name to the bot
                // int randomSuffix = UnityEngine.Random.Range(100, 10001);
                // matchInfo.opponentPublicProfile.name = "Guest" + randomSuffix;

                DateTime start = new DateTime(2017, 1, 1);
                int range = (DateTime.UtcNow.AddMinutes(-10000) - start).Days;
                DateTime creationDateTime = start.AddDays(new System.Random().Next(range));
                opponentPublicProfile.creationDate = creationDateTime.ToLocalTime().ToLongDateString();
                opponentPublicProfile.creationDateShort = creationDateTime.ToLocalTime().ToString("d MMM yyyy");
                opponentPublicProfile.lastSeenDateTime = DateTime.UtcNow.AddMinutes(UnityEngine.Random.Range(10, 10000) * -1);
                opponentPublicProfile.lastSeen = opponentPublicProfile.lastSeenDateTime.ToLocalTime().ToLongDateString();

                //Assigning same league as player's
                var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
                opponentPublicProfile.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;
                opponentPublicProfile.league = playerModel.league;
            }
            else
            {
                long creationDateUTC = opponentProfile.GetLong(GSBackendKeys.PublicProfile.CREATION_DATE).Value;
                DateTime creationDateTime = TimeUtil.ToDateTime(creationDateUTC);
                opponentPublicProfile.creationDate = creationDateTime.ToLocalTime().ToLongDateString();
                opponentPublicProfile.creationDateShort = creationDateTime.ToLocalTime().ToString("d MMM yyyy");

                long lastSeenDateUTC = opponentProfile.GetLong(GSBackendKeys.PublicProfile.LAST_SEEN).Value;
                opponentPublicProfile.lastSeenDateTime = TimeUtil.ToDateTime(lastSeenDateUTC);
                opponentPublicProfile.lastSeen = opponentPublicProfile.lastSeenDateTime.ToLocalTime().ToLongDateString();

                opponentPublicProfile.isOnline = opponentProfile.GetBoolean(GSBackendKeys.PublicProfile.IS_ONLINE).Value;

                var opponentLeague = GSParser.GetSafeInt(opponentProfile, GSBackendKeys.PublicProfile.LEAGUE);
                var leagueAssets = tournamentsModel.GetLeagueSprites(opponentLeague.ToString());
                opponentPublicProfile.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;
            }

            matchInfo.opponentPublicProfile = opponentPublicProfile;
        }

        private void UpdateMatch(string challengeId, GSData matchData)
        {
            MatchInfo matchInfo = matchInfoModel.matches[challengeId];

            // Update accept status
            matchInfo.acceptStatus = matchData.GetString(GSBackendKeys.Match.ACCEPT_STATUS_KEY);

            // Update winner if any
            matchInfo.winnerId = matchData.GetString(GSBackendKeys.Match.WINNER_ID);

            // Update elo numbers
            GSData eloChangeData = matchData.GetGSData(GSBackendKeys.Match.ELO_CHANGE);
            if (playerModel.id != null && eloChangeData.ContainsKey(playerModel.id))
            {
                matchInfo.playerEloScoreDelta = eloChangeData.GetInt(playerModel.id).Value;
            }
        }

        private void HandleActiveNewMatch(string challengeId, bool forceStart = false)
        {
            MatchInfo matchInfo = matchInfoModel.matches[challengeId];

            if (matchInfo.isLongPlay)
            {
                string opponentId = (playerModel.id == matchInfo.challengerId) ?
                    matchInfo.challengedId : matchInfo.challengerId;

                if ((opponentId == matchInfoModel.activeLongMatchOpponentId &&
                    matchInfoModel.activeChallengeId == null) || forceStart)
                {
                    startLongMatchSignal.Dispatch(challengeId);
                    preferencesModel.gameStartCount++;
                    hAnalyticsService.LogMultiplayerGameEvent(AnalyticsEventId.game_started.ToString(), "gameplay", "long_match", matchInfoModel.activeChallengeId);
                    appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_started, preferencesModel.gameStartCount);
                    findRandomLongMatchCompleteSignal.Dispatch();
                }
            }
            else
            {
                findMatchCompleteSignal.Dispatch(challengeId);
            }
        }

        private void UpdateEndGameStats(string challengeId, GSData data)
        {
            GSData updatedStatsData = data.GetGSData(GSBackendKeys.UPDATED_STATS);

            // Handle end game stats
            if (updatedStatsData != null)
            {
                playerModel.eloScore = updatedStatsData.GetInt(GSBackendKeys.ELO_SCORE).Value;
                playerModel.totalGamesWon = updatedStatsData.GetInt(GSBackendKeys.GAMES_WON).Value;
                playerModel.totalGamesLost = updatedStatsData.GetInt(GSBackendKeys.GAMES_LOST).Value;
                playerModel.trophies = GSParser.GetSafeInt(updatedStatsData, GSBackendKeys.PlayerDetails.TROPHIES);

                var playerLeague = playerModel.league;
                playerModel.league = GSParser.GetSafeInt(updatedStatsData, GSBackendKeys.PlayerDetails.LEAGUE, playerModel.league);
                playerModel.leaguePromoted = playerLeague != playerModel.league;

                if (playerModel.leaguePromoted)
                {
                    InBoxOpGet();
                }

                if (updatedStatsData.ContainsKey(GSBackendKeys.FRIEND))
                {
                    GSData friendData = updatedStatsData.GetGSData(GSBackendKeys.FRIEND);
                    string friendId = updatedStatsData.GetString(GSBackendKeys.Friend.FRIEND_ID);

                    if (friendId != null && playerModel.friends.ContainsKey(friendId))
                    {
                        Friend updatedFriend = LoadFriend(friendId, friendData);

                        Friend savedFriend = playerModel.friends[friendId];
                        savedFriend.gamesWon = updatedFriend.gamesWon;
                        savedFriend.gamesLost = updatedFriend.gamesLost;
                        savedFriend.gamesDrawn = updatedFriend.gamesDrawn;
                        savedFriend.lastMatchTimestamp = updatedFriend.lastMatchTimestamp;
                        savedFriend.flagMask = updatedFriend.flagMask;
                        savedFriend.publicProfile.eloScore = updatedFriend.publicProfile.eloScore;

                        EloVO vo;
                        vo.opponentId = friendId;
                        vo.opponentEloScore = savedFriend.publicProfile.eloScore;
                        vo.playerEloScore = playerModel.eloScore;
                        updateEloScoresSignal.Dispatch(vo);
                    }
                }
                else
                {
                    EloVO vo;
                    vo.opponentId = updatedStatsData.GetString(GSBackendKeys.OPPONENT_ID);
                    vo.opponentEloScore = updatedStatsData.GetInt(GSBackendKeys.OPPONENT_ELO).Value;
                    vo.playerEloScore = playerModel.eloScore;
                    updateEloScoresSignal.Dispatch(vo);
                }
            }
        }

        private void HandleTournamentEndMatch(GSData tournamentGSData)
        {
            string tournamentId = GSParser.GetSafeString(tournamentGSData, GSBackendKeys.Tournament.TOURNAMENT_ID);
            GSData tournamentDetailsGSData = tournamentGSData.GetGSData(GSBackendKeys.Tournament.TOURNAMENT_KEY);
            JoinedTournamentData joinedTournament = null;

            var tournament = tournamentsModel.GetJoinedTournament(tournamentId);
            int previousRank = tournament.rank;

            if (tournamentDetailsGSData != null && tournamentDetailsGSData.BaseData.Count > 0)
            {
                joinedTournament = ParseJoinedTournament(tournamentDetailsGSData, tournamentId, tournament);
                joinedTournament.ended = false;
                joinedTournament.lastFetchedTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            if (joinedTournament == null)
            {
                // Tournament has ended
                updateTournamentLeaderboardSuccessSignal.Dispatch(tournamentId);
                tournamentsModel.currentMatchTournament = tournament;
            }
            else
            {
                if (tournament != null)
                {
                    tournamentsModel.SetJoinedTournament(joinedTournament);
                }
                else
                {
                    joinedTournament.locked = true;
                    tournamentsModel.joinedTournaments.Add(joinedTournament);
                    tournamentsModel.RemoveFromOpenTournament(joinedTournament.shortCode);
                }

                if (tournamentsModel.HasTournamentEnded(joinedTournament))
                {
                    backendService.InBoxOpGet();
                }

                updateTournamentLeaderboardSuccessSignal.Dispatch(tournamentId);
                tournamentsModel.currentMatchTournament = joinedTournament;

                if (joinedTournament.score > 0 &&
                    joinedTournament.rank < joinedTournament.entries.Count &&
                    (joinedTournament.matchesPlayedCount == 1 || joinedTournament.rank < previousRank))
                {
                    metaDataModel.ShowChampionshipNewRankDialog = true;
                }
            }

            updateTournamentsViewSignal.Dispatch();
        }
    }
}
