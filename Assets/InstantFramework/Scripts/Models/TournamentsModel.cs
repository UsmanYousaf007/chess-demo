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

        // Coroutine runner
        [Inject] public IRoutineRunner routineRunner { get; set; }

        public DateTime lastFetchedTime { get; set; }

        public List<JoinedTournamentData> joinedTournaments { get; set; }
        public List<LiveTournamentData> openTournaments { get; set; }
        public List<LiveTournamentData> upcomingTournaments { get; set; }

        public string currentMatchTournamentType { get; set; }

        Coroutine tournamentsScheduleCoroutine = null;
        
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

        public void StartSchedulingCoroutine()
        {
            if (tournamentsScheduleCoroutine != null)
            {
                return;
            }

            LogUtil.Log("StartSchedulingCoroutine", "green");

            int joinedTournamentsCount = joinedTournaments.Count;
            long minTimeLeft = joinedTournamentsCount > 0 ? CalculateTournamentTimeLeftSeconds(joinedTournaments[0]) : 0;
            for (int i = 1; i < joinedTournamentsCount; i++)
            {
                long timeLeftSeconds = CalculateTournamentTimeLeftSeconds(joinedTournaments[i]);
                if (timeLeftSeconds < minTimeLeft)
                {
                    minTimeLeft = timeLeftSeconds;
                }
            }

            int openTournamentsCount = openTournaments.Count;
            if (minTimeLeft == 0)
            {
                minTimeLeft = openTournamentsCount > 0 ? CalculateTournamentTimeLeftSeconds(openTournaments[0]) : 0;
            }
            for (int i = 1; i < openTournamentsCount; i++)
            {
                long timeLeftSeconds = CalculateTournamentTimeLeftSeconds(openTournaments[i]);
                if (timeLeftSeconds < minTimeLeft)
                {
                    minTimeLeft = timeLeftSeconds;
                }
            }

            int upcomingTournamentsCount = upcomingTournaments.Count;
            if (minTimeLeft == 0)
            {
                minTimeLeft = upcomingTournamentsCount > 0 ? CalculateTournamentTimeLeftSeconds(upcomingTournaments[0]) : 0;
            }
            for (int i = 1; i < upcomingTournamentsCount; i++)
            {
                long timeLeftSeconds = CalculateTournamentTimeLeftSeconds(upcomingTournaments[i]);
                if (timeLeftSeconds < minTimeLeft)
                {
                    minTimeLeft = timeLeftSeconds;
                }
            }

            if (minTimeLeft > 0)
            {
                tournamentsScheduleCoroutine = routineRunner.StartCoroutine(UpdateTournamentsCoroutine(minTimeLeft + 5));
            }
            else
            {
                UpdateTournaments();
            }
        }

        public void StopScheduledCoroutine()
        {
            LogUtil.Log("StopScheduledCoroutine : ");
            routineRunner.StopCoroutine(tournamentsScheduleCoroutine);
            tournamentsScheduleCoroutine = null;
        }

        public long CalculateCurrentStartTime(long waitTimeSeconds, long durationSeconds, long firstStartTimeSeconds)
        {
            long currentTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();

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
            long currentTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            long durationSeconds = liveTournament.durationMinutes * 60;
            if (currentTimeSeconds > liveTournament.currentStartTimeInSeconds)
            {
                if (currentTimeSeconds < (liveTournament.currentStartTimeInSeconds + durationSeconds))
                {
                    return true;
                }
            }

            return false;
        }

        public long CalculateTournamentTimeLeftSeconds(LiveTournamentData liveTournament)
        {
            long currentTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
            long durationSeconds = liveTournament.durationMinutes * 60;
            if (currentTimeSeconds > liveTournament.currentStartTimeInSeconds)
            {
                return (liveTournament.currentStartTimeInSeconds + durationSeconds - currentTimeSeconds);
            }
            else
            {
                return (liveTournament.currentStartTimeInSeconds - currentTimeSeconds);
            }
        }

        public long CalculateTournamentTimeLeftSeconds(JoinedTournamentData joinedTournament)
        {
            long currentTimeSeconds = DateTimeOffset.Now.ToUnixTimeSeconds();
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

        #region Scheduling Coroutines
        private IEnumerator UpdateTournamentsCoroutine(long seconds)
        {
            LogUtil.Log("UpdateTournamentsCoroutine::Start : " + seconds, "green");

            yield return new WaitForSecondsRealtime(seconds);

            // Dispatch update tournaments signal here
            LogUtil.Log("UpdateTournamentsCoroutine::Finish : " + seconds, "green");

            UpdateTournaments();
        }

        private void UpdateTournaments()
        {
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                long timeLeft = CalculateTournamentTimeLeftSeconds(joinedTournaments[i]);
                if (timeLeft <= 0)
                {
                    joinedTournaments.RemoveAt(i);
                }
            }

            List<LiveTournamentData> expiredOpenTournaments = new List<LiveTournamentData>();
            for (int i = 0; i < openTournaments.Count; i++)
            {
                long timeLeft = CalculateTournamentTimeLeftSeconds(openTournaments[i]);
                if (timeLeft <= 0)
                {
                    expiredOpenTournaments.Add(openTournaments[i]);
                    openTournaments.RemoveAt(i);
                }
            }

            List<LiveTournamentData> openedUpcomingTournaments = new List<LiveTournamentData>();
            for (int i = 0; i < upcomingTournaments.Count; i++)
            {
                long timeLeft = CalculateTournamentTimeLeftSeconds(upcomingTournaments[i]);
                if (timeLeft <= 0)
                {
                    openedUpcomingTournaments.Add(upcomingTournaments[i]);
                    upcomingTournaments.RemoveAt(i);
                }
            }

            openTournaments.AddRange(openedUpcomingTournaments);
            upcomingTournaments.AddRange(expiredOpenTournaments);

            updateTournamentsSignal.Dispatch();
        }
        #endregion
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

        public DateTime lastFetchedTime;
        public Dictionary<int, TournamentReward> rewardsDict = new Dictionary<int, TournamentReward>();
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

        public long currentStartTimeInSeconds;
        public DateTime lastFetchedTime;
        public Dictionary<int, TournamentReward> rewardsDict = new Dictionary<int, TournamentReward>();

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
