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
        public GameObject chatBubblePrefabLeft;
        public GameObject chatBubblePrefabRight;
        public GameObject chatDayLinePrefab;
        public Button editorSubmit;

        public List<GameObject> chatObjs = new List<GameObject>();
        public List<int> dayLines = new List<int>();

        [HideInInspector]
        public Sprite opponentProfilePic;
        [HideInInspector] 
        public Sprite playerProfilePic;


        GameObject chatBubbleCloneSourceLeft;
        GameObject chatBubbleCloneSourceRight;

        public void InitChat()
        {
            inputField.onSubmit.AddListener(OnSubmit);
            maximizeChatDlgBtn.onClick.AddListener(OnOpenChatDlg);
            minimizeChatDlgBtn.onClick.AddListener(OnCloseChatDlg);

            #if UNITY_EDITOR
            editorSubmit.gameObject.SetActive(true);
            editorSubmit.onClick.AddListener(()=>{OnSubmit(inputField.text);});
            #else
            editorSubmit.gameObject.SetActive(false);
            #endif
        }

        public void OnParentShowChat()
        {
            
        }

        public void EnableGameChat(ChatVO vo)
        {
            foreach (GameObject obj in chatObjs)
            {
                GameObject.Destroy(obj);
            }

            chatObjs.Clear();
            dayLines.Clear();

            playerProfilePic = vo.playerProfilePic;
            opponentProfilePic = vo.opponentProfilePic;

            foreach (ChatMessage message in vo.chatMessages.messageList)
            {
                AddChatBubble(message, vo.playerId == message.senderId);
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

        public void OnReceive(ChatMessage message)
        {
            if (message.text.Length > 0)
            {
                opponentChatBubble.gameObject.SetActive(true);
                opponentChatBubble.SetText(message.text, false);
                AddChatBubble(message, false);
            }
        }

        void OnSubmit(string text)
        {
            ChatMessage message;
            message.recipientId = null;
            message.senderId = null;
            message.text = text;
            message.timestamp = TimeUtil.unixTimestampMilliseconds;

            if (text.Length > 0)
            {
                playerChatBubble.gameObject.SetActive(true);
                playerChatBubble.SetText(text, true);
                AddChatBubble(message, true);

                inputField.text = "";

                chatSubmitSignal.Dispatch(text);
            }
        }

        void OnOpenChatDlg()
        {
            openChatDlgSignal.Dispatch();
            chessboardBlocker.SetActive(true);
        }

        void OnCloseChatDlg()
        {
            closeChatDlgSignal.Dispatch();
            chessboardBlocker.SetActive(false);
        }

        void AddChatBubble(ChatMessage message, bool isPlayer)
        {
            GameObject chatBubbleContainer;
            DateTime dt = TimeUtil.ToDateTime(message.timestamp);

            // Handle daylines
            int daysSinceNow = DateTime.UtcNow.Subtract(dt).Days;

            if (dayLines.IndexOf(daysSinceNow) < 0)
            {
                dayLines.Add(daysSinceNow);
                GameObject dayLine = GameObject.Instantiate(chatDayLinePrefab);
                chatObjs.Add(dayLine);
                Text dayLineText = dayLine.GetComponent<Text>();

                if (daysSinceNow == 0)
                {
                    dayLineText.text = localizationService.Get(LocalizationKey.CHAT_TODAY);
                }
                else if (daysSinceNow == 1)
                {
                    dayLineText.text = localizationService.Get(LocalizationKey.CHAT_YESTERDAY);
                }
                else if (daysSinceNow > 1)
                {
                    dayLineText.text = dt.ToString("MMMM dd");
                }

                dayLine.transform.SetParent(scrollViewContent, false);
            }
                
            // Now render the text
            ChatBubble bubble;

            if (isPlayer)
            {
                if (chatBubbleCloneSourceRight == null)
                {
                    chatBubbleContainer = GameObject.Instantiate(chatBubblePrefabRight);
                    chatBubbleCloneSourceRight = chatBubbleContainer;
                }
                else
                {
                    chatBubbleContainer = GameObject.Instantiate(chatBubbleCloneSourceRight);
                }

                bubble = chatBubbleContainer.GetComponent<ChatBubble>();
                bubble.profilePic.sprite = playerProfilePic;
            }
            else
            {
                if (chatBubbleCloneSourceLeft == null)
                {
                    chatBubbleContainer = GameObject.Instantiate(chatBubblePrefabLeft);
                    chatBubbleCloneSourceLeft = chatBubbleContainer;
                }
                else
                {
                    chatBubbleContainer = GameObject.Instantiate(chatBubbleCloneSourceLeft);
                }

                bubble = chatBubbleContainer.GetComponent<ChatBubble>();
                bubble.profilePic.sprite = opponentProfilePic;
            }



            chatBubbleContainer.transform.SetParent(scrollViewContent, false);
            bubble.SetText(message.text, isPlayer);
            bubble.timer.text = dt.ToString("h:mm tt");

            StartCoroutine(SetScrollPosition());
        }

        IEnumerator SetScrollPosition()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
