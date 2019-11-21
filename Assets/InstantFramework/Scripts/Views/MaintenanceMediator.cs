using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class MaintenanceMediator : Mediator
    {
        // View injection
        [Inject] public MaintenanceView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOW_MAINTENANCE_SCREEN)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOW_MAINTENANCE_SCREEN)
            {
                view.Hide();
            }
        }
    }
}
