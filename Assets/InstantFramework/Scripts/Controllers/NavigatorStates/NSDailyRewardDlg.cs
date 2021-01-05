
namespace TurboLabz.InstantFramework
{
    public class NSDailyRewardDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.DAILY_REWARD_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

            if (evt == NavigatorEvent.ESCAPE)
            {
                //cmd.hideViewSignal.Dispatch(NavigatorViewId.DAILY_REWARD_DLG);
                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }
            else if (evt == NavigatorEvent.SHOW_LOBBY)
            {
                return new NSLobby();
            }

            return null;
        }
    }
}
