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

            return null;
        }
    }
}