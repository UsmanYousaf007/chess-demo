using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using TurboLabz.TLUtils;

[System.CLSCompliantAttribute(false)]
public class PromotionRemoveAdsDlgView : View
{
    public string shortCode;
    public string saleShortCode;
    public Text title;
    public Button closeButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;
    public GameObject loading;

    public GameObject saleObj;
    public Text goAdsFreeText;
    public Text ribbonText;
    public TMP_Text limitedTimeOnlyText;
    public Text endsInTime;
    private WaitForSecondsRealtime waitForOneRealSecond;

    [HideInInspector] public bool isOnSale;

    private bool isStoreAvailable;
    private bool isShown;
    private Coroutine timer;

    // Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

    // Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    // Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal<string> purchaseSignal = new Signal<string>();

    public void InitOnce()
    {
        UIDlgManager.Setup(gameObject);
        waitForOneRealSecond = new WaitForSecondsRealtime(1f);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    public void OnStoreAvailable(bool isAvailable)
    {
        isStoreAvailable = isAvailable;
        purchaseButton.interactable = isAvailable;
        purchaseText.enabled = isAvailable;
        loading.SetActive(!isAvailable);

        if (isShown && isAvailable)
        {
            SetupPopup();
        }
    }

    public void UpdateView()
    {
        isShown = true;
        saleObj.SetActive(isOnSale);
        goAdsFreeText.enabled = !isOnSale;
        limitedTimeOnlyText.enabled = isOnSale && isStoreAvailable;

        if (isStoreAvailable)
        {
            SetupPopup();
        }
    }

    public void Show()
    {
        UpdateView();
        UIDlgManager.Show(gameObject).Then(OnShowComplete);
    }

    public void OnShowComplete()
    {
        timer = StartCoroutine(CountdownTimer());
    }

    public void Hide()
    {
        StopCoroutine(timer);
        UIDlgManager.Hide(gameObject);
    }

    private void SetupPopup()
    {
        var storeItem = storeSettingsModel.items[shortCode];
        var saleItem = storeSettingsModel.items[saleShortCode];
        title.text = storeItem.displayName;

        if (isOnSale)
        {
            purchaseText.text = saleItem.remoteProductPrice;
            var discount = 1 - (float)(saleItem.productPrice / storeItem.productPrice);
            limitedTimeOnlyText.text = $"Limited Time Only! <s>{storeItem.remoteProductPrice}</s>";
            ribbonText.text = $"{(int)(discount * 100)}%";
            limitedTimeOnlyText.enabled = true;
        }
        else
        {
            purchaseText.text = $"{storeItem.remoteProductPrice} only";
        }
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch(isOnSale ? saleShortCode : shortCode);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    IEnumerator CountdownTimer()
    {
        while (gameObject.activeInHierarchy)
        {
            endsInTime.text = TimeUtil.FormatTournamentClock(DateTime.Today.AddDays(1) - DateTime.Now);
            yield return waitForOneRealSecond;
        }

        yield return null;
    }

    public void ShowProcessing(bool show, bool showProcessingUi)
    {
        processingUi.SetActive(showProcessingUi);
        uiBlocker.SetActive(show);
    }
}


