using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TournamentsModel : ITournamentsModel
    {
        private ChestIconsContainer chestIconsContainer;
        private TournamentAssetsContainer tournamentAssetsContainer;
        private static LeagueTierIconsContainer leagueTierIconsContainer;

        // Signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public UpdateTournamentsSignal updateTournamentsSignal { get; set; }
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardViewSignal updateTournamentLeaderboardView { get; set; }

        // Service
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public DateTime lastFetchedTime { get; set; }

        public List<JoinedTournamentData> joinedTournaments { get; set; }
        public List<LiveTournamentData> openTournaments { get; set; }
        public List<LiveTournamentData> upcomingTournaments { get; set; }

        public string currentMatchTournamentType { get; set; }
        public JoinedTournamentData currentMatchTournament { get; set; }

        public bool updating { get; set; }
        
        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);

            chestIconsContainer = chestIconsContainer == null ? ChestIconsContainer.Load() : chestIconsContainer;
            tournamentAssetsContainer = tournamentAssetsContainer == null ? TournamentAssetsContainer.Load() : tournamentAssetsContainer;
            leagueTierIconsContainer = leagueTierIconsContainer == null ? LeagueTierIconsContainer.Load() : leagueTierIconsContainer;
        }

        private void Reset()
        {
            joinedTournaments = new List<JoinedTournamentData>();
            openTournaments = new List<LiveTournamentData>();
            upcomingTournaments = new List<LiveTournamentData>();
        }

        public void UpdateSchedule()
        {
            if (updating)
            {
                return;
            }

            bool updateLocal = false;
            bool updateRemote = false;
            long currentTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // When joined tournament ends then we update tournaments on server
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].locked == false && joinedTournaments[i].ended == false)
                {
                    if (currentTimeUTCSeconds > joinedTournaments[i].endTimeUTCSeconds)
                    {
                        updateRemote = true;
                        analyticsService.Event($"{AnalyticsEventId.finish_rank}_{joinedTournaments[i].type.ToLower()}", AnalyticsParameter.context, GetRankContext(joinedTournaments[i].rank));
                    }
                }
            }

            if (updateLocal == false)
            {
                for (int i = 0; i < openTournaments.Count; i++)
                {
                    if (openTournaments[i].joined)
                    {
                        updateLocal = true;
                    }

                    if (currentTimeUTCSeconds > openTournaments[i].endTimeUTCSeconds)
                    {
                        updateLocal = true;
                    }
                    else if (openTournaments[i].concluded == false)
                    {
                        if (currentTimeUTCSeconds > openTournaments[i].concludeTimeUTCSeconds)
                        {
                            openTournaments[i].concluded = true;
                            updateLocal = true;
                        }
                    }
                }
            }

            if (updateLocal == false)
            {
                for (int i = 0; i < upcomingTournaments.Count; i++)
                {
                    if (currentTimeUTCSeconds > upcomingTournaments[i].concludeTimeUTCSeconds)
                    {
                        updateLocal = true;
                        break;
                    }
                }
            }

            if (updateRemote)
            {
                // Signal for update tournaments on server call. This needs to be called before tournaments are updated locally.
                updateTournamentsSignal.Dispatch();

                UpdateTournamentsLocal();
                updateTournamentsViewSignal.Dispatch();
                updateTournamentLeaderboardView.Dispatch();
            }
            else if (updateLocal)
            {
                UpdateTournamentsLocal();
                updateTournamentsViewSignal.Dispatch();
                updateTournamentLeaderboardView.Dispatch();
            }
        }

        public long CalculateCurrentStartTime(long waitTimeSeconds, long durationSeconds, long firstStartTimeSeconds)
        {
            long currentTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long currentStartTimeInSeconds = 0;

            if (currentTimeSeconds > firstStartTimeSeconds)
            {
                long tournamentGap = durationSeconds + waitTimeSeconds;
                long currentTimeGap = (currentTimeSeconds - firstStartTimeSeconds) % tournamentGap;

                if (currentTimeGap < durationSeconds)
                {
                    currentStartTimeInSeconds = currentTimeSeconds - currentTimeGap;
                }
                else
                {
                    currentStartTimeInSeconds = (currentTimeSeconds - currentTimeGap) + tournamentGap;
                }
            }
            else
            {
                currentStartTimeInSeconds = firstStartTimeSeconds;
            }

            return currentStartTimeInSeconds;
        }

        public bool isTournamentOpen(LiveTournamentData liveTournament)
        {
            long currentTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long durationSeconds = liveTournament.durationMinutes * 60;
            if (currentTimeSeconds > liveTournament.currentStartTimeUTCSeconds)
            {
                if (currentTimeSeconds < (liveTournament.currentStartTimeUTCSeconds + durationSeconds))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasTournamentEnded(JoinedTournamentData joinedTournament)
        {
            long currentTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return currentTimeUTCSeconds > joinedTournament.concludeTimeUTCSeconds;
        }

        public bool HasTournamentEnded(LiveTournamentData liveTournament)
        {
            long currentTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return currentTimeUTCSeconds > liveTournament.concludeTimeUTCSeconds;
        }

        public long CalculateTournamentTimeLeftSeconds(LiveTournamentData liveTournament)
        {
            long currentTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            long durationSeconds = liveTournament.durationMinutes * 60;
            if (currentTimeSeconds > liveTournament.currentStartTimeUTCSeconds)
            {
                return (liveTournament.currentStartTimeUTCSeconds + durationSeconds - currentTimeSeconds);
            }
            else
            {
                return (liveTournament.currentStartTimeUTCSeconds - currentTimeSeconds);
            }
        }

        public long CalculateTournamentTimeLeftSeconds(JoinedTournamentData joinedTournament)
        {
            long currentTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long durationSeconds = joinedTournament.durationMinutes * 60;
            long startTime = joinedTournament.startTimeUTC / 1000;
            return (startTime + durationSeconds - currentTimeSeconds);
        }

        public LiveTournamentData GetOpenTournament(string shortCode)
        {
            for (int i = 0; i < openTournaments.Count; i++)
            {
                if (openTournaments[i].shortCode == shortCode)
                {
                    return openTournaments[i];
                }
            }

            return null;
        }

        public void SetOpenTournament(LiveTournamentData openTournament)
        {
            for (int i = 0; i < openTournaments.Count; i++)
            {
                if (openTournaments[i].shortCode == openTournament.shortCode)
                {
                    openTournaments[i] = openTournament;
                }
            }
        }

        public LiveTournamentData GetOpenTournament()
        {
            return openTournaments.Count > 0 ? openTournaments[0] : null;
        }

        public JoinedTournamentData GetJoinedTournament()
        {
            return joinedTournaments.Count > 0 ? joinedTournaments[0] : null;
        }

        public JoinedTournamentData GetJoinedTournament(string tournamentId)
        {
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].id == tournamentId)
                {
                    return joinedTournaments[i];
                }
            }

            return null;
        }

        public void SetJoinedTournament(JoinedTournamentData joinedTournament)
        {
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].id == joinedTournament.id)
                {
                    joinedTournaments[i] = joinedTournament;
                }
            }
        }

        public LiveTournamentData GetUpcomingTournament(string shortCode)
        {
            for (int i = 0; i < upcomingTournaments.Count; i++)
            {
                if (upcomingTournaments[i].shortCode == shortCode)
                {
                    return upcomingTournaments[i];
                }
            }

            return null;
        }

        public string GetEndedTournamentId()
        {
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].ended)
                {
                    return joinedTournaments[i].id;
                }
            }

            return null;
        }

        public bool RemoveFromJoinedTournament(string tournamentId)
        {
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].id == tournamentId)
                {
                    joinedTournaments.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool RemoveFromOpenTournament(string shortCode)
        {
            for (int i = 0; i < openTournaments.Count; i++)
            {
                if (openTournaments[i].shortCode == shortCode)
                {
                    openTournaments.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public Sprite GetLiveTournamentSticker()
        {
            var joinedTournament = GetJoinedTournament();
            if (joinedTournament != null)
                return GetStickerSprite(joinedTournament.type);

            var openTournament = GetOpenTournament();
            if (openTournament != null)
                return GetStickerSprite(openTournament.type);

            return null;
        }

        public TournamentReward GetTournamentGrandPrize(string id)
        {
            var joinedTournament = GetJoinedTournament(id);
            if (joinedTournament != null)
            {
                return joinedTournament.grandPrize;
            }

            var openTournament = GetOpenTournament(id);
            if (openTournament != null)
            {
                return openTournament.grandPrize;
            }

            var upcomingTournament = GetUpcomingTournament(id);
            if (upcomingTournament != null)
            {
                return upcomingTournament.grandPrize;
            }

            return null;
        }

        #region Tournament Sprites API
        public Sprite GetStickerSprite(string tournamentType)
        {
            return tournamentAssetsContainer.GetSticker(tournamentType);
        }

        public Sprite GetTileSprite(string tournamentType)
        {
            return tournamentAssetsContainer.GetTile(tournamentType);
        }

        public TournamentAssetsContainer.TournamentAsset GetAllSprites(string tournamentType)
        {
            return tournamentAssetsContainer.GetAssets(tournamentType);
        }

        public LeagueTierIconsContainer.LeagueAsset GetLeagueSprites(string leagueType)
        {
            return leagueTierIconsContainer.GetAssets(leagueType);
        }

        #endregion

        private void UpdateTournamentsLocal()
        {
            long currentTimeUTCSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            List<LiveTournamentData> finishedTournaments = new List<LiveTournamentData>();

            for (int i = joinedTournaments.Count - 1; i >= 0; i--)
            {
                if (joinedTournaments[i].locked == false)
                {
                    if (currentTimeUTCSeconds > joinedTournaments[i].endTimeUTCSeconds)
                    {
                        if (joinedTournaments[i].ended == false)
                        {
                            LiveTournamentData upcomingTournament = new LiveTournamentData(joinedTournaments[i]);
                            finishedTournaments.Add(upcomingTournament);

                            analyticsService.Event($"{AnalyticsEventId.finish_rank}_{joinedTournaments[i].type.ToLower()}", AnalyticsParameter.context, GetRankContext(joinedTournaments[i].rank));
                        }

                        joinedTournaments.RemoveAt(i);
                    }

                }
            }

            List<LiveTournamentData> expiredOpenTournaments = new List<LiveTournamentData>();
            for (int i = openTournaments.Count - 1; i >= 0; i--)
            {
                if (currentTimeUTCSeconds > openTournaments[i].endTimeUTCSeconds)
                {
                    if (openTournaments[i].joined == false)
                    {
                        expiredOpenTournaments.Add(openTournaments[i]);
                    }

                    openTournaments.RemoveAt(i);
                }
            }

            List<LiveTournamentData> openedUpcomingTournaments = new List<LiveTournamentData>();
            for (int i = upcomingTournaments.Count - 1; i >= 0; i--)
            {
                bool opened = currentTimeUTCSeconds > upcomingTournaments[i].concludeTimeUTCSeconds;
                if (opened)
                {
                    openedUpcomingTournaments.Add(upcomingTournaments[i]);
                    upcomingTournaments.RemoveAt(i);
                }
            }

            for (int i = 0; i < expiredOpenTournaments.Count; i++)
            {
                var expiredTournament = expiredOpenTournaments[i];
                long waitTimeSeconds = expiredTournament.waitTimeMinutes * 60;
                long durationSeconds = expiredTournament.durationMinutes * 60;
                long firstStartTimeSeconds = expiredTournament.firstStartTimeUTC / 1000;

                expiredTournament.currentStartTimeUTCSeconds = CalculateCurrentStartTime(waitTimeSeconds, durationSeconds, firstStartTimeSeconds);
                expiredTournament.concludeTimeUTCSeconds = expiredTournament.currentStartTimeUTCSeconds + durationSeconds;
                expiredTournament.endTimeUTCSeconds = expiredTournament.concludeTimeUTCSeconds;

                expiredTournament.concluded = false;
            }

            for (int i = 0; i < openedUpcomingTournaments.Count; i++)
            {
                var upcomingTournament = openedUpcomingTournaments[i];
                long waitTimeSeconds = upcomingTournament.waitTimeMinutes * 60;
                long durationSeconds = upcomingTournament.durationMinutes * 60;
                long firstStartTimeSeconds = upcomingTournament.firstStartTimeUTC / 1000;

                upcomingTournament.currentStartTimeUTCSeconds = CalculateCurrentStartTime(waitTimeSeconds, durationSeconds, firstStartTimeSeconds);
                upcomingTournament.endTimeUTCSeconds = upcomingTournament.currentStartTimeUTCSeconds + durationSeconds;
                upcomingTournament.concludeTimeUTCSeconds = upcomingTournament.endTimeUTCSeconds - (TournamentConstants.BUFFER_TIME_MINS * 60);

                upcomingTournament.concluded = false;
            }

            openTournaments.AddRange(openedUpcomingTournaments);
            upcomingTournaments.AddRange(expiredOpenTournaments);
            upcomingTournaments.AddRange(finishedTournaments);
        }

        private string GetRankContext(int rank)
        {
            if (rank > 25)
            {
                return "less_than_25";
            }
            else if(rank <= 25 && rank >= 10)
            {
                return "25_to_10";
            }
            else if (rank <= 9 && rank >= 2)
            {
                return "9_to_2";
            }

            return "1";
        }

        public void LogConcludedJoinedTournaments()
        {
            foreach (var t in joinedTournaments)
            {
                if (t.ended)
                {
                    analyticsService.Event($"{AnalyticsEventId.finish_rank}_{t.type.ToLower()}", AnalyticsParameter.context, GetRankContext(t.rank));
                }
            }
        }
    }

    [Serializable]
    public class JoinedTournamentData
    {
        public string id;
        public string shortCode;
        public string type;
        public string name;
        public int rank;
        public TournamentReward grandPrize;
        public long startTimeUTC;
        public int durationMinutes;
        public long currentStartTimeInSeconds;
        public List<TournamentEntry> entries = new List<TournamentEntry>();

        public long lastFetchedTimeUTCSeconds;
        public Dictionary<int, TournamentReward> rewardsDict = new Dictionary<int, TournamentReward>();
        public long concludeTimeUTCSeconds;
        public long endTimeUTCSeconds;
        public bool locked = false;
        public bool ended = false;
    }

    [Serializable]
    public class LiveTournamentData
    {
        public string shortCode;
        public string name;
        public string type;
        public bool active;
        public TournamentReward grandPrize;
        public long firstStartTimeUTC;
        public int durationMinutes;
        public int waitTimeMinutes;

        public long currentStartTimeUTCSeconds;
        public long lastFetchedTimeUTCSeconds;
        public Dictionary<int, TournamentReward> rewardsDict = new Dictionary<int, TournamentReward>();
        public long concludeTimeUTCSeconds;
        public long endTimeUTCSeconds;
        public bool concluded = false;
        public bool joined = false;

        public LiveTournamentData() { }

        public LiveTournamentData(JoinedTournamentData joinedTournament)
        {
            long gapTimeSeconds = joinedTournament.endTimeUTCSeconds - joinedTournament.currentStartTimeInSeconds;
            joinedTournament.currentStartTimeInSeconds += gapTimeSeconds;
            joinedTournament.endTimeUTCSeconds = joinedTournament.currentStartTimeInSeconds + gapTimeSeconds;

            shortCode = joinedTournament.shortCode;
            name = joinedTournament.name;
            type = joinedTournament.type;
            active = true;
            grandPrize = joinedTournament.grandPrize;
            firstStartTimeUTC = joinedTournament.currentStartTimeInSeconds;
            durationMinutes = joinedTournament.durationMinutes;

            currentStartTimeUTCSeconds = joinedTournament.currentStartTimeInSeconds + gapTimeSeconds;
            endTimeUTCSeconds = currentStartTimeUTCSeconds + gapTimeSeconds;
            concludeTimeUTCSeconds = endTimeUTCSeconds;
            waitTimeMinutes = (int)(endTimeUTCSeconds - (durationMinutes * 60)) / 60;

            lastFetchedTimeUTCSeconds = joinedTournament.lastFetchedTimeUTCSeconds;
            rewardsDict = joinedTournament.rewardsDict;
            concluded = false;
            joined = false;
    }
}

    [Serializable]
    public class TournamentReward
    {
        public string type;
        public int quantity;
        public int trophies;
        public string chestType;
        public int gems;
        public int hints;
        public int ratingBoosters;
        public int minRank;
        public int maxRank;
    }
}
