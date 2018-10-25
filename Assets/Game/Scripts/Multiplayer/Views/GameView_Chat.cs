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
        public Image opponentHeaderProfilePic;
        public Text opponentHeaderName;
        public Image opponentOnlineStatus;
        public Image opponentInGameOnlineStatus;
        public Sprite online;
        public Sprite offline;
        public GameObject[] defaultInfoSet;
        public Text defaultDayLineHeader;
        public TMP_Text defaultSystemMessage;

        public ChatBubble opponentChatBubble;
        public ChatBubble playerChatBubble;
        public Button opponentChatBubbleButton;
        public Button playerChatBubbleButton;
        public TMP_InputField inputField; 
        public GameObject unreadMessagesIndicator;


        public GameObject chatPanel;
        public Button maximizeChatDlgBtn;
        public Button minimizeChatDlgBtn;
        public Button clearActiveChatBtn;
        public Text clearChatBtnTxt;

        public Signal<string> chatSubmitSignal = new Signal<string>();
        public Signal openChatDlgSignal = new Signal();
        public Signal closeChatDlgSignal = new Signal();
        public Signal clearActiveChatSignal = new Signal();
        public Signal clearUnreadMessagesSignal = new Signal();

        public Transform scrollViewContent;
        public ScrollRect scrollRect;
        public GameObject chatBubblePrefabLeft;
        public GameObject chatBubblePrefabRight;
        public GameObject chatDayLinePrefab;
        public Button editorSubmit;
        public Button backToGameBtn;
        public Text backToGameBtnTxt;

        public GameObject[] chatInputSet;

        [HideInInspector]
        public Sprite opponentProfilePic;
        [HideInInspector] 
        public Sprite playerProfilePic;


        List<GameObject> chatObjs = new List<GameObject>();
        List<int> dayLines = new List<int>();
        GameObject chatBubbleCloneSourceLeft;
        GameObject chatBubbleCloneSourceRight;
        string opponentId;

        public void InitChat()
        {
            backToGameBtnTxt.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            clearChatBtnTxt.text = localizationService.Get(LocalizationKey.CHAT_CLEAR);
            defaultDayLineHeader.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_DAY_LINE);
            defaultSystemMessage.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE);

            inputField.onSubmit.AddListener(OnSubmit);
            maximizeChatDlgBtn.onClick.AddListener(OnOpenChatDlg);
            minimizeChatDlgBtn.onClick.AddListener(OnCloseChatDlg);
            backToGameBtn.onClick.AddListener(OnCloseChatDlg);
            opponentChatBubbleButton.onClick.AddListener(OnOpenChatDlg);
            playerChatBubbleButton.onClick.AddListener(OnOpenChatDlg);
            clearActiveChatBtn.onClick.AddListener(OnClearActiveChat);

            #if UNITY_EDITOR
            editorSubmit.gameObject.SetActive(true);
            editorSubmit.onClick.AddListener(()=>{OnSubmit(inputField.text);});
            #else
            editorSubmit.gameObject.SetActive(false);
            #endif
        }

        public void OnParentShowChat()
        {
            // Nothing to do for now
        }

        public void EnableGameChat(ChatVO vo)
        {
            CleanUpChat();

            chatBubbleCloneSourceLeft = null;
            chatBubbleCloneSourceRight = null;

            opponentHeaderName.text = vo.opponentName;

            if (vo.playerProfilePic != null)
            {
                playerProfilePic = vo.playerProfilePic;
            }

            if (vo.opponentProfilePic != null)
            {
                opponentProfilePic = vo.opponentProfilePic;
                opponentHeaderProfilePic.sprite = vo.opponentProfilePic;
            }

            foreach (ChatMessage message in vo.chatMessages.messageList)
            {
                bool isPlayerMessage = vo.playerId == message.senderId;
                AddChatBubble(message, isPlayerMessage);
            }

            unreadMessagesIndicator.SetActive(vo.hasUnreadMessages);
            opponentId = vo.opponentId;

            opponentOnlineStatus.sprite = opponentInGameOnlineStatus.sprite;

        }

        public void UpdateChatOpponentPic(Sprite sprite)
        {
            if (sprite != null)
            {
                opponentProfilePic = sprite;
                opponentHeaderProfilePic.sprite = sprite;
            }
        }

        public void UpdateFriendOnlineStatusSignal(string friendId, bool isOnline)
        {
            if (friendId == opponentId)
            {
                opponentOnlineStatus.sprite = isOnline ? online : offline;
            }
        }

        public void ShowChatDlg()
        {
            chatPanel.SetActive(true);
            minimizeChatDlgBtn.gameObject.SetActive(true);
            maximizeChatDlgBtn.gameObject.SetActive(false);
            backToFriendsButton.gameObject.SetActive(false);
            backToFriendsLabel.gameObject.SetActive(false);
            backToGameBtnTxt.gameObject.SetActive(true);
            StartCoroutine(SetScrollPosition());

            unreadMessagesIndicator.SetActive(false);
            clearUnreadMessagesSignal.Dispatch();
        }

        public void HideChatDlg()
        {
            chatPanel.SetActive(false);
            minimizeChatDlgBtn.gameObject.SetActive(false);
            maximizeChatDlgBtn.gameObject.SetActive(true);
            backToFriendsButton.gameObject.SetActive(isLongPlay);
            backToFriendsLabel.gameObject.SetActive(isLongPlay);
            backToGameBtnTxt.gameObject.SetActive(false);
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
            foreach (GameObject obj in defaultInfoSet)
            {
                obj.SetActive(false);
            }

            GameObject chatBubbleContainer;
            DateTime dt = TimeUtil.ToDateTime(message.timestamp).ToLocalTime();

            // Handle daylines
            double daysSinceNow = DateTime.UtcNow.Subtract(dt).TotalDays;
            int dayLineIndex = Mathf.FloorToInt((float)daysSinceNow);

            if (dayLines.IndexOf(dayLineIndex) < 0)
            {
                dayLines.Add(dayLineIndex);
                GameObject dayLine = GameObject.Instantiate(chatDayLinePrefab);
                chatObjs.Add(dayLine);
                Text dayLineText = dayLine.GetComponent<Text>();

                if (daysSinceNow < 1)
                {
                    dayLineText.text = localizationService.Get(LocalizationKey.CHAT_TODAY);
                }
                else if (daysSinceNow < 2)
                {
                    dayLineText.text = localizationService.Get(LocalizationKey.CHAT_YESTERDAY);
                }
                else
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
                if (playerProfilePic != null) bubble.profilePic.sprite = playerProfilePic;
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
                if (opponentProfilePic != null) bubble.profilePic.sprite = opponentProfilePic;
            }



            chatBubbleContainer.transform.SetParent(scrollViewContent, false);
            chatObjs.Add(chatBubbleContainer);
            bubble.SetText(message.text, isPlayer);
            bubble.timer.text = dt.ToString("h:mm tt");

            StartCoroutine(SetScrollPosition());
        }

        IEnumerator SetScrollPosition()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }

        void CleanUpChat()
        {
            foreach (GameObject obj in chatObjs)
            {
                GameObject.Destroy(obj);
            }

            chatObjs.Clear();
            dayLines.Clear();

            foreach (GameObject obj in defaultInfoSet)
            {
                obj.SetActive(true);
            }
        }

        void OnClearActiveChat()
        {
            CleanUpChat();
            clearActiveChatSignal.Dispatch();
        }
    }
}
