/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-17 12:37:18 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Score")]
        public Text playerScore;
        public Text opponentScore;

        public void OnParentShowScore()
        {
            EmptyScores();
        }

        public void UpdateScores(MoveVO moveVO, bool isResume)
        {
            SetAdvantage(playerScore, moveVO.playerScore);
            SetAdvantage(opponentScore, moveVO.opponentScore);
        }

        public void InitScore()
        {
            EmptyScores();
        }

        public void CleanupScore()
        {
        }

        private void EmptyScores()
        {
            playerScore.text = "";
            opponentScore.text = "";
        }

        private void SetAdvantage(Text label, int score)
        {
            label.text = (score > 0) ? (localizationService.Get(LocalizationKey.GM_ADVANTAGE) + " +" + score) : "";
        }
    }
}
