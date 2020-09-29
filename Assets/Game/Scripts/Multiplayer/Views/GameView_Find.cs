/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:37:45 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections;
using TurboLabz.InstantGame;
using System;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Find")]
        public GameObject findDlg;
        public Text searchingLabel;

        public GameObject findAvatarRoller;
        public Image findAvatar;
        public Sprite[] profilePicturesCache;
        public Sprite defaultAvatar;

        public Image playerFindProfilePic;
        public Image playerFindAvatarBg;
        public Image playerFindAvatarIcon;
        //public GameObject playerPremiumBorder;
        public Image playerFindLeageBorder;

        public GameObject opponentFindProfile;
        public Image opponentFindProfilePic;
        public Image opponentFindAvatarBg;
        public Image opponentFindAvatarIcon;
        //public GameObject opponentPremiumBorder;
        public Image opponentFindLeagueBorder;

        public Text timerLabel;

        private IEnumerator rollOpponentProfilePictureEnumerator;
        private Coroutine findMatchTimeoutCR = null;
        private TimeSpan countDownTimer;
        public Signal findMatchTimeoutSignal = new Signal();
        private string oppoenentId;

        public void InitFind()
        {
            playerFindProfilePic.sprite = defaultAvatar;
            opponentFindProfilePic.sprite = defaultAvatar;
        }

        void SetProfileDisplayPic(ref Image bg, ref Image icon, ref Image pic,
                                    Sprite profilePic, string avatarId, string bgColorId)
        {
            bg.gameObject.SetActive(false);
            icon.gameObject.SetActive(false);
            pic.gameObject.SetActive(false);

            if (profilePic != null)
            {
                pic.gameObject.SetActive(true);
                pic.sprite = profilePic;
            }
            else if (avatarId != null)
            {
                bg.gameObject.SetActive(true);
                icon.gameObject.SetActive(true);
                bg.color = Colors.Color(bgColorId);
                icon.sprite = defaultAvatarContainer.GetSprite(avatarId);
            }
            else
            {
                pic.gameObject.SetActive(true);
                pic.sprite = defaultAvatar;
            }
        }

        public void UpdateFind(FindViewVO vo)
        {
            findAvatarRoller.gameObject.SetActive(false);
            opponentFindProfile.SetActive(false);
            playerId = vo.player.playerId;
            SetProfileDisplayPic(ref playerFindAvatarBg, ref playerFindAvatarIcon, ref playerFindProfilePic,
                                vo.player.playerPic, vo.player.avatarId, vo.player.avatarColorId);
            SetLeagueBorder(playerFindLeageBorder, vo.player.leagueBorder);

            if (vo.opponent.playerId != null)
            {
                opponentId = vo.opponent.playerId;
                opponentFindProfile.SetActive(true);
                searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_WAITING_FOR_OPPONENT);
                SetProfileDisplayPic(ref opponentFindAvatarBg, ref opponentFindAvatarIcon, ref opponentFindProfilePic,
                        vo.opponent.playerPic, vo.opponent.avatarId, vo.opponent.avatarColorId);
                SetLeagueBorder(opponentFindLeagueBorder, vo.opponent.leagueBorder);
            }
            else
            {
                findAvatarRoller.SetActive(true);
                findAvatar.gameObject.SetActive(true);
                searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_SEARCHING);
                RollOpponentProfilePicture();
            }
        }

        public void SetProfilePicById(string id, Sprite sprite)
        {
            if (id.Equals(playerId))
            {
                SetProfileDisplayPic(ref playerFindAvatarBg, ref playerFindAvatarIcon, ref playerFindProfilePic,
                                sprite, null, null);
            }
            else if (id.Equals(oppoenentId))
            {
                SetProfileDisplayPic(ref opponentFindAvatarBg, ref opponentFindAvatarIcon, ref opponentFindProfilePic,
                        sprite, null, null);
            }
        }

        public void ShowFind()
        {
            searchingLabel.color = Colors.WHITE;

            SetupFindMode();
            EnableModalBlocker();
            findDlg.SetActive(true);
            DisableMenuButton();

            // disable chat thingies
            foreach (GameObject obj in chatInputSet)
            {
                obj.SetActive(false);
            }

            OnParentHideAdBanner();
            ResetCapturedPieces();
            OnParentShowScore();
        }

        public void HideFind()
        {
            FindMatchTimeoutEnable(false);
            StopRollingOpponentProfilePicture();
            DisableModalBlocker();
            findDlg.SetActive(false);

            EnableMenuButton();

            // enable chat thingies
            foreach (GameObject obj in chatInputSet)
            {
                obj.SetActive(true);
            }

            OnParentShowAdBanner();
        }

        void SetupFindMode()
        {
            chessContainer.SetActive(true);
            InitClickAndDrag();

            fileRankLabelsForward.SetActive(false);
            fileRankLabelsBackward.SetActive(false);
            playerFromIndicator.SetActive(false);
            playerToIndicator.SetActive(false);
            opponentFromIndicator.SetActive(false);
            opponentToIndicator.SetActive(false);
            kingCheckIndicator.SetActive(false);

            HidePossibleMoves();
            DisableMenuButton();
            playerInfoPanel.SetActive(false);
            opponentInfoPanel.SetActive(false);

            // Reset the piece image pool
            foreach (GameObject obj in activatedPieceImages)
            {
                pool.ReturnObject(obj);
            }

            activatedPieceImages.Clear();

            ToggleTopPanel(false);

            EnableModalBlocker();
        }

        public void MatchFound(ProfileVO vo)
        {
            StopRollingOpponentProfilePicture();
            FindMatchTimeoutEnable(false);
            findAvatar.gameObject.SetActive(false);
            opponentFindProfile.SetActive(true);
            opponentId = vo.playerId;
            SetProfileDisplayPic(ref opponentFindAvatarBg, ref opponentFindAvatarIcon, ref opponentFindProfilePic,
                        vo.playerPic, vo.avatarId, vo.avatarColorId);
            //opponentPremiumBorder.SetActive(vo.isPremium);
            SetLeagueBorder(opponentFindLeagueBorder, vo.leagueBorder);
            searchingLabel.color = Colors.YELLOW;
            searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_FOUND);
        }

        private void RollOpponentProfilePicture()
        {
            if (rollOpponentProfilePictureEnumerator != null)
            {
                StopRollingOpponentProfilePicture();
            }

            rollOpponentProfilePictureEnumerator = RollOpponentProfilePictureCR();
            if (IsVisible())
            {
                StartCoroutine(rollOpponentProfilePictureEnumerator);
            }
        }

        private void StopRollingOpponentProfilePicture()
        {
            if (rollOpponentProfilePictureEnumerator != null)
            {
                StopCoroutine(rollOpponentProfilePictureEnumerator);
                rollOpponentProfilePictureEnumerator = null;
            }
        }

        private IEnumerator RollOpponentProfilePictureCR()
        {
            CollectionsUtil.Shuffle<Sprite>(profilePicturesCache);
            int length = profilePicturesCache.Length;
            int index = 0;

            while (true)
            {
                findAvatar.sprite = profilePicturesCache[index];
                ++index;

                if (index >= length)
                {
                    index = 0;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        public void FindMatchTimeoutEnable(bool enable, int seconds = 0)
        {
            if (enable)
            {
                countDownTimer = new TimeSpan(0, 0, seconds);
                UpdateCountDownTimerText();

                if (findMatchTimeoutCR != null)
                {
                    routineRunner.StopCoroutine(findMatchTimeoutCR);
                }

                if (IsVisible())
                {
                    findMatchTimeoutCR = routineRunner.StartCoroutine(FindMatchTimeoutCR(seconds));
                }
            }
            else
            {
                if (findMatchTimeoutCR != null)
                {
                    routineRunner.StopCoroutine(findMatchTimeoutCR);
                    findMatchTimeoutCR = null;
                }
            }
        }

        private IEnumerator FindMatchTimeoutCR(float seconds)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                countDownTimer = countDownTimer.Subtract(new TimeSpan(0, 0, 1));
                UpdateCountDownTimerText();

                if (countDownTimer.Seconds <= 0)
                {
                    findMatchTimeoutSignal.Dispatch();
                }
            }
        }

        private void UpdateCountDownTimerText()
        {
            timerLabel.text = "Timeout in " + TimeUtil.FormatPlayerClock(countDownTimer);
        }

        private void SetLeagueBorder(Image border, Sprite borderSprite)
        {
            border.gameObject.SetActive(borderSprite != null);
            border.sprite = borderSprite;
        }
    }
}
