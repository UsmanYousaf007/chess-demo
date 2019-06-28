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

namespace TurboLabz.InstantGame
{
    public class NotificationView : View
    {
        public GameObject notificationPrefab;
        public GameObject positionDummy;
        private List<GameObject> notifications;
        private Coroutine processNotificaitonCR;
        private SpritesContainer defaultAvatarContainer;

        // Models
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Dispatch Signals
        [Inject] public PreShowNotificationSignal preShowNotificationSignal { get; set; }
        [Inject] public PostShowNotificationSignal postShowNotificationSignal { get; set; }

        public void Init()
        {
            notifications = new List<GameObject>();
            processNotificaitonCR = StartCoroutine(ProcessNotificationCR());
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator ProcessNotificationCR()
        {
            while (true)
            {
                if (notifications.Count != 0)
                {
                    preShowNotificationSignal.Dispatch();
                    notifications[0].SetActive(true);
                    yield return new WaitForSeconds(5);
                    notifications[0].SetActive(false);
                    GameObject obj = notifications[0];
                    notifications.Remove(notifications[0]);
                    Destroy(obj);
                    postShowNotificationSignal.Dispatch();
                }
                yield return new WaitForSeconds(1);
            }
        }

        public void AddNotification(NotificationVO notificationVO) 
        {
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
            Sprite pic = picsModel.GetPlayerPic(notificationVO.senderPlayerId);
            if (pic != null)
            {
                notification.senderPic.sprite = pic;
            }
            else if(playerModel.friends.ContainsKey(notificationVO.senderPlayerId))
            {
                if(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId != null)
                {
                    Sprite newSprite = defaultAvatarContainer.GetSprite(playerModel.friends[notificationVO.senderPlayerId].publicProfile.avatarId);
                    notification.senderPic.sprite = newSprite;
                }
            }
            notification.closeButton.onClick.AddListener(OnCloseButtonClicked);

            notifidationObj.transform.SetParent(gameObject.transform);
            notifidationObj.transform.position = positionDummy.transform.position;
            notifications.Add(notifidationObj);
        }

        private void OnCloseButtonClicked()
        {
            notifications[0].SetActive(false);
        }
    }
}