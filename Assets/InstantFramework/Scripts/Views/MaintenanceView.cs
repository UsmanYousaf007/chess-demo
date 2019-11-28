using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class MaintenanceView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        public Text maintenanceMsgLabel;

        public void Init()
        {
         
        }

        public void Show()
        {
            maintenanceMsgLabel.text = settingsModel.maintenanceMessage;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}