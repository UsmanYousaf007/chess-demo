/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-31 03:15:24 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using DG.Tweening;
using TurboLabz.Common;
using System.Collections;
using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class ShopView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        public Button currency1BuyButton;
        public Text currency1Label;

        public Button currency2BuyButton;
        public Text currency2Label;

        public GameObject overlay;
        public Image overlayImage;
        public Button overlayButton;

        public Text headingLabel;

        public Transform menuPanelTransform;
        public Transform itemSelected;

        public Button lootBoxesButton;
        public Text lootBoxesLabel;

        public Button avatarsButton;
        public Text avatarsLabel;

        public Button chessSkinsButton;
        public Text chessSkinsLabel;

        public Button chatButton;
        public Text chatLabel;

        public Button currencyButton;
        public Text currencyLabel;

        public Button backButton;

        public Button freeCurrency1Button;
        public Text freeCurrencyLabel;

        public Button menuPanelButton;

        public Image lootBoxesIcon;
        public Image avatarsIcon;
        public Image chessSkinsIcon;
        public Image chatIcon;
        public Image currencyIcon;

        public Transform lootBoxesButtonTransform;
        public Transform avatarsButtonTransform;
        public Transform chessSkinsButtonTransform;
        public Transform chatButtonTransform;
        public Transform currencyButtonTransform;

        // View signals
        public Signal currency1BuyButtonClickedSignal = new Signal();
        public Signal currency2BuyButtonClickedSignal = new Signal();
        public Signal lootBoxesButtonClickedSignal = new Signal();
        public Signal avatarsButtonClickedSignal = new Signal();
        public Signal chessSkinsClickedSignal = new Signal();
        public Signal chatButtonClickedSignal = new Signal();
        public Signal currencyButtonClickedSignal = new Signal();
        public Signal backButtonClickedSignal = new Signal();
        public Signal freeCurrency1ButtonClickedSignal = new Signal();
        public Signal menuPanelButtonClickedSignal = new Signal();

        public Signal<string, ShopVO> lootBoxesThumbnailClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, ShopVO> avatarsThumbnailClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, ShopVO> avatarsBorderThumbnailClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, ShopVO> chessSkinThumbnailClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, ShopVO> currency1ThumbnailClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, ShopVO> currency2ThumbnailClickedSignal = new Signal<string, ShopVO>();

        private float menuPanelLocalPosX;
        private bool menuPanelIsOpened;

        public GameObject scrollContent;

        public TextSubheading textSubheadingPrefab;
        public GameObject thumbnailGridGroupPrefab;
        public GameObject currencyThumbnailGridGroupPrefab;
        public GameObject scrollerSpacerPrefab;

        public LootBoxesThumbnail lootBoxesThumbnailPrefab;

        public SkinThumbnail skinThumbnailPrefab;

        public AvatarsThumbnail avatarsThumbnailPrefab;
        public AvatarsBorderThumbnail avatarsBorderThumbnailPrefab;

        public CurrencyThumbnail currencyThumbnail;

        public ScrollerSubheading scrollerSubheadingPrefab;

        private GameObject thumbnailGridGroup = null;
        private GameObject currencyThumbnailGridGroup = null;

        private LootThumbsContainer lootThumbsContainer;
        private SkinThumbsContainer skinThumbsContainer;
        private AvatarThumbsContainer avatarThumbsContainer;
        private AvatarBorderThumbsContainer avatarBorderThumbsContainer;
        private CoinPackThumbsContainer currencyThumbsContainer;

        public void Init()
        {
            // To get initial position of menuPanel for DoTween animatio. As it would be different on different devices.
            menuPanelLocalPosX = menuPanelTransform.localPosition.x;

            lootThumbsContainer = LootThumbsContainer.Load();
            skinThumbsContainer = SkinThumbsContainer.Load();
            avatarThumbsContainer = AvatarThumbsContainer.Load();
            avatarBorderThumbsContainer = AvatarBorderThumbsContainer.Load();
            currencyThumbsContainer = CoinPackThumbsContainer.Load();

            currency1BuyButton.onClick.AddListener(OnCurrency1BuyButtonClicked);
            currency2BuyButton.onClick.AddListener(OnCurrency2BuyButtonClicked);
            lootBoxesButton.onClick.AddListener(OnLootBoxesButtonClicked);
            avatarsButton.onClick.AddListener(OnAvatarsButtonClicked);
            chessSkinsButton.onClick.AddListener(OnChessSkinsButtonClicked);
            chatButton.onClick.AddListener(OnChatButtonClicked);
            currencyButton.onClick.AddListener(OnCurrencyButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            freeCurrency1Button.onClick.AddListener(OnFreeCurrency1ButtonClicked);
            overlayButton.onClick.AddListener(OnOverlayButtonClicked);
            menuPanelButton.onClick.AddListener(OnMenuPanelButtonClicked);
        }

        public void UpdateView(ShopVO vo)
        {
            currency1Label.text = vo.playerModel.currency1.ToString("N0");
            currency2Label.text = vo.playerModel.currency2.ToString("N0");

            lootBoxesLabel.text = localizationService.Get(LocalizationKey.S_LOOT_BOXES_LABEL);
            avatarsLabel.text = localizationService.Get(LocalizationKey.S_AVATARS_LABEL);
            chessSkinsLabel.text = localizationService.Get(LocalizationKey.S_CHESS_SKINS_LABEL);
            chatLabel.text = localizationService.Get(LocalizationKey.S_CHAT_LABEL);
            currencyLabel.text = localizationService.Get(LocalizationKey.S_CURRENCY_LABEL);
            freeCurrencyLabel.text = localizationService.Get(LocalizationKey.S_FREE_CURRENCY_1_LABEL);

            if (vo.subShopViewId == SubShopViewId.LOOTBOXES)
            {
                headingLabel.text = localizationService.Get(LocalizationKey.S_LOOT_BOXES_LABEL);
                CreateLootBoxesScreen(vo);
            }
            else if(vo.subShopViewId == SubShopViewId.AVATARS)
            {
                headingLabel.text = localizationService.Get(LocalizationKey.S_AVATARS_BORDERS_LABEL);
                CreateAvatarsScreen(vo);
            }
            else if(vo.subShopViewId == SubShopViewId.CHESSSKINS)
            {
                headingLabel.text = localizationService.Get(LocalizationKey.S_CHESS_SKINS_LABEL);
                CreateChessSkinsScreen(vo);
            }
            else if(vo.subShopViewId == SubShopViewId.CHAT)
            {
                headingLabel.text = localizationService.Get(LocalizationKey.S_CHAT_LABEL);
                CreateChatScreen(vo);
            }
            else if(vo.subShopViewId == SubShopViewId.CURRENCY)
            {
                headingLabel.text = localizationService.Get(LocalizationKey.S_BUCKS_COINS_LABEL);
                CreateCurrencyScreen(vo);
            }
        }

        public void CreateLootBoxesScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);

            itemSelected.DOLocalMoveY(lootBoxesButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            lootBoxesIcon.color = tempColor;

            CreateSpacer();
            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, LootBoxShopItem> skinShopItem in vo.shopSettings.lootBoxShopItems)
            {
                LootBoxShopItem item = skinShopItem.Value; 

                LootBoxesThumbnail thumbnail = Instantiate<LootBoxesThumbnail>(lootBoxesThumbnailPrefab);

                thumbnail.nameLabel.text = item.displayName;
                thumbnail.image.sprite = lootThumbsContainer.GetThumb(item.id).thumbnail;
                thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                thumbnail.amountLabel.text = item.currency2Cost.ToString();
                thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);
                thumbnail.price.SetActive(true);
                thumbnail.owned.SetActive(false);
                thumbnail.thumbnailButton.onClick.AddListener(() => OnLootBoxThumbnailButtonClicked(item.id,vo));

                thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);   
            }
        }

        public void CreateAvatarsScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);

            itemSelected.DOLocalMoveY(avatarsButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            avatarsIcon.color = tempColor;

            CreateSpacer();
            CreateSubheading(LocalizationKey.S_AVATARS_LABEL);
            CreateSpacer();

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, AvatarShopItem> avatarShopItems in vo.shopSettings.avatarShopItems)
            {
                
                AvatarShopItem item = avatarShopItems.Value; 

                AvatarsThumbnail thumbnail = Instantiate<AvatarsThumbnail>(avatarsThumbnailPrefab);

                thumbnail.nameLabel.text = item.displayName;
                thumbnail.image.sprite = avatarThumbsContainer.GetThumb(item.id).thumbnail;
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                thumbnail.amountLabel.text = item.currency2Cost.ToString();
                thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                {
                    thumbnail.price.SetActive(false);
                    thumbnail.owned.SetActive(true);
                }
                else
                {
                    thumbnail.price.SetActive(true);
                    thumbnail.owned.SetActive(false);
                }

                thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarThumbnailButtonClicked(item.id, vo));

                thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);    
            }

            CreateSpacer();

            CreateSubheading(LocalizationKey.S_BORDER_LABEL);

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_COMMON_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, AvatarBorderShopItem> avatarBorderShopItem in vo.shopSettings.avatarBorderShopItems)
            {
                AvatarBorderShopItem item = avatarBorderShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON)
                {
                    AvatarsBorderThumbnail thumbnail = Instantiate<AvatarsBorderThumbnail>(avatarsBorderThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarBorderThumbnailButtonClicked(item.id, vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false); 
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_RARE_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, AvatarBorderShopItem> avatarBorderShopItem in vo.shopSettings.avatarBorderShopItems)
            {
                AvatarBorderShopItem item = avatarBorderShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE)
                {
                    AvatarsBorderThumbnail thumbnail = Instantiate<AvatarsBorderThumbnail>(avatarsBorderThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarBorderThumbnailButtonClicked(item.id, vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false); 
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_EPIC_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, AvatarBorderShopItem> avatarBorderShopItem in vo.shopSettings.avatarBorderShopItems)
            {
                AvatarBorderShopItem item = avatarBorderShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC)
                {
                    AvatarsBorderThumbnail thumbnail = Instantiate<AvatarsBorderThumbnail>(avatarsBorderThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarBorderThumbnailButtonClicked(item.id, vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false); 
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_LEGENDARY_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, AvatarBorderShopItem> avatarBorderShopItem in vo.shopSettings.avatarBorderShopItems)
            {
                AvatarBorderShopItem item = avatarBorderShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY)
                {
                    AvatarsBorderThumbnail thumbnail = Instantiate<AvatarsBorderThumbnail>(avatarsBorderThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarBorderThumbnailButtonClicked(item.id, vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false); 
                }     
            }
        }

        public void CreateChessSkinsScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);

            itemSelected.DOLocalMoveY(chessSkinsButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            chessSkinsIcon.color = tempColor;

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_COMMON_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, SkinShopItem> skinShopItem in vo.shopSettings.skinShopItems)
            {
                SkinShopItem item = skinShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON)
                {
                    SkinThumbnail thumbnail = Instantiate<SkinThumbnail>(skinThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnChessSkinsThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_RARE_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, SkinShopItem> skinShopItem in vo.shopSettings.skinShopItems)
            {
                SkinShopItem item = skinShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE)
                {
                    SkinThumbnail thumbnail = Instantiate<SkinThumbnail>(skinThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnChessSkinsThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_EPIC_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, SkinShopItem> skinShopItem in vo.shopSettings.skinShopItems)
            {
                SkinShopItem item = skinShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC)
                {
                    SkinThumbnail thumbnail = Instantiate<SkinThumbnail>(skinThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnChessSkinsThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
                }     
            }

            CreateTextSubheading(localizationService.Get(LocalizationKey.S_LEGENDARY_LABEL), GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY);

            CreateThumbnailGridGroup();

            foreach(KeyValuePair<string, SkinShopItem> skinShopItem in vo.shopSettings.skinShopItems)
            {
                SkinShopItem item = skinShopItem.Value; 

                if (item.tier == GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY)
                {
                    SkinThumbnail thumbnail = Instantiate<SkinThumbnail>(skinThumbnailPrefab);

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);
                    thumbnail.amountLabel.text = item.currency2Cost.ToString();
                    thumbnail.ownedLabel.text = localizationService.Get(LocalizationKey.S_OWNED_LABEL);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id) && (vo.inventorySettings.allShopItems[item.id] >= item.maxQuantity))
                    {
                        thumbnail.price.SetActive(false);
                        thumbnail.owned.SetActive(true);
                    }
                    else
                    {
                        thumbnail.price.SetActive(true);
                        thumbnail.owned.SetActive(false);
                    }

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnChessSkinsThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
                }     
            }
        }

        public void CreateChatScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);

            itemSelected.DOLocalMoveY(chatButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            chatIcon.color = tempColor;
        }

        public void CreateCurrencyScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);

            itemSelected.DOLocalMoveY(currencyButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            currencyIcon.color = tempColor;

            CreateSpacer();
            CreateSubheading(LocalizationKey.S_BUCKS_LABEL);
            CreateSpacer();

            CreateCurrencyThumbnailGridGroup();

            foreach (KeyValuePair<string, CurrencyShopItem> currencyShopItem in vo.shopSettings.currencyShopItems)
            {
                CurrencyShopItem item = currencyShopItem.Value; 

                if (item.type == GSBackendKeys.ShopItem.SHOP_ITEM_TYPE_CURRENCY)
                {
                    CurrencyThumbnail thumbnail = Instantiate<CurrencyThumbnail>(currencyThumbnail);

                    thumbnail.iapAmout.SetActive(true);

                    thumbnail.background.sprite = spriteCache.GetShopCurrencyItemsFrameBackground(LocalizationKey.BUCKS);
                    thumbnail.iapPrice.text = InAppPurchase.instance.GetItemLocalizedPrice(item.id);
                    thumbnail.imageFrame.sprite = spriteCache.GetShopCurrencyItemsFrame(LocalizationKey.BUCKS);
                    thumbnail.title.text = localizationService.Get(LocalizationKey.S_BUCKS_LABEL);
                    thumbnail.rewardWithBonus.text = item.currency2Cost.ToString();
                    thumbnail.icon.sprite = currencyThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.rewardWithoutBonus.text = (item.currency2Cost - item.bonusAmount).ToString();
                    thumbnail.bonus.text = localizationService.Get(LocalizationKey.S_BONUS_LABEL,item.bonusAmount);

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnCurrency2ThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(currencyThumbnailGridGroup.transform, false);
                }
            }

            CreateSpacer();

            CreateSubheading(LocalizationKey.S_COINS_LABEL);

            CreateSpacer();

            CreateCurrencyThumbnailGridGroup();

            foreach (KeyValuePair<string, CurrencyShopItem> currencyShopItem in vo.shopSettings.currencyShopItems)
            {
                CurrencyShopItem item = currencyShopItem.Value; 

                if (item.type == GSBackendKeys.ShopItem.SHOP_ITEM_TYPE_VGOOD)
                {
                    CurrencyThumbnail thumbnail = Instantiate<CurrencyThumbnail>(currencyThumbnail);

                    thumbnail.bucksAmout.SetActive(true);

                    thumbnail.background.sprite = spriteCache.GetShopCurrencyItemsFrameBackground(LocalizationKey.COINS);
                    thumbnail.bucksPrice.text = item.currency2Cost.ToString();
                    thumbnail.imageFrame.sprite = spriteCache.GetShopCurrencyItemsFrame(LocalizationKey.COINS);
                    thumbnail.title.text = localizationService.Get(LocalizationKey.S_BUCKS_LABEL);
                    thumbnail.rewardWithBonus.text = item.currency1Cost.ToString();
                    thumbnail.icon.sprite = currencyThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.rewardWithoutBonus.text = (item.currency1Cost - item.bonusAmount).ToString();
                    thumbnail.bonus.text = localizationService.Get(LocalizationKey.S_BONUS_LABEL,item.bonusAmount);

                    thumbnail.thumbnailButton.onClick.AddListener(() => OnCurrency1ThumbnailButtonClicked(item.id,vo));

                    thumbnail.transform.SetParent(currencyThumbnailGridGroup.transform, false);
                }
            }
        }

        public void CreateTextSubheading(string headingLabel, string tier)
        {
            TextSubheading textSubheading = Instantiate<TextSubheading>(textSubheadingPrefab);
            textSubheading.subheadingLabel.text = headingLabel;
            textSubheading.subheadingLabel.color = textColorCache.GetShopItemsHeadingSeperatorColor(tier);
            textSubheading.rightSeperator.sprite = spriteCache.GetShopItemsHeadingRightSeperator(tier);
            textSubheading.leftSeperator.sprite = spriteCache.GetShopItemsHeadingLeftSeperator(tier);
            textSubheading.transform.SetParent(scrollContent.transform, false);
        }

        public void CreateSubheading(string headingLabel)
        {
            ScrollerSubheading scrollerSubheading = Instantiate<ScrollerSubheading>(scrollerSubheadingPrefab);
            scrollerSubheading.subheadingLabel.text = localizationService.Get(headingLabel);
            scrollerSubheading.transform.SetParent(scrollContent.transform, false);
        }

        public void CreateThumbnailGridGroup()
        {
            thumbnailGridGroup  = Instantiate(thumbnailGridGroupPrefab);
            thumbnailGridGroup.transform.SetParent(scrollContent.transform, false);
        }

        public void CreateCurrencyThumbnailGridGroup()
        {
            currencyThumbnailGridGroup  = Instantiate(currencyThumbnailGridGroupPrefab);
            currencyThumbnailGridGroup.transform.SetParent(scrollContent.transform, false);
        }

        public void CreateSpacer()
        {
            GameObject spacer = Instantiate(scrollerSpacerPrefab);
            spacer.transform.SetParent(scrollContent.transform, false);
        }

        public void OnLootBoxThumbnailButtonClicked(string id,ShopVO vo)
        {
            lootBoxesThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void OnAvatarThumbnailButtonClicked(string id,ShopVO vo)
        {
            avatarsThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void OnAvatarBorderThumbnailButtonClicked(string id,ShopVO vo)
        {
            avatarsBorderThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void OnChessSkinsThumbnailButtonClicked(string id,ShopVO vo)
        {
            chessSkinThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void OnCurrency1ThumbnailButtonClicked(string id,ShopVO vo)
        {
            currency1ThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void OnCurrency2ThumbnailButtonClicked(string id,ShopVO vo)
        {
            currency2ThumbnailClickedSignal.Dispatch(id,vo);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCurrency1BuyButtonClicked()
        {
            closeMenuPanel();

            currency1BuyButtonClickedSignal.Dispatch();
        }

        private void OnCurrency2BuyButtonClicked()
        {
            closeMenuPanel();

            currency2BuyButtonClickedSignal.Dispatch();
        }

        private void OnLootBoxesButtonClicked()
        {
            lootBoxesButtonClickedSignal.Dispatch();
        }

        private void OnAvatarsButtonClicked()
        {
            avatarsButtonClickedSignal.Dispatch();
        }

        private void OnChessSkinsButtonClicked()
        {
            chessSkinsClickedSignal.Dispatch();
        }

        private void OnChatButtonClicked()
        {
            chatButtonClickedSignal.Dispatch();
        }

        private void OnCurrencyButtonClicked()
        {
            currencyButtonClickedSignal.Dispatch();
        }

        private void OnBackButtonClicked()
        {
            closeMenuPanel();

            backButtonClickedSignal.Dispatch();
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            closeMenuPanel();

            freeCurrency1ButtonClickedSignal.Dispatch();
        }

        private void OnOverlayButtonClicked()
        {
            closeMenuPanel();
        }

        private void OnMenuPanelButtonClicked()
        {
            if (menuPanelIsOpened)
            {
                overlayImage.DOFade(ShopViewConstants.overlayMinOpacity,ShopViewConstants.menuPanelAnimationDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(DisableOverlay);
                
                menuPanelTransform.DOLocalMoveX(menuPanelLocalPosX, ShopViewConstants.menuPanelAnimationDuration)
                    .SetEase(Ease.InExpo);
                
                menuPanelIsOpened = false;
            }
            else
            {
                overlay.SetActive(true);
                overlayImage.DOFade(ShopViewConstants.overlayMaxOpacity,ShopViewConstants.menuPanelAnimationDuration);

                menuPanelTransform.DOLocalMoveX(menuPanelLocalPosX - ShopViewConstants.menuPanelOpenedXValue, ShopViewConstants.menuPanelAnimationDuration)
                    .SetEase(Ease.OutExpo);
                
                menuPanelIsOpened = true;
            }
        }

        private void closeMenuPanel()
        {
            overlayImage.DOFade(ShopViewConstants.overlayMinOpacity,ShopViewConstants.menuPanelAnimationDuration)
                .SetEase(Ease.Linear)
                .OnComplete(DisableOverlay);

            menuPanelTransform.DOLocalMoveX(menuPanelLocalPosX, ShopViewConstants.menuPanelAnimationDuration)
                .SetEase(Ease.InExpo);

            menuPanelIsOpened = false;
        }

        private void SetHalfOpacityOfMenuPanelIcons()
        {
            Color tempColorFullOpacity = Color.white;
            tempColorFullOpacity.a = ShopViewConstants.halfOpacity;

            lootBoxesIcon.color = tempColorFullOpacity;
            avatarsIcon.color = tempColorFullOpacity;
            chessSkinsIcon.color = tempColorFullOpacity;
            chatIcon.color = tempColorFullOpacity;
            currencyIcon.color = tempColorFullOpacity;
        }

        private void DisableOverlay()
        {
            overlay.SetActive(false);
        }
    }
}