using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.CLSCompliantAttribute(false)]
public class PromotionBundleDlgView : PromotionBundleView
{
    public Button closeButton;
    public Text wasPriceText;
    public GameObject ribbon;
    public GameObject gemsRibbon;
    public CanvasGroup canvasGroup;

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

        purchaseText.text = bundleStoreItem.remoteProductPrice;
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
        UIDlgManager.Show(gameObject, Colors.BLUR_BG_BRIGHTNESS_NORMAL, false, 0.5f).Then(()=> Animate());
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

    public void AnimateRibbons()
    {
        ribbon.transform.DOScale(1.0f, 1f).SetEase(Ease.OutBounce);
        gemsRibbon.transform.DOScale(0.67f, 1f).SetEase(Ease.OutBounce);
    }

    public void Reset()
    {
        ribbon.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        gemsRibbon.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        canvasGroup.alpha = 0;
    }

    private void Animate()
    {
        Sequence animationSequence;

        animationSequence = DOTween.Sequence();
        animationSequence.AppendCallback(() => Reset());
        animationSequence.AppendInterval(1f);
        animationSequence.AppendCallback(() => AnimateRibbons());
        animationSequence.AppendInterval(1f);
        animationSequence.AppendCallback(() => canvasGroup.DOFade(1.0f, 1f));
        animationSequence.PlayForward();
    }
}


