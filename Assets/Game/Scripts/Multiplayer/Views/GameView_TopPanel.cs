/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Top Panel")]
        public GameObject[] topPanelObjects;

        public void ToggleTopPanel(bool show)
        {
            foreach(GameObject obj in topPanelObjects)
            {
                obj.SetActive(show);
            }
        }
    }
}
