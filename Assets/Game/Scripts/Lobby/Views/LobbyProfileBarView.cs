﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LobbyProfileBarView : View
    {
        public TMP_Text playerName;
        public Text playerTitleLabel;
        public TMP_Text eloScoreValue;
        public TMP_Text currentTrophies;
        public Image playerFlag;
        public TMP_Text leaderboardText;
        public Button leaderboardButton;
        public Image playerTitleImg;
        public Button chestButton;
        public TMP_Text chestTimeText;
        public TMP_Text leaderboardTimer;
        public GameObject championShipTrophies;
        public RectTransform[] layouts;
        public Image chestVideoIcon;
        public Sprite videoAvailable;
        public Sprite videoNotAvailable;
        public TMP_Text chestTapToOpen;
        public GameObject chestTimer;

        private long endTimeUTCSeconds;
        private long chestTimeUTC;
        private bool chestTimeOver;

        //Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        //Signals
        public Signal leaderboardButtonClickedSignal = new Signal();
        public Signal chestButtonClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        [HideInInspector] public bool isVideoAvailable;

        public void Init()
        {
            leaderboardButton.onClick.AddListener(OnLeaderboardBtnClicked);
            chestButton.onClick.AddListener(OnChestButtonClicked);
        }

        public void UpdateView(ProfileVO vo)
        {
            playerName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);
            championShipTrophies.SetActive(vo.trophies2 > 0);
            currentTrophies.text = vo.trophies2.ToString();

            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            if (leagueAssets != null)
            {
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
                    playerTitleImg.SetNativeSize();
                    playerTitleLabel.gameObject.SetActive(false);
                    playerTitleImg.gameObject.SetActive(true);
                }
            }

            SetupChampionshipTimer();
            StartCoroutine(ChampionshipTimer());

            isVideoAvailable = adsService.IsRewardedVideoAvailable(AdPlacements.Rewarded_lobby_chest);
            SetupChest();
            StartCoroutine(CountdownChestTimer());
            RebuildLayout();
        }

        public void SetupChampionshipTimer()
        {
            var joinedTournament = tournamentsModel.GetJoinedTournament();
            if (joinedTournament != null)
            {
                endTimeUTCSeconds = joinedTournament.endTimeUTCSeconds;
            }
            else
            {
                leaderboardTimer.text = "0s";
            }
        }

        public void SetupChest()
        {
            var isUnlocked = playerModel.chestUnlockTimestamp <= backendService.serverClock.currentTimestamp;
            chestTimeUTC = playerModel.chestUnlockTimestamp;
            SetupChestState(isUnlocked);
            SetupVideoIcon(isVideoAvailable, isUnlocked);
        }

        public void Hide()
        {
            StopCoroutine(ChampionshipTimer());
            StopCoroutine(CountdownChestTimer());
        }

        public void UpdateEloScores(EloVO vo)
        {
            eloScoreValue.text = vo.playerEloScore.ToString();
        }

        void OnLeaderboardBtnClicked()
        {
            audioService.PlayStandardClick();
            leaderboardButtonClickedSignal.Dispatch();
        }

        void OnChestButtonClicked()
        {
            audioService.PlayStandardClick();
            chestButtonClickedSignal.Dispatch();
        }

        public IEnumerator ChampionshipTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                UpdateLeaderboardTime();
                yield return new WaitForSecondsRealtime(1);
            }

            yield return null;
        }

        void UpdateLeaderboardTime()
        {
            long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (timeLeft > 0)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                leaderboardTimer.text = timeLeftText;
            }
            else
            {
                leaderboardTimer.text = "0s";
            }
        }

        IEnumerator CountdownChestTimer()
        {
            while (gameObject.activeInHierarchy)
            {
                UpdateChestTime();
                yield return new WaitForSecondsRealtime(1);
            }

            yield return null;
        }

        void UpdateChestTime()
        {
            var timeLeft = chestTimeUTC - backendService.serverClock.currentTimestamp;
            if (timeLeft > 0)
            {
                timeLeft -= 1000;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft));
                chestTimeText.text = timeLeftText;

                if (chestTimeOver)
                {
                    chestTimeOver = false;
                    RebuildLayout();
                }
            }
            else
            {
                chestTimeText.text = "0s";
                chestTimeOver = true;
                SetupChest();
            }
        }

        void SetupChestState(bool unlocked)
        {
            chestTapToOpen.gameObject.SetActive(unlocked);
            chestTimer.SetActive(!unlocked);
            chestButton.interactable = unlocked;
        }

        void SetupVideoIcon(bool available, bool unlocked)
        {
            chestVideoIcon.color = unlocked ? Colors.WHITE : Colors.DISABLED_WHITE;
            chestVideoIcon.sprite = available && unlocked ? videoAvailable : videoNotAvailable;
        }

        void RebuildLayout()
        {
            foreach (var layout in layouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
            }
        }
    }
}
