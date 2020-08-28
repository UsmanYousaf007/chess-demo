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
    public class TournamentLeaderboardView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public Button backButton;

        public Transform listContainer;
        public GameObject tournamentLeaderboardPlayerBarPrefab;
        public TournamentLiveItem header;
        public TournamentLeaderboardInfoBar infoBar;
        public TournamentLeaderboardFooter footer;

        // Player bar click signal
        [HideInInspector]
        public Signal<TournamentLeaderboardPlayerBar> playerBarClickedSignal = new Signal<TournamentLeaderboardPlayerBar>();
        public Signal backSignal = new Signal();
        public Signal<TournamentReward> playerBarChestClickSignal = new Signal<TournamentReward>();

        //private Dictionary<string, TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new Dictionary<string, TournamentLeaderboardPlayerBar>();
        private List<TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new List<TournamentLeaderboardPlayerBar>();

        private LiveTournamentData liveTournament = null;
        private JoinedTournamentData joinedTournament = null;

        public void Init()
        {
            header.Init();

            PopulateTournamentInfoBar();
            PopulateFooter();

            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void Populate(LiveTournamentData liveTournament)
        {
            int itemBarsCount = tournamentLeaderboardPlayerBars.Count;
            if (itemBarsCount < 3)
            {
                for (int i = itemBarsCount; i < 3; i++)
                {
                    tournamentLeaderboardPlayerBars.Add(AddPlayerBar());
                }
            }

            for (int i = 0; i < 3; i++)
            {
                var playerBar = tournamentLeaderboardPlayerBars[i];
                PopulateBar(playerBar, i + 1, liveTournament.rewards[i]);
            }

            for (int i = 4; i < itemBarsCount; i++)
            {
                var playerBar = tournamentLeaderboardPlayerBars[i];
                playerBar.gameObject.SetActive(false);
            }

            PopulateTournamentHeader(header, liveTournament);

            // TODO: Disable scroll view here
        }

        public void Populate(JoinedTournamentData joinedTournament)
        {
            int itemBarsCount = tournamentLeaderboardPlayerBars.Count;
            if (itemBarsCount < joinedTournament.entries.Count)
            {
                for (int i = itemBarsCount; i < joinedTournament.entries.Count; i++)
                {
                    tournamentLeaderboardPlayerBars.Add(AddPlayerBar());
                }
            }

            for (int i = 0; i < joinedTournament.entries.Count; i++)
            {
                var playerBar = tournamentLeaderboardPlayerBars[i];
                PopulateBar(playerBar, joinedTournament.entries[i], joinedTournament.rewardsDict.ContainsKey(i) ? joinedTournament.rewardsDict[i] : null);
            }

            PopulateTournamentHeader(header, joinedTournament);

            // TODO: Enable scrolling here
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(JoinedTournamentData joinedTournament)
        {
            this.joinedTournament = joinedTournament;
            Populate(joinedTournament);
            Sort();
        }

        public void UpdateView(LiveTournamentData liveTournament)
        {
            this.liveTournament = liveTournament;
            Populate(liveTournament);
            Sort();
        }

        private void Sort()
        {
            List<TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new List<TournamentLeaderboardPlayerBar>();

            // Copy all player bars into a list
            //foreach (KeyValuePair<string, TournamentLeaderboardPlayerBar> item in this.tournamentLeaderboardPlayerBars)
            //{
            //    tournamentLeaderboardPlayerBars.Add(item.Value);
            //}

            // Todo: Sort

            // Adust order
            int index = 0;
            for (int i = 0; i < tournamentLeaderboardPlayerBars.Count; i++)
            {
                tournamentLeaderboardPlayerBars[i].transform.SetSiblingIndex(index++);
            }
        }

        public void PopulateTournamentHeader(TournamentLiveItem item, JoinedTournamentData joinedTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(joinedTournament);
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            string timeLeftString = TimeUtil.FormatPlayerClock(TimeSpan.FromMilliseconds(timeLeft * 1000));

            item.UpdateItem(joinedTournament, timeLeftString);
        }

        public void PopulateTournamentHeader(TournamentLiveItem item, LiveTournamentData liveTournament)
        {
            long timeLeft = tournamentsModel.CalculateTournamentTimeLeftSeconds(liveTournament);
            if (timeLeft < 0)
            {
                timeLeft = 0;
            }

            string timeLeftString = TimeUtil.FormatPlayerClock(TimeSpan.FromMilliseconds(timeLeft * 1000));

            item.UpdateItem(liveTournament, timeLeftString);
        }

        private void PopulateTournamentInfoBar()
        {
            TournamentLeaderboardInfoBar item = infoBar;

            item.rulesLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_RULES);
            item.totalScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_TOTAL_SCORE);

            item.columnHeaderRankLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_RANK);
            item.columnHeaderScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_TOTAL_PLAYER_SCORE);
            item.columnHeaderRewardsLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_REWARDS);
        }

        private void PopulateFooter()
        {
            TournamentLeaderboardFooter item = footer;

            item.bg.sprite = Resources.Load("AM.png") as Sprite;

            item.youHaveLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_YOU_HAVE);
            item.enterButtonFreePlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_FREE_PLAY);
            item.enterButtonTicketPlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_TICKET_PLAY);

            item.ticketsCountText.text = "3/5";
            item.enterButtonTicketPlayCountText.text = "12";

            item.freePlayButtonGroup.gameObject.SetActive(false);
            item.ticketPlayButtonGroup.gameObject.SetActive(true);
        }

        private TournamentLeaderboardPlayerBar AddPlayerBar()
        {
            GameObject obj = GameObject.Instantiate(tournamentLeaderboardPlayerBarPrefab);
            TournamentLeaderboardPlayerBar item = obj.GetComponent<TournamentLeaderboardPlayerBar>();
            item.transform.SetParent(listContainer, false);
            AddPlayerBarListeners(item);
            return item;
            
        }

        private void PopulateBar(TournamentLeaderboardPlayerBar playerBar, TournamentEntry entry, TournamentReward reward)
        {
            playerBar.Populate(entry, reward);

            //tournamentLeaderboardPlayerBars.Add(item.name + tournamentLeaderboardPlayerBars.Count.ToString(), item);
        }

        private void PopulateBar(TournamentLeaderboardPlayerBar playerBar, int rank, TournamentReward reward)
        {
            playerBar.Populate(rank, reward);

            //tournamentLeaderboardPlayerBars.Add(item.name + tournamentLeaderboardPlayerBars.Count.ToString(), item);
        }

        private void AddPlayerBarListeners(TournamentLeaderboardPlayerBar playerBar)
        {
            playerBar.button.onClick.AddListener(() => playerBarClickedSignal.Dispatch(playerBar));
            playerBar.chestButton.onClick.AddListener(() => playerBarChestClickSignal.Dispatch(playerBar.reward));
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }
    }
}