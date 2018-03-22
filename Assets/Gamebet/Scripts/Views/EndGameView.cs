/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-09 14:31:07 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    public class EndGameView : View
    {
        [System.Serializable]
        public struct _RoomInfo
        {
            public Image banner;
            public Image flag;
            public Text nameLabel;
            public Text durationLabel;
        }

        [System.Serializable]
        public struct _PlayerInfo
        {
            public Text nameLabel;
            public Image profilePicture;
            public Image profilePictureBorder;
            public Image countryFlag;
            public Text levelLabel;
            public Text roomTitleLabel;
        }

        [System.Serializable]
        public struct _Results
        {
            public Text playerLabel;
            public Text drawLabel;
            public Text opponentLabel;
        }

        [System.Serializable]
        public struct _LevelPromotion
        {
            public GameObject view;
            public Image profilePicture;
            public Text promotionTitleLabel;
            public Text fromLevelLabel;
            public Text toLevelLabel;
            public Text rewardTitleLabel;
            public Text rewardCurrency2Label;
            public Text nextPromotionMessageLabel;
            public Button continueButton;
            public Text continueButtonLabel;
        }

        [System.Serializable]
        public struct _LeaguePromotion
        {
            public GameObject view;
            public Text promotionTitleLabel;
            public Image fromLeagueBadge;
            public Text fromLeagueLabel;
            public Image toLeagueBadge;
            public Text toLeagueLabel;
            public Text nextPromotionMessageLabel;
            public Button continueButton;
            public Text continueButtonLabel;
        }

        [System.Serializable]
        public struct _TrophyPromotion
        {
            public GameObject view;
            public Image roomFlag;
            public Text roomNameLabel;
            public Text roomDurationLabel;
            public Image profilePicture;
            public Text promotionTitleLabel;
            public Text trophyCountTitleLabel;
            public Text trophyCountLabel;
            public Text nextPromotionMessageLabel;
            public Button continueButton;
            public Text continueButtonLabel;
        }

        [System.Serializable]
        public struct _RoomTitlePromotion
        {
            public GameObject view;
            public Image roomFlag;
            public Text roomNameLabel;
            public Text roomDurationLabel;
            public Image profilePicture;
            public Text promotionTitleLabel;
            public Text fromRoomTitleLabel;
            public Text toRoomTitleLabel;
            public Text rewardTitleLabel;
            public Text rewardCurrency2Label;
            public Text nextPromotionMessageLabel;
            public Button continueButton;
            public Text continueButtonLabel;
        }

        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Sprite cache is taken from the SpriteCache gameobject in the scene
        public SpriteCache spriteCache;

        public Animator endGameAnimator;

        public Text currency1Label;
        public Text currency2Label;

        public _RoomInfo room;
        public _PlayerInfo player;
        public _PlayerInfo opponent;
        public _Results results;

        public Text prizeTitleLabel;
        public Text prizeLabel;

        public Button newMatchButton;
        public Text newMatchButtonLabel;

        public Button backButton;

        public Signal newMatchButtonClickedSignal = new Signal();
        public Signal backButtonClickedSignal = new Signal();

        #region Promotions

        public _LevelPromotion levelPromotion;
        public _LeaguePromotion leaguePromotion;
        public _TrophyPromotion trophyPromotion;
        public _RoomTitlePromotion roomTitlePromotion;

        #endregion

        private string announceResultsAnimationTrigger;
        private string rewardAnimationTrigger;
        private Promotions promotions;

        public void Init()
        {
            newMatchButton.onClick.AddListener(OnNewMatchButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);

            // Promotions
            levelPromotion.continueButton.onClick.AddListener(OnLevelPromotionContinueButtonClicked);
            leaguePromotion.continueButton.onClick.AddListener(OnLeaguePromotionContinueButtonClicked);
            trophyPromotion.continueButton.onClick.AddListener(OnTrophyPromotionContinueButtonClicked);
            roomTitlePromotion.continueButton.onClick.AddListener(OnRoomTitlePromotionContinueButtonClicked);
        }

        public void UpdateView(EndGameVO vo)
        {
            // Cache promotions for later use.
            promotions = vo.promotions;

            currency1Label.text = vo.currency1.ToString("N0");
            currency2Label.text = vo.currency2.ToString("N0");

            RoomSetting roomInfo = vo.roomInfo;

            room.banner.sprite = spriteCache.GetRoomBanner(roomInfo.id);
            room.flag.sprite = spriteCache.GetRoomFlagMinor(roomInfo.id);
            room.nameLabel.text = localizationService.Get(roomInfo.id);

            int gameDurationMinutes = (int)(roomInfo.gameDuration / 60000);
            room.durationLabel.text = localizationService.Get(LocalizationKey.EG_ROOM_DURATION_LABEL, gameDurationMinutes);

            PublicProfile playerPublicProfile = vo.playerPublicProfile;

            player.nameLabel.text = playerPublicProfile.name;

            // Updating profile picture here to display cached profile picture
            // right away.
            UpdatePlayerProfilePicture(playerPublicProfile.profilePicture);
            UpdateProfilePictureBorder(vo.playerModel.profilePictureBorder);

            player.countryFlag.sprite = spriteCache.GetCountryFlag(playerPublicProfile.countryId);
            player.levelLabel.text = localizationService.Get(LocalizationKey.EG_PLAYER_LEVEL_LABEL, playerPublicProfile.level);

            RoomRecord playerRoomRecord = playerPublicProfile.roomRecords[roomInfo.id];
            player.roomTitleLabel.text = localizationService.GetRoomTitle(playerRoomRecord.roomTitleId);

            PublicProfile opponentPublicProfile = vo.opponentPublicProfile;

            opponent.nameLabel.text = opponentPublicProfile.name;
            UpdateOpponentProfilePicture(opponentPublicProfile.profilePicture);
            opponent.countryFlag.sprite = spriteCache.GetCountryFlag(opponentPublicProfile.countryId);
            opponent.levelLabel.text = localizationService.Get(LocalizationKey.EG_OPPONENT_LEVEL_LABEL, opponentPublicProfile.level);

            RoomRecord opponentRoomRecord = opponentPublicProfile.roomRecords[roomInfo.id];
            opponent.roomTitleLabel.text = localizationService.GetRoomTitle(opponentRoomRecord.roomTitleId);

            if (vo.endGameResult == EndGameResult.PLAYER_WON)
            {
                results.playerLabel.text = localizationService.Get(LocalizationKey.EG_RESULTS_PLAYER_WON_LABEL);
                announceResultsAnimationTrigger = AnimationConstants.EndGame.ANNOUNCE_PLAYER_WINNER;
                rewardAnimationTrigger = AnimationConstants.EndGame.REWARD_PLAYER;
            }
            else if (vo.endGameResult == EndGameResult.OPPONENT_WON)
            {
                results.opponentLabel.text = localizationService.Get(LocalizationKey.EG_RESULTS_OPPONENT_WON_LABEL);
                announceResultsAnimationTrigger = AnimationConstants.EndGame.ANNOUNCE_OPPONENT_WINNER;
                rewardAnimationTrigger = AnimationConstants.EndGame.REWARD_OPPONENT;
            }
            else if (vo.endGameResult == EndGameResult.GAME_DRAWN)
            {
                results.drawLabel.text = localizationService.Get(LocalizationKey.EG_RESULTS_DRAWN_LABEL);
                announceResultsAnimationTrigger = AnimationConstants.EndGame.ANNOUNCE_DRAW;
                rewardAnimationTrigger = AnimationConstants.EndGame.REWARD_BOTH;
            }

            prizeTitleLabel.text = localizationService.Get(LocalizationKey.EG_PRIZE_TITLE_LABEL);
            prizeLabel.text = roomInfo.prize.ToString("N0");

            newMatchButtonLabel.text = localizationService.Get(LocalizationKey.EG_NEW_MATCH_BUTTON_LABEL);

            #region Promotions

            if (promotions.levelPromotions != null)
            {
                IList<LevelPromotion> levelPromotions = promotions.levelPromotions;
                int count = levelPromotions.Count;
                LevelPromotion lastLevelPromotion = levelPromotions[count - 1];

                int fromLevel = levelPromotions[0].from;
                int toLevel = lastLevelPromotion.to;
                long rewardAmount = 0;

                for (int i = 0; i < count; ++i)
                {
                    rewardAmount += levelPromotions[i].reward.currency2;
                }

                NextLeaguePromotion nextLeaguePromotion = lastLevelPromotion.nextLeaguePromotion;
                string nextPromotionMessage = null;

                if (nextLeaguePromotion != null)
                {
                    string nextLeagueId = nextLeaguePromotion.id;
                    int levelsToNextLeague = nextLeaguePromotion.startLevel - toLevel;

                    string nextLeagueName = localizationService.Get(nextLeagueId);
                    nextPromotionMessage = localizationService.Get(LocalizationKey.LVLP_NEXT_PROMOTION_MESSAGE_LABEL, levelsToNextLeague, nextLeagueName);
                }

                levelPromotion.profilePicture.sprite = playerPublicProfile.profilePicture;
                levelPromotion.promotionTitleLabel.text = localizationService.Get(LocalizationKey.LVLP_PROMOTION_TITLE_LABEL);
                levelPromotion.fromLevelLabel.text = fromLevel.ToString();
                levelPromotion.toLevelLabel.text = toLevel.ToString();
                levelPromotion.rewardTitleLabel.text = localizationService.Get(LocalizationKey.LVLP_REWARD_TITLE_LABEL);
                levelPromotion.rewardCurrency2Label.text = rewardAmount.ToString();
                levelPromotion.nextPromotionMessageLabel.text = nextPromotionMessage;
                levelPromotion.continueButtonLabel.text = localizationService.Get(LocalizationKey.LVLP_CONTINUE_BUTTON_LABEL);
            }

            if (promotions.leaguePromotion != null)
            {
                LeaguePromotion lp = promotions.leaguePromotion;

                NextLeaguePromotion nextLeaguePromotion = lp.nextLeaguePromotion;
                string nextPromotionMessage = null;

                if (nextLeaguePromotion != null)
                {
                    nextPromotionMessage = localizationService.Get(LocalizationKey.LGP_NEXT_PROMOTION_MESSAGE_LABEL, nextLeaguePromotion.startLevel);
                }

                leaguePromotion.promotionTitleLabel.text = localizationService.Get(LocalizationKey.LGP_PROMOTION_TITLE_LABEL);
                leaguePromotion.fromLeagueBadge.sprite = spriteCache.GetLeagueBadge(lp.from);
                leaguePromotion.fromLeagueLabel.text = localizationService.Get(lp.from);
                leaguePromotion.toLeagueBadge.sprite = spriteCache.GetLeagueBadge(lp.to);
                leaguePromotion.toLeagueLabel.text = localizationService.Get(lp.to);
                leaguePromotion.nextPromotionMessageLabel.text = nextPromotionMessage;
                leaguePromotion.continueButtonLabel.text = localizationService.Get(LocalizationKey.LGP_CONTINUE_BUTTON_LABEL);
            }
            
            if (promotions.trophyPromotion != null)
            {
                trophyPromotion.roomFlag.sprite = spriteCache.GetRoomFlagMinor(roomInfo.id);
                trophyPromotion.roomNameLabel.text = localizationService.Get(roomInfo.id);
                trophyPromotion.roomDurationLabel.text = localizationService.Get(LocalizationKey.TP_ROOM_DURATION_LABEL, gameDurationMinutes);
                trophyPromotion.profilePicture.sprite = playerPublicProfile.profilePicture;
                trophyPromotion.promotionTitleLabel.text = localizationService.Get(LocalizationKey.TP_PROMOTION_TITLE_LABEL);
                trophyPromotion.trophyCountTitleLabel.text = localizationService.Get(LocalizationKey.TP_TROPHY_COUNT_TITLE_LABEL);
                trophyPromotion.trophyCountLabel.text = promotions.trophyPromotion.to.ToString();
                trophyPromotion.nextPromotionMessageLabel.text = "??";
                trophyPromotion.continueButtonLabel.text = localizationService.Get(LocalizationKey.TP_CONTINUE_BUTTON_LABEL);
            }

            if (promotions.roomTitlePromotion != null)
            {
                RoomTitlePromotion rtp = promotions.roomTitlePromotion;

                roomTitlePromotion.roomFlag.sprite = spriteCache.GetRoomFlagMinor(roomInfo.id);
                roomTitlePromotion.roomNameLabel.text = localizationService.Get(roomInfo.id);
                roomTitlePromotion.roomDurationLabel.text = localizationService.Get(LocalizationKey.RTP_ROOM_DURATION_LABEL, gameDurationMinutes);
                roomTitlePromotion.profilePicture.sprite = playerPublicProfile.profilePicture;
                roomTitlePromotion.promotionTitleLabel.text = localizationService.Get(LocalizationKey.RTP_PROMOTION_TITLE_LABEL);
                roomTitlePromotion.fromRoomTitleLabel.text = localizationService.Get(rtp.from);
                roomTitlePromotion.toRoomTitleLabel.text = localizationService.Get(rtp.to);
                roomTitlePromotion.rewardTitleLabel.text = localizationService.Get(LocalizationKey.RTP_REWARD_TITLE_LABEL);
                roomTitlePromotion.rewardCurrency2Label.text = rtp.reward.currency2.ToString();
                roomTitlePromotion.nextPromotionMessageLabel.text = "??";
                roomTitlePromotion.continueButtonLabel.text = localizationService.Get(LocalizationKey.RTP_CONTINUE_BUTTON_LABEL);
            }

            #endregion
        }

        public void UpdatePlayerProfilePicture(Sprite sprite)
        {
            player.profilePicture.sprite = sprite;
            player.profilePicture.gameObject.SetActive(sprite != null);
        }

        public void UpdateProfilePictureBorder(Sprite sprite)
        {
            player.profilePictureBorder.sprite = sprite;
            player.profilePictureBorder.gameObject.SetActive(sprite != null);
        }

        public void UpdateOpponentProfilePicture(Sprite sprite)
        {
            opponent.profilePicture.sprite = sprite;
            opponent.profilePicture.gameObject.SetActive(sprite != null);
        }

        public void Show()
        {
            ResetView();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // Called from Animation event.
        public void OnEntryAnimationComplete()
        {
            endGameAnimator.SetTrigger(announceResultsAnimationTrigger);
        }

        // Called from Animation event.
        public void OnAnnounceWinnerAnimationComplete()
        {
            endGameAnimator.SetTrigger(rewardAnimationTrigger);
        }

        // Called from Animation event.
        public void OnRewardAnimationComplete()
        {
            ShowPromotions();
        }

        private void ResetView()
        {
            levelPromotion.view.SetActive(false);
            leaguePromotion.view.SetActive(false);
            trophyPromotion.view.SetActive(false);
            roomTitlePromotion.view.SetActive(false);
        }

        private void OnNewMatchButtonClicked()
        {
            newMatchButtonClickedSignal.Dispatch();
        }

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }

        #region Promotions

        private void ShowPromotions()
        {
            ShowLevelPromotion();
        }

        // Level promotion.
        private void ShowLevelPromotion()
        {
            if (promotions.levelPromotions != null)
            {
                levelPromotion.view.SetActive(true);
            }
            else
            {
                OnPostLevelPromotion();
            }
        }

        private void OnLevelPromotionContinueButtonClicked()
        {
            levelPromotion.view.SetActive(false);
            OnPostLevelPromotion();
        }

        private void OnPostLevelPromotion()
        {
            ShowLeaguePromotion();
        }

        // League promotion.
        private void ShowLeaguePromotion()
        {
            if (promotions.leaguePromotion != null)
            {
                leaguePromotion.view.SetActive(true);
            }
            else
            {
                OnPostLeaguePromotion();
            }
        }

        private void OnLeaguePromotionContinueButtonClicked()
        {
            leaguePromotion.view.SetActive(false);
            OnPostLeaguePromotion();
        }

        private void OnPostLeaguePromotion()
        {
            ShowTrophyPromotion();
        }

        // Trophy promotion.
        private void ShowTrophyPromotion()
        {
            if (promotions.trophyPromotion != null)
            {
                trophyPromotion.view.SetActive(true);
            }
            else
            {
                OnPostTrophyPromotion();
            }
        }

        private void OnTrophyPromotionContinueButtonClicked()
        {
            trophyPromotion.view.SetActive(false);
            OnPostTrophyPromotion();
        }

        private void OnPostTrophyPromotion()
        {
            ShowRoomTitlePromotion();
        }

        // Room title promotion.
        private void ShowRoomTitlePromotion()
        {
            if (promotions.roomTitlePromotion != null)
            {
                roomTitlePromotion.view.SetActive(true);
            }
            else
            {
                OnPostRoomTitlePromotion();
            }
        }

        private void OnRoomTitlePromotionContinueButtonClicked()
        {
            roomTitlePromotion.view.SetActive(false);
            OnPostRoomTitlePromotion();
        }

        private void OnPostRoomTitlePromotion()
        {
            endGameAnimator.SetTrigger(AnimationConstants.EndGame.SHOW_BUTTONS);
        }

        #endregion
    }
}
