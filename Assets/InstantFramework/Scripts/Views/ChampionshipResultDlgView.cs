/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using TurboLabz.InstantGame;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using System.Linq;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class ChampionshipResultDlgView : View
    {
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        [Inject] public IPlayerModel playerModel { get; set; }

        // Local Signals
        public Signal continueBtnClickedSignal = new Signal();
        public Signal<GetProfilePictureVO> loadPictureSignal = new Signal<GetProfilePictureVO>();

        // Set in inspector
        [SerializeField] private TextMeshProUGUI headingText;
        [SerializeField] private Text playerTitleLabel;
        [SerializeField] private Image playerTitleImg;
        [SerializeField] protected Button continueButton;
        [SerializeField] protected Text continueButtonText;
        [SerializeField] private Transform championshipLeaderboardListContainer;
        [SerializeField] protected GameObject championshipLeaderboardPlayerBarPrefab;
        [SerializeField] protected ScrollRect scrollView;

        protected GameObjectsPool championshipBarsPool;
        protected List<LeaderboardPlayerBar> championshipleaderboardPlayerBars = new List<LeaderboardPlayerBar>();

        public ScrollRect scrollRectChampionship;

        public virtual void Init()
        {
            scrollRectChampionship = scrollView;
            championshipBarsPool = new GameObjectsPool(championshipLeaderboardPlayerBarPrefab, 50);

            headingText.text = "Last Week Standings";
            continueButton.onClick.AddListener(() => {
                audioService.PlayStandardClick();
                continueBtnClickedSignal.Dispatch();
                    });
        }

        public ScrollRect GetScrollView()
        {
            return scrollView;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void UpdateView(string playerId, JoinedTournamentData joinedTournament)
        {
            PopulateEntries(playerId, joinedTournament);
            Sort();
        }


        public void UpdatePicture(string playerId, Sprite picture)
        {
            var playerBar = (from bar in championshipleaderboardPlayerBars
                             where bar.profile.playerId.Equals(playerId)
                             select bar).FirstOrDefault();

            if (playerBar != null)
            {
                playerBar.profile.SetProfilePicture(picture);
            }
        }

        private void UpdateScrollViewChampionship(float value)
        {
            scrollRectChampionship.verticalNormalizedPosition = value;
        }

        public void UpdateContinueButtonText(bool playerRewarded)
        {
            /// TODO: Add localizaztion here.
            if (playerRewarded)
            {
                continueButtonText.text = "Collect Reward";
            }
            else
            {
                continueButtonText.text = "Continue";
            }
        }

        public void ResetView()
        {
            ClearBars(championshipleaderboardPlayerBars, championshipBarsPool);
        }

        public void PopulateEntries(string playerId, JoinedTournamentData joinedTournament)
        {
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
                PopulateBar(playerId, championshipleaderboardPlayerBars[i], joinedTournament.entries[i], joinedTournament.rewardsDict[i + 1]);

                if (joinedTournament.entries[i].publicProfile.playerId == playerModel.id)
                {
                    playerIndex = i;
                }
            }

            if (playerIndex == -1)
            {
                scrollRectChampionship.verticalNormalizedPosition = 1.0f;
            }
            else
            {
                if (playerIndex == 0)
                {
                    scrollRectChampionship.verticalNormalizedPosition = 1.0f;
                }
                else
                {
                    float target = 1.0f - ((((float)playerIndex + 1) / (float)joinedTournament.entries.Count));

                    iTween.ValueTo(gameObject,
                        iTween.Hash(
                        "from", 0f,
                        "to", target,
                        "time", 0.1f,
                        "onupdate", "UpdateScrollViewChampionship",
                        "onupdatetarget", gameObject
                        ));
                }
            }
        }

        protected void Sort()
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

        public void UpdateLeagueTitle(IPlayerModel playerModel, ITournamentsModel tournamentsModel)
        {
            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            if (playerModel.league == 0)
            {
                playerTitleLabel.text = "TRAINEE";
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

        protected void ClearBars(List<LeaderboardPlayerBar> barsList, GameObjectsPool pool)
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

        protected LeaderboardPlayerBar AddPlayerBar(GameObjectsPool pool, Transform parent)
        {
            GameObject obj = pool.GetObject();
            LeaderboardPlayerBar item = obj.GetComponent<LeaderboardPlayerBar>();
            item.transform.SetParent(parent, false);
            AddPlayerBarListeners(item);
            item.gameObject.SetActive(true);
            return item;
        }

        protected void AddPlayerBarListeners(LeaderboardPlayerBar playerBar)
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

        protected void RemovePlayerBarListeners(LeaderboardPlayerBar playerBar)
        {
            playerBar.button?.onClick.RemoveAllListeners();
            playerBar.chestButton?.onClick.RemoveAllListeners();
        }

        protected void PopulateBar(string playerId, LeaderboardPlayerBar playerBar, TournamentEntry entry, TournamentReward reward)
        {
            var isPlayerStrip = entry.publicProfile.playerId.Equals(playerId);

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
    }
}