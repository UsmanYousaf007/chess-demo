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
        public Image chatPanelBackground;

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
        string opponentId;
        List<Image> opponentEmptyPics = new List<Image>();

        public void InitChat()
        {
            backToGameBtnTxt.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            clearChatBtnTxt.text = localizationService.Get(LocalizationKey.CHAT_CLEAR);
            defaultDayLineHeader.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_DAY_LINE);
            defaultSystemMessage.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE);

            inputField.onEndEdit.AddListener(OnSubmit);

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
            chatPanelBackground.color = Colors.ColorAlpha(chatPanelBackground.color, Colors.FULL_ALPHA);
        }

        public void EnableGameChat(ChatVO vo)
        {
            CleanUpChat();

            opponentHeaderName.text = vo.opponentName;

            playerProfilePic = vo.playerProfilePic ?? defaultAvatar;

            if (vo.opponentProfilePic == null)
            {
                opponentProfilePic = defaultAvatar;
                opponentHeaderProfilePic.sprite = defaultAvatar;
            }
            else
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
            if (sprite == null)
            {
                opponentProfilePic = defaultAvatar;
                opponentHeaderProfilePic.sprite = defaultAvatar;
            }
            else
            {
                opponentProfilePic = sprite;
                opponentHeaderProfilePic.sprite = sprite;

                foreach (Image img in opponentEmptyPics)
                {
                    img.sprite = sprite;
                }
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

            chessboardBlocker.SetActive(true);
        }

        public void HideChatDlg()
        {
            chatPanel.SetActive(false);
            minimizeChatDlgBtn.gameObject.SetActive(false);
            maximizeChatDlgBtn.gameObject.SetActive(true);
            backToFriendsButton.gameObject.SetActive(isLongPlay);
            backToFriendsLabel.gameObject.SetActive(isLongPlay);
            backToGameBtnTxt.gameObject.SetActive(false);

            chessboardBlocker.SetActive(false);
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
        }

        void OnCloseChatDlg()
        {
            closeChatDlgSignal.Dispatch();
            
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
                GameObject dayLine = Instantiate(chatDayLinePrefab);
                dayLine.SetActive(true);
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
                chatBubbleContainer = Instantiate(chatBubblePrefabRight);
                chatBubbleContainer.SetActive(true);
                bubble = chatBubbleContainer.GetComponent<ChatBubble>();
                bubble.profilePic.sprite = playerProfilePic ?? defaultAvatar;
            }
            else
            {
                chatBubbleContainer = Instantiate(chatBubblePrefabLeft);
                chatBubbleContainer.SetActive(true);
                bubble = chatBubbleContainer.GetComponent<ChatBubble>();
                bubble.profilePic.sprite = opponentProfilePic ?? defaultAvatar;

                if (bubble.profilePic.sprite.name == defaultAvatar.name)
                {
                    opponentEmptyPics.Add(bubble.profilePic);
                }
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
                Destroy(obj);
            }

            chatObjs.Clear();
            dayLines.Clear();
            opponentEmptyPics.Clear();

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
