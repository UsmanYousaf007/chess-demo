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

        // Models
        [Inject] public IPicsModel picsModel { get; set; }

        public void Init()
        {
            notifications = new List<GameObject>();
            processNotificaitonCR = StartCoroutine(ProcessNotificationCR());
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
                    notifications[0].SetActive(true);
                    yield return new WaitForSeconds(5);
                    notifications[0].SetActive(false);
                    GameObject obj = notifications[0];
                    notifications.Remove(notifications[0]);
                    Destroy(obj);

                }
                yield return new WaitForSeconds(1);
            }
        }

        public void AddNotification(NotificationVO notificationVO) 
        {
            GameObject notifidationObj = Instantiate(notificationPrefab);
            notifidationObj.SetActive(false);
            Notification notification = notifidationObj.GetComponent<Notification>();
            notification.title.text = notificationVO.title;
            notification.body.text = notificationVO.body;
            notification.senderPic.sprite = picsModel.GetPlayerPic(notificationVO.senderPlayerId);
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