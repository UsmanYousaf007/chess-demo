using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{

    public class ConfirmDlgView : View
    {
        public Text title;
        public Text description;
        public Text okButtonText;
        public Button noButton;
        public Button yesButton;

        //Signals
        public Signal closeSignal = new Signal();

        public void Init()
        {
            noButton.onClick.AddListener(OnNoClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateDlg(ConfirmDlgVO vo)
        {
            title.text = vo.title;
            description.text = vo.desc;
            okButtonText.text = vo.yesButtonText;
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() => vo.onClickYesButton());
        }

        void OnNoClicked()
        {
            closeSignal.Dispatch();
        }
    }
}
