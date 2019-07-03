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

        public void OnRegisterChat()
        {
            view.InitChat();
            view.chatSubmitSignal.AddListener(OnChatSubmit);
            view.openChatDlgSignal.AddListener(OnOpenChatDlg);
            view.closeChatDlgSignal.AddListener(OnCloseChatDlg);
            view.clearActiveChatSignal.AddListener(OnClearActiveChat);
            view.clearUnreadMessagesSignal.AddListener(OnClearUnreadMessages);
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
        public void OnAddUnreadMessages(string friendId)
        {
            view.EnableUnreadIndicator(friendId);
        }

        [ListensTo(typeof(UpdateChatOpponentPicSignal))]
        public void OnUpdateChatOpponentPic(Sprite sprite)
        {
            view.UpdateChatOpponentPic(sprite);
        }

        [ListensTo(typeof(UpdateFriendOnlineStatusSignal))]
        public void OnUpdateFriendOnlineStatusSignal(ProfileVO vo)
        {
            view.UpdateFriendOnlineStatusSignal(vo.playerId, vo.isOnline, vo.isActive);
        }

        void OnChatSubmit(ChatMessage message)
        {
            sendChatMessageSignal.Dispatch(message);
        }

        void OnOpenChatDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_CHAT_DLG);
        }

        void OnCloseChatDlg()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }

        void OnClearActiveChat(string opponentId)
        {
            clearActiveChatSignal.Dispatch(opponentId);
        }

        void OnClearUnreadMessages(string opponentId)
        {
            clearUnreadMessagesSignal.Dispatch(opponentId);
        }

    }
}
