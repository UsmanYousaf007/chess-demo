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
        [Inject] public SendChatMessageSignal sendChatMessageSignal { get; set; }

        public void OnRegisterChat()
        {
            view.InitChat();
            view.chatSubmitSignal.AddListener(OnChatSubmit);
            view.openChatDlgSignal.AddListener(OnOpenChatDlg);
            view.closeChatDlgSignal.AddListener(OnCloseChatDlg);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowChatDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_CHAT_DLG) 
            {
                view.ShowChatDlg();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideChatDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_CHAT_DLG)
            {
                view.HideChatDlg();
            }
        }


        [ListensTo(typeof(EnableGameChatSignal))]
        public void OnEnableGameChat(bool enable)
        {
          //  view.EnableGameChat(enable);
        }

        [ListensTo(typeof(DisplayChatMessageSignal))]
        public void OnDisplayChatMessage(ChatMessage msg)
        {
            view.OnReceive(msg.text);
        }

        void OnChatSubmit(string text)
        {
            sendChatMessageSignal.Dispatch(text);
        }

        void OnOpenChatDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_CHAT_DLG);
        }

        void OnCloseChatDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }
    }
}
