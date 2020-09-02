/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ITournamentsModel
    {
        DateTime lastFetchedTime { get; set; }

        List<JoinedTournamentData> joinedTournaments { get; set; }
        List<LiveTournamentData> openTournaments { get; set; }
        List<LiveTournamentData> upcomingTournaments { get; set; }

        string currentMatchTournamentType { get; set; }

        void StartSchedulingCoroutine();
        void StopScheduledCoroutine();
        long CalculateCurrentStartTime(long waitTimeSeconds, long durationSeconds, long firstStartTimeSeconds);
        long CalculateTournamentTimeLeftSeconds(JoinedTournamentData joinedTournament);
        long CalculateTournamentTimeLeftSeconds(LiveTournamentData liveTournament);
        bool isTournamentOpen(LiveTournamentData liveTournament);
        void SetOpenTournament(LiveTournamentData openTournament);
        bool RemoveFromJoinedTournament(string tournamentId);
        LiveTournamentData GetOpenTournament();
        LiveTournamentData GetOpenTournament(string shortCode);
        JoinedTournamentData GetJoinedTournament();
        JoinedTournamentData GetJoinedTournament(string tournamentId);
        LiveTournamentData GetUpcomingTournament(string shortCode);
        TournamentReward GetTournamentGrandPrize(string id);
        Sprite GetLiveTournamentSticker();

        Sprite GetStickerSprite(string tournamentType);
        Sprite GetTileSprite(string tournamentType);
        TournamentAssetsContainer.TournamentAsset GetAllSprites(string tournamentType);
        LeagueTierIconsContainer.LeagueAsset GetLeagueSprites(string leagueType);
    }
}

