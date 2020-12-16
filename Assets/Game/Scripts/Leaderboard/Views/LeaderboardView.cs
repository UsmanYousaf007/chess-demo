using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using System;
using strange.extensions.signal.impl;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using TMPro;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    [Serializable]
    public class LeaderboardReward
    {
        public string type;
        public int quantity;
        public int trophies;
        public string chestType;
        public int gems;
        public int hints;
        public int ratingBoosters;
    }

    [CLSCompliant(false)]
    public class LeaderboardView : View
    {
        [Serializable]
        public class LeaderboardTab
        {
            public Button button;
            public Text title;
            public Image selected;
            public GameObject tab;
            public Image unSelected;
        }

        public LeaderboardTab championship;
        public LeaderboardTab world;
        public GameObject worldAlert;

        public Text playerTitleLabel;
        public Text countdownTimerText;
        public Image playerTitleImg;
        public Button backButton;

        public Transform listContainer;
        public GameObject leaderboardPlayerBarPrefab;
        public ScrollRect scrollView;

        public Text rankText;
        public Text winningsText;
        public Text rewardsText;

        private GameObjectsPool barsPool;
        private List<LeaderboardPlayerBar> leaderboardPlayerBars = new List<LeaderboardPlayerBar>();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models 
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        //Signals
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }
        public Signal backSignal = new Signal();
        public Signal<GetProfilePictureVO> loadPictureSignal = new Signal<GetProfilePictureVO>();

        private WaitForSecondsRealtime waitForOneRealSecond;
        private long endTimeUTCSeconds;

        public void Init()
        {
            barsPool = new GameObjectsPool(leaderboardPlayerBarPrefab, 50);
            championship.title.text = localizationService.Get(LocalizationKey.LEADERBOARD_CHAMPIONSHIP);
            world.title.text = localizationService.Get(LocalizationKey.LEADERBOARD_WORLD);

            rankText.text = localizationService.Get(LocalizationKey.LEADERBOARD_RANK);
            winningsText.text = localizationService.Get(LocalizationKey.LEADERBOARD_WINNINGS);
            rewardsText.text = localizationService.Get(LocalizationKey.LEADERBOARD_REWARDS);

            championship.button.onClick.AddListener(OnClickChampionship);
            world.button.onClick.AddListener(OnClickWorld);
            backButton.onClick.AddListener(OnBackButtonClicked);
            waitForOneRealSecond = new WaitForSecondsRealtime(1f);

            //rankText.text =
            //winningsText.text =
            //rewardsText.text =
        }

        public void Show()
        {
            //endTimeUTCSeconds = liveTournamentData.endTimeUTCSeconds - (TournamentConstants.BUFFER_TIME_MINS * 60);
            SetupTab(championship, world);
            gameObject.SetActive(true);
            worldAlert.SetActive(!preferencesModel.themesTabVisited);
            //StartCoroutine(CountdownTimer());
        }

        public void Hide()
        {
            //StopCoroutine(CountdownTimer());
            gameObject.SetActive(false);
        }

        public void OnDataAvailable(bool isAvailable)
        {
            if (!isAvailable)
            {
                
            }
            else
            {
                
            }
        }

        public void UpdateView(ProfileVO vo)
        {
            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            if (playerModel.league == 0)
            {
                playerTitleLabel.text = "NO RANK";
                playerTitleLabel.gameObject.SetActive(true);
                playerTitleImg.gameObject.SetActive(false);
            }
            else
            {
                playerTitleLabel.text = leagueAssets.typeName;
                playerTitleImg.sprite = leagueAssets.nameImg;
                playerTitleLabel.gameObject.SetActive(false);
                playerTitleImg.gameObject.SetActive(true);
            }

        }

        public void PopulateEntries(LiveTournamentData liveTournament)
        {
            ClearBars();

            int itemBarsCount = leaderboardPlayerBars.Count;
            if (itemBarsCount <= 50)
            {
                for (int i = itemBarsCount; i < 50; i++)
                {
                    leaderboardPlayerBars.Add(AddPlayerBar());
                }
            }

            for (int i = 0; i < 50; i++)
            {
                var playerBar = leaderboardPlayerBars[i];
                //PopulateBar(playerBar, i + 1, liveTournament.rewardsDict[i + 1]);
            }
        }

        public void ClearBars()
        {
            for (int i = 0; i < leaderboardPlayerBars.Count; i++)
            {
                leaderboardPlayerBars[i].rankIcon.enabled = false;
                leaderboardPlayerBars[i].playerRankCountText.color = Colors.WHITE;
                leaderboardPlayerBars[i].playerPanel.SetActive(false);
                RemovePlayerBarListeners(leaderboardPlayerBars[i]);
                barsPool.ReturnObject(leaderboardPlayerBars[i].gameObject);
            }

            leaderboardPlayerBars.Clear();
        }

        public void UpdateView()
        {
            //PopulateEntries(joinedTournament);
            Sort();
        }

        private void Sort()
        {
            List<LeaderboardPlayerBar> leaderboardPlayerBars = new List<LeaderboardPlayerBar>();

            // Todo: Sort

            // Adust order
            int index = 0;
            for (int i = 0; i < leaderboardPlayerBars.Count; i++)
            {
                leaderboardPlayerBars[i].transform.SetSiblingIndex(index++);
            }

            scrollView.verticalNormalizedPosition = 1;
        }

        private LeaderboardPlayerBar AddPlayerBar()
        {
            GameObject obj = barsPool.GetObject();
            LeaderboardPlayerBar item = obj.GetComponent<LeaderboardPlayerBar>();
            item.transform.SetParent(listContainer, false);
            AddPlayerBarListeners(item);
            item.gameObject.SetActive(true);
            return item;
        }

        private void PopulateBar(LeaderboardPlayerBar playerBar, TournamentEntry entry, LeaderboardReward reward)
        {
            var isPlayerStrip = entry.publicProfile.playerId.Equals(playerModel.id);

            playerBar.Populate(entry, reward, isPlayerStrip);

            var loadPicture = (!string.IsNullOrEmpty(entry.publicProfile.uploadedPicId)
                || !string.IsNullOrEmpty(entry.publicProfile.facebookUserId))
                && entry.publicProfile.profilePicture == null;

            if (loadPicture)
            {
                var loadPicVO = new GetProfilePictureVO();
                loadPicVO.playerId = entry.publicProfile.playerId;
                loadPicVO.uploadedPicId = entry.publicProfile.uploadedPicId;
                loadPicVO.facebookUserId = entry.publicProfile.facebookUserId;
                loadPicVO.saveOnDisk = false;
                loadPictureSignal.Dispatch(loadPicVO);
            }
        }

        private void PopulateBar(LeaderboardPlayerBar playerBar, int rank, LeaderboardReward reward)
        {
            playerBar.Populate(rank, reward);
        }

        private void AddPlayerBarListeners(LeaderboardPlayerBar playerBar)
        {
            playerBar.button.onClick.AddListener(() =>
            {
                //playerBarClickedSignal.Dispatch(playerBar);
                audioService.PlayStandardClick();
            });

            playerBar.chestButton.onClick.AddListener(() =>
            {
                //playerBarChestClickSignal.Dispatch(playerBar.reward);
                audioService.PlayStandardClick();
            });
        }

        private void RemovePlayerBarListeners(LeaderboardPlayerBar playerBar)
        {
            playerBar.button.onClick.RemoveAllListeners();
            playerBar.chestButton.onClick.RemoveAllListeners();
        }

        private void OnBackButtonClicked()
        {
            backSignal.Dispatch();
        }

        private void OnClickChampionship()
        {
            audioService.PlayStandardClick();
            SetupTab(championship, world);
        }

        public void OnClickWorld()
        {
            audioService.PlayStandardClick();
            preferencesModel.themesTabVisited = true;
            worldAlert.SetActive(false);
            SetupTab(world, championship);
        }

        private void SetupTab(LeaderboardTab newTab, LeaderboardTab oldTab)
        {
            newTab.selected.enabled = true;
            oldTab.selected.enabled = false;
            newTab.tab.SetActive(true);
            oldTab.tab.SetActive(false);
            newTab.unSelected.enabled = false;
            oldTab.unSelected.enabled = true;
        }

        IEnumerator CountdownTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return waitForOneRealSecond;

                UpdateTime();
            }

            yield return null;
        }

        public void UpdateTime()
        {
            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (timeLeft > 0)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                countdownTimerText.text = timeLeftText;
            }
            else
            {
                countdownTimerText.text = "0:00";
            }
        }
    }
}
