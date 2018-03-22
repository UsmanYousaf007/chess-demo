/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-23 19:06:16 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using TurboLabz.Common;

using TurboLabz.Gamebet;

namespace TurboLabz.MPChess
{
    public partial class GameView
    {
        public Text playerNameLabel;
        public Text playerRoomTitleLabel;
        public Text playerLevelLabel;
        public Image playerFlag;
        public Text opponentNameLabel;
        public Text opponentLevelLabel;
        public Text opponentRoomTitleLabel;
        public Image opponentFlag;
        public Image roomFlag;
        public Text roomName;
        public Text roomPrize;
        public GameObject waitingForOpponent;
        public Text waitingForOpponentLabel;
        public Image playerProfilePicture;
        public Image opponentProfilePicture;

        public void InitMatchInfo()
        {
            waitingForOpponentLabel.text = localizationService.Get(LocalizationKey.GM_WAITING_FOR_OPPONENT);
        }

        public void OnShowMatchInfo()
        {
            waitingForOpponent.SetActive(false);
        }

        public void UpdateMatchInfo(MatchInfoVO vo)
        {
            playerNameLabel.text = vo.playerName;
            playerRoomTitleLabel.text = localizationService.GetRoomTitle(vo.playerRoomTitleId);
            playerLevelLabel.text = localizationService.Get(LocalizationKey.GM_PLAYER_LEVEL, vo.playerLevel.ToString());
            playerFlag.sprite = spriteCache.GetCountryFlag(vo.playerCountryId);
            opponentNameLabel.text = vo.opponentName;
            opponentRoomTitleLabel.text = localizationService.GetRoomTitle(vo.opponentRoomTitleId);
            opponentLevelLabel.text = localizationService.Get(LocalizationKey.GM_PLAYER_LEVEL, vo.opponentLevel.ToString());
            opponentFlag.sprite = spriteCache.GetCountryFlag(vo.opponentCountryId);
            roomFlag.sprite = spriteCache.GetRoomFlagMinor(vo.roomId);
            roomName.text = localizationService.Get(vo.roomId);
            roomPrize.text = FormatUtil.AbbreviateNumber(vo.prize);

            UpdateOpponentProfilePicture(vo.opponentProfilePictureSprite);
        }

        public void UpdatePlayerProfilePicture(Sprite sprite)
        {
            playerProfilePicture.sprite = sprite;
            playerProfilePicture.gameObject.SetActive(sprite != null);
        }

        public void UpdateOpponentProfilePicture(Sprite sprite)
        {
            opponentProfilePicture.sprite = sprite;
            opponentProfilePicture.gameObject.SetActive(sprite != null);
        }

        public void OpponentDisconnected()
        {
            waitingForOpponent.SetActive(true);
        }

        public void OpponentReconnected()
        {
            waitingForOpponent.SetActive(false);
        }

        private void HideWaitingForOpponentIndicator()
        {
            waitingForOpponent.SetActive(false);
        }
    }
}
