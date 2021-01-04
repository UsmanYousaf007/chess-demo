
namespace TurboLabz.InstantFramework
{
    public class NSRewardDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.REWARD_DLG_V2);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                NavigatorViewId viewId = CameFrom(NavigatorViewId.LOBBY);

                if (viewId == NavigatorViewId.LOBBY)
                {
                    return new NSLobby();
                }
            }

            return null;
        }
    }
}
