﻿/// @license Propriety <http://license.url>
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

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Dispatch Signals
        [Inject] public PreShowNotificationSignal preShowNotificationSignal { get; set; }
        [Inject] public PostShowNotificationSignal postShowNotificationSignal { get; set; }
        [Inject] public TapLongMatchSignal tapLongMatchSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public TurboLabz.CPU.SaveGameSignal saveGameSignal { get; set; }
        [Inject] public FindMatchSignal findMatchSignal { get; set; }

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }


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
                    notifications[0].obj.SetActive(true);
                    yield return new WaitForSeconds(notifications[0].duration);
                    notifications[0].obj.SetActive(false);
                    GameObject obj = notifications[0].obj;
                    notifications.Remove(notifications[0]);
                    Destroy(obj);
                    postShowNotificationSignal.Dispatch();
                }
                yield return new WaitForSeconds(1);
            }
        }

        public void AddNotification(NotificationVO notificationVO) 
        {
            float duration = NOTIFICATION_DURATION;

            // Check if on the same long match board
            if (matchInfoModel.activeLongMatchOpponentId == notificationVO.senderPlayerId)
            {
                return;
            }
            // Check if on the same quick match board
            if (matchInfoModel.activeMatch != null)
            {
                if (notificationVO.senderPlayerId == matchInfoModel.activeMatch.challengedId ||
                    notificationVO.senderPlayerId == matchInfoModel.activeMatch.challengerId)
                {
                    return;
                }

            }

            GameObject notifidationObj = Instantiate(notificationPrefab);
            notifidationObj.SetActive(false);
            Notification notification = notifidationObj.GetComponent<Notification>();
            notification.title.text = notificationVO.title;
            notification.body.text = notificationVO.body;
            notification.playButtonLabel.text = localizationService.Get(LocalizationKey.PLAY);
            notification.avatarBg.sprite = notification.defaultAvatar;
            Sprite pic = picsModel.GetPlayerPic(notificationVO.senderPlayerId);
            if (pic != null)
            {
                notification.senderPic.gameObject.SetActive(true);
                notification.senderPic.sprite = pic;
            }
            else if(playerModel.friends.ContainsKey(notificationVO.senderPlayerId))
            {
                if(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId != null)
                {
                    Sprite newSprite = defaultAvatarContainer.GetSprite(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId);
                    notification.avatarIcon.sprite = newSprite;
                    notification.avatarBg.sprite = notification.whiteAvatar;
                    notification.avatarBg.color = Colors.Color(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarBgColorId) ;
                    notification.senderPic.gameObject.SetActive(false);
                }
            }

            notification.closeButton.onClick.AddListener(OnCloseButtonClicked);
            notification.playButton.onClick.AddListener(OnPlayButtonClicked);
            notification.acceptQuickMatchButton.onClick.AddListener(OnAcceptQuickMatchButton);

            notification.playButton.gameObject.SetActive(false);

            string challengeId = GetChallengeId(notificationVO.senderPlayerId);
            if (challengeId != null)
            {
                MatchInfo matchInfo = matchInfoModel.matches[challengeId];
                if (matchInfo.isLongPlay && matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED)
                {
                    notification.playButton.gameObject.SetActive(true);
                }
            }

            // Quick Match invitation notification
            notification.acceptQuickMatchButton.gameObject.SetActive(false);
            notification.titleLarge.gameObject.SetActive(false);
            if (notificationVO.matchGroup != null)
            {
                Color specialColor = Colors.YELLOW;
                specialColor.a = notification.background.color.a;
                notification.background.color = specialColor;
                notification.acceptQuickMatchButton.gameObject.SetActive(true);
                notification.acceptQuickMatchButtonText.text = "Accept";
                duration = NOTIFICATION_QUICKMATCH_DURATION;

                notification.titleLarge.gameObject.SetActive(true);
                notification.title.gameObject.SetActive(false);
                notification.titleLarge.text = "Speed Chess";
                notification.body.text = notificationVO.title;
            }

            if (appInfoModel.gameMode == GameMode.QUICK_MATCH || appInfoModel.gameMode == GameMode.CPU)
            {
                notification.playButton.gameObject.SetActive(false);
                notification.acceptQuickMatchButton.gameObject.SetActive(false);
            }
     
            notifidationObj.transform.SetParent(gameObject.transform);
            notifidationObj.transform.position = positionDummy.transform.position;
            NotificationContainer notificationContainer = new NotificationContainer();
            notificationContainer.obj = notifidationObj;
            notificationContainer.playerId = notificationVO.senderPlayerId;
            notificationContainer.matchGroup = notificationVO.matchGroup;
            notificationContainer.duration = duration;
            notifications.Add(notificationContainer);
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

            FindMatchAction.Accept(findMatchSignal, notifications[0].matchGroup);
            FadeBlocker();
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

    }
}