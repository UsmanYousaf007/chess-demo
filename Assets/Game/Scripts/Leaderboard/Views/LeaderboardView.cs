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

        public ScrollRect scrollRectChampionship;
        public ScrollRect scrollRectAllStars;

        public Text playerTitleLabel;
        public Text countdownTimerText;
        public Image playerTitleImg;
        public Button backButton;

        public Transform championshipLeaderboardListContainer;
        public Transform allStarLeaderboardListContainer;
        public GameObject championshipLeaderboardPlayerBarPrefab;
        public GameObject allStarLeaderboardPlayerBarPrefab;

        public Text rankText;
        public Text winningsText;
        public Text rewardsText;

        public GameObject pleaseWaitPanel;

        private GameObjectsPool championshipBarsPool;
        private List<LeaderboardPlayerBar> championshipleaderboardPlayerBars = new List<LeaderboardPlayerBar>();

        private GameObjectsPool allStarBarsPool;
        private List<LeaderboardPlayerBar> allStarPlayerBars = new List<LeaderboardPlayerBar>();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models 
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        //Signals
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getChampionshipTournamentLeaderboardSignal { get; set; }
        [Inject] public GetAllStarLeaderboardSignal getAllStarLeaderboardSignal { get; set; }

        public Signal backSignal = new Signal();
        public Signal<GetProfilePictureVO> loadPictureSignal = new Signal<GetProfilePictureVO>();

        private WaitForSecondsRealtime waitForOneRealSecond;
        private long endTimeUTCSeconds;
        private JoinedTournamentData _joinedTournament;
        private List<AllStarLeaderboardEntry> _allStarLeaderboardEntries;

        public void Init()
        {
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);
            allStarBarsPool = new GameObjectsPool(allStarLeaderboardPlayerBarPrefab, 50);

            championship.title.text = localizationService.Get(LocalizationKey.LEADERBOARD_CHAMPIONSHIP);
            world.title.text = localizationService.Get(LocalizationKey.LEADERBOARD_WORLD);

            rankText.text = localizationService.Get(LocalizationKey.LEADERBOARD_RANK);
            winningsText.text = localizationService.Get(LocalizationKey.LEADERBOARD_WINNINGS);
            rewardsText.text = localizationService.Get(LocalizationKey.LEADERBOARD_REWARDS);

            championship.button.onClick.AddListener(OnClickChampionship);
            world.button.onClick.AddListener(OnClickWorld);
            backButton.onClick.AddListener(OnBackButtonClicked);
            waitForOneRealSecond = new WaitForSecondsRealtime(1f);

            scrollRectChampionship.gameObject.SetActive(false);
            scrollRectChampionship.verticalNormalizedPosition = 1.0f;

            pleaseWaitPanel.SetActive(false);

            //rankText.text =
            //winningsText.text =
            //rewardsText.text =
        }

        public void Show(JoinedTournamentData joinedTournament)
        {
            _joinedTournament = joinedTournament;
            ClearBars(allStarPlayerBars, allStarBarsPool);
            if (_joinedTournament != null)
            {
                endTimeUTCSeconds = _joinedTournament.endTimeUTCSeconds;

                if (_joinedTournament.entries.Count > 0)
                {
                    PopulateEntries(_joinedTournament);
                }
                else
                {
                    pleaseWaitPanel.SetActive(true);
                }
            }
            //else
            //{
            //    getChampionshipTournamentLeaderboardSignal.Dispatch(tournamentsModel.GetJoinedTournament().id, false);
            //}

            SetupTab(championship, world);
            gameObject.SetActive(true);
            worldAlert.SetActive(!preferencesModel.allStarTabVisited);
            StartCoroutine(CountdownTimer());

            //scrollRectChampionship.gameObject.SetActive(false);
            scrollRectAllStars.gameObject.SetActive(false);
        }

        private void UpdateScrollViewChampionship(float value)
        {
            //scrollRectChampionship.gameObject.SetActive(true);
            scrollRectChampionship.verticalNormalizedPosition = value;
        }
        private void UpdateScrollViewAllStar(float value)
        {
            //scrollRectAllStars.gameObject.SetActive(true);
            scrollRectAllStars.verticalNormalizedPosition = value;
        }

        public void Hide()
        {
            StopCoroutine(CountdownTimer());
            gameObject.SetActive(false);
            scrollRectChampionship.gameObject.SetActive(false);
            scrollRectAllStars.gameObject.SetActive(false);
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

        public void UpdatePicture(string playerId, Sprite picture)
        {
            var playerBar = (from bar in championshipleaderboardPlayerBars
                             where bar.profile.playerId.Equals(playerId)
                             select bar).FirstOrDefault();

            var playerBarAllStar = (from bar in allStarPlayerBars
                             where bar.profile.playerId.Equals(playerId)
                             select bar).FirstOrDefault();

            if (playerBar != null)
            {
                playerBar.profile.SetProfilePicture(picture);
            }
            if (playerBarAllStar != null)
            {
                playerBarAllStar.profile.SetProfilePicture(picture);
            }
        }

        public void UpdateLeague()
        {
            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            playerTitleLabel.text = leagueAssets.typeName;
            playerTitleImg.sprite = leagueAssets.nameImg;
            playerTitleImg.SetNativeSize();
            playerTitleLabel.gameObject.SetActive(false);
            playerTitleImg.gameObject.SetActive(true);
        }

        public void PopulateEntries(JoinedTournamentData joinedTournament)
        {
            pleaseWaitPanel.SetActive(false);

            ClearBars(championshipleaderboardPlayerBars, championshipBarsPool);

            int itemBarsCount = championshipleaderboardPlayerBars.Count;
            if (itemBarsCount < joinedTournament.entries.Count)
            {
                for (int i = itemBarsCount; i < joinedTournament.entries.Count; i++)
                {
                    championshipleaderboardPlayerBars.Add(AddPlayerBar(championshipBarsPool, championshipLeaderboardListContainer));
                }
            }

            int playerIndex = -1;

            for (int i = 0; i < joinedTournament.entries.Count; i++)
            {
                PopulateBar(championshipleaderboardPlayerBars[i], joinedTournament.entries[i], joinedTournament.rewardsDict[i + 1]);

                if (joinedTournament.entries[i].publicProfile.playerId == playerModel.id)
                {
                    playerIndex = i;
                }
            }

            scrollRectChampionship.gameObject.SetActive(true);

            if (playerIndex >= -1 && playerIndex <= 6)
            {
                scrollRectChampionship.verticalNormalizedPosition = 1.0f;
            }
            else
            {
                float target = 1.0f - ((((float)playerIndex + 1f) / (float)joinedTournament.entries.Count));

                target -= (1.0f / (float)joinedTournament.entries.Count) * 3.0f;
                if (target < 0.0f) target = 0.0f;


                //scrollRectChampionship.verticalNormalizedPosition = target;

                iTween.ValueTo(gameObject,
                    iTween.Hash(
                    "from", scrollRectChampionship.verticalNormalizedPosition,
                    "to", target,
                    "time", 1f - target,
                    "onupdate", "UpdateScrollViewChampionship",
                    "onupdatetarget", gameObject
                    ));
            }
        }

        public void PopulateEntries(List<AllStarLeaderboardEntry> allStarLeaderboardEntries)
        {
            pleaseWaitPanel.SetActive(false);

            ClearBars(allStarPlayerBars, allStarBarsPool);

            int itemBarsCount = allStarPlayerBars.Count;
            if (itemBarsCount < allStarLeaderboardEntries.Count)
            {
                for (int i = itemBarsCount; i < allStarLeaderboardEntries.Count; i++)
                {
                    allStarPlayerBars.Add(AddPlayerBar(allStarBarsPool, allStarLeaderboardListContainer));
                }
            }

            int playerIndex = -1;

            for (int i = 0; i < allStarLeaderboardEntries.Count; i++)
            {
                PopulateBar(allStarPlayerBars[i], allStarLeaderboardEntries[i]);

                if (allStarLeaderboardEntries[i].publicProfile.playerId == playerModel.id)
                {
                    playerIndex = i;
                }
            }

            if (playerIndex >= -1 && playerIndex <= 6)
            {
                scrollRectChampionship.verticalNormalizedPosition = 1.0f;
            }
            else
            {
                float target = 1.0f - ((((float)playerIndex + 1f) / (float)allStarLeaderboardEntries.Count));

                target -= (1.0f / (float)allStarLeaderboardEntries.Count) * 3.0f;
                if (target < 0.0f) target = 0.0f;

                iTween.ValueTo(gameObject,
                    iTween.Hash(
                    "from", scrollRectAllStars.verticalNormalizedPosition,
                    "to", target,
                    "time", 1f - target,
                    "onupdate", "UpdateScrollViewAllStar",
                    "onupdatetarget", gameObject
                    ));
            }

            scrollRectAllStars.gameObject.SetActive(true);
        }

        public void ClearBars(List<LeaderboardPlayerBar> barsList, GameObjectsPool pool)
        {
            for (int i = 0; i < barsList.Count; i++)
            {
                barsList[i].rankIcon.enabled = false;
                barsList[i].playerRankCountText.color = Colors.WHITE;
                barsList[i].playerPanel.SetActive(false);
                RemovePlayerBarListeners(barsList[i]);
                pool.ReturnObject(barsList[i].gameObject);
            }

            barsList.Clear();
        }

        public void UpdateView(JoinedTournamentData joinedTournament)
        {
            if (joinedTournament != null && championshipleaderboardPlayerBars.Count == 0)
            {
                pleaseWaitPanel.SetActive(true);

                _joinedTournament = joinedTournament;

                PopulateEntries(_joinedTournament);
                Sort();

                endTimeUTCSeconds = _joinedTournament.endTimeUTCSeconds;
                StartCoroutine(CountdownTimer());
            }
        }

        public void ResetChampionshipView()
        {
            ClearBars(championshipleaderboardPlayerBars, championshipBarsPool);
            ClearBars(allStarPlayerBars, allStarBarsPool);
        }

        public void UpdateView(List<AllStarLeaderboardEntry> allStarLeaderboardEntries)
        {
            if (allStarLeaderboardEntries != null && allStarPlayerBars.Count == 0)
            {
                _allStarLeaderboardEntries = allStarLeaderboardEntries;
                PopulateEntries(_allStarLeaderboardEntries);
                Sort();
            }
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
        }

        private LeaderboardPlayerBar AddPlayerBar(GameObjectsPool pool, Transform parent)
        {
            GameObject obj = pool.GetObject();
            LeaderboardPlayerBar item = obj.GetComponent<LeaderboardPlayerBar>();
            item.transform.SetParent(parent, false);
            //AddPlayerBarListeners(item);
            item.gameObject.SetActive(true);
            return item;
        }

        private void PopulateBar(LeaderboardPlayerBar playerBar, TournamentEntry entry, TournamentReward reward)
        {
            var isPlayerStrip = entry.publicProfile.playerId.Equals(playerModel.id);

            playerBar.Populate(entry, reward, isPlayerStrip);

            Sprite picture = picsModel.GetPlayerPic(entry.publicProfile.playerId);
            if (picture != null)
            {
                playerBar.profile.SetProfilePicture(picture);
            }
            else
            {
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
        }

        private void PopulateBar(LeaderboardPlayerBar playerBar, AllStarLeaderboardEntry entry)
        {
            var isPlayerStrip = entry.publicProfile.playerId.Equals(playerModel.id);

            playerBar.Populate(entry, isPlayerStrip);

            Sprite picture = picsModel.GetPlayerPic(entry.publicProfile.playerId);
            if (picture != null)
            {
                playerBar.profile.SetProfilePicture(picture);
            }
            else
            {
                var loadPicture = (!string.IsNullOrEmpty(entry.publicProfile.uploadedPicId)
                    || !string.IsNullOrEmpty(entry.publicProfile.facebookUserId))
                    && entry.publicProfile.profilePicture == null;

                if (loadPicture)
                {
                    var loadPicVO = new GetProfilePictureVO();
                    loadPicVO.playerId = entry.playerId;
                    loadPicVO.uploadedPicId = entry.publicProfile.uploadedPicId;
                    loadPicVO.facebookUserId = entry.publicProfile.facebookUserId;
                    loadPicVO.saveOnDisk = false;
                    loadPictureSignal.Dispatch(loadPicVO);
                }
            }
        }

        private void AddPlayerBarListeners(LeaderboardPlayerBar playerBar)
        {
            playerBar.button?.onClick.AddListener(() =>
            {
                //playerBarClickedSignal.Dispatch(playerBar);
                audioService.PlayStandardClick();
            });

            playerBar.chestButton?.onClick.AddListener(() =>
            {
                //playerBarChestClickSignal.Dispatch(playerBar.reward);
                audioService.PlayStandardClick();
            });
        }

        private void RemovePlayerBarListeners(LeaderboardPlayerBar playerBar)
        {
            playerBar.button?.onClick.RemoveAllListeners();
            playerBar.chestButton?.onClick.RemoveAllListeners();
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }

        private void OnClickChampionship()
        {
            audioService.PlayStandardClick();

            /*ClearBars(allStarPlayerBars, allStarBarsPool);

            if (_joinedTournament != null && _joinedTournament.entries.Count > 0)
            {
                PopulateEntries(_joinedTournament);
            }
            else
            {
                getChampionshipTournamentLeaderboardSignal.Dispatch(tournamentsModel.GetJoinedTournament().id, false);
            }
            */

            SetupTab(championship, world);
            scrollRectChampionship.gameObject.SetActive(true);
        }

        public void OnClickWorld()
        {
            audioService.PlayStandardClick();
            
            //ClearBars(championshipleaderboardPlayerBars, championshipBarsPool);

            if (_allStarLeaderboardEntries != null && _allStarLeaderboardEntries.Count > 0)
            {
                if (allStarPlayerBars.Count == 0)
                {
                    PopulateEntries(_allStarLeaderboardEntries);
                }
            }
            else
            {
                getAllStarLeaderboardSignal.Dispatch();
            }

            worldAlert.SetActive(false);
            SetupTab(world, championship);
            scrollRectAllStars.gameObject.SetActive(true);
            preferencesModel.allStarTabVisited = true;
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
                UpdateTime();
                yield return waitForOneRealSecond;
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
                countdownTimerText.text = "Ends in " + timeLeftText;
            }
            else
            {
                countdownTimerText.text = "Ended";
            }
        }
    }
}
