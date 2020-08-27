/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public Transform listContainer;
        public GameObject tournamentLeaderboardPlayerBarPrefab;
        public TournamentLiveItem header;
        public TournamentLeaderboardInfoBar infoBar;
        public TournamentLeaderboardFooter footer;

        // Player bar click signal
        [HideInInspector]
        public Signal<TournamentLeaderboardPlayerBar> playerBarClickedSignal = new Signal<TournamentLeaderboardPlayerBar>();
        private Dictionary<string, TournamentLeaderboardPlayerBar> tournamentLeaderboardPlayerBars = new Dictionary<string, TournamentLeaderboardPlayerBar>();

        public void Init()
        {
            //PopulateTournamentInfoBar();
            //PopulateTournamentHeader();
            //PopulateFooter();

            //AddPlayerBar();
            //AddPlayerBar();
            //AddPlayerBar();
            //AddPlayerBar();
            //AddPlayerBar();
            //AddPlayerBar();
            //Sort();
        }

        private void Sort()
        {
            List<TournamentLeaderboardPlayerBar> items = new List<TournamentLeaderboardPlayerBar>();

            // Copy all player bars into a list
            foreach (KeyValuePair<string, TournamentLeaderboardPlayerBar> item in tournamentLeaderboardPlayerBars)
            {
                items.Add(item.Value);
            }

            // Todo: Sort

            // Adust order
            int index = 0;
            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.SetSiblingIndex(index++);
            }
        }

        private void PopulateTournamentHeader()
        {
            TournamentLiveItem item = header;

            item.bg.sprite = Resources.Load("AM.png") as Sprite;
            item.headingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_HEADING);
            item.subHeadingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_SUB_HEADING);
            item.tournamentImage.sprite = Resources.Load("AD") as Sprite;
            item.prizeImage.sprite = Resources.Load("AL") as Sprite;
            item.countdownTimerText.text = "2h 23n";
            item.grandPrizeTrophiesCountText.text = "8";
            item.playerRankCountText.text = "4";
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

        public void AddPlayerBar()
        {
            GameObject obj = GameObject.Instantiate(tournamentLeaderboardPlayerBarPrefab);
            TournamentLeaderboardPlayerBar item = obj.GetComponent<TournamentLeaderboardPlayerBar>();

            item.playerNameText.text = "Radio Monkey";
            item.playerScoreCountText.text = "4384";
            item.playerRankCountText.text = "23";
            item.trophiesRewardCountText.text = "50";

            item.rankIcon.gameObject.SetActive(false);

            item.button.onClick.AddListener(() => playerBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            tournamentLeaderboardPlayerBars.Add(item.name + tournamentLeaderboardPlayerBars.Count.ToString(), item);
        }
    }
}
