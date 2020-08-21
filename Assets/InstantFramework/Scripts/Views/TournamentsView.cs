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
    public class TournamentsView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLeagueProfileStripSignal updateLeagueProfileStripSignal { get; set; }

        public Transform listContainer;
        public GameObject tournamentLiveItemPrefab;
        public GameObject tournamentUpcomingItemPrefab;
        public GameObject sectionTournamentUpcomingItems;

        // Button click signals
        public Signal<TournamentLiveItem> liveItemClickedSignal = new Signal<TournamentLiveItem>();
        public Signal<TournamentUpcomingItem> upcomingItemClickedSignal = new Signal<TournamentUpcomingItem>();

        private Dictionary<string, TournamentLiveItem> tournamentLiveItems = new Dictionary<string, TournamentLiveItem>();
        private Dictionary<string, TournamentUpcomingItem> tournamentUpcomingItems = new Dictionary<string, TournamentUpcomingItem>();

        public void Init()
        {
            LeagueProfileStripVO leagueProfileStripVO = new LeagueProfileStripVO();
            leagueProfileStripVO.playerLeagueTitle = "SILVER";
            leagueProfileStripVO.playerLeagueThumbnailImage = null;
            leagueProfileStripVO.playerRankCount = 5;
            leagueProfileStripVO.playerTrophiesCount = 100;
            leagueProfileStripVO.playerRankStatusImage = null;
            leagueProfileStripVO.tournamentCountdownTimer = "6d 5m";

            updateLeagueProfileStripSignal.Dispatch(leagueProfileStripVO);

            AddTournamentLiveItem();
            AddTournamentUpcomingItem();
            Sort();
        }

        private void Sort()
        {
            List<TournamentLiveItem> tournamentLiveItemsSort = new List<TournamentLiveItem>();
            List<TournamentUpcomingItem> tournamentUpcomingItemsSort = new List<TournamentUpcomingItem>();

            // Copy all tournament items into lists
            foreach(KeyValuePair<string, TournamentLiveItem> item in tournamentLiveItems)
            {
                tournamentLiveItemsSort.Add(item.Value);
            }
            foreach (KeyValuePair<string, TournamentUpcomingItem> item in tournamentUpcomingItems)
            {
                tournamentUpcomingItemsSort.Add(item.Value);
            }

            // Todo: Sort

            // Adust order
            int index = 0;
            for (int i = 0; i < tournamentLiveItemsSort.Count; i++)
            {
                tournamentLiveItemsSort[i].transform.SetSiblingIndex(index++);
            }

            sectionTournamentUpcomingItems.transform.SetSiblingIndex(index++);
            for (int i = 0; i < tournamentUpcomingItemsSort.Count; i++)
            {
                tournamentUpcomingItemsSort[i].transform.SetSiblingIndex(index++);
            }
        }

        public void AddTournamentLiveItem()
        {
            GameObject obj = GameObject.Instantiate(tournamentLiveItemPrefab);
            TournamentLiveItem item = obj.GetComponent<TournamentLiveItem>();

            item.bg.sprite = Resources.Load("AM.png") as Sprite;
            item.headingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_HEADING);
            item.subHeadingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_SUB_HEADING);
            item.tournamentImage.sprite = Resources.Load("AD") as Sprite;
            item.prizeImage.sprite = Resources.Load("AL") as Sprite;
            item.countdownTimerText.text = "2h 23n";
            item.playerTrophiesCountText.text = "8";
            item.playerRankCountText.text = "4";

            item.button.onClick.AddListener(() => liveItemClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            tournamentLiveItems.Add(item.name, item);
        }

        public void AddTournamentUpcomingItem()
        {
            GameObject obj = GameObject.Instantiate(tournamentUpcomingItemPrefab);
            TournamentUpcomingItem item = obj.GetComponent<TournamentUpcomingItem>();

            item.bg.sprite = Resources.Load("AM.png") as Sprite;
            item.tournamentImage.sprite = Resources.Load("AL") as Sprite;
            item.countdownTimerText.text = "5h 28n";

            item.button.onClick.AddListener(() => upcomingItemClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            tournamentUpcomingItems.Add(item.name, item);
        }
    }
}
