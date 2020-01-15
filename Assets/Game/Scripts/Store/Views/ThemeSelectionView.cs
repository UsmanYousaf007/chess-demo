using System;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSelectionView : View
{
    public SkinItemView[] skinMenuItems;
    public Button applyButton;
    public Text chooseThemeText;
    public Image scrollUpArrow;
    public Image scrollDownArrow;
    public ScrollRect scrollRect;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }

    //Signals
    public Signal applyThemeSignal = new Signal();

    private string originalSkinId;

    public void Init()
    {
        chooseThemeText.text = localizationService.Get(LocalizationKey.CHOOSE_THEME);

        applyButton.onClick.AddListener(OnApplyButtonClicked);

        scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        var tempColor = scrollUpArrow.color;
        tempColor.a = 0.3f;
        scrollUpArrow.color = tempColor;

        SetupSkinMenuItems();
    }

    private void OnScrollRectValueChanged(Vector2 arg0)
    {

        if (scrollRect.verticalNormalizedPosition >= 0.9)
            SetAlpha(scrollUpArrow, 0.3f);
        else
        {
            if (scrollUpArrow.color.a < 1f)
                SetAlpha(scrollUpArrow, 1f);
        }

        if (scrollRect.verticalNormalizedPosition <= 0.1)
            SetAlpha(scrollDownArrow, 0.3f);
        else
        {
            if(scrollDownArrow.color.a < 1f)
                SetAlpha(scrollDownArrow, 1f);
        }

    }

    private void SetAlpha(Image image, float alpha)
    {
        var tempColor = image.color;
        tempColor.a = alpha;
        image.color = tempColor;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        originalSkinId = playerModel.activeSkinId;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetupSkinMenuItems()
    {
        foreach (var entry in metaDataModel.store.items)
        {
            if (entry.Value.skinIndex != -1)
            {
                skinMenuItems[entry.Value.skinIndex].key = entry.Value.key;
                skinMenuItems[entry.Value.skinIndex].Init();
            }
        }
    }

    private void OnApplyButtonClicked()
    {
        applyThemeSignal.Dispatch();
    }

    public bool HasSkinChanged()
    {
        return originalSkinId != playerModel.activeSkinId;
    }
}
