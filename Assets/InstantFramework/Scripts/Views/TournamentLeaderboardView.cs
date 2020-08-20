/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.mediation.impl;
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
        public GameObject header;
        public GameObject infoBar;
        public GameObject footer;

        public GameObject tournamentLeaderboardPlayerBarPrefab;

        public void Init()
        {
            PopulateTournamentInfoBar();
            PopulateTournamentHeader();
            PopulateFooter();

            AddPlayerBar();
        }

        private void Sort()
        {
        }

        private void PopulateTournamentHeader()
        {
            TournamentLiveItem item = header.GetComponent<TournamentLiveItem>();

            item.bg.sprite = Resources.Load("AM.png") as Sprite;
            item.headingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_HEADING);
            item.subHeadingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_SUB_HEADING);
            item.tournamentImage.sprite = Resources.Load("AD") as Sprite;
            item.prizeImage.sprite = Resources.Load("AL") as Sprite;
            item.countdownTimerText.text = "2h 23n";
            item.playerTrophiesCountText.text = "8";
            item.playerRankCountText.text = "4";
        }

        private void PopulateTournamentInfoBar()
        {
            TournamentLeaderboardInfoBar item = infoBar.GetComponent<TournamentLeaderboardInfoBar>();

            item.rulesLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_RULES);
            item.totalScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_TOTAL_SCORE);

            item.columnHeaderRankLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_RANK);
            item.columnHeaderScoreLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_TOTAL_PLAYER_SCORE);
            item.columnHeaderRewardsLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_COLUMN_HEADER_REWARDS);
        }

        private void PopulateFooter()
        {
            TournamentLeaderboardFooter item = footer.GetComponent<TournamentLeaderboardFooter>();

            item.bg.sprite = Resources.Load("AM.png") as Sprite;

            item.youHaveLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_YOU_HAVE);
            item.enterButtonFreePlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_FREE_PLAY);
            item.enterButtonTicketPlayLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LEADERBOARD_FOOTER_TICKET_PLAY);

            item.ticketsCountText.text = "3/5";
            item.enterButtonTicketPlayCountText.text = "12";

            item.freePlayButtonGroup.gameObject.SetActive(false);
            item.ticketPlayButtonGroup.gameObject.SetActive(true);

            /*
                public Button enterButton;
            */
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

            /* public Button button; */

            item.transform.SetParent(listContainer, false);
            //tournamentLiveItems.Add(item.name, item);

        }

        public void AddTournamentLiveItem()
        {
        }
    }
}
