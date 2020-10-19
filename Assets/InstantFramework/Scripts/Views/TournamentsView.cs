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
    [CLSCompliant(false)]
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
        [Inject] public INotificationsModel notificationsModel { get; set; }

        // Dispatch Signal
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLeagueProfileStripSignal updateLeagueProfileStripSignal { get; set; }
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }
        public Signal<TournamentReward> playerBarChestClickSignal = new Signal<TournamentReward>();

        public Transform listContainer;
        public GameObject tournamentLiveItemPrefab;
        public GameObject tournamentUpcomingItemPrefab;
        public GameObject sectionTournamentUpcomingItems;
        public Text upcomingTournamentsText;

        // Button click signals
        public Signal<TournamentLiveItem> liveItemClickedSignal = new Signal<TournamentLiveItem>();
        public Signal<LiveTournamentData> upcomingItemClickedSignal = new Signal<LiveTournamentData>();

        private List<TournamentLiveItem> tournamentLiveItems = new List<TournamentLiveItem>();
        private List<TournamentUpcomingItem> tournamentUpcomingItems = new List<TournamentUpcomingItem>();
        private GameObjectsPool liveItemsPool;
        private GameObjectsPool upcomingItemsPool;

        private WaitForSecondsRealtime waitForOneRealSecond;

        public void Init()
        {
            upcomingTournamentsText.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING);
            liveItemsPool = new GameObjectsPool(tournamentLiveItemPrefab);
            upcomingItemsPool = new GameObjectsPool(tournamentUpcomingItemPrefab);

            waitForOneRealSecond = new WaitForSecondsRealtime(1f);
        }

        public void Populate()
        {
            var joinedTournaments = tournamentsModel.joinedTournaments;
            var openTournaments = tournamentsModel.openTournaments;
            var upcomingTournaments = tournamentsModel.upcomingTournaments;

            // Joined tournaments
            for (int i = 0; i < joinedTournaments.Count; i++)
            {
                if (joinedTournaments[i].ended == false)
                {
                    var joinedLiveItem = AddLiveTournamentItemPrefab();
                    joinedLiveItem.gameObject.SetActive(true);
                    PopulateTournamentLiveItem(joinedLiveItem, joinedTournaments[i]);
                    tournamentLiveItems.Add(joinedLiveItem);
                }
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
            showBottomNavSignal.Dispatch(true);
            gameObject.SetActive(true);
            StartCoroutine(CountdownTimer());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            StopCoroutine(CountdownTimer());
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

            LayoutRebuilder.ForceRebuildLayoutImmediate(listContainer.GetComponent<RectTransform>());
        }

        private void ClearAllItems()
        {
            for (int i = 0; i < tournamentLiveItems.Count; i++)
            {
                liveItemsPool.ReturnObject(tournamentLiveItems[i].gameObject);
                tournamentLiveItems[i].button.onClick.RemoveAllListeners();
            }

            for (int i = 0; i < tournamentUpcomingItems.Count; i++)
            {
                upcomingItemsPool.ReturnObject(tournamentUpcomingItems[i].gameObject);
                tournamentUpcomingItems[i].button.onClick.RemoveAllListeners();
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
            item.button.onClick.AddListener(() =>
            {
                liveItemClickedSignal.Dispatch(item);
                audioService.PlayStandardClick();
            });

            return item;
        }

        public void PopulateTournamentLiveItem(TournamentLiveItem item, JoinedTournamentData joinedTournament)
        {
            item.prizeBtn.onClick.AddListener(() =>
            {
                playerBarChestClickSignal.Dispatch(joinedTournament.grandPrize);
                audioService.PlayStandardClick();
            });
            item.UpdateItem(joinedTournament);
        }

        public void PopulateTournamentLiveItem(TournamentLiveItem item, LiveTournamentData liveTournament)
        {
            item.prizeBtn.onClick.AddListener(() =>
            {
                playerBarChestClickSignal.Dispatch(liveTournament.grandPrize);
                audioService.PlayStandardClick();
            });
            item.UpdateItem(liveTournament);
        }

        public TournamentUpcomingItem AddTournamentUpcomingItemPrefab()
        {
            GameObject obj = upcomingItemsPool.GetObject();
            obj.transform.SetParent(listContainer, false);

            TournamentUpcomingItem item = obj.GetComponent<TournamentUpcomingItem>();
            item.Init();
            //item.Reset();

            return item;
        }

        public void PopulateTournamentUpcomingItem(TournamentUpcomingItem item, LiveTournamentData liveTournament)
        {
            var getNotified = notificationsModel.IsNotificationRegistered($"{liveTournament.type}_upcoming");

            item.UpdateItem(liveTournament, getNotified);
            item.startsInLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING_STARTS_IN);
            item.notificationEnabledText.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING_NOTICATION_ENABLED);
            item.getNotifiedLabel.text = localizationService.Get(LocalizationKey.TOURNAMENT_UPCOMING_GET_NOTIFIED);

            item.notificationEnabledText.gameObject.SetActive(false);

            item.button.onClick.AddListener(() =>
            {
                upcomingItemClickedSignal.Dispatch(liveTournament);
                item.button.gameObject.SetActive(false);
                item.DisableNotificationEnabledText();
                audioService.PlayStandardClick();
            });
        }

        IEnumerator CountdownTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return waitForOneRealSecond;

                for (int i = 0; i < tournamentLiveItems.Count; i++)
                {
                    tournamentLiveItems[i].UpdateTime();
                }

                for (int i = 0; i < tournamentUpcomingItems.Count; i++)
                {
                    tournamentUpcomingItems[i].UpdateTime();
                }
            }

            yield return null;
        }
    }
}
