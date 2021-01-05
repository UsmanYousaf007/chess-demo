/// @license Propriety <http://license.url>
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
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

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


        string playerId;

        public TMP_Text leaderboardTimer;

        //Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }


        //Signals
        public Signal leaderboardButtonClickedSignal = new Signal();
        public Signal chestButtonClickedSignal = new Signal();


        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            leaderboardButton.onClick.AddListener(OnLeaderboardBtnClicked);
            chestButton.onClick.AddListener(OnChestButtonClicked);

            var currentTime = backendService.serverClock.currentTimestamp;
            var diff = currentTime - playerModel.chestUnlockTimestamp;


            if (diff > 0)
            {
                chestTimeText.text = "CLAIM";
            }
            else
            {
                chestTimeText.text = "sec " + diff + "left";
            }

        }

        public void UpdateView(ProfileVO vo)
        {
            playerName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);
            playerId = vo.playerId;

            LeagueTierIconsContainer.LeagueAsset leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            if (leagueAssets != null)
            {
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
           /* if (playerScoreLeague != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(playerScoreLeague);
            }*/
        }

        public void UpdateEloScores(EloVO vo)
        {
            eloScoreValue.text = vo.playerEloScore.ToString();
        }

        void OnLeaderboardBtnClicked()
        {
            leaderboardButtonClickedSignal.Dispatch();
        }

        void OnChestButtonClicked()
        {
            chestButtonClickedSignal.Dispatch();
        }
    }
}
