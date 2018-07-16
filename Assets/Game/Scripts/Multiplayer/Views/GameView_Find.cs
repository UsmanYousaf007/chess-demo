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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public GameObject findDlg;
        public Text searchingLabel;
        public Image findAvatar;
        public Sprite[] profilePicturesCache;
        public Sprite defaultAvatar;

        private IEnumerator rollOpponentProfilePictureEnumerator;

        public void InitFind()
        {
        }

        public void ShowFind()
        {
            searchingLabel.color = Colors.WHITE;
            searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_SEARCHING);

            SetupFindMode();

            EnableModalBlocker();
            findDlg.SetActive(true);

            DisableMenuButton();

            RollOpponentProfilePicture();
        }

        public void HideFind()
        {
            DisableModalBlocker();
            findDlg.SetActive(false);

            EnableMenuButton();
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

            EnableModalBlocker();
        }

        public void MatchFound(ProfileVO vo)
        {
            StopRollingOpponentProfilePicture();

            if (vo.playerPic == null)
            {
                findAvatar.sprite = defaultAvatar;
            }
            else
            {
                findAvatar.sprite = vo.playerPic;
            }

            searchingLabel.color = Colors.GREEN;
            searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_FOUND);
        }

        private void RollOpponentProfilePicture()
        {
            Assertions.Assert(rollOpponentProfilePictureEnumerator == null, "Opponent profile picture must not already be rolling!");

            findAvatar.gameObject.SetActive(true);
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
                findAvatar.sprite = profilePicturesCache[index];
                ++index;

                if (index >= length)
                {
                    index = 0;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
