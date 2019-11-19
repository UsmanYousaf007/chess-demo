/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Chat")]     
        public ChatBubble opponentChatBubble;
        public ChatBubble playerChatBubble;
        public Button opponentChatBubbleButton;
        public Button playerChatBubbleButton;
        public TMP_InputField inputField; 
        public GameObject unreadMessagesIndicator;
        public Text unreadMessagesCount;
        public Button maximizeChatDlgBtn;
        public Button editorSubmit;

        public Signal<ChatMessage> chatSubmitSignal = new Signal<ChatMessage>();
        public Signal<string> openChatDlgSignal = new Signal<string>();

        public GameObject[] chatInputSet;

        string opponentId;
        string playerId;

        private SpritesContainer defaultAvatarContainer;

        public void InitChat()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);

            inputField.onEndEdit.AddListener(OnSubmit);
            maximizeChatDlgBtn.onClick.AddListener(OnOpenChatDlg);
            opponentChatBubbleButton.onClick.AddListener(OnOpenChatDlg);
            playerChatBubbleButton.onClick.AddListener(OnOpenChatDlg);

            #if UNITY_EDITOR
            editorSubmit.gameObject.SetActive(true);
            editorSubmit.onClick.AddListener(()=>{OnSubmit(inputField.text);});
#else
            editorSubmit.gameObject.SetActive(false);
#endif
        }

        public void EnableGameChat(ChatVO vo)
        {
            unreadMessagesIndicator.SetActive(vo.hasUnreadMessages);
            unreadMessagesCount.text = vo.unreadMessagesCount.ToString();
            opponentId = vo.opponentId;
            playerId = vo.playerId;
            inputField.enabled = vo.isChatEnabled;
        }

        public void EnableUnreadIndicator(string friendId, int messagesCount)
        {
            if (friendId == opponentId)
            {
                unreadMessagesIndicator.SetActive(true);
                unreadMessagesCount.text = messagesCount.ToString();
            }
        }

        public void OnReceive(ChatMessage message)
        {
            if (opponentId == message.senderId && 
                message.text.Length > 0)
            {
                opponentChatBubble.gameObject.SetActive(true);
                opponentChatBubble.SetText(message.text, false);
            }
        }

        void OnSubmit(string text)
        {
            ChatMessage message = new ChatMessage();
            message.recipientId = opponentId;
            message.senderId = playerId;
            message.text = text;
            message.timestamp = TimeUtil.unixTimestampMilliseconds;
            message.guid = Guid.NewGuid().ToString();

            if (text.Length > 0)
            {
                playerChatBubble.gameObject.SetActive(true);
                playerChatBubble.SetText(text, true);

                inputField.text = "";

                chatSubmitSignal.Dispatch(message);

                showAdOnBack = true;
            }
        }

        void OnOpenChatDlg()
        {
            unreadMessagesIndicator.SetActive(false);
            openChatDlgSignal.Dispatch(opponentId);
        }
    }
}