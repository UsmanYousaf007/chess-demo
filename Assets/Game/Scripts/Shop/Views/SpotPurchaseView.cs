using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SpotPurchaseView : View
    {
        public Text title;
        public Text subTitle;
        public Button close;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();

        public void Init()
        {
            title.text = localizationService.Get(LocalizationKey.SPOT_PURHCASE_TITLE);
            subTitle.text = localizationService.Get(LocalizationKey.SPOT_PURCHASE_SUB_TITLE);
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
    }
}
