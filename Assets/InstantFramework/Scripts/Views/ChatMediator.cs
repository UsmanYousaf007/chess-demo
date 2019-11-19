/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.Multiplayer;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ChatMediator : Mediator
    {
        // View injection
        [Inject] public ChatView view { get; set; }

        // Dispatch Signals
        [Inject] public SendChatMessageSignal sendChatMessageSignal { get; set; }
        [Inject] public ClearActiveChatSignal clearActiveChatSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ClearUnreadMessagesSignal clearUnreadMessagesSignal { get; set; }

        //Models
        [Inject] public IChatModel chatModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.chatSubmitSignal.AddListener(OnSubmit);
            view.closeChatSignal.AddListener(OnClose);
            view.clearChatSignal.AddListener(OnClear);
            view.clearUnreadMessagesSignal.AddListener(OnClearUnreadMessages);
        }

        void OnSubmit(ChatMessage message)
        {
            sendChatMessageSignal.Dispatch(message);
        }

        void OnClose()
        {
            chatModel.activeChatId = null;
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        void OnClear(string opponentId)
        {
            clearActiveChatSignal.Dispatch(opponentId);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAT)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAT)
            {
                view.Hide();
            }
        }

        void OnClearUnreadMessages(string opponentId)
        {
            clearUnreadMessagesSignal.Dispatch(opponentId);
        }

        [ListensTo(typeof(DisplayChatMessageSignal))]
        public void OnDisplayChatMessage(ChatMessage msg)
        {
            view.OnReceive(msg);
        }

        [ListensTo(typeof(UpdateChatView))]
        public void OnLoadChat(ChatVO chatVO)
        {
            view.Load(chatVO);
        }

        [ListensTo(typeof(UpdateFriendOnlineStatusSignal))]
        public void OnUpdateOnlineStatus(ProfileVO vo)
        {
            view.UpdateOnlineStatusSignal(vo.playerId, vo.isOnline, vo.isActive);
        }

        [ListensTo(typeof(UpdateFriendPicSignal))]
        public void OnUpdatePic(string playerId, Sprite sprite)
        {
            view.SetOpponentProfilePic(sprite, playerId);
        }

        [ListensTo(typeof(ChallengeAcceptedSignal))]
        public void OnEnableChat()
        {
            view.EnableChat();
        }
    }
}
