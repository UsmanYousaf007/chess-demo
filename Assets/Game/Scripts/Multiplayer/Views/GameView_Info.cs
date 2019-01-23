using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Multiplayer
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
            EnableModalBlocker();
            infoDlg.SetActive(true);
        }

        public void HideInfo()
        {
            DisableModalBlocker();
            infoDlg.SetActive(false);
        }

        void OnInfoButtonClicked()
        {
            LogUtil.Log("SHOW MULTIPLAYER INFO", "red");
            infoButtonClickedSignal.Dispatch();
        }

        void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }
    }
}
