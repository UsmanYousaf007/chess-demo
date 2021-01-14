namespace TurboLabz.InstantFramework
{
    public class NSPurchaseSuccessDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.PURCHASE_SUCCESS_DLG);
        }

        public override NS HandleEvent(NavigatorEvent evt)
        {
            if (evt == NavigatorEvent.ESCAPE)
            {
                return new NSShop();
            }

            return null;
        }
    }
}
