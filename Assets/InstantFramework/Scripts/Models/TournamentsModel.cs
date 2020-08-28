using System;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class TournamentsModel : ITournamentsModel
    {
        private static ChestIconsContainer chestIconsContainer;
        private static TournamentAssetsContainer tournamentAssetsContainer;

        // Signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        public DateTime lastFetchedTime { get; set; }

        public List<JoinedTournamentData> joinedTournaments { get; set; }
        public List<LiveTournamentData> openTournaments { get; set; }
        public List<LiveTournamentData> upcomingTournaments { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);

            chestIconsContainer = chestIconsContainer == null ? ChestIconsContainer.Load() : chestIconsContainer;
            tournamentAssetsContainer = tournamentAssetsContainer == null ? TournamentAssetsContainer.Load() : tournamentAssetsContainer;
        }

        private void Reset()
        {
            joinedTournaments = new List<JoinedTournamentData>();
            openTournaments = new List<LiveTournamentData>();
            upcomingTournaments = new List<LiveTournamentData>();
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
            long currentTimeSeconds = TimeUtil.unixTimestampMilliseconds / 1000;
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
            long currentTimeSeconds = TimeUtil.unixTimestampMilliseconds / 1000;
            long durationSeconds = joinedTournament.durationMinutes * 60;
            long startTime = joinedTournament.startTimeUTC * 60;
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
        #endregion
    }

    [Serializable]
    public class JoinedTournamentData
    {
        //-- Server tournament model
        //type: tournament.type,
        //name: tournament.name,
        //rank: tournamentPlayer.rank,
        //grandPrize: tournament.rewards[0],
        //startTime: tournament.startTime,
        //duration: tournament.duration

        public string id;
        public string shortCode;
        public string type;
        public string name;
        public int rank;
        public TournamentReward grandPrize;
        public long startTimeUTC;
        public int durationMinutes;
        public long currentStartTimeInSeconds;
        public List<TournamentReward> rewards = new List<TournamentReward>();
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
        public List<TournamentReward> rewards = new List<TournamentReward>();

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
