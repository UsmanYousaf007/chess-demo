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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Text playerScore;
        public Text opponentScore;

        public void OnParentShowScore()
        {
            EmptyScores();
        }

        public void UpdateScores(MoveVO moveVO)
        {
            playerScore.text = (moveVO.playerScore > 0 ) ? ("+" + moveVO.playerScore.ToString()) : "";
            opponentScore.text = (moveVO.opponentScore > 0 ) ? ("+" + moveVO.opponentScore.ToString()) : "";
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

    }
}
