/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using TMPro;
using TurboLabz.TLUtils;
using System;
using UnityEngine;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Collections;
using TurboLabz.InstantGame;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class ChatView : View
    {
        //Public references
        public Text backBtnTxt;
        public Text clearChatBtnTxt;
        public Text defaultDayLineHeader;
        public Text defaultSystemMessage;
        public TMP_InputField inputField;
        public Button backBtn;
        public Button clearBtn;
        public GameObject[] defaultInfoSet;
        public GameObject chatDayLinePrefab;
        public Transform scrollViewContent;
        public ScrollRect scrollRect;
        public GameObject chatBubblePrefabRight;
        public GameObject chatBubblePrefabLeft;
        public Sprite defaultAvatar;
        public Image opponentHeaderProfilePic;
        public Image opponentHeaderAvatarBG;
        public Image opponentHeaderAvatarIcon;
        public Text opponentHeaderName;
        public Image opponentOnlineStatus;
        public Image chatPanelBackground;
        public Sprite online;
        public Sprite offline;
        public Sprite active;
        public Button minimizeChatBtn;
        public Text inputFieldDefaultText;
        public Image inputFieldIcon;
        public GameObject premiumBorder;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Signals
        public Signal<ChatMessage> chatSubmitSignal = new Signal<ChatMessage>();
        public Signal closeChatSignal = new Signal();
        public Signal<string> clearChatSignal = new Signal<string>();
        public Signal<string> clearUnreadMessagesSignal = new Signal<string>();

        //Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        private SpritesContainer defaultAvatarContainer;
        private bool hasUnreadMessages;
        private string opponentId;
        private string playerId;
        private List<GameObject> chatObjs = new List<GameObject>();
        private List<DateTime> handledDayLines = new List<DateTime>();
        private List<Image> opponentEmptyPics = new List<Image>();
        private Sprite playerProfilePic;
        private Sprite playerAvatarIconSprite;
        private Color playerAvatarBGColor;
        private Sprite opponentProfilePic;
        private Sprite opponentAvatarIconSprite;
        private Color opponentAvatarBGColor;
        private bool inGame;
        private bool isPlayerPremium;
        private bool isOpponentPremium;

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            backBtnTxt.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            clearChatBtnTxt.text = localizationService.Get(LocalizationKey.CHAT_CLEAR);
            defaultDayLineHeader.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_DAY_LINE);
            defaultSystemMessage.text = localizationService.Get(LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE);

            inputField.onEndEdit.AddListener(OnSubmit);

            backBtn.onClick.AddListener(OnClose);
            minimizeChatBtn.onClick.AddListener(OnClose);
            clearBtn.onClick.AddListener(OnClear);

            hasUnreadMessages = false;
        }

        public void Show()
        {
            if (hasUnreadMessages)
            {
                hasUnreadMessages = false;
            }

            chatPanelBackground.color = Colors.ColorAlpha(chatPanelBackground.color, Colors.FULL_ALPHA);
            gameObject.SetActive(true);
            StartCoroutine(SetScrollPosition());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Load(ChatVO vo)
        {
            opponentId = vo.opponentId;
            playerId = vo.playerId;
            inGame = vo.inGame;
            opponentHeaderName.text = vo.opponentName;
            hasUnreadMessages = vo.hasUnreadMessages;
            clearUnreadMessagesSignal.Dispatch(vo.opponentId);
            minimizeChatBtn.gameObject.SetActive(vo.inGame);
            premiumBorder.SetActive(vo.isOpponentPremium);
            isOpponentPremium = vo.isOpponentPremium;
            isPlayerPremium = vo.isPlayerPremium;

            if (!vo.isOnline && vo.isActive)
            {
                opponentOnlineStatus.sprite = active;
            }
            else
            {
                opponentOnlineStatus.sprite = vo.isOnline ? online : offline;
            }

            if (vo.playerProfilePic != null)
            {
                playerProfilePic = vo.playerProfilePic;
            }
            else
            {
                playerProfilePic = null;
                if (vo.avatarId != null)
                {
                    playerAvatarIconSprite = defaultAvatarContainer.GetSprite(vo.avatarId);
                    playerAvatarBGColor = Colors.Color(vo.avatarBgColorId);
                }
            }

            //For Opponent
            opponentHeaderAvatarIcon.gameObject.SetActive(false);
            opponentHeaderAvatarBG.gameObject.SetActive(false);

            if (vo.opponentProfilePic != null)
            {
                opponentHeaderProfilePic.gameObject.SetActive(true);
                opponentProfilePic = vo.opponentProfilePic;
                opponentHeaderProfilePic.sprite = vo.opponentProfilePic;
            }
            else if (vo.oppAvatarId != null)
            {
                opponentProfilePic = null;
                opponentHeaderAvatarIcon.gameObject.SetActive(true);
                opponentHeaderAvatarBG.gameObject.SetActive(true);
                opponentHeaderProfilePic.gameObject.SetActive(false);

                opponentHeaderAvatarBG.color = Colors.DISABLED_WHITE;
                opponentAvatarBGColor = Colors.DISABLED_WHITE;

                if (vo.oppAvatarBgColorId != null)
                {
                    opponentHeaderAvatarBG.color = Colors.Color(vo.oppAvatarBgColorId);
                    opponentAvatarBGColor = Colors.Color(vo.oppAvatarBgColorId);
                }

                opponentHeaderAvatarIcon.sprite = defaultAvatarContainer.GetSprite(vo.oppAvatarId);
                opponentAvatarIconSprite = defaultAvatarContainer.GetSprite(vo.oppAvatarId);

            }
            else
            {
                opponentProfilePic = defaultAvatar;
                opponentHeaderProfilePic.sprite = defaultAvatar;
                opponentHeaderProfilePic.gameObject.SetActive(true);
            }

            EnableChat(vo.isChatEnabled);
            CleanUpChat();

            if (vo.isChatEnabled)
            {
                foreach (ChatMessage message in vo.chatMessages.messageList)
                {
                    bool isPlayerMessage = vo.playerId == message.senderId;
                    AddChatBubble(message, isPlayerMessage);
                }
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
                AddChatBubble(message, true);
                inputField.text = "";
                chatSubmitSignal.Dispatch(message);
            }
        }

        public void OnReceive(ChatMessage message)
        {
            if (opponentId == message.senderId &&
                message.text.Length > 0)
            {
                AddChatBubble(message, false);
            }
        }

        public void EnableChat(bool isChatEnabled)
        {
            inputField.enabled = isChatEnabled;
            defaultSystemMessage.text = isChatEnabled ?
                localizationService.Get(LocalizationKey.CHAT_DEFAULT_SYSTEM_MESSAGE) :
                localizationService.Get(LocalizationKey.CHAT_DISABLED_SYSTEM_MESSAGE);
            inputFieldDefaultText.color = isChatEnabled ?
                Colors.WHITE_150 :
                Colors.DISABLED_WHITE;
            inputFieldIcon.color = isChatEnabled ?
                Colors.WHITE_150 :
                Colors.DISABLED_WHITE;

            if (!isChatEnabled)
            {
                clearChatSignal.Dispatch(opponentId);
            }
        }

        void OnClose()
        {
            closeChatSignal.Dispatch();

        }

        void OnClear()
        {
            CleanUpChat();
            clearChatSignal.Dispatch(opponentId);
        }

        void CleanUpChat()
        {
            foreach (GameObject obj in chatObjs)
            {
                Destroy(obj);
            }

            chatObjs.Clear();
            handledDayLines.Clear();
            opponentEmptyPics.Clear();

            foreach (GameObject obj in defaultInfoSet)
            {
                obj.SetActive(true);
            }
        }

        void AddChatBubble(ChatMessage message, bool isPlayer)
        {
            foreach (GameObject obj in defaultInfoSet)
            {
                obj.SetActive(false);
            }

            // Handle daylines
            DateTime messageLocalTime = TimeUtil.ToDateTime(message.timestamp).ToLocalTime();
            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            bool createDayLine = true;

            foreach (DateTime dt in handledDayLines)
            {
                if (dt.Date == messageLocalTime.Date)
                {
                    createDayLine = false;
                    break;
                }
            }

            if (createDayLine)
            {
                string dayLineText = "";

                if (messageLocalTime.Date == DateTime.Now.Date)
                {
                    dayLineText = localizationService.Get(LocalizationKey.CHAT_TODAY);
                }
                else if (messageLocalTime.Date == DateTime.Now.Date.Subtract(oneDay))
                {
                    dayLineText = localizationService.Get(LocalizationKey.CHAT_YESTERDAY);
                }
                else
                {
                    dayLineText = messageLocalTime.ToString("MMMM dd");
                }

                GameObject dayLine = Instantiate(chatDayLinePrefab);
                dayLine.SetActive(true);
                chatObjs.Add(dayLine);
                dayLine.GetComponent<Text>().text = dayLineText;
                dayLine.transform.SetParent(scrollViewContent, false);

                handledDayLines.Add(messageLocalTime.Date);
            }


            // Now render the text
            ChatBubble bubble;
            GameObject chatBubbleContainer;

            if (isPlayer)
            {
                chatBubbleContainer = Instantiate(chatBubblePrefabRight);
                chatBubbleContainer.SetActive(true);
                bubble = chatBubbleContainer.GetComponent<ChatBubble>();

                bubble.avatarBg.gameObject.SetActive(false);
                bubble.avatarIcon.gameObject.SetActive(false);
                bubble.premiumBorder.SetActive(isPlayerPremium);

                if (playerProfilePic != null)
                {
                    bubble.profilePic.sprite = playerProfilePic;
                }
                else
                {
                    if (playerAvatarIconSprite != null)
                    {
                        bubble.profilePic.gameObject.SetActive(false);
                        bubble.avatarBg.gameObject.SetActive(true);
                        bubble.avatarIcon.gameObject.SetActive(true);

                        bubble.avatarBg.color = playerAvatarBGColor;
                        bubble.avatarIcon.sprite = playerAvatarIconSprite;
                    }
                    else
                    {
                        bubble.profilePic.sprite = defaultAvatar;
                    }
                }
            }
            else
            {
                chatBubbleContainer = Instantiate(chatBubblePrefabLeft);
                chatBubbleContainer.SetActive(true);
                bubble = chatBubbleContainer.GetComponent<ChatBubble>();
                bubble.profilePic.sprite = opponentProfilePic ?? defaultAvatar;

                bubble.avatarBg.gameObject.SetActive(false);
                bubble.avatarIcon.gameObject.SetActive(false);
                bubble.premiumBorder.SetActive(isOpponentPremium);

                if (opponentProfilePic != null)
                {
                    bubble.profilePic.sprite = opponentProfilePic;
                }
                else
                {
                    if (opponentHeaderAvatarIcon != null)
                    {
                        bubble.profilePic.gameObject.SetActive(false);
                        bubble.avatarBg.gameObject.SetActive(true);
                        bubble.avatarIcon.gameObject.SetActive(true);

                        bubble.avatarBg.color = opponentAvatarBGColor;
                        bubble.avatarIcon.sprite = opponentAvatarIconSprite;
                    }
                    else
                    {
                        bubble.profilePic.sprite = defaultAvatar;
                    }
                }
                if (bubble.profilePic.sprite.name == defaultAvatar.name)
                {
                    opponentEmptyPics.Add(bubble.profilePic);
                }
            }

            chatBubbleContainer.transform.SetParent(scrollViewContent, false);
            chatObjs.Add(chatBubbleContainer);
            bubble.SetText(message.text, isPlayer);
            bubble.timer.text = messageLocalTime.ToString("h:mm tt");

            if (this.gameObject.activeInHierarchy)
                StartCoroutine(SetScrollPosition());
        }

        IEnumerator SetScrollPosition()
        {
            yield return null;
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void SetOpponentProfilePic(Sprite sprite, string playerId = null)
        {
            if (playerId == null || playerId != opponentId)
            {
                return;
            }

            if (sprite != null)
            {
                opponentHeaderAvatarIcon.gameObject.SetActive(false);
                opponentHeaderAvatarBG.gameObject.SetActive(false);

                opponentHeaderProfilePic.gameObject.SetActive(true);
                opponentProfilePic = sprite;
                opponentHeaderProfilePic.sprite = sprite;
            }
        }

        public void UpdateOnlineStatusSignal(string friendId, bool isOnline, bool isActive)
        {
            if (friendId == opponentId)
            {
                if (!isOnline && isActive)
                {
                    opponentOnlineStatus.sprite = active;
                }
                else
                {
                    opponentOnlineStatus.sprite = isOnline ? online : offline;
                }
            }
        }

        bool IsAnyMatchActiveWithOpponent()
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId.Equals(opponentId))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
