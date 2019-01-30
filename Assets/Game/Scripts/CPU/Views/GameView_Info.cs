using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        [Header("Info")]
        public GameObject infoDlg;
        public Button infoButton;
        public Button closeInfoButton;

        public Signal infoButtonClickedSignal = new Signal();
        public Signal closeButtonClickedSignal = new Signal();

        public void InitInfo()
        {
            closeInfoButton.onClick.AddListener(OnCloseButtonClicked);
            infoButton.onClick.AddListener(OnInfoButtonClicked);
        }

        public void OnParentShowInfo()
        {
            infoDlg.SetActive(false);
        }

        public void ShowInfo()
        {
            EnableModalBlocker(Colors.UI_BLOCKER_DARK_ALPHA);
            infoDlg.SetActive(true);
        }

        public void HideInfo()
        {
            DisableModalBlocker();
            infoDlg.SetActive(false);
        }

        void OnInfoButtonClicked()
        {
            infoButtonClickedSignal.Dispatch();

            analyticsService.Event(AnalyticsEventId.tap_pow_info, AnalyticsContext.computer_match);
        }

        void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }
    }
}
