using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SpotPurchaseView : View
    {
        public Text title;
        public Text subTitle;
        public Button close;
        public GameObject uiBlocker;
        public GameObject processing;
        public Text finePrint;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();

        public void Init()
        {
            title.text = localizationService.Get(LocalizationKey.SPOT_PURHCASE_TITLE);
            subTitle.text = localizationService.Get(LocalizationKey.SPOT_PURCHASE_SUB_TITLE);
            finePrint.text = localizationService.Get(LocalizationKey.SPOT_PURCHASE_FINE_PRINT);
            close.onClick.AddListener(OnCloseButtonClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeDlgSignal.Dispatch();
        }

        public void ShowProcessing(bool showUiBlocked, bool showProcessing)
        {
            uiBlocker.SetActive(showUiBlocked);
            processing.SetActive(showProcessing);
        }
    }
}
