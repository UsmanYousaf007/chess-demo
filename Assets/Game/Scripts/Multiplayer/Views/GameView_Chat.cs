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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Chat")]
        public ChatBubble opponentChatBubble;
        public ChatBubble playerChatBubble;
        public TMP_InputField inputField; 
        public GameObject chatCounter;
        public Text chatCounterLabel;

        public GameObject chatDlgBg;
        public Button maximizeChatDlgBtn;
        public Button minimizeChatDlgBtn;

        public Signal<string> chatSubmitSignal = new Signal<string>();
        public Signal openChatDlgSignal = new Signal();
        public Signal closeChatDlgSignal = new Signal();

        public Transform scrollViewContent;
        public ScrollRect scrollRect;
        public GameObject chatBubblePrefab;
        GameObject chatBubbleCloneSource;

        public void InitChat()
        {
            inputField.onSubmit.AddListener(OnSubmit);
            maximizeChatDlgBtn.onClick.AddListener(OnOpenChatDlg);
            minimizeChatDlgBtn.onClick.AddListener(OnCloseChatDlg);
        }

        public void OnParentShowChat()
        {
            
        }

        public void EnableGameChat(ChatMessages chatMessages, string playerId)
        {
            LogUtil.Log("*************************************** RECEIVED CHAT HISTORY ***************************************", "cyan");

            foreach (ChatMessage message in chatMessages.messageList)
            {
                AddChatBubble(message.text, playerId == message.senderId);
            }
        }

        public void ShowChatDlg()
        {
            chatDlgBg.SetActive(true);
            minimizeChatDlgBtn.gameObject.SetActive(true);
            maximizeChatDlgBtn.gameObject.SetActive(false);
            StartCoroutine(SetScrollPosition());
        }

        public void HideChatDlg()
        {
            chatDlgBg.SetActive(false);
            minimizeChatDlgBtn.gameObject.SetActive(false);
            maximizeChatDlgBtn.gameObject.SetActive(true);
        }

        public void OnReceive(string text)
        {
            if (text.Length > 0)
            {
                opponentChatBubble.gameObject.SetActive(true);
                opponentChatBubble.SetText(text);
                AddChatBubble(text, false);
            }
        }

        void OnSubmit(string text)
        {
            if (text.Length > 0)
            {
                playerChatBubble.gameObject.SetActive(true);
                playerChatBubble.SetText(text);
                AddChatBubble(text, true);

                inputField.text = "";

                chatSubmitSignal.Dispatch(text);
            }
        }

        void OnOpenChatDlg()
        {
            openChatDlgSignal.Dispatch();
        }

        void OnCloseChatDlg()
        {
            closeChatDlgSignal.Dispatch();
        }

        void AddChatBubble(string text, bool isPlayer)
        {
            GameObject chatBubbleContainer;
            if (chatBubbleCloneSource == null)
            {
                chatBubbleContainer = GameObject.Instantiate(chatBubblePrefab);
                chatBubbleCloneSource = chatBubbleContainer;
            }
            else
            {
                chatBubbleContainer = GameObject.Instantiate(chatBubbleCloneSource);
            }

            chatBubbleContainer.transform.SetParent(scrollViewContent, false);
            ChatBubble bubble = chatBubbleContainer.transform.GetChild(0).GetComponent<ChatBubble>();
            bubble.SetText(text);

            StartCoroutine(SetScrollPosition());
        }

        IEnumerator SetScrollPosition()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
