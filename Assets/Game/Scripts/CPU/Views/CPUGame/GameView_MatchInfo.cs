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

using TurboLabz.TLUtils;

using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    public partial class GameView
    {
        public Text playerNameLabel;
        public Text playerLevelLabel;
        public Image playerFlag;
        public Text cpuNameLabel;
        public Text cpuStrengthLabel;
        public Image playerProfilePicture;

        public void InitMatchInfo()
        {
            cpuNameLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_CPU_NAME);
        }

        public void OnParentShowMatchInfo()
        {
            // Nothing
        }

        public void UpdateCPUGameInfo(CPUGameInfoVO vo)
        {
            playerNameLabel.text = vo.playerName;
            playerLevelLabel.text = localizationService.Get(LocalizationKey.GM_PLAYER_LEVEL, vo.playerLevel.ToString());
            cpuStrengthLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_CPU_STRENGTH, vo.cpuStrength.ToString());
        }

        public void UpdatePlayerProfilePicture(Sprite sprite)
        {
            playerProfilePicture.sprite = sprite;
            playerProfilePicture.gameObject.SetActive(sprite != null);
        }
    }
}
