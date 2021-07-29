using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using System.Collections;
using System;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class MaintenanceView : View
    {
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public ShowMaintenanceViewSignal showMaintenanceViewSignal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }

        public Signal<bool> schedulerSubscription = new Signal<bool>();

        public GameObject maintenancePanel;
        public GameObject maintenanceWarningStrip;
        public Text maintenanceMsgLabel;
        public Text maintenanceWarningMsgLabel;
        public Image maintenanceWarningBgColor;


        public void Init()
        {
            maintenancePanel.SetActive(false);
            maintenanceWarningStrip.SetActive(false);
        }

        public void ShowMaintenance()
        {
            toggleBannerSignal.Dispatch(false);
            maintenanceMsgLabel.text = settingsModel.maintenanceMessage;
            gameObject.SetActive(true);
            maintenancePanel.SetActive(true);
            maintenanceWarningStrip.SetActive(false);
        }

        public void HideMaintenance()
        {
            gameObject.SetActive(false);
            maintenancePanel.SetActive(false);
            maintenanceWarningStrip.SetActive(false);
        }

        public void ShowMaintenanceWarning()
        {
            toggleBannerSignal.Dispatch(false);
            //maintenanceWarningMsgLabel.text = settingsModel.maintenanceWarningMessege;
            maintenanceWarningBgColor.color = Colors.Color(settingsModel.maintenanceWarningBgColor);

            if (maintenanceWarningStrip.activeSelf == false)
            {
                gameObject.SetActive(true);
                maintenancePanel.SetActive(false);
                maintenanceWarningStrip.SetActive(true);

                schedulerSubscription.Dispatch(true);
            }
        }

        public void HideMaintenanceWarning()
        {
            if (maintenanceWarningStrip.activeSelf == true)
            {
                gameObject.SetActive(false);
                maintenancePanel.SetActive(false);
                maintenanceWarningStrip.SetActive(false);

                schedulerSubscription.Dispatch(false);
            }
        }

        public void SchedulerCallback()
        {
            if (gameObject.activeInHierarchy)
            {
                UpdateTime();
            }

            else
            {
                schedulerSubscription.Dispatch(false);
            }
        }

        public void UpdateTime()
        {
            long timeLeft = settingsModel.maintenanceWarningTimeStamp - backendService.serverClock.currentTimestamp;
            string timerText;

            if (timeLeft > 1)
            {
                timeLeft--;
                var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft));
                timerText = timeLeftText;
            }
            else
            {
                timerText = "0:00";
                showMaintenanceViewSignal.Dispatch(1);
                schedulerSubscription.Dispatch(false);
            }

            maintenanceWarningMsgLabel.text = settingsModel.maintenanceWarningMessege + " " + timerText;
        }
    }
}