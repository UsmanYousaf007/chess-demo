/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class TournamentUpcomingItem : MonoBehaviour
    {
        [SerializeField] private ChestIconsContainer chestIconsContainer;
        [SerializeField] private TournamentAssetsContainer tournamentAssetsContainer;

        public Image bg;
        public Text startsInLabel;
        public Image tournamentImage;
        public Text countdownTimerText;
        public Text getNotifiedLabel;
        public Button button;
        public Text notificationEnabledText;

        public SkinLink skinLink;
        public Image thumbnailBg;
        public GameObject startsInPanel;
        public Transform startsInPanelPos;
        private Vector3 startsInPanelDefaultPos;

        private long currentStartTimeUTCSeconds;

        private void Start()
        {
            ChestIconsContainer.Load();
            TournamentAssetsContainer.Load();
            startsInPanelDefaultPos = startsInPanel.transform.localPosition;
        }

        public void Init()
        {
            button.gameObject.SetActive(true);
            startsInPanel.transform.localPosition = startsInPanelDefaultPos;
            startsInPanel.transform.localScale = new Vector3(1f, 1f, 1f);
            notificationEnabledText.DOFade(0.75f, 0);
        }

        public void UpdateItem(LiveTournamentData liveTournamentData, bool getNotified)
        {
            skinLink.InitPrefabSkin();
            thumbnailBg.sprite = tournamentAssetsContainer.GetTile(liveTournamentData.type);
            tournamentImage.sprite = tournamentAssetsContainer.GetSticker(liveTournamentData.type);
            tournamentImage.SetNativeSize();

            currentStartTimeUTCSeconds = liveTournamentData.currentStartTimeUTCSeconds;

            long timeLeft = currentStartTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string timeLeftText;
            if (timeLeft > 0)
            {
                timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            }
            else
            {
                timeLeftText = "0:00";
            }
            countdownTimerText.text = timeLeftText;

            button.gameObject.SetActive(!getNotified);
            if (getNotified)
            {
                startsInPanel.transform.localPosition = startsInPanelPos.localPosition;
                startsInPanel.transform.localScale = startsInPanelPos.localScale;
            }else
            {
                startsInPanel.transform.localPosition = startsInPanelDefaultPos;
                startsInPanel.transform.localScale = new Vector3(1f,1f,1f);
            }
        }

        public void UpdateTime()
        {
            long timeLeft = currentStartTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (timeLeft > 0)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
                countdownTimerText.text = timeLeftText;
            }
            else
            {
                countdownTimerText.text = "0:00";
            }
        }

        public void DisableNotificationEnabledText()
        {
            notificationEnabledText.DOFade(0, 0f);
            notificationEnabledText.gameObject.SetActive(true);
            StartCoroutine(DisableNotificationEnabledTextWithDelay());
        }

        private IEnumerator DisableNotificationEnabledTextWithDelay()
        {
            notificationEnabledText.DOFade(0.75f, 0.5f);
            yield return new WaitForSeconds(2.3f);
            notificationEnabledText.DOFade(0, 1);
            yield return new WaitForSeconds(1f);
            notificationEnabledText.gameObject.SetActive(false);
            iTween.MoveTo(startsInPanel,
            iTween.Hash(
               "position", startsInPanelPos.position,
               "time", 0.4f));
            iTween.ScaleTo(startsInPanel,
            iTween.Hash(
                "scale", startsInPanelPos.localScale,
                "time", 0.4f));
        }

        public void Reset()
        {
            button.gameObject.SetActive(true);
            startsInPanel.transform.localPosition = startsInPanelDefaultPos;
            startsInPanel.transform.localScale = new Vector3(1f, 1f, 1f);
            notificationEnabledText.DOFade(0.75f, 0);
        }
    }
}
