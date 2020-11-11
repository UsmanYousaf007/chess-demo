﻿using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

[System.CLSCompliantAttribute(false)]
public class PromotionChessCourseBundleDlgView : View
{
    public string key;
    public Text title;
    public Button closeButton;
    public Text purchaseText;
    public Button purchaseButton;
    public GameObject uiBlocker;
    public GameObject processingUi;
    public GameObject lessonPrefab;
    public Transform lessonsContainer;
    public GameObject loading;

    //Models 
    [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public ILessonsModel lessonsModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal purchaseSignal = new Signal();

    private StoreIconsContainer iconsContainer;

    public void InitOnce()
    {
        loading.SetActive(true);
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
        iconsContainer = StoreIconsContainer.Load();
    }

    public void Init()
    {
        var storeItem = storeSettingsModel.items[key];

        if (storeItem == null)
            return;

        title.text = storeItem.displayName;
        purchaseText.text = localizationService.Get(LocalizationKey.SUBSCRIPTION_DLG_PURCHASE_BUTTON)+" "+storeItem.productPrice;

        // Fill only once
        if (lessonsContainer.childCount == 0)
        {
            //var lessons = storeItem.description.Split(',');
            var lessons = lessonsModel.GetTopicsWithDurationInMinutes();
            foreach (var lesson in lessons)
            {
                var lessonObj = Instantiate(lessonPrefab, lessonsContainer, false) as GameObject;
                lessonObj.GetComponent<LessonInfo>().Init(lesson.name,lesson.total.ToString(), lesson.durationInMinutes.ToString());
            }
        }
        loading.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnCloseButtonClicked()
    {
        audioService.PlayStandardClick();
        closeDailogueSignal.Dispatch();
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
        purchaseText.color = isAvailable ? Colors.WHITE : Colors.DISABLED_WHITE;
    }
}


