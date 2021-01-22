namespace TurboLabz.InstantFramework
{
    public class NSCPUPowerplay : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.CPU_POWER_PLAY);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.SHOW_CPU ||
                evt == NavigatorEvent.ESCAPE)
            {
                return new NSCPU();
            }
            else if (evt == NavigatorEvent.SHOW_SPOT_PURCHASE)
            {
                return new NSSpotPurchase();
            }
            else if (evt == NavigatorEvent.SHOW_CPU_RESULTS_DLG)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
                return new NSCPUResultsDlg();
            }

            return null;
        }
    }
}