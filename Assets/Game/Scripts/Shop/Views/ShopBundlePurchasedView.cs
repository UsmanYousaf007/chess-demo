using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class ShopBundlePurchasedView : View
    {
        public Text title;
        public Image icon;
        public ShopItemView.ShopPayout currencyPayout;
        public ShopItemView.ShopPayout[] payouts;
        public Text gainedLabel;
        public Button okButton;
        public Text okButtonLabel;

        private StoreIconsContainer iconsContainer;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();

        public void Init()
        {
            iconsContainer = StoreIconsContainer.Load();
            okButton.onClick.AddListener(OnOkButtonClicked);
            okButtonLabel.text = localizationService.Get(LocalizationKey.SHOP_PURHCASED_DLG_OK);
            gainedLabel.text = localizationService.Get(LocalizationKey.SHOP_PURCHASED_DLG_GAINED);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(StoreItem storeItem)
        {
            title.text = $"{storeItem.displayName} Unlocked!";
            icon.sprite = iconsContainer.GetSprite(storeItem.key);
            icon.SetNativeSize();

            if (storeItem.bundledItems != null)
            {
                if (storeItem.currency3Cost > 0)
                {
                    currencyPayout.icon.sprite = iconsContainer.GetSprite("Gem");
                    currencyPayout.count.text = storeItem.currency3Cost.ToString();
                }

                var i = 0;
                foreach (var item in storeItem.bundledItems)
                {
                    if (i < payouts.Length)
                    {
                        payouts[i].icon.sprite = iconsContainer.GetSprite(item.Key);
                        payouts[i].count.text = $"x{item.Value}";
                        i++;
                    }
                }
            }

        }

        private void OnOkButtonClicked()
        {
            audioService.PlayStandardClick();
            closeDlgSignal.Dispatch();
        }
    }
}
