namespace TurboLabz.InstantFramework
{
    public class NSShopBundlePurchasedDlg : NS
    {
        public override void RenderDisplayOnEnter()
        {
            ShowDialog(NavigatorViewId.SHOP_BUNDLE_PURCHASED);
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
