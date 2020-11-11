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

    public GameObject saleObj;
    public Text goAdsFreeText;
    public Text ribbonText;
    public TMP_Text limitedTimeOnlyText;
    public Text endsInTime;
    private WaitForSecondsRealtime waitForOneRealSecond;

    public bool isOnSale;
    private StoreItem storeItem;
    private StoreItem saleItem;

    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IMetaDataModel metaDataModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal<string> purchaseSignal = new Signal<string>();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        waitForOneRealSecond = new WaitForSecondsRealtime(1f);

        storeItem = storeSettingsModel.items[shortCode];
        if (storeItem == null)
            return;
        title.text = storeItem.displayName;
        purchaseText.text = $"{storeItem.remoteProductCurrencyCode} {storeItem.productPrice} only";

        SetupSalePrice();
    }

    public void UpdateView()
    {
        var key = shortCode;

        if (isOnSale)
            key = saleShortCode;

        var storeItem = storeSettingsModel.items[key];

        if (storeItem == null)
            return;
        title.text = storeItem.displayName;

        if(isOnSale)
        {
            saleObj.SetActive(true);
            goAdsFreeText.enabled = false;
            closeButton.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -221f, closeButton.transform.localPosition.z);
            title.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -14f, closeButton.transform.localPosition.z);
            purchaseText.text = $"{saleItem.remoteProductCurrencyCode} {saleItem.remoteProductPrice}";

        }
        else
        {
            saleObj.SetActive(false);
            goAdsFreeText.enabled = true;
            closeButton.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, -103f, closeButton.transform.localPosition.z);
            title.transform.localPosition = new Vector3(closeButton.transform.localPosition.x, 0, closeButton.transform.localPosition.z);
            purchaseText.text = $"{storeItem.remoteProductCurrencyCode} {storeItem.productPrice} only";
        }
    }

    public void Show()
    {
        UpdateView();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetupSalePrice()
    {
        saleItem = storeSettingsModel.items[saleShortCode];

        if (saleItem == null)
        {
            return;
        }
        var discount = 1 - (saleItem.productPrice / storeItem.productPrice);
        limitedTimeOnlyText.text = $"Limited Time Only! <s>{storeItem.remoteProductCurrencyCode}{storeItem.remoteProductPrice}</s>";
        ribbonText.text = $"{(int)discount * 100}% OFF";
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

    public void ShowProcessing(bool show, bool showProcessingUi)
    {
        processingUi.SetActive(showProcessingUi);
        uiBlocker.SetActive(show);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public void SetupPurchaseButton(bool isAvailable)
    {
        purchaseButton.interactable = isAvailable;
        purchaseText.color = isAvailable ? Colors.WHITE : Colors.DISABLED_WHITE;
    }

    public void UpdateTime(int hours, int minutes, int seconds)
    {
        //Create Desired time
        DateTime target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        //Get the current time
        DateTime now = DateTime.Now;
        TimeSpan t = now.TimeOfDay;

        //Convert both to seconds
        int targetSec = target.Hour * 60 * 60 + target.Minute * 60 + target.Second;
        int nowSec = now.Hour * 60 * 60 + now.Minute * 60 + now.Second;

        TimeSpan timeLeft = target - now;
        if (timeLeft.Hours > 0)
        {
            endsInTime.text = timeLeft.Hours + ":" + timeLeft.Minutes + ":" + timeLeft.Seconds;
        }
        else if (timeLeft.Minutes > 0)
        {
            endsInTime.text = timeLeft.Minutes + ":" + timeLeft.Seconds;
        }
        else if (timeLeft.Seconds > 0)
        {
            endsInTime.text = timeLeft.Seconds.ToString();
        }
        else
        {
            endsInTime.gameObject.SetActive(false);
        }
    }

    IEnumerator CountdownTimer()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return waitForOneRealSecond;

            UpdateTime(24, 0, 0);
        }

        yield return null;
    }
}


