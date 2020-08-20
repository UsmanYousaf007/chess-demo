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

        private GameObject tournamentLiveItemPrefab;
        private GameObject tournamentUpcomingItemPrefab;
        private Dictionary<string, TournamentLiveItem> tournametLiveItems = new Dictionary<string, TournamentLiveItem>();
        private Dictionary<string, TournamentUpcomingItem> tournametUpcomingItems = new Dictionary<string, TournamentUpcomingItem>();

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

            LoadAssets();


            AddTournamentLiveItem();
        }

        public void LoadAssets()
        {
            tournamentLiveItemPrefab = Resources.Load("TournamentLiveItem") as GameObject;
            tournamentUpcomingItemPrefab = Resources.Load("TournamentUpcomingItem") as GameObject;
        }

        public void Show() 
        {
        }

        public void Hide()
        {
            gameObject.SetActive(false); 
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void AddTournamentLiveItem()
        {
            GameObject obj = GameObject.Instantiate(tournamentLiveItemPrefab);
            TournamentLiveItem item = obj.GetComponent<TournamentLiveItem>();

            item.headingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_HEADING);
            item.subHeadingLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_LIVE_ITEM_SUB_HEADING);
            item.tournamentImage = Resources.Load("AD") as Image;
            item.prizeImage = Resources.Load("AL") as Image; ;
            item.countdownTimerText.text = "2h 23n";
            item.playerTrophiesCountText.text = "8";
            item.playerRankCountText.text = "4";
            item.button.onClick.AddListener(OnTournamentLiveItemClicked);

            item.transform.SetParent(listContainer, false);
            tournametLiveItems.Add(item.name, item);
        }

        public void AddTournamentUpcomingItem()
        {
            GameObject obj = GameObject.Instantiate(tournamentLiveItemPrefab);
            TournamentUpcomingItem item = obj.GetComponent<TournamentUpcomingItem>();

            item.tournamentImage = null;
            item.countdownTimerText.text = "5h 28n";
            item.button.onClick.AddListener(OnTournamentUpcomingItemClicked);

            item.transform.SetParent(listContainer, false);
            tournametUpcomingItems.Add(item.name, item);
        }

        public void OnTournamentLiveItemClicked()
        {
            TLUtils.LogUtil.Log("TournamentsView::OnTournamentLiveItemClicked()");
        }

        public void OnTournamentUpcomingItemClicked()
        {
            TLUtils.LogUtil.Log("TournamentsView::OnTournamentUpcomingItemClicked()");
        }


    }
}
