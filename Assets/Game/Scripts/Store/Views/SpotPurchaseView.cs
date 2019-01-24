using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class SpotPurchaseView : View
    {
        public Button closeButton;

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }

        // View signals
        public Signal closeClickedSignal = new Signal();

        public void Init()
        {
            closeButton.onClick.AddListener(OnCloseClicked);
        }

        public void UpdateView(StoreVO vo)
        {
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnCloseClicked()
        {
            closeClickedSignal.Dispatch();
        }

    }
}
