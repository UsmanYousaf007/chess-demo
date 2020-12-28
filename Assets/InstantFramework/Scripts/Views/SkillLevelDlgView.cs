using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    public class SkillLevelDlgView : View
    {
        public Button yesButton;
        public Button noButton;
        public Image icon;
        public CanvasGroup view;
        public CanvasGroup buttons;

        private const string BEGINNER_STR = "beginner";
        private const string DEFAULT_STR = "default";

        // Dispatch Signals
        [Inject] public ReceptionSignal receptionSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public InitFacebookSignal initFacebookSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void Init()
        {
            yesButton.onClick.AddListener(() =>
            {
                OnButtonClicked(BEGINNER_STR);
            });

            noButton.onClick.AddListener(() =>
            {
                OnButtonClicked(DEFAULT_STR);
            });

            buttons.alpha = Settings.MIN_ALPHA;
            icon.transform.localScale = Settings.MIN_SCALE;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            buttons.DOFade(Settings.MAX_ALPHA, Settings.TWEEN_DURATION);
            icon.transform.DOScale(Settings.MAX_SCALE, Settings.TWEEN_DURATION);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnButtonClicked(string skillLevel)
        {
            playerModel.skillLevel = skillLevel;
            view.DOFade(Settings.MIN_ALPHA, Settings.TWEEN_DURATION).OnComplete(() => {
                GotoReception();
                Hide();
            });
        }

        private void GotoReception()
        {
            initFacebookSignal.Dispatch();
            receptionSignal.Dispatch(false);
        }

        public void SetDefaultSkillLevel()
        {
            OnButtonClicked(DEFAULT_STR);
        }
    }
}
