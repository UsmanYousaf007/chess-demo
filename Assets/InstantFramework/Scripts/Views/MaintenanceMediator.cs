using System;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class MaintenanceMediator : Mediator
    {
        // View injection
        [Inject] public MaintenanceView view { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOW_MAINTENANCE_SCREEN)
            {
                view.ShowMaintenance();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOW_MAINTENANCE_SCREEN)
            {
                view.HideMaintenance();
            }
        }

        [ListensTo(typeof(ShowMaintenanceViewSignal))]
        public void ShowMaintenanceView(int maintenanceCounter)
        {
            if(maintenanceCounter == 1) //Show maintenance
            {
                view.ShowMaintenance();
            }
            else if (maintenanceCounter == 2) // Show maintenance warning
            {
                view.ShowMaintenanceWarning();
            }
            else if (maintenanceCounter == 3) // Hide maintenance warning
            {
                view.HideMaintenanceWarning();
            }

        }

        private void OnSchedulerSubscriptionToggle(bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(view.SchedulerCallback);
            }
            else
            {
                schedulerService.UnSubscribe(view.SchedulerCallback);
            }
        }
    }
}
