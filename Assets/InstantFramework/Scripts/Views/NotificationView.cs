/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class NotificationView : View
    {
        private struct NotificationContainer
        {
            public GameObject obj;
            public string playerId;
            public string matchGroup;
            public float duration;
            public NotificationVO notificationVO;
            public Notification notification;
        };

        public GameObject notificationPrefab;
        public GameObject positionDummy;
        public Image uiBlocker;
        private List<NotificationContainer> notifications;
        private Coroutine processNotificaitonCR;
        private SpritesContainer defaultAvatarContainer;
        private bool isPaused = false;

        private const float NOTIFICATION_DURATION = 5.0f;
        private const float NOTIFICATION_QUICKMATCH_DURATION = 15.0f;

        // Models
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IChatModel chatModel { get; set; }

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Dispatch Signals
        [Inject] public PreShowNotificationSignal preShowNotificationSignal { get; set; }
        [Inject] public PostShowNotificationSignal postShowNotificationSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public TurboLabz.CPU.SaveGameSignal saveGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSingal { get; set; }
        [Inject] public LoadChatSignal loadChatSignal { get; set; }
        [Inject] public UpdateFriendPicSignal updateFriendPicSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        public void Init()
        {
            notifications = new List<NotificationContainer>();
            processNotificaitonCR = StartCoroutine(ProcessNotificationCR());
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            uiBlocker.gameObject.SetActive(false);
            isPaused = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Pause(bool enable)
        {
            isPaused = enable;

            if (isPaused && notifications.Count != 0)
            {
                notifications[0].obj.SetActive(false);
            }
        }

        private IEnumerator ProcessNotificationCR()
        {
            while (true)
            {
                while (isPaused)
                {
                    yield return null;
                }

                if (notifications.Count != 0)
                {
                    preShowNotificationSignal.Dispatch();
                    PreShowNotificationSetup(notifications[0]);
                    notifications[0].obj.SetActive(true);
                    appInfoModel.isNotificationActive = true;
                    yield return new WaitForSeconds(notifications[0].duration);
                    notifications[0].obj.SetActive(false);
                    GameObject obj = notifications[0].obj;
                    notifications.Remove(notifications[0]);
                    Destroy(obj);
                    appInfoModel.isNotificationActive = false;
                    postShowNotificationSignal.Dispatch();
                }
                yield return new WaitForSeconds(1);
            }
        }

        private void PreShowNotificationSetup(NotificationContainer notificationContainer)
        {
            if (appInfoModel.gameMode == GameMode.QUICK_MATCH || appInfoModel.gameMode == GameMode.CPU)
            {
                Notification notificationObj = notificationContainer.obj.GetComponent<Notification>();
                notificationObj.playButton.gameObject.SetActive(false);
                notificationObj.acceptQuickMatchButton.gameObject.SetActive(false);
            }
        }

        public void AddNotification(NotificationVO notificationVO)
        {
            float duration = NOTIFICATION_DURATION;

            // Check if on the same long match board
            if (matchInfoModel.activeLongMatchOpponentId == notificationVO.senderPlayerId
                && chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                return;
            }
            // Check if on the same quick match board
            if (matchInfoModel.activeMatch != null
                && matchInfoModel.activeChallengeId != null)
            {
                if (notificationVO.senderPlayerId == matchInfoModel.activeMatch.challengedId ||
                    notificationVO.senderPlayerId == matchInfoModel.activeMatch.challengerId)
                {
                    return;
                }
            }

            // Check if its message notifcation and on same chat window
            if (notificationVO.title.Contains("sent you a message")
                && notificationVO.senderPlayerId.Equals(chatModel.activeChatId))
            {
                return;
            }

            GameObject notifidationObj = Instantiate(notificationPrefab);
            notifidationObj.SetActive(false);
            Notification notification = notifidationObj.GetComponent<Notification>();
            notification.title.text = notificationVO.title;
            notification.body.text = notificationVO.body;
            notification.playButtonLabel.text = localizationService.Get(LocalizationKey.PLAY);
            notification.avatarBg.sprite = notification.defaultAvatar;
            notification.premiumBorder.SetActive(notificationVO.isPremium);
            notification.senderPic.gameObject.SetActive(false);

            Sprite pic = picsModel.GetPlayerPic(notificationVO.senderPlayerId);
            if (pic != null)
            {
                notification.senderPic.gameObject.SetActive(true);
                notification.senderPic.sprite = pic;
            }
            else
            {
                if (!(notificationVO.profilePicURL.Equals("unassignedURL") || notificationVO.Equals("undefined")))
                {
                    facebookService.GetSocialPic(notificationVO.profilePicURL, notificationVO.senderPlayerId).Then(OnGetSocialPic);
                }

                if (notificationVO.senderPlayerId != null && playerModel.friends.ContainsKey(notificationVO.senderPlayerId))
                {
                    if (playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId != null)
                    {
                        Sprite newSprite = defaultAvatarContainer.GetSprite(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId);
                        notification.avatarIcon.sprite = newSprite;
                        notification.avatarBg.sprite = notification.whiteAvatar;
                        notification.avatarBg.color = Colors.Color(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarBgColorId);
                        notification.senderPic.gameObject.SetActive(false);
                    }
                }
            }
            notification.closeButton.onClick.AddListener(OnCloseButtonClicked);
            notification.playButton.onClick.AddListener(OnPlayButtonClicked);
            notification.acceptQuickMatchButton.onClick.AddListener(OnAcceptQuickMatchButton);

            notification.playButton.gameObject.SetActive(false);

            string challengeId = GetChallengeId(notificationVO.senderPlayerId);

            // case: challengeId == notificationVO.challengeId indicates current challenge
            // case  notificationVO.challengeId == "undefined" indicates running challenge
            if (challengeId != null && ((challengeId == notificationVO.challengeId) || (notificationVO.challengeId == "undefined")))
            {
                MatchInfo matchInfo = matchInfoModel.matches[challengeId];
                if (matchInfo.isLongPlay && matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED)
                {
                    notification.playButton.gameObject.SetActive(true);
                }
            }

            if (notificationVO.body.Contains("View in chat"))
            {
                notification.playButtonLabel.text = localizationService.Get(LocalizationKey.VIEW);
                notification.playButton.gameObject.SetActive(true);
                notification.playButton.onClick.RemoveAllListeners();
                notification.playButton.onClick.AddListener(OnViewChatClicked);
            }

            // Quick Match invitation notification
            notification.acceptQuickMatchButton.gameObject.SetActive(false);
            notification.titleLarge.gameObject.SetActive(false);
            if (notificationVO.matchGroup != "undefined")
            {
                if ((TimeUtil.unixTimestampMilliseconds - notificationVO.timeSent) / 1000 > NOTIFICATION_QUICKMATCH_DURATION)
                {
                    return;
                }

                Color specialColor = Colors.YELLOW;
                specialColor.a = notification.background.color.a;
                notification.background.color = specialColor;
                notification.playButton.gameObject.SetActive(false);
                notification.acceptQuickMatchButton.gameObject.SetActive(true);
                notification.acceptQuickMatchButtonText.text = "Accept";
                duration = NOTIFICATION_QUICKMATCH_DURATION;

                notification.titleLarge.gameObject.SetActive(true);
                notification.title.gameObject.SetActive(false);
                notification.titleLarge.text = "5-Minutes Game";
                notification.body.text = notificationVO.title;

                if (!notification.senderPic.gameObject.activeSelf)
                {
                    Sprite newSprite = defaultAvatarContainer.GetSprite(notificationVO.avatarId);
                    notification.avatarIcon.sprite = newSprite;
                    notification.avatarBg.sprite = notification.whiteAvatar;
                    notification.avatarBg.color = Colors.Color(notificationVO.avaterBgColorId);
                }
            }

            if (appInfoModel.gameMode == GameMode.QUICK_MATCH || appInfoModel.gameMode == GameMode.CPU)
            {
                notification.playButton.gameObject.SetActive(false);
                notification.acceptQuickMatchButton.gameObject.SetActive(false);
            }

            notifidationObj.transform.SetParent(gameObject.transform);
            var notificationPosition = new Vector3(0, positionDummy.transform.position.y, 0); 
            notifidationObj.transform.position = notificationPosition;
            var rt = notifidationObj.GetComponent<RectTransform>();
            rt.offsetMax = new Vector2(0, rt.offsetMax.y);

            //if (notifidationObj.gameObject.transform.localScale.x > 1.0f)
            //{
                notifidationObj.gameObject.transform.localScale = Vector3.one;
            //}

            NotificationContainer notificationContainer = new NotificationContainer();
            notificationContainer.obj = notifidationObj;
            notificationContainer.playerId = notificationVO.senderPlayerId;
            notificationContainer.matchGroup = notificationVO.matchGroup;
            notificationContainer.duration = duration;
            notificationContainer.notificationVO = notificationVO;
            notificationContainer.notification = notification;
            notifications.Add(notificationContainer);
        }

        public void ProcessOpenedNotification(NotificationVO notificationVO)
        {
            string challengeId = GetChallengeId(notificationVO.senderPlayerId);
            if (challengeId != null)
            {
                MatchInfo matchInfo = matchInfoModel.matches[challengeId];
                if (matchInfo.isLongPlay && matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED)
                {
                    tapLongMatchSignal.Dispatch(notificationVO.senderPlayerId, false);
                }
            }

            if (notificationVO.matchGroup != "undefined")
            {
                if((TimeUtil.unixTimestampMilliseconds - notificationVO.timeSent)/1000 > NOTIFICATION_QUICKMATCH_DURATION)
                {
                    //show dailogue
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

                    var vo = new ConfirmDlgVO
                    {
                        title = localizationService.Get(LocalizationKey.QUICK_MATCH_EXPIRED),
                        desc = localizationService.Get(LocalizationKey.QUICK_MATCH_EXPIRED_REASON),
                        yesButtonText = localizationService.Get(LocalizationKey.LONG_PLAY_OK),
                        onClickYesButton = delegate
                        {
                            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                        }
                    };

                    updateConfirmDlgSignal.Dispatch(vo);
                    return;
                }

                Sprite pic = picsModel.GetPlayerPic(notificationVO.senderPlayerId);
                if (pic == null)
                {
                    if (!(notificationVO.profilePicURL.Equals("unassignedURL") || notificationVO.Equals("undefined")))
                    {
                        facebookService.GetSocialPic(notificationVO.profilePicURL, notificationVO.senderPlayerId).Then(OnGetSocialPic);
                    }
                }
                FindMatchAction.Accept(findMatchSignal, notificationVO.senderPlayerId, notificationVO.matchGroup,
                                        notificationVO.avatarId, notificationVO.avaterBgColorId);
            }
        }

        private void OnCloseButtonClicked()
        {
            notifications[0].obj.SetActive(false);
        }

        private void OnPlayButtonClicked()
        {
            notifications[0].obj.SetActive(false);
            if (appInfoModel.gameMode == GameMode.CPU)
            {
                saveGameSignal.Dispatch();
            }
            loadLobbySignal.Dispatch();
            cancelHintSingal.Dispatch();
            tapLongMatchSignal.Dispatch(notifications[0].playerId, false);
            FadeBlocker();
        }

        private void OnAcceptQuickMatchButton()
        {
            notifications[0].obj.SetActive(false);
            if (appInfoModel.gameMode == GameMode.CPU)
            {
                saveGameSignal.Dispatch();
            }
            loadLobbySignal.Dispatch();
            cancelHintSingal.Dispatch();
            FindMatchAction.Accept(findMatchSignal, notifications[0].playerId, notifications[0].matchGroup,
                notifications[0].notificationVO.avatarId, notifications[0].notificationVO.avaterBgColorId);
            FadeBlocker();
        }

        private void OnViewChatClicked()
        {
            notifications[0].obj.SetActive(false);
            loadChatSignal.Dispatch(notifications[0].notificationVO.senderPlayerId, false);
        }

        void FadeBlocker()
        {
            uiBlocker.color = Colors.WHITE_150;
            uiBlocker.gameObject.SetActive(true);
            DOTween.ToAlpha(() => uiBlocker.color, x => uiBlocker.color = x, 0.0f, 1.0f).OnComplete(OnFadeComplete);
        }

        void OnFadeComplete()
        {
            uiBlocker.gameObject.SetActive(false);
        }

        private string GetChallengeId(string opponentId)
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId == opponentId)
                {
                    return entry.Key;
                }
            }

            return null;
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string playerId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                picsModel.SetPlayerPic(playerId, sprite, false);
                updateFriendPicSignal.Dispatch(playerId, sprite);

                if (notifications.Count > 0 && playerId.Equals(notifications[0].notificationVO.senderPlayerId))
                {
                    notifications[0].notification.senderPic.gameObject.SetActive(true);
                    notifications[0].notification.senderPic.sprite = sprite;
                }
            }
        }
    }
}