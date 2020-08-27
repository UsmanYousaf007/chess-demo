namespace TurboLabz.InstantFramework
{
    public class NSRewardDlgView : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowView(NavigatorViewId.REWARD_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.INBOX_VIEW);

            if (evt == NavigatorEvent.ESCAPE)
            {
                if (viewId == NavigatorViewId.INBOX_VIEW)
                {
                    return new NSInboxView();
                }
            }

            return null;
        }
    }
}
