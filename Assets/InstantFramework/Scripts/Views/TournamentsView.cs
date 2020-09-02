/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class TournamentsView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // Dispatch Signal
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLeagueProfileStripSignal updateLeagueProfileStripSignal { get; set; }

        public Transform listContainer;
        public GameObject tournamentLiveItemPrefab;
        public GameObject tournamentUpcomingItemPrefab;
        public GameObject sectionTournamentUpcomingItems;

        // Button click signals
        public Signal<TournamentLiveItem> liveItemClickedSignal = new Signal<TournamentLiveItem>();
        public Signal<TournamentUpcomingItem> upcomingItemClickedSignal = new Signal<TournamentUpcomingItem>();

        private List<TournamentLiveItem> tournamentLiveItems = new List<TournamentLiveItem>();
        private List<TournamentUpcomingItem> tournamentUpcomingItems = new List<TournamentUpcomingItem>();
        //private Dictionary<string, TournamentLiveItem> tournamentLiveItems = new Dictionary<string, TournamentLiveItem>();
        //private Dictionary<string, TournamentUpcomingItem> tournamentUpcomingItems = new Dictionary<string, TournamentUpcomingItem>();

        public void Init()
        {
            //Sort();
        }

        public void Populate()
        {
            var joinedTournaments = tournamentsModel.joinedTournaments;
            var openTournaments = tournamentsModel.openTournaments;
            var upcomingTournaments = tournamentsModel.upcomingTournaments;

            // Joined plus open tournaments
            int totalLiveTournaments = (joinedTournaments.Count + openTournaments.Count);
            if (tournamentLiveItems.Count < totalLiveTournaments)
            {
                while (tournamentLiveItems.Count < totalLiveTournaments)
                {
                    tournamentLiveItems.Add(AddLiveTournamentItemPrefab());
                }
            }

            if (tournamentUpcomingItems.Count < upcomingTournaments.Count)
            {
                while (tournamentUpcomingItems.Count < upcomingTournaments.Count)
                {
                    tournamentUpcomingItems.Add(AddTournamentUpcomingItemPrefab());
                }
            }

            // Populating joined and open tournaments
            int liveTournamentsCount = 0;
            for (;  liveTournamentsCount < joinedTournaments.Count; liveTournamentsCount++)
            {
                PopulateTournamentLiveItem(tournamentLiveItems[liveTournamentsCount], joinedTournaments[liveTournamentsCount]);
            }
            for (; liveTournamentsCount < openTournaments.Count; liveTournamentsCount++)
            {
                PopulateTournamentLiveItem(tournamentLiveItems[liveTournamentsCount], openTournaments[liveTournamentsCount]);
            }

            for (int i = 0; i < upcomingTournaments.Count; i++)
            {
                PopulateTournamentUpcomingItem(tournamentUpcomingItems[i], upcomingTournaments[i]);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView()
        {
            Populate();
            Sort();

            LeagueProfileStripVO leagueProfileStripVO = new LeagueProfileStripVO();
            leagueProfileStripVO.playerLeagueTitle = TournamentConstants.LeagueType.DIAMOND;
            leagueProfileStripVO.playerLeagueID = playerModel.league.ToString();
            leagueProfileStripVO.playerLeagueThumbnailImage = null;
            leagueProfileStripVO.playerRankCount = 5;
            leagueProfileStripVO.playerTrophiesCount = playerModel.trophies;
            leagueProfileStripVO.playerRankStatusImage = null;
            leagueProfileStripVO.tournamentCountdownTimer = "6d 5m";

            updateLeagueProfileStripSignal.Dispatch(leagueProfileStripVO);
        }

        private void Sort()
        {
            // Todo: Sort

            // Adust order
            int index = 0;
            for (int i = 0; i < tournamentLiveItems.Count; i++)
            {
                tournamentLiveItems[i].transform.SetSiblingIndex(index++);
            }

            sectionTournamentUpcomingItems.transform.SetSiblingIndex(index++);
            for (int i = 0; i < tournamentUpcomingItems.Count; i++)
            {
                tournamentUpcomingItems[i].transform.SetSiblingIndex(index++);
            }
        }

        public TournamentLiveItem AddLiveTournamentItemPrefab()
        {
            GameObject obj = GameObject.Instantiate(tournamentLiveItemPrefab);
            obj.transform.SetParent(listContainer, false);

            TournamentLiveItem item = obj.GetComponent<TournamentLiveItem>();
            item.Init();
            item.button.onClick.AddListener(() => liveItemClickedSignal.Dispatch(item));

            return item;
        }

        public void PopulateTournamentLiveItem(TournamentLiveItem item, JoinedTournamentData joinedTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(joinedTournament);
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            string timeLeftString = TimeUtil.FormatPlayerClock(TimeSpan.FromMilliseconds(timeLeft * 1000));

            item.UpdateItem(joinedTournament, timeLeftString);
        }

        public void PopulateTournamentLiveItem(TournamentLiveItem item, LiveTournamentData liveTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(liveTournament);
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            string timeLeftString = TimeUtil.FormatPlayerClock(TimeSpan.FromMilliseconds(timeLeft * 1000));

            item.UpdateItem(liveTournament, timeLeftString);
        }

        public TournamentUpcomingItem AddTournamentUpcomingItemPrefab()
        {
            GameObject obj = GameObject.Instantiate(tournamentUpcomingItemPrefab);
            obj.transform.SetParent(listContainer, false);

            TournamentUpcomingItem item = obj.GetComponent<TournamentUpcomingItem>();
            item.Init();
            item.button.onClick.AddListener(() => upcomingItemClickedSignal.Dispatch(item));

            return item;
        }

        public void PopulateTournamentUpcomingItem(TournamentUpcomingItem item, LiveTournamentData liveTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(liveTournament);
            string timeLeftString = TimeUtil.FormatPlayerClock(TimeSpan.FromMilliseconds(timeLeft * 1000));

            item.UpdateItem(liveTournament, timeLeftString);
        }
    }
}
