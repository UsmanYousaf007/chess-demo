using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SkillLevelDlgMediator : Mediator
    {
        // View injection
        [Inject] public SkillLevelDlgView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Dispatch Signals
        [Inject] public ShowSplashContentSignal showSplashContentSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SKILL_LEVEL_DLG)
            {
                view.Show();
                showSplashContentSignal.Dispatch(false);
                analyticsService.ScreenVisit(AnalyticsScreen.skill_level_dlg);

                if (SplashLoader.FTUE)
                {
                    analyticsService.DesignEvent(AnalyticsEventId.ftue_skill_level_dlg);
                }
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SKILL_LEVEL_DLG)
            {
                view.Hide();
                showSplashContentSignal.Dispatch(true);
            }
        }

        [ListensTo(typeof(SkillSelectedSignal))]
        public void OnSkillLevelSelected()
        {
            view.SetDefaultSkillLevel();
        }
    }
}
