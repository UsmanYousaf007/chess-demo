/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-11 13:42:53 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class MatchmakingView : View
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
            public Text wagerLabel;
        }

        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Sprite cache is taken from the SpriteCache gameobject in the scene
        public SpriteCache spriteCache;

        public Animator matchmakingAnimator;

        public Sprite[] profilePicturesCache;

        public Text currency1Label;
        public Text currency2Label;

        public _RoomInfo room;
        public _PlayerInfo player;
        public _PlayerInfo opponent;

        public Text prizeTitleLabel;
        public Text prizeLabel;

        public Signal viewDurationCompleteSignal = new Signal();

        private IEnumerator rollOpponentProfilePictureEnumerator;

        private long currency1PreMatchFound;
        private long currency1PostMatchFound;
        private RoomSetting roomInfo;

        public void Init()
        {
            // init here
        }

        public void UpdateViewPreMatchFound(PreMatchmakingVO vo)
        {
            currency1PreMatchFound = vo.currency1;
            roomInfo = vo.roomInfo;

            currency1Label.text = vo.currency1.ToString("N0");
            currency2Label.text = vo.currency2.ToString("N0");

            room.banner.sprite = spriteCache.GetRoomBanner(roomInfo.id);
            room.flag.sprite = spriteCache.GetRoomFlagMinor(roomInfo.id);
            room.nameLabel.text = localizationService.Get(roomInfo.id);

            int gameDurationMinutes = (int)(roomInfo.gameDuration / 60000);
            room.durationLabel.text = localizationService.Get(LocalizationKey.MM_ROOM_DURATION, gameDurationMinutes);

            PublicProfile playerPublicProfile = vo.playerPublicProfile;

            player.nameLabel.text = playerPublicProfile.name;

            // Updating profile picture here to display cached profile picture
            // right away.
            UpdatePlayerProfilePicture(playerPublicProfile.profilePicture);
            UpdateProfilePictureBorder(vo.playerModel.profilePictureBorder);

            player.countryFlag.sprite = spriteCache.GetCountryFlag(playerPublicProfile.countryId);
            player.levelLabel.text = localizationService.Get(LocalizationKey.MM_PLAYER_LEVEL, playerPublicProfile.level);

            RoomRecord roomRecord = playerPublicProfile.roomRecords[roomInfo.id];
            player.roomTitleLabel.text = localizationService.GetRoomTitle(roomRecord.roomTitleId);

            player.wagerLabel.text = roomInfo.wager.ToString("N0");
            opponent.wagerLabel.text = roomInfo.wager.ToString("N0");
            prizeTitleLabel.text = localizationService.Get(LocalizationKey.MM_PRIZE_TITLE_LABEL);
            prizeLabel.text = 0.ToString("N0");

            RollOpponentProfilePicture();
            matchmakingAnimator.SetTrigger(AnimationConstants.Matchmaking.FIND_MATCH);
        }

        public void UpdateViewPostMatchFound(PostMatchmakingVO vo)
        {
            currency1PostMatchFound = vo.currency1;

            matchmakingAnimator.SetTrigger(AnimationConstants.Matchmaking.MATCH_FOUND);

            PublicProfile opponentPublicProfile = vo.opponentPublicProfile;

            opponent.nameLabel.text = opponentPublicProfile.name;
            UpdateOpponentProfilePicture(opponentPublicProfile.profilePicture);
            opponent.countryFlag.sprite = spriteCache.GetCountryFlag(opponentPublicProfile.countryId);
            opponent.levelLabel.text = localizationService.Get(LocalizationKey.MM_OPPONENT_LEVEL, opponentPublicProfile.level);

            RoomRecord roomRecord = opponentPublicProfile.roomRecords[roomInfo.id];
            opponent.roomTitleLabel.text = localizationService.GetRoomTitle(roomRecord.roomTitleId);
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
            StopRollingOpponentProfilePicture();
            opponent.profilePicture.sprite = sprite;
            opponent.profilePicture.gameObject.SetActive(sprite != null);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnAnimatePrize()
        {
            // Animate currency 1 in the top bar down to the value after
            // subtracting wager.
            long currency1 = currency1PreMatchFound;

            DOTween.To(() => { return currency1; },
                       (val) => {
                           currency1 = val;
                           currency1Label.text = currency1.ToString("N0");
                       },
                       currency1PostMatchFound,
                       Settings.MATCHMAKING_CURRENCY_1_ANIMATION_DURATION);

            long wager = roomInfo.wager;

            // Animate player and opponent wagers down to zero.
            DOTween.To(() => { return wager; },
                       (val) => {
                           wager = val;
                           player.wagerLabel.text = wager.ToString("N0");
                           opponent.wagerLabel.text = wager.ToString("N0");
                       },
                       0,
                       Settings.MATCHMAKING_CURRENCY_1_ANIMATION_DURATION);

            long prize = 0;

            // Animate prize value from zero up.
            DOTween.To(() => { return prize; },
                       (val) => {
                           prize = val;
                           prizeLabel.text = prize.ToString("N0");
                       },
                       roomInfo.prize,
                       Settings.MATCHMAKING_CURRENCY_1_ANIMATION_DURATION);
        }

        public void OnAnimationComplete()
        {
            StartCoroutine(ViewDurationCompleteCR());
        }

        private void RollOpponentProfilePicture()
        {
            Assertions.Assert(rollOpponentProfilePictureEnumerator == null, "Opponent profile picture must not already be rolling!");

            opponent.profilePicture.gameObject.SetActive(true);
            rollOpponentProfilePictureEnumerator = RollOpponentProfilePictureCR();
            StartCoroutine(rollOpponentProfilePictureEnumerator);
        }

        private void StopRollingOpponentProfilePicture()
        {
            Assertions.Assert(rollOpponentProfilePictureEnumerator != null, "Opponent profile picture must already be rolling!");

            StopCoroutine(rollOpponentProfilePictureEnumerator);
            rollOpponentProfilePictureEnumerator = null;
        }

        private IEnumerator RollOpponentProfilePictureCR()
        {
            CollectionsUtil.Shuffle<Sprite>(profilePicturesCache);
            int length = profilePicturesCache.Length;
            int index = 0;

            while (true)
            {
                opponent.profilePicture.sprite = profilePicturesCache[index];
                ++index;

                if (index >= length)
                {
                    index = 0;
                }

                yield return new WaitForSeconds(Settings.FIND_MATCH_OPPONENT_PROFILE_PICTURE_ROLLING_INTERVAL);
            }
        }

        private IEnumerator ViewDurationCompleteCR()
        {
            yield return new WaitForSeconds(Settings.MATCHMAKING_VIEW_DURATION_AFTER_ANIMATION);
            viewDurationCompleteSignal.Dispatch();
        }
    }
}
