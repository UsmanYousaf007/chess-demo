using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine.UI;

public class ThemeSelectionView : View
{
    public SkinItemView[] skinMenuItems;
    public Button closeButton;
    public Button applyButton;
    public Text applyButtonText;

    //Models 
    [Inject] public IMetaDataModel metaDataModel { get; set; }
    [Inject] public IPlayerModel playerModel { get; set; }

    //Services
    [Inject] public ILocalizationService localizationService { get; set; }

    //Signals
    public Signal closeDailogueSignal = new Signal();
    public Signal applyThemeSignal = new Signal();

    private string originalSkinId;

    public void Init()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        applyButton.onClick.AddListener(OnApplyButtonClicked);
        applyButtonText.text = localizationService.Get(LocalizationKey.DONE);

        SetupSkinMenuItems();
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

    private void OnCloseButtonClicked()
    {
        closeDailogueSignal.Dispatch();
    }

    public bool HasSkinChanged()
    {
        return originalSkinId != playerModel.activeSkinId;
    }
}
