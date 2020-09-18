using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class SpotInventoryView : View
    {
        public Text title;
        public Text subTitle;
        public Button close;
        public Image icon;
        public Button gemsButton;
        public Text gemsPrice;
        public Sprite enoughGemsSprite;
        public Sprite notEnoughGemsSprite;
        public Button watchAdButton;
        public Text watchAdText;
        public GameObject toolTip;
        public Text toolTipText;
        public GameObject bar;
        public Text barText;
        public RectTransform barFiller;
        public ParticleSystem barParticleSystem;
        public Text orText;

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        //Dispatch Signals
        public Signal closeDlgSignal = new Signal();
        public Signal<string> buyButtonSignal = new Signal<string>();
        public Signal<InventoryVideoVO> watchAdSignal = new Signal<InventoryVideoVO>();
        public Signal notEnoughCurrencyToUnlockSignal = new Signal();

        private float rewardBarOriginalWidth;
        private int currentRewardedPoints;
        private int requiredRewardedPoints;
        private bool haveEnoughGems;

        [HideInInspector] public StoreItem storeItem;

        protected override void OnEnable()
        {
            base.OnEnable();
            barParticleSystem.Stop();
        }

        public void Init()
        {
            subTitle.text = localizationService.Get(LocalizationKey.SPOT_INVENTORY_SUB_TITLE);
            watchAdText.text = localizationService.Get(LocalizationKey.INVENTORY_WATCH_AD);
            toolTipText.text = localizationService.Get(LocalizationKey.INVENTORY_TOOL_TIP);
            orText.text = localizationService.Get(LocalizationKey.INVENTORY_OR);

            close.onClick.AddListener(OnCloseButtonClicked);
            gemsButton.onClick.AddListener(OnBuyButtonClicked);
            watchAdButton.onClick.AddListener(OnWatchAdButtonClicked);

            toolTip.SetActive(false);
            rewardBarOriginalWidth = barFiller.sizeDelta.x;
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
            closeDlgSignal.Dispatch();
        }

        public void UpdateView(SpotInventoryVO vo)
        {
            storeItem = vo.storeItem;
            currentRewardedPoints = vo.currentRewardedPoints;
            requiredRewardedPoints = vo.requiredRewardedPoints;

            title.text = $"{localizationService.Get(LocalizationKey.SPOT_INVENTORY_TITLE)} {storeItem.displayName}s?";
            icon.sprite = vo.icon;
            icon.SetNativeSize();
            SetupPrice(vo.gemsCount);
            SetupRewardBar();
        }

        private void SetupRewardBar()
        {
            bar.SetActive(requiredRewardedPoints > 1);
            var barFillPercentage = (float)currentRewardedPoints / requiredRewardedPoints;
            barFiller.sizeDelta = new Vector2(rewardBarOriginalWidth * barFillPercentage, barFiller.sizeDelta.y);
            barText.text = $"{currentRewardedPoints}/{requiredRewardedPoints}";
        }

        public void SetupPrice(long gems)
        {
            if (storeItem == null)
            {
                return;
            }

            gemsPrice.text = storeItem.currency3Cost.ToString();
            haveEnoughGems = gems >= storeItem.currency3Cost;
            gemsButton.image.sprite = haveEnoughGems ? enoughGemsSprite : notEnoughGemsSprite;
        }

        public void OnRewardedPointAdded()
        {
            currentRewardedPoints++;
            var to = currentRewardedPoints;
            var from = to - 1;
            var barFillPercentageFrom = (float)from / requiredRewardedPoints;
            var barFillPercentageTo = (float)to / requiredRewardedPoints;
            var barValueFrom = rewardBarOriginalWidth * barFillPercentageFrom;
            var barValurTo = rewardBarOriginalWidth * barFillPercentageTo;
            barText.text = $"{to}/{requiredRewardedPoints}";
            AnimateRewardedVideoProgressBar(barValueFrom, barValurTo);
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
        }

        private void AnimateRewardedVideoProgressBar(float from, float to)
        {
            if (!barParticleSystem.isPlaying)
            {
                barParticleSystem.Play();
            }

            iTween.ValueTo(this.gameObject,
                iTween.Hash(
                    "from", from,
                    "to", to,
                    "time", 1f,
                    "onupdate", "AnimateBar",
                    "onupdatetarget", this.gameObject,
                    "oncomplete", "AnimationComplete"
                ));
        }

        private void AnimateBar(float value)
        {
            barFiller.sizeDelta = new Vector2(value, barFiller.sizeDelta.y);
        }

        private void AnimationComplete()
        {
            barParticleSystem.Stop();
        }

        private void OnBuyButtonClicked()
        {
            audioService.PlayStandardClick();

            if (haveEnoughGems)
            {
                buyButtonSignal.Dispatch(storeItem.key);
            }
            else
            {
                notEnoughCurrencyToUnlockSignal.Dispatch();
            }
        }

        private void OnWatchAdButtonClicked()
        {
            audioService.PlayStandardClick();
            var vo = new InventoryVideoVO();
            vo.itemKey = storeItem.key;
            vo.itemPointsKey = $"{storeItem.key}Points";
            vo.isPopup = true;
            watchAdSignal.Dispatch(vo);
        }

        public void ShowTooltip()
        {
            toolTip.SetActive(true);
        }
    }
}
