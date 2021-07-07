using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class ShopCoinItemView : View
    {
        public string shortCode;
        public Text title;
        public Text price;
        public Text extraSavingText;
        public Image icon;
        public Button buyButton;
        public GameObject extraSavings;
        public Button thumbnailButton;
        public GameObject loading;
        public Image gemIcon;

        private static StoreIconsContainer iconsContainer;
        private bool isInitlialised = false;
        private StoreItem storeItem;
        private bool haveEnoughGems;

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal<VirtualGoodsTransactionVO> buyButtonSignal = new Signal<VirtualGoodsTransactionVO>();
        public Signal<long> notEnoughGemsSignal = new Signal<long>();

        public void Init()
        {
            if (iconsContainer == null)
            {
                iconsContainer = StoreIconsContainer.Load();
            }

            //buyButton.onClick.AddListener(OnBuyButtonClicked);
            thumbnailButton.onClick.AddListener(OnBuyButtonClicked);
            SetupLoading(true);
        }

        public void OnStoreAvailable(bool available, bool forceUpdate = false)
        {
            if ((storeItem == null || forceUpdate) && storeSettingsModel.items.ContainsKey(shortCode))
            {
                storeItem = storeSettingsModel.items[shortCode];
            }

            if (storeItem == null)
            {
                return;
            }

            if (!isInitlialised || forceUpdate)
            {
                title.text = storeItem.displayName.Split(' ')[0];
                icon.sprite = iconsContainer.GetSprite(shortCode);
                icon.SetNativeSize();
                extraSavings.SetActive(!(string.IsNullOrEmpty(storeItem.description) || string.IsNullOrWhiteSpace(storeItem.description)));
                //extraSavingText.text = storeItem.description;
                extraSavingText.text = "<size=27%>" + storeItem.description + "</size>" + "% Extra";
                price.text = storeItem.currency3Payout.ToString();
                haveEnoughGems = playerModel.gems >= storeItem.currency3Payout;
                SetupLoading(false);
                isInitlialised = true;
            }
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();

            if (haveEnoughGems)
            {
                var vo = new VirtualGoodsTransactionVO();
                vo.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
                vo.buyQuantity = (int)storeItem.currency4Payout;
                vo.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                vo.consumeQuantity = storeItem.currency3Payout;
                buyButtonSignal.Dispatch(vo);
                SetupLoading(true);
            }
            else
            {
                notEnoughGemsSignal.Dispatch(storeItem.currency4Payout);
            }
        }

        public void OnPurchaseCompleted(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            }

            SetupLoading(false);
        }

        private void SetupLoading(bool isLoading)
        {
            loading.SetActive(isLoading);
            price.enabled = !isLoading;
            buyButton.interactable = !isLoading;
            thumbnailButton.enabled = !isLoading;
            gemIcon.enabled = !isLoading;
        }

        public void OnInventoryUpdated()
        {
            if (storeItem == null)
            {
                return;
            }

            haveEnoughGems = playerModel.gems >= storeItem.currency3Payout;
        }

        public void Setup(string key)
        {
            shortCode = key;
            OnStoreAvailable(true, true);
        }
    }
}
