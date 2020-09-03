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
using System.Collections;

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
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public Button backButton;

        public Transform listContainer;
        public GameObject tournamentLeaderboardPlayerBarPrefab;
        public TournamentLiveItem header;
        public TournamentLeaderboardInfoBar infoBar;
        public TournamentLeaderboardFooter footer;
        public GameObject tournamentLeaderboardPlayerEnterBar;

        // Player bar click signal
        public Signal<TournamentLeaderboardPlayerBar> playerBarClickedSignal = new Signal<TournamentLeaderboardPlayerBar>();
        public Signal backSignal = new Signal();
        public Signal<TournamentReward> playerBarChestClickSignal = new Signal<TournamentReward>();
        public StoreItem ticketStoreItem;

        //private Dictionary<string, TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new Dictionary<string, TournamentLeaderboardPlayerBar>();
        private List<TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new List<TournamentLeaderboardPlayerBar>();

        private LiveTournamentData liveTournament = null;
        private JoinedTournamentData joinedTournament = null;
        private GameObjectsPool barsPool;
        private TournamentAssetsContainer tournamentAssetsContainer;
        private WaitForSecondsRealtime waitForOneRealSecond;

        public void Init()
        {
            barsPool = new GameObjectsPool(tournamentLeaderboardPlayerBarPrefab, 50);
            header.Init();
            PopulateTournamentInfoBar();
            backButton.onClick.AddListener(OnBackButtonClicked);
            tournamentLeaderboardPlayerEnterBar.SetActive(false);
            PopulateTournamentLeaderboardPlayerEnterBar();
            tournamentAssetsContainer = TournamentAssetsContainer.Load();
            waitForOneRealSecond = new WaitForSecondsRealtime(1f);
        }

        public void PopulateTournamentLeaderboardPlayerEnterBar()
        {
            TournamentLeaderboardPlayerEnterBar playerEnterBar = tournamentLeaderboardPlayerEnterBar.GetComponent<TournamentLeaderboardPlayerEnterBar>();
            playerEnterBar.bodyText.text = "Enter this Tournament to earn a rank!";
            playerEnterBar.rankText.text = "?";
         }

        public void Populate(LiveTournamentData liveTournament)
        {
            ClearBars();

            int itemBarsCount = tournamentLeaderboardPlayerBars.Count;
            if (itemBarsCount <= 50)
            {
                for (int i = itemBarsCount; i < 50; i++)
                {
                    tournamentLeaderboardPlayerBars.Add(AddPlayerBar());
                }
            }

            for (int i = 0; i < 50; i++)
            {
                var playerBar = tournamentLeaderboardPlayerBars[i];
                PopulateBar(playerBar, i + 1, liveTournament.rewardsDict[i + 1]);
            }

            //for (int i = 4; i < itemBarsCount; i++)
            //{
            //    var playerBar = tournamentLeaderboardPlayerBars[i];
            //    playerBar.gameObject.SetActive(false);
            //}

            PopulateTournamentHeader(header, liveTournament);
            footer.bg.color = tournamentAssetsContainer.GetColor(liveTournament.type);
            tournamentLeaderboardPlayerEnterBar.SetActive(true);
        }

        public void Populate(JoinedTournamentData joinedTournament)
        {
            ClearBars();

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
                PopulateBar(playerBar, joinedTournament.entries[i], joinedTournament.rewardsDict.ContainsKey(i+1) ? joinedTournament.rewardsDict[i + 1] : null);
            }

            PopulateTournamentHeader(header, joinedTournament);
            PopulateFooter();
            footer.bg.color = tournamentAssetsContainer.GetColor(joinedTournament.type);
            tournamentLeaderboardPlayerEnterBar.SetActive(false);
        }

        public void ClearBars()
        {
            for (int i = 0; i < tournamentLeaderboardPlayerBars.Count; i++)
            {
                tournamentLeaderboardPlayerBars[i].rankIcon.enabled = false;
                barsPool.ReturnObject(tournamentLeaderboardPlayerBars[i].gameObject);
            }

            tournamentLeaderboardPlayerBars.Clear();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(CountdownTimer());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            StopCoroutine(CountdownTimer());
        }

        public void UpdateView(JoinedTournamentData joinedTournament)
        {
            this.joinedTournament = joinedTournament;
            this.liveTournament = null;
            Populate(joinedTournament);
            Sort();
            infoBar.gameModeTooltipText.text = $"This is a {joinedTournament.type} tournament.";
        }

        public void UpdateView(LiveTournamentData liveTournament)
        {
            this.liveTournament = liveTournament;
            this.joinedTournament = null;
            Populate(liveTournament);
            Sort();
            infoBar.gameModeTooltipText.text = $"This is a {liveTournament.type} tournament.";
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
            item.UpdateItem(joinedTournament);
        }

        public void PopulateTournamentHeader(TournamentLiveItem item, LiveTournamentData liveTournament)
        {
            item.UpdateItem(liveTournament);
        }

        private void PopulateTournamentInfoBar()
        {
            TournamentLeaderboardInfoBar item = infoBar;

            item.rulesLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_RULES);
            item.totalScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_TOTAL_SCORE);

            item.columnHeaderRankLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_RANK);
            item.columnHeaderScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_TOTAL_PLAYER_SCORE);
            item.columnHeaderRewardsLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_REWARDS);

            infoBar.rulesTooltipButton.onClick.AddListener(OnRulesButtonClicked);
            infoBar.totalScoreTooltipButton.onClick.AddListener(OnTotalScoresButtonClicked);
            infoBar.gameModeTooltipButton.onClick.AddListener(OnGameModesButtonClicked);
        }

        public void PopulateFooter()
        {
            TournamentLeaderboardFooter item = footer;

            if (!storeSettingsModel.items.ContainsKey(item.itemToConsumeShortCode))
            {
                return;
            }

            var itemsOwned = playerModel.GetInventoryItemCount(item.itemToConsumeShortCode);
            var alreadyPlayed = joinedTournament != null;

            ticketStoreItem = storeSettingsModel.items[item.itemToConsumeShortCode];
            item.haveEnoughItems = itemsOwned > 0;
            item.haveEnoughGems = playerModel.gems >= ticketStoreItem.currency3Cost;
            item.youHaveLabel.text = $"{localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_YOU_HAVE)} {itemsOwned}/5";
            item.enterButtonFreePlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_FREE_PLAY);
            item.enterButtonTicketPlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_TICKET_PLAY);
            item.enterButtonTicketPlayCountText.text = "1";
            item.gemsCost.text = ticketStoreItem.currency3Cost.ToString();
            item.enterButtonFreePlayLabel.gameObject.SetActive(!alreadyPlayed);
            item.ticketPlayButtonGroup.gameObject.SetActive(alreadyPlayed);
            item.gemsBg.sprite = item.haveEnoughGems ? item.haveEnoughGemsSprite : item.notEnoughGemsSprite;
            item.gemsBg.gameObject.SetActive(!item.haveEnoughItems && alreadyPlayed);
        }

        private TournamentLeaderboardPlayerBar AddPlayerBar()
        {
            GameObject obj = barsPool.GetObject();
            TournamentLeaderboardPlayerBar item = obj.GetComponent<TournamentLeaderboardPlayerBar>();
            item.transform.SetParent(listContainer, false);
            AddPlayerBarListeners(item);
            item.gameObject.SetActive(true);
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

        void OnRulesButtonClicked()
        {
            infoBar.rulesTooltip.SetActive(!infoBar.rulesTooltip.activeSelf);
        }

        void OnTotalScoresButtonClicked()
        {
            infoBar.totalScoresTooltip.SetActive(!infoBar.totalScoresTooltip.activeSelf);
        }

        void OnGameModesButtonClicked()
        {
            infoBar.gameModesTooltip.SetActive(!infoBar.gameModesTooltip.activeSelf);
        }

        IEnumerator CountdownTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return waitForOneRealSecond;

                header.UpdateTime();
            }

            yield return null;
        }
    }
}
