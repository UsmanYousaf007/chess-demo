using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class MaintenanceView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

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
            maintenanceWarningMsgLabel.text = settingsModel.maintenanceWarningMessege;
            maintenanceWarningBgColor.color = Colors.Color(settingsModel.maintenanceWarningBgColor);

            if (maintenanceWarningStrip.activeSelf == false)
            {
                gameObject.SetActive(true);
                maintenancePanel.SetActive(false);
                maintenanceWarningStrip.SetActive(true);
            }
        
        }

        public void HideMaintenanceWarning()
        {
            if (maintenanceWarningStrip.activeSelf == true)
            {
                gameObject.SetActive(false);
                maintenancePanel.SetActive(false);
                maintenanceWarningStrip.SetActive(false);
            }

        }
    }
}