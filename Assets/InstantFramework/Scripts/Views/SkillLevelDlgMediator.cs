using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SkillLevelDlgMediator : Mediator
    {
        // View injection
        [Inject] public SkillLevelDlgView view { get; set; }

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
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SKILL_LEVEL_DLG)
            {
                view.Hide();
            }
        }
    }
}
