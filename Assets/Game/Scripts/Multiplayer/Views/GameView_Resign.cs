/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-19 11:47:21 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.signal.impl;

namespace TurboLabz.MPChess
{
    public partial class GameView
    {
        public Signal resignSignal = new Signal();
        public Button resignButton;

        public void InitResign()
        {
            resignButton.onClick.AddListener(OnResign);
        }

        public void CleanupResign()
        {
            resignButton.onClick.RemoveAllListeners();
        }

        private void OnResign()
        {
            EnableModalBlocker();
            resignSignal.Dispatch();
        }
    }
}
