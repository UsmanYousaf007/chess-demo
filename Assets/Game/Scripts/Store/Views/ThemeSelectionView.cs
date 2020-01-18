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
    public Button scrollUpButton;
    public Image scrollDownArrow;
    public Button scrollDownButton;
    public ScrollRect scrollRect;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }
    [Inject] public IAudioService audioService { get; set; }

    //Signals
    public Signal applyThemeSignal = new Signal();

    private string originalSkinId;
    private float moveScrollRectBy = 0.2f;

    public void Init()
    {
        chooseThemeText.text = localizationService.Get(LocalizationKey.CHOOSE_THEME);
        applyButton.onClick.AddListener(OnApplyButtonClicked);
        scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        scrollUpButton.onClick.AddListener(OnScrollUpButton);
        scrollDownButton.onClick.AddListener(OnScrollDownButton);

        SetAlpha(scrollUpArrow, 0.3f);
        SetupSkinMenuItems();
    }

    private void OnScrollRectValueChanged(Vector2 arg0)
    {

        if (scrollRect.verticalNormalizedPosition >= 0.9)
        {
            EnableArrowButton(false, scrollUpButton, scrollUpArrow);
        }
        else
        {
            if (scrollUpArrow.color.a < 1f)
            {
                EnableArrowButton(true, scrollUpButton, scrollUpArrow);
            }
        }

        if (scrollRect.verticalNormalizedPosition <= 0.1)
        {
            EnableArrowButton(false, scrollDownButton, scrollDownArrow);
        }
        else
        {
            if (scrollDownArrow.color.a < 1f)
            {
                EnableArrowButton(true, scrollDownButton, scrollDownArrow);
            }
        }

    }

    private void EnableArrowButton(bool enable, Button button, Image image)
    {
        SetAlpha(image, enable ? 1f : 0.3f);
        button.interactable = enable;
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
        audioService.PlayStandardClick();
        applyThemeSignal.Dispatch();
    }

    public bool HasSkinChanged()
    {
        return originalSkinId != playerModel.activeSkinId;
    }

    private void OnScrollUpButton()
    {
        audioService.PlayStandardClick();
        scrollRect.verticalNormalizedPosition += moveScrollRectBy;
    }

    private void OnScrollDownButton()
    {
        audioService.PlayStandardClick();
        scrollRect.verticalNormalizedPosition -= moveScrollRectBy;
    }
}
