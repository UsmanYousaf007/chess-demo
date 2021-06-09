using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class PromotionBundleDlgView : PromotionBundleView
{
    public Button closeButton;
    public Text wasPriceText;

    //Signals
    public Signal closeDailogueSignal = new Signal();

    public override void InitOnce()
    {
        base.InitOnce();
        UIDlgManager.Setup(gameObject);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public override void Init()
    {
       bundleStoreItem = storeSettingsModel.items[key];

        if (bundleStoreItem == null)
            return;

        purchaseText.text = "Now" + bundleStoreItem.remoteProductPrice;
        CalculateDiscount();

        if (bundleStoreItem.currency3Cost > 0)
        {
            currencyPayout.count.text = bundleStoreItem.currency3Cost.ToString("N0");
        }

        if (bundleStoreItem.currency4Cost > 0)
        {
            currency2Payout.count.text = FormatUtil.AbbreviateNumber(bundleStoreItem.currency4Cost, false);
        }

        wasPriceText.text = string.Format("Was {0} {1:0.##}", bundleStoreItem.remoteProductCurrencyCode, totalBundlePrice);
    }

    public void Show()
    {
        UIDlgManager.Show(gameObject);
    }

    public void Hide()
    {
        UIDlgManager.Hide(gameObject);
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

}


