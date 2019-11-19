/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using UnityEngine;


namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public SendChatMessageSignal sendChatMessageSignal { get; set; }
        [Inject] public ClearActiveChatSignal clearActiveChatSignal { get; set; }
        [Inject] public ClearUnreadMessagesSignal clearUnreadMessagesSignal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }

        public void OnRegisterChat()
        {
            view.InitChat();
            view.chatSubmitSignal.AddListener(OnChatSubmit);
            view.openChatDlgSignal.AddListener(OnOpenChatDlg);
        }

        [ListensTo(typeof(EnableGameChatSignal))]
        public void OnEnableGameChat(ChatVO vo)
        {
            view.EnableGameChat(vo);
        }

        [ListensTo(typeof(DisplayChatMessageSignal))]
        public void OnDisplayChatMessage(ChatMessage msg)
        {
            view.OnReceive(msg);
        }

        [ListensTo(typeof(AddUnreadMessagesToBarSignal))]
        public void OnAddUnreadMessages(string friendId, int messagesCount)
        {
            view.EnableUnreadIndicator(friendId, messagesCount);
        }

        void OnChatSubmit(ChatMessage message)
        {
            sendChatMessageSignal.Dispatch(message);
        }

        void OnOpenChatDlg(string opponentId)
        {
            loadChatSignal.Dispatch(opponentId, true);
        }
    }
}
