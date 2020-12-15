using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class LoginDlgView : View
    {
        public Button fbButton;
        public Button siwaButton;
        public Button guestButton;
        public RectTransform layout;
        public GameObject processing;

        public Signal facebookLoginSignal = new Signal();
        public Signal siwaSignal = new Signal();
        public Signal guestSignal = new Signal();

        [Inject] public IAudioService audioService { get; set; }

        public void Init(bool siwaSupported)
        {
            fbButton.onClick.AddListener(OnFBButtonClicked);
            siwaButton.onClick.AddListener(OnSIWAButtonClicked);
            guestButton.onClick.AddListener(OnGuestButtonClicked);
            siwaButton.gameObject.SetActive(siwaSupported);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnFBButtonClicked()
        {
            audioService.PlayStandardClick();
            processing.SetActive(true);
            facebookLoginSignal.Dispatch();
        }

        private void OnSIWAButtonClicked()
        {
            audioService.PlayStandardClick();
            processing.SetActive(true);
            siwaSignal.Dispatch();
        }

        private void OnGuestButtonClicked()
        {
            audioService.PlayStandardClick();
            guestSignal.Dispatch();
        }

        public void OnSignInCompleted()
        {
            processing.SetActive(false);
        }
    }
}
