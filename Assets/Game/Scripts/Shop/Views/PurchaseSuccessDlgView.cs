using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class PurchaseSuccessDlgView : View
    {
        public Text title;
        public Text subTitle;
        public Image icon;
        public Image thumbnail;
        public Button okButton;

        private static StoreIconsContainer iconsContainer;
        private static StoreThumbsContainer thumbsContainer;

        public Signal okPressedSignal = new Signal();

        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            iconsContainer = StoreIconsContainer.Load();
            thumbsContainer = StoreThumbsContainer.Load();
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(StoreItem item)
        {
            if (item == null)
            {
                return;
            }

            var titles = item.displayName.Split(' ');
            title.text = titles[0];
            subTitle.text = titles[1];
            icon.sprite = iconsContainer.GetSprite(item.key);
            thumbnail.sprite = thumbnail.sprite = thumbsContainer.GetSprite(titles[1].Equals("Gems") ? "Gem" : "Coin");
            icon.SetNativeSize();
        }

        private void OnOkButtonClicked()
        {
            audioService.PlayStandardClick();
            okPressedSignal.Dispatch();
        }
    }
}
