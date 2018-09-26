/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public LoadChatSignal loadChatSignal { get; set; }

        public void OnRegisterChat()
        {
            view.InitChat();
            view.chatButtonClickedSignal.AddListener(OnChatButtonClicked);
        }

        private void OnChatButtonClicked()
        {
            loadChatSignal.Dispatch();
        }
    }
}
