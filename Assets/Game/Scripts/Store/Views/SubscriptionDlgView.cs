using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using TurboLabz.TLUtils;

[System.CLSCompliantAttribute(false)]
public class SubscriptionDlgView : View
{
    public string key;
    public Text title;
    public GameObject offerPrefab;
    public Transform offersContainer;
    public Button closeButton;
    public Text termsOfUseText;
    public Button termsOfUseButton;
    public Text restorePurchaseText;
    public Button restorePurchaseButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;
    public VerticalLayoutGroup offerBg;

    public Button privacyPolicyButton;
    public Text privacyPolicyText;

    //Fetching data
    public GameObject thinking;
    public Image[] radioButtons;
    public Image titleImg;

    public SubscriptionTierView[] subscriptionTiers;

    [Header("Sale")]
    public GameObject saleRibbon;
    public Text megaSale;
    public Text saleOfferPercentage;
    public Text offText;
    public TMP_Text limitedTimeOnlyText;
    public Text endsInText;
    public Text endsInTime;
    public bool isSaleOffer;
    Vector3 titleImgPos;
    Vector3 offersContainerPos;
    long endTimeUTCSeconds;
    private WaitForSecondsRealtime waitForOneRealSecond;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public ISettingsModel settingsModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal restorePurchasesSignal = new Signal();
    public Signal purchaseSignal = new Signal();
    public Signal showTermsSignal = new Signal();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        termsOfUseButton.onClick.AddListener(OnTermsOfUseClicked);
        restorePurchaseButton.onClick.AddListener(OnRestorePurchaseClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        privacyPolicyButton.onClick.AddListener(OnPrivacyPolicyClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        waitForOneRealSecond = new WaitForSecondsRealtime(1f);
        title.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TITLE);
        restorePurchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_RESTORE_PURCHASE);
        termsOfUseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_TERMS_OF_USE);
        purchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON);
        titleImgPos = titleImg.gameObject.transform.localPosition;
        offersContainerPos = offersContainer.localPosition;

        var storeItem = storeSettingsModel.items[key];
        if (storeItem == null)
            return;
        SetupDefaultSubscriptionTier();

        // Fill only once
        if (offersContainer.childCount == 0)
        {
            var offers = storeItem.description.Split(',');
            foreach (var offer in offers)
            {
                var offerObj = Instantiate(offerPrefab, offersContainer, false) as GameObject;
                var text = offer.Trim();
                offerObj.GetComponent<SubscriptionOffer>().Init(iconsContainer.GetSprite("Sub" + GSBackendKeys.ShopItem.GetOfferItemKey(text)), text);
            }
        }

        limitedTimeOnlyText.text = $"Limited Time Only! <s>{storeItem.remoteProductCurrencyCode} {storeItem.productPrice} / Year</s>";

        if (isSaleOffer)
        {
            saleRibbon.SetActive(true);
            titleImg.gameObject.transform.localPosition = new Vector3(titleImg.gameObject.transform.localPosition.x, titleImg.gameObject.transform.localPosition.y - 100, titleImg.gameObject.transform.localPosition.z);
            limitedTimeOnlyText.enabled = true;
            endsInText.enabled = true;
            offersContainer.localPosition = new Vector3(offersContainer.localPosition.x, offersContainer.localPosition.y - 100, offersContainer.localPosition.z);
            offerBg.gameObject.SetActive(false);
            endsInTime.gameObject.SetActive(true);
        }
        else
        {
            saleRibbon.SetActive(false);
            titleImg.gameObject.transform.localPosition = titleImgPos;
            limitedTimeOnlyText.enabled = false;
            endsInText.enabled = false;
            offersContainer.localPosition = offersContainerPos;
            offerBg.gameObject.SetActive(true);
            endsInTime.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(CountdownTimer());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        StopCoroutine(CountdownTimer());
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
    }

    private void OnPrivacyPolicyClicked()
    {
        audioService.PlayStandardClick();
        Application.OpenURL(metaDataModel.appInfo.privacyPolicyURL);
    }

    private void OnTermsOfUseClicked()
    {
        audioService.PlayStandardClick();
        showTermsSignal.Dispatch();
        Application.OpenURL(metaDataModel.appInfo.termsOfUseURL);
    }

    private void OnRestorePurchaseClicked()
    {
        audioService.PlayStandardClick();
        restorePurchasesSignal.Dispatch();
    }

    private void OnPurchaseButtonClicked()
    {
        audioService.PlayStandardClick();
        purchaseSignal.Dispatch();
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
        thinking.SetActive(!isAvailable);
        foreach(var btn in radioButtons)
        {
            var tempColor = btn.color;
            if(isAvailable)
                tempColor.a = 1f;
            else
                tempColor.a = 0.3f;
            btn.color = tempColor;
        }
        //purchaseText.color = isAvailable ? Colors.WHITE : Colors.DISABLED_WHITE;
    }

    private void SetupDefaultSubscriptionTier()
    {
        foreach (var t in subscriptionTiers)
        {
            t.isSelected = t.key.Equals(settingsModel.defaultSubscriptionKey);
            key = settingsModel.defaultSubscriptionKey;

            /*if (t.isSelected)
            {
                t.transform.SetAsFirstSibling();
            }*/
        }

    }

    /* The Code to get seconds left */
    //Give the end time in digital format i.e. 21, 0, 0 == 9 pm 
    public void UpdateTime(int hours, int minutes, int seconds)
    {
        //Create Desired time
        DateTime target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        //DateTime target = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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

        //Get the difference in seconds
        //int diff = targetSec - nowSec;

        //return diff;
    }

    /*public void UpdateTime()
    {
        long timeLeft = endTimeUTCSeconds - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (timeLeft > 0)
        {
            timeLeft--;
            var timeLeftText = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft * 1000));
            endsInTime.text = timeLeftText;
        }
        else
        {
            endsInTime.text = "0:00";
        }
    }*/

    IEnumerator CountdownTimer()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return waitForOneRealSecond;

            UpdateTime(24, 0, 0);
        }

        yield return null;
    }

    /*IEnumerator StartTimeRoutine()
    {
        int secsLeft = GetSecondsLeft(24, 0, 0);
        while (secsLeft > 0)
        {
            yield return new WaitForSeconds(1 / 60);
            secsLeft = GetSecondsLeft(24, 0, 0);
        }
        yield return null;
    }*/
}


