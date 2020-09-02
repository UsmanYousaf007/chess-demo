﻿/// @license Propriety <http://license.url>
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
        public Text upcomingTournamentsText;

        // Button click signals
        public Signal<TournamentLiveItem> liveItemClickedSignal = new Signal<TournamentLiveItem>();
        public Signal<TournamentUpcomingItem> upcomingItemClickedSignal = new Signal<TournamentUpcomingItem>();

        private List<TournamentLiveItem> tournamentLiveItems = new List<TournamentLiveItem>();
        private List<TournamentUpcomingItem> tournamentUpcomingItems = new List<TournamentUpcomingItem>();
        private GameObjectsPool liveItemsPool;
        private GameObjectsPool upcomingItemsPool;


        public void Init()
        {
            upcomingTournamentsText.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING);
            liveItemsPool = new GameObjectsPool(tournamentLiveItemPrefab);
            upcomingItemsPool = new GameObjectsPool(tournamentUpcomingItemPrefab);
        }

        public void Populate()
        {
            var joinedTournaments = tournamentsModel.joinedTournaments;
            var openTournaments = tournamentsModel.openTournaments;
            var upcomingTournaments = tournamentsModel.upcomingTournaments;

            // Joined tournaments
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                var joinedLiveItem = AddLiveTournamentItemPrefab();
                joinedLiveItem.gameObject.SetActive(true);
                PopulateTournamentLiveItem(joinedLiveItem, joinedTournaments[i]);
                tournamentLiveItems.Add(joinedLiveItem);
            }

            // Open tournaments
            for (int i = 0; i < openTournaments.Count; i++)
            {
                var openedLiveItem = AddLiveTournamentItemPrefab();
                openedLiveItem.gameObject.SetActive(true);
                PopulateTournamentLiveItem(openedLiveItem, openTournaments[i]);
                tournamentLiveItems.Add(openedLiveItem);
            }

            // Upcoming tournaments
            for (int i = 0; i < upcomingTournaments.Count; i++)
            {
                var upcomingItem = AddTournamentUpcomingItemPrefab();
                upcomingItem.gameObject.SetActive(true);
                PopulateTournamentUpcomingItem(upcomingItem, upcomingTournaments[i]);
                tournamentUpcomingItems.Add(upcomingItem);
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
            ClearAllItems();
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

        private void ClearAllItems()
        {
            for (int i = 0; i < tournamentLiveItems.Count; i++)
            {
                liveItemsPool.ReturnObject(tournamentLiveItems[i].gameObject);
            }

            for (int i = 0; i < tournamentUpcomingItems.Count; i++)
            {
                upcomingItemsPool.ReturnObject(tournamentUpcomingItems[i].gameObject);
            }

            tournamentLiveItems.Clear();
            tournamentUpcomingItems.Clear();
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
            GameObject obj = liveItemsPool.GetObject();
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

            item.UpdateItem(joinedTournament, timeLeft);
        }

        public void PopulateTournamentLiveItem(TournamentLiveItem item, LiveTournamentData liveTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(liveTournament);

            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            item.UpdateItem(liveTournament, timeLeft);
        }

        public TournamentUpcomingItem AddTournamentUpcomingItemPrefab()
        {
            GameObject obj = upcomingItemsPool.GetObject();
            obj.transform.SetParent(listContainer, false);

            TournamentUpcomingItem item = obj.GetComponent<TournamentUpcomingItem>();
            item.Init();
            item.button.onClick.AddListener(() => upcomingItemClickedSignal.Dispatch(item));

            return item;
        }

        public void PopulateTournamentUpcomingItem(TournamentUpcomingItem item, LiveTournamentData liveTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(liveTournament);
            item.UpdateItem(liveTournament, timeLeft);
            item.startsInLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING_STARTS_IN);
            item.getNotifiedLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING_GET_NOTIFIED);
        }
    }
}
