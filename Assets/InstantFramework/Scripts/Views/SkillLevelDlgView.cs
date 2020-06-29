using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SkillLevelDlgView : View
    {
        public GameObject mainPanel;
        public Button yesButton;
        public Button noButton;
        public Transform startPivot;
        public Transform endPivot;

        private const string BEGINNER_STR = "beginner";
        private const string DEFAULT_STR = "default";

        // Dispatch Signals
        [Inject] public ReceptionSignal receptionSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void Init()
        {
            mainPanel.transform.localPosition = startPivot.localPosition;

            yesButton.onClick.AddListener(() =>
            {
                OnButtonClicked(BEGINNER_STR);
            });

            noButton.onClick.AddListener(() =>
            {
                OnButtonClicked(DEFAULT_STR);
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
            iTween.MoveTo(mainPanel,
                iTween.Hash(
                    "position",endPivot.localPosition,
                    "time", 0.3f,
                    "islocal", true
                ));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            mainPanel.transform.localPosition = startPivot.localPosition;
        }

        private void OnButtonClicked(string skillLevel)
        {
            playerModel.skillLevel = skillLevel;
            GotoReception();
            Hide();
        }

        private void GotoReception()
        {
            receptionSignal.Dispatch(false);
        }
    }
}
