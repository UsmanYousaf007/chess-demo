/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-22 12:26:08 UTC+05:00

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
    public class InventoryView : View
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

        public Text collectionLabel;
        public Text lootLabel;

        public Button collectionButton;
        public Button lootButton;

        public GameObject collectionTabSelected;
        public GameObject lootTabSelected;

        public Text headingLabel;

        //public Button infoButton;
        //public GameObject infoButtonObject;

        //public Button LootInfoButton;
        //public GameObject LootInfoButtonObject;

        public GameObject menuPanelObject;
        public Transform menuPanelTransform;
        public Transform itemSelected;

        public Button avatarsButton;
        public Text avatarsLabel;

        public Button chessSkinsButton;
        public Text chessSkinsLabel;

        public Button backButton;

        public Button freeCurrency1Button;
        public Text freeCurrencyLabel;

        public Button menuPanelButton;
        public GameObject menuPanelButtonObject;

        public Image avatarsIcon;
        public Image chessSkinsIcon;

        public Transform avatarsButtonTransform;
        public Transform chessSkinsButtonTransform;

        // View signals
        public Signal currency1BuyButtonClickedSignal = new Signal();
        public Signal currency2BuyButtonClickedSignal = new Signal();
        public Signal avatarsButtonClickedSignal = new Signal();
        public Signal chessSkinsClickedSignal = new Signal();
        public Signal<string, ShopVO> chessSkinsThumbnailInfoButtonClickedSignal = new Signal<string, ShopVO>();
        public Signal<string, string, ShopVO> lootForgeCardInfoButtonClickedSignal = new Signal<string, string, ShopVO>();
        public Signal<string, string, ShopVO> lootForgeCardDismantleButtonClickedSignal = new Signal<string, string, ShopVO>();
        public Signal<string> lootBoxThumbnailClickedSignal = new Signal<string>();
        public Signal facebookAvatarLogInButtonClicked = new Signal();
        public Signal lootClickedSignal = new Signal();
        public Signal collectionClickedSignal = new Signal();
        public Signal backButtonClickedSignal = new Signal();
        public Signal freeCurrency1ButtonClickedSignal = new Signal();
        public Signal menuPanelButtonClickedSignal = new Signal();

        public GameObject scrollerChessSkins;
        public GameObject scrollerAvatars;
        public GameObject scrollerLoot;

        public Text emptyLootScreenLabel;
        public GameObject emptyLootScreenTextObject;

        public GameObject scrollContentChessSkins;
        public GameObject scrollContentAvatars;
        public GameObject scrollContentLoot;
        public GameObject avatarsScreen;

        public Image avatarsScreenImage;
        public Image avatarsScreenBorderImage;

        public Transform infoPanelParent;

        public GameObject infoPanelOverlayObject;
        public Button infoPanelOverlayButton;

        public TextSubheading textSubheadingPrefab;
        public GameObject thumbnailGridGroupPrefab;
        public GameObject thumbnailLootGridGroupPrefab;
        public GameObject scrollerSpacerPrefab;

        public InventoryChessSkinsThumbnail inventoryChessSkinsThumbnailPrefab;
        public InfoEquipPanel infoEquipPanelPrefab;
        public InfoPanel infoPanelPrefab;
        public InventoryAvatarsThumbnail inventoryAvatarsThumbnailPrefab;
        public InventoryAvatarsBorderThumbnail inventoryAvatarsBorderThumbnailPrefab;

        public LootInfoChessSkinPanel lootInfoChessSkinPanel;
        public LootInfoAvatarPanel lootInfoAvatarPanel;
        public LootInfoAvatarBorderPanel lootInfoAvatarBorderPanel;
        public InventoryLootAvatarThumbnail inventoryLootAvatarThumbnailPrefab;
        public InventoryLootAvatarBorderThumbnail inventoryLootAvatarBorderThumbnailPrefab;
        public InventoryLootChessSkinThumbnail inventoryLootChessSkinThumbnailPrefab;
        public InventoryLootLootBoxThumbnail inventoryLootLootBoxThumbnailPrefab;

        public ScrollerSubheading scrollerSubheadingPrefab;

        private LootThumbsContainer lootThumbsContainer;
        private SkinThumbsContainer skinThumbsContainer;
        private AvatarThumbsContainer avatarThumbsContainer;
        private AvatarBorderThumbsContainer avatarBorderThumbsContainer;

        private GameObject thumbnailGridGroup = null;

        private float menuPanelLocalPosX;
        private bool menuPanelIsOpened;

        private InfoEquipPanel currentInfoEquipPanel;
        private InfoPanel currentInfoPanel;
        private LootInfoChessSkinPanel currentLootInfoChessSkinPanel;
        private LootInfoAvatarPanel currentLootInfoAvatarPanel;
        private LootInfoAvatarBorderPanel currentLootInfoAvatarBorderPanel;

        private InventoryChessSkinsThumbnail currentChessSkinsThumbnailClicked;
        private InventoryLootAvatarThumbnail currentLootAvatarThumbnailClicked;
        private InventoryLootAvatarBorderThumbnail currentLootAvatarBorderThumbnailClicked;
        private InventoryLootChessSkinThumbnail currentLootChessSkinThumbnailClicked;

        private InventoryAvatarsBorderThumbnail currentAvatarsBorderThumbnail;
        private InventoryAvatarsThumbnail currentAvatarsThumbnail;
        private InventoryChessSkinsThumbnail currentChessSkinsThumbnail ;

        void OnDisable()
        {
            UnityUtil.DestroyChildren(scrollContentChessSkins.transform);
            UnityUtil.DestroyChildren(scrollContentChessSkins.transform);
            DestroyInfoPanel();
            emptyLootScreenTextObject.SetActive(false);
        }

        void Update()
        {
            if (currentInfoEquipPanel != null)
            {
                currentInfoEquipPanel.transform.position = new Vector3(currentChessSkinsThumbnailClicked.transform.position.x, currentChessSkinsThumbnailClicked.transform.position.y - 125f, currentChessSkinsThumbnailClicked.transform.position.z);
            }
            else if (currentInfoPanel != null)
            {
                currentInfoPanel.transform.position = new Vector3(currentChessSkinsThumbnailClicked.transform.position.x, currentChessSkinsThumbnailClicked.transform.position.y - 50f, currentChessSkinsThumbnailClicked.transform.position.z);
            }
            else if (currentLootInfoChessSkinPanel != null)
            {
                currentLootInfoChessSkinPanel.transform.position = new Vector3(currentLootChessSkinThumbnailClicked.transform.position.x, currentLootChessSkinThumbnailClicked.transform.position.y - 135f, currentLootChessSkinThumbnailClicked.transform.position.z);
            }
            else if (currentLootInfoAvatarPanel != null)
            {
                currentLootInfoAvatarPanel.transform.position = new Vector3(currentLootAvatarThumbnailClicked.transform.position.x, currentLootAvatarThumbnailClicked.transform.position.y - 135f, currentLootAvatarThumbnailClicked.transform.position.z);
            }
            else if (currentLootInfoAvatarBorderPanel != null)
            {
                currentLootInfoAvatarBorderPanel.transform.position = new Vector3(currentLootAvatarBorderThumbnailClicked.transform.position.x, currentLootAvatarBorderThumbnailClicked.transform.position.y - 135f, currentLootAvatarBorderThumbnailClicked.transform.position.z);
            }
        }

        public void Init()
        {
            // To get initial position of menuPanel for DoTween animatio. As it would be different on different devices.
            menuPanelLocalPosX = menuPanelTransform.localPosition.x;

            lootThumbsContainer = LootThumbsContainer.Load();
            skinThumbsContainer = SkinThumbsContainer.Load();
            avatarThumbsContainer = AvatarThumbsContainer.Load();
            avatarBorderThumbsContainer = AvatarBorderThumbsContainer.Load();

            currency1BuyButton.onClick.AddListener(OnCurrency1BuyButtonClicked);
            currency2BuyButton.onClick.AddListener(OnCurrency2BuyButtonClicked);
            collectionButton.onClick.AddListener(OnCollectionButtonClicked);
            lootButton.onClick.AddListener(OnLootButtonClicked);
            //infoButton.onClick.AddListener(OnInfoButtonClicked);
            avatarsButton.onClick.AddListener(OnAvatarsButtonClicked);
            chessSkinsButton.onClick.AddListener(OnChessSkinsButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            freeCurrency1Button.onClick.AddListener(OnFreeCurrency1ButtonClicked);
            overlayButton.onClick.AddListener(OnOverlayButtonClicked);
            menuPanelButton.onClick.AddListener(OnMenuPanelButtonClicked);
            infoPanelOverlayButton.onClick.AddListener(DestroyInfoPanel);
        }

        public void UpdateView(ShopVO vo)
        {
            currency1Label.text = vo.playerModel.currency1.ToString("N0");
            currency2Label.text = vo.playerModel.currency2.ToString("N0");

            collectionLabel.text = localizationService.Get(LocalizationKey.S_COLLECTION_LABEL);
            lootLabel.text = localizationService.Get(LocalizationKey.S_LOOT_LABEL);
            avatarsLabel.text = localizationService.Get(LocalizationKey.S_AVATARS_LABEL);
            chessSkinsLabel.text = localizationService.Get(LocalizationKey.S_CHESS_SKINS_LABEL);
            freeCurrencyLabel.text = localizationService.Get(LocalizationKey.S_FREE_CURRENCY_1_LABEL);

            if (vo.subInventoryViewId == SubInventoryViewId.LOOT)
            {
                headingLabel.text = "";
                collectionTabSelected.SetActive(false);
                menuPanelButtonObject.SetActive(false);
                menuPanelObject.SetActive(false);
                //infoButtonObject.SetActive(false);
                scrollerChessSkins.SetActive(false);
                avatarsScreen.SetActive(false);

                lootTabSelected.SetActive(true);
                scrollerLoot.SetActive(true);
                //LootInfoButtonObject.SetActive(true);

                CreateLootScreen(vo);
            }
            else
            {
                collectionTabSelected.SetActive(true);
                menuPanelButtonObject.SetActive(true);
                menuPanelObject.SetActive(true);
                //infoButtonObject.SetActive(true);

                lootTabSelected.SetActive(false);
                scrollerLoot.SetActive(false);
                //LootInfoButtonObject.SetActive(false);

                if(vo.subInventoryViewId == SubInventoryViewId.AVATARS)
                {
                    headingLabel.text = localizationService.Get(LocalizationKey.S_AVATARS_BORDERS_LABEL);
                    scrollerChessSkins.SetActive(false);
                    avatarsScreen.SetActive(true);
                    CreateAvatarsScreen(vo);
                }
                else if(vo.subInventoryViewId == SubInventoryViewId.CHESSSKINS)
                {
                    headingLabel.text = localizationService.Get(LocalizationKey.S_CHESS_SKINS_LABEL);
                    avatarsScreen.SetActive(false);
                    scrollerChessSkins.SetActive(true);
                    CreateChessSkinsScreen(vo);
                }
            }
        }

        public void UpdateProfilePicture(Sprite sprite)
        {
            avatarsScreenImage.sprite = sprite;
        }

        public void CreateAvatarsScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContentAvatars.transform);

            itemSelected.DOLocalMoveY(avatarsButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            avatarsIcon.color = tempColor;
           
            CreateSpacer();

            CreateAvatarsSubheading(LocalizationKey.S_AVATARS_LABEL);

            CreateSpacer();

            CreateAvatarsThumbnailGridGroup();

            foreach (KeyValuePair<string, int> itemId in vo.inventorySettings.allShopItems)
            {
                if (vo.shopSettings.allShopItems[itemId.Key].kind != GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
                {
                    continue;
                }

                AvatarShopItem item = vo.shopSettings.allShopItems[itemId.Key] as AvatarShopItem; 
               
                InventoryAvatarsThumbnail thumbnail = Instantiate<InventoryAvatarsThumbnail>(inventoryAvatarsThumbnailPrefab);

                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);

                thumbnail.nameLabel.text = item.displayName;

                if (vo.inventorySettings.activeAvatarsId == item.id)
                {
                    avatarsScreenImage.sprite = vo.playerModel.profilePicture;
                    currentAvatarsThumbnail = thumbnail;
                    thumbnail.tickIcon.SetActive(true);    
                }

                if (item.id == "AvatarFacebook")
                {
                    if (vo.playerModel.hasExternalAuth)
                    {
                        thumbnail.image.sprite = vo.playerModel.profilePictureFB;
                        LogUtil.Log("I am connected to facebook boy: " + vo.playerModel.name);
                        thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarFacebookThumbnailButtonClicked(thumbnail, item.id, vo));
                    }
                    else
                    {
                        thumbnail.image.sprite = avatarThumbsContainer.GetThumb(item.id).thumbnail;
                        LogUtil.Log("I am not connected to facebook boy boy boy : ", "yellow");
                        thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarFacebookLogInButtonClicked(thumbnail, item.id, vo));
                    }
                }
                else
                {
                    thumbnail.image.sprite = avatarThumbsContainer.GetThumb(item.id).thumbnail;
                    thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarThumbnailButtonClicked(thumbnail, item.id, vo));
                }

                thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
            }

            CreateSpacer();

            CreateAvatarsSubheading(LocalizationKey.S_BORDER_LABEL);

            CreateSpacer();

            CreateAvatarsThumbnailGridGroup();

            foreach (KeyValuePair<string, int> itemId in vo.inventorySettings.allShopItems)
            {
                if (vo.shopSettings.allShopItems[itemId.Key].kind != GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG)
                {
                    continue;
                }

                AvatarBorderShopItem item = vo.shopSettings.allShopItems[itemId.Key] as AvatarBorderShopItem;

                InventoryAvatarsBorderThumbnail thumbnail = Instantiate<InventoryAvatarsBorderThumbnail>(inventoryAvatarsBorderThumbnailPrefab);

                thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);

                thumbnail.nameLabel.text = item.displayName;

                if (vo.inventorySettings.activeAvatarsBorderId == item.id)
                {
                    avatarsScreenBorderImage.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
                    currentAvatarsBorderThumbnail = thumbnail;  
                    thumbnail.tickIcon.SetActive(true);    
                }

                thumbnail.thumbnailButton.onClick.AddListener(() => OnAvatarBorderThumbnailButtonClicked(thumbnail, item.id, vo));

                thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
            }

        }

        public void CreateChessSkinsScreen(ShopVO vo)
        {
            UnityUtil.DestroyChildren(scrollContentChessSkins.transform);

            itemSelected.DOLocalMoveY(chessSkinsButtonTransform.localPosition.y, ShopViewConstants.itemSelectedAnimationDuration)
                .SetEase(Ease.Linear);

            SetHalfOpacityOfMenuPanelIcons();

            Color tempColor = Color.white;
            tempColor.a = ShopViewConstants.fullOpacity;
            chessSkinsIcon.color = tempColor;

            CreateChessSkinsThumbnailGridGroup();

            foreach (KeyValuePair<string, int> itemId in vo.inventorySettings.allShopItems)
            {
                if (vo.shopSettings.allShopItems[itemId.Key].kind != GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                {
                    continue;
                }
                SkinShopItem item = vo.shopSettings.allShopItems[itemId.Key] as SkinShopItem; 

                InventoryChessSkinsThumbnail thumbnail = Instantiate<InventoryChessSkinsThumbnail>(inventoryChessSkinsThumbnailPrefab);

                thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                thumbnail.background.sprite = spriteCache.GetShopItemsFrameBackground(item.tier);

                thumbnail.nameLabel.text = item.displayName;

                if (vo.inventorySettings.activeChessSkinsId == item.id)
                {
                    currentChessSkinsThumbnail = thumbnail;
                    thumbnail.tickIcon.SetActive(true);    
                }

                thumbnail.thumbnailButton.onClick.AddListener(() => OnChessSkinsThumbnailButtonClicked(thumbnail, item, vo));

                thumbnail.transform.SetParent(thumbnailGridGroup.transform, false);
            }
        }

        public void CreateLootScreen(ShopVO vo)
        {
            bool isInventoryLootEmpty = true;

            UnityUtil.DestroyChildren(scrollContentLoot.transform);

            GameObject thumbnailLootGridGroup  = Instantiate(thumbnailLootGridGroupPrefab);
            thumbnailLootGridGroup.transform.SetParent(scrollContentLoot.transform, false);

            foreach (KeyValuePair<string, int> forgeCardShopItem in vo.inventorySettings.allShopItems)
            {
                if (vo.shopSettings.allShopItems[forgeCardShopItem.Key].kind != GSBackendKeys.ShopItem.FORGECARD_SHOP_TAG)
                {
                    continue;
                }

                isInventoryLootEmpty = false;

                ShopItem item = vo.shopSettings.allShopItems[vo.forgeSettingsModel.forgeItems[forgeCardShopItem.Key].forgeItemKey] as ShopItem; 

                LogUtil.Log("Key:"+ forgeCardShopItem.Key + " quantity: " + vo.inventorySettings.allShopItems[forgeCardShopItem.Key]  + " RealId: " + vo.forgeSettingsModel.forgeItems[forgeCardShopItem.Key].forgeItemKey + " Kind: " + item.kind , "yellow");

                if (item.kind == GSBackendKeys.ShopItem.AVATAR_SHOP_TAG)
                {
                    InventoryLootAvatarThumbnail thumbnail = Instantiate<InventoryLootAvatarThumbnail>(inventoryLootAvatarThumbnailPrefab);
                    thumbnail.image.sprite = avatarThumbsContainer.GetThumb(item.id).thumbnail;

                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(item.tier);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, forgeCardShopItem.Value);
                    }
                    else
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, forgeCardShopItem.Value, vo.forgeSettingsModel.forgeItems[forgeCardShopItem.Key].requiredQuantity);
                    }

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.thumbnailButton.onClick.AddListener(() => OnForgeAvatarThumbnailButtonClicked(thumbnail, item, vo, forgeCardShopItem.Key, forgeCardShopItem.Value));
                    thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
                }

                else if (item.kind == GSBackendKeys.ShopItem.AVATARBORDER_SHOP_TAG)
                {
                    InventoryLootAvatarBorderThumbnail thumbnail = Instantiate<InventoryLootAvatarBorderThumbnail>(inventoryLootAvatarBorderThumbnailPrefab);
                    thumbnail.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;

                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(item.tier);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, forgeCardShopItem.Value);
                    }
                    else
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, forgeCardShopItem.Value, vo.forgeSettingsModel.forgeItems[forgeCardShopItem.Key].requiredQuantity);
                    }

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.thumbnailButton.onClick.AddListener(() => OnForgeAvatarBorderThumbnailButtonClicked(thumbnail, item, vo, forgeCardShopItem.Key, forgeCardShopItem.Value));
                    thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
                }

                else if (item.kind == GSBackendKeys.ShopItem.SKIN_SHOP_TAG)
                {
                    InventoryLootChessSkinThumbnail thumbnail = Instantiate<InventoryLootChessSkinThumbnail>(inventoryLootChessSkinThumbnailPrefab);
                    thumbnail.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;

                    thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);
                    thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(item.tier);

                    if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, forgeCardShopItem.Value);
                    }
                    else
                    {
                        thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, forgeCardShopItem.Value, vo.forgeSettingsModel.forgeItems[forgeCardShopItem.Key].requiredQuantity);
                    }

                    thumbnail.nameLabel.text = item.displayName;
                    thumbnail.thumbnailButton.onClick.AddListener(() => OnForgeChessSkinThumbnailButtonClicked(thumbnail, item, vo, forgeCardShopItem.Key, forgeCardShopItem.Value));
                    thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
                }
            }

            IList<LootBox> lootboxTier1 = new List<LootBox>();
            IList<LootBox> lootboxTier2 = new List<LootBox>();
            IList<LootBox> lootboxTier3 = new List<LootBox>();
            IList<LootBox> lootboxTier4 = new List<LootBox>();

            foreach(LootBox item in vo.inventorySettings.lootBoxItems)
            {
                isInventoryLootEmpty = false;
                
                LogUtil.Log("Key:"+ item.key + " LootBoxKey: " + item.lootBoxKey + " coins: " + item.coins + " shopItems: " + item.shopItems, "red");

                foreach (LootShopItem shopItem in item.shopItems)
                {
                    LogUtil.Log("ShopItemThings: " + shopItem.shopItemKey + " quantity: " + shopItem.shopItemKey , "blue");
                }

                if (item.lootBoxKey == GSBackendKeys.ShopItem.LOOT_BOX_TIER_1)
                {
                    lootboxTier1.Add(item);
                }
                else if (item.lootBoxKey == GSBackendKeys.ShopItem.LOOT_BOX_TIER_2)
                {
                    lootboxTier2.Add(item);
                }
                else if (item.lootBoxKey == GSBackendKeys.ShopItem.LOOT_BOX_TIER_3)
                {
                    lootboxTier3.Add(item);
                }
                else if (item.lootBoxKey == GSBackendKeys.ShopItem.LOOT_BOX_TIER_4)
                {
                    lootboxTier4.Add(item);
                }
            }

            if (lootboxTier1.Count != 0)
            {
                InventoryLootLootBoxThumbnail thumbnail = Instantiate<InventoryLootLootBoxThumbnail>(inventoryLootLootBoxThumbnailPrefab);

                thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.nameLabel.text = lootboxTier1[0].lootBoxKey;
                thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, lootboxTier1.Count.ToString());
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.image.sprite = lootThumbsContainer.GetThumb(lootboxTier1[0].lootBoxKey).thumbnail;

                thumbnail.thumbnailButton.onClick.AddListener(() => OnInventoryLootBoxThumbnailClicked(lootboxTier1[0].key, lootboxTier1[0].lootBoxKey));

                thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
            }

            if (lootboxTier2.Count != 0)
            {
                InventoryLootLootBoxThumbnail thumbnail = Instantiate<InventoryLootLootBoxThumbnail>(inventoryLootLootBoxThumbnailPrefab);

                thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.nameLabel.text = lootboxTier2[0].lootBoxKey;
                thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, lootboxTier2.Count.ToString());
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.image.sprite = lootThumbsContainer.GetThumb(lootboxTier2[0].lootBoxKey).thumbnail;

                thumbnail.thumbnailButton.onClick.AddListener(() => OnInventoryLootBoxThumbnailClicked(lootboxTier2[0].key, lootboxTier2[0].lootBoxKey));

                thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
            }

            if (lootboxTier3.Count != 0)
            {
                InventoryLootLootBoxThumbnail thumbnail = Instantiate<InventoryLootLootBoxThumbnail>(inventoryLootLootBoxThumbnailPrefab);

                thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.nameLabel.text = lootboxTier3[0].lootBoxKey;
                thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, lootboxTier3.Count.ToString());
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.image.sprite = lootThumbsContainer.GetThumb(lootboxTier3[0].lootBoxKey).thumbnail;

                thumbnail.thumbnailButton.onClick.AddListener(() => OnInventoryLootBoxThumbnailClicked(lootboxTier3[0].key, lootboxTier3[0].lootBoxKey));

                thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
            }

            if (lootboxTier4.Count != 0)
            {
                InventoryLootLootBoxThumbnail thumbnail = Instantiate<InventoryLootLootBoxThumbnail>(inventoryLootLootBoxThumbnailPrefab);

                thumbnail.background.sprite = spriteCache.GetShopItemsFrameDoubleBackground(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.nameLabel.text = lootboxTier4[0].lootBoxKey;
                thumbnail.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, lootboxTier4.Count.ToString());
                thumbnail.frame.sprite = spriteCache.GetShopItemsFrame(GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON);
                thumbnail.image.sprite = lootThumbsContainer.GetThumb(lootboxTier4[0].lootBoxKey).thumbnail;

                thumbnail.thumbnailButton.onClick.AddListener(() => OnInventoryLootBoxThumbnailClicked(lootboxTier4[0].key, lootboxTier4[0].lootBoxKey));

                thumbnail.transform.SetParent(thumbnailLootGridGroup.transform, false);
            }

            if (isInventoryLootEmpty)
            {
                emptyLootScreenTextObject.SetActive(true);
                emptyLootScreenLabel.text = localizationService.Get(LocalizationKey.I_EMPTY_LOOT_SCREEN_LABEL);
            }

            closeMenuPanel();
        }

        public void OnForgeAvatarThumbnailButtonClicked(InventoryLootAvatarThumbnail thumbnail, ShopItem item, ShopVO vo, string forgeCardKey, int forgeCardQuantity)
        {
            
            LogUtil.Log("forgeAvatarThumbnail in infoPanel ID: " + item.id , "red");
            DestroyInfoPanel();

            infoPanelOverlayObject.SetActive(true);

            currentLootAvatarThumbnailClicked = thumbnail;

            currentLootInfoAvatarPanel = Instantiate<LootInfoAvatarPanel>(lootInfoAvatarPanel);

            currentLootInfoAvatarPanel.image.sprite = avatarThumbsContainer.GetThumb(item.id).thumbnail;
            currentLootInfoAvatarPanel.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);

            currentLootInfoAvatarPanel.nameLabel.text = item.displayName;

            if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
            {
                currentLootInfoAvatarPanel.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, vo.inventorySettings.allShopItems[forgeCardKey]);
                currentLootInfoAvatarPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_INFO_LABEL);
                currentLootInfoAvatarPanel.infoButton.onClick.AddListener(() => OnLootInfoButtonClicked(item.id, forgeCardKey, vo));
            }
            else
            {
                currentLootInfoAvatarPanel.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, vo.inventorySettings.allShopItems[forgeCardKey], vo.forgeSettingsModel.forgeItems[forgeCardKey].requiredQuantity);
                currentLootInfoAvatarPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
                currentLootInfoAvatarPanel.infoButton.onClick.AddListener(() => OnLootBuildButtonClicked(item.id, forgeCardKey, vo));
            }

            currentLootInfoAvatarPanel.dismantleOrBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_DISMANTLE_BUTTON_LABEL);

            currentLootInfoAvatarPanel.thumbnailButton.onClick.AddListener(() => DestroyInfoPanel());
            currentLootInfoAvatarPanel.dismantleButton.onClick.AddListener(() => OnLootDismantleButtonClicked(item.id, forgeCardKey, vo));

            currentLootInfoAvatarPanel.transform.SetParent(infoPanelParent, false);
        }

        public void OnForgeAvatarBorderThumbnailButtonClicked(InventoryLootAvatarBorderThumbnail thumbnail, ShopItem item, ShopVO vo, string forgeCardKey, int forgeCardQuantity)
        {
            LogUtil.Log("forgeAvatarThumbnail in infoPanel ID: " + item.id , "red");
            DestroyInfoPanel();

            infoPanelOverlayObject.SetActive(true);

            currentLootAvatarBorderThumbnailClicked = thumbnail;

            currentLootInfoAvatarBorderPanel = Instantiate<LootInfoAvatarBorderPanel>(lootInfoAvatarBorderPanel);

            currentLootInfoAvatarBorderPanel.image.sprite = avatarBorderThumbsContainer.GetThumb(item.id).thumbnail;
            currentLootInfoAvatarBorderPanel.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);

            currentLootInfoAvatarBorderPanel.nameLabel.text = item.displayName;

            if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
            {
                currentLootInfoAvatarBorderPanel.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, vo.inventorySettings.allShopItems[forgeCardKey]);
                currentLootInfoAvatarBorderPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_INFO_LABEL);
                currentLootInfoAvatarBorderPanel.infoButton.onClick.AddListener(() => OnLootInfoButtonClicked(item.id, forgeCardKey, vo));
            }
            else
            {
                currentLootInfoAvatarBorderPanel.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, vo.inventorySettings.allShopItems[forgeCardKey], vo.forgeSettingsModel.forgeItems[forgeCardKey].requiredQuantity);
                currentLootInfoAvatarBorderPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
                currentLootInfoAvatarBorderPanel.infoButton.onClick.AddListener(() => OnLootBuildButtonClicked(item.id, forgeCardKey, vo));
            }

            currentLootInfoAvatarBorderPanel.dismantleOrBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_DISMANTLE_BUTTON_LABEL);

            currentLootInfoAvatarBorderPanel.thumbnailButton.onClick.AddListener(() => DestroyInfoPanel());
            currentLootInfoAvatarBorderPanel.dismantleButton.onClick.AddListener(() => OnLootDismantleButtonClicked(item.id, forgeCardKey, vo));

            currentLootInfoAvatarBorderPanel.transform.SetParent(infoPanelParent, false);
        }

        public void OnForgeChessSkinThumbnailButtonClicked(InventoryLootChessSkinThumbnail thumbnail, ShopItem item, ShopVO vo, string forgeCardKey, int forgeCardQuantity)
        {
            LogUtil.Log("forgeAvatarThumbnail in infoPanel ID: " + item.id , "red");
            DestroyInfoPanel();

            infoPanelOverlayObject.SetActive(true);

            currentLootChessSkinThumbnailClicked = thumbnail;

            currentLootInfoChessSkinPanel = Instantiate<LootInfoChessSkinPanel>(lootInfoChessSkinPanel);

            currentLootInfoChessSkinPanel.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
            currentLootInfoChessSkinPanel.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);

            currentLootInfoChessSkinPanel.nameLabel.text = item.displayName;

            if (vo.inventorySettings.allShopItems.ContainsKey(item.id))
            {
                currentLootInfoChessSkinPanel.cardNo.text = localizationService.Get(LocalizationKey.I_X_LABEL, vo.inventorySettings.allShopItems[forgeCardKey]);
                currentLootInfoChessSkinPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_INFO_LABEL);
                currentLootInfoChessSkinPanel.infoButton.onClick.AddListener(() => OnLootInfoButtonClicked(item.id, forgeCardKey, vo));
            }
            else
            {
                currentLootInfoChessSkinPanel.cardNo.text = localizationService.Get(LocalizationKey.I_DIVIDER_LABEL, vo.inventorySettings.allShopItems[forgeCardKey], vo.forgeSettingsModel.forgeItems[forgeCardKey].requiredQuantity);
                currentLootInfoChessSkinPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_BUILD_BUTTON_LABEL);
                currentLootInfoChessSkinPanel.infoButton.onClick.AddListener(() => OnLootBuildButtonClicked(item.id, forgeCardKey, vo)); 
            }

            currentLootInfoChessSkinPanel.dismantleOrBuildButtonLabel.text = localizationService.Get(LocalizationKey.I_DISMANTLE_BUTTON_LABEL);

            currentLootInfoChessSkinPanel.thumbnailButton.onClick.AddListener(() => DestroyInfoPanel());
            currentLootInfoChessSkinPanel.dismantleButton.onClick.AddListener(() => OnLootDismantleButtonClicked(item.id, forgeCardKey, vo));

            currentLootInfoChessSkinPanel.transform.SetParent(infoPanelParent, false);
        }

        public void OnInventoryLootBoxThumbnailClicked(string purchasingId, string id)
        {
            LogUtil.Log("Loot box PurchasingId: " + purchasingId + " Id: " + id , "red");

            lootBoxThumbnailClickedSignal.Dispatch(purchasingId);
        }

        public void CreateAvatarsSubheading(string headingLabel)
        {
            ScrollerSubheading scrollerSubheading = Instantiate<ScrollerSubheading>(scrollerSubheadingPrefab);
            scrollerSubheading.subheadingLabel.text = localizationService.Get(headingLabel);
            scrollerSubheading.transform.SetParent(scrollContentAvatars.transform, false);
        }

        public void CreateAvatarsThumbnailGridGroup()
        {
            thumbnailGridGroup = Instantiate(thumbnailGridGroupPrefab);
            thumbnailGridGroup.transform.SetParent(scrollContentAvatars.transform, false);
        }

        public void CreateChessSkinsThumbnailGridGroup()
        {
            thumbnailGridGroup = Instantiate(thumbnailGridGroupPrefab);
            thumbnailGridGroup.transform.SetParent(scrollContentChessSkins.transform, false);
        }

        public void CreateSpacer()
        {
            GameObject spacer = Instantiate(scrollerSpacerPrefab);
            spacer.transform.SetParent(scrollContentAvatars.transform, false);
        }

        public void OnAvatarThumbnailButtonClicked(InventoryAvatarsThumbnail thumbnail, string id,ShopVO vo)
        {
            LogUtil.Log("The inventory simple avatar thumbnail ID is: " + id, "yellow");

            if (currentAvatarsThumbnail != null)
            {
                currentAvatarsThumbnail.tickIcon.SetActive(false);
            }

            currentAvatarsThumbnail = thumbnail;
            currentAvatarsThumbnail.tickIcon.SetActive(true);

            avatarsScreenImage.sprite = avatarThumbsContainer.GetThumb(id).thumbnail;
            vo.inventorySettings.activeAvatarsId = id;
        }

        public void OnAvatarFacebookThumbnailButtonClicked(InventoryAvatarsThumbnail thumbnail, string id,ShopVO vo)
        {
            LogUtil.Log("The inventory Facebook avatar thumbnail ID is: " + id, "yellow");

            if (currentAvatarsThumbnail != null)
            {
                currentAvatarsThumbnail.tickIcon.SetActive(false);
            }

            currentAvatarsThumbnail = thumbnail;
            currentAvatarsThumbnail.tickIcon.SetActive(true);

            avatarsScreenImage.sprite = avatarThumbsContainer.GetThumb(id).thumbnail;
            vo.inventorySettings.activeAvatarsId = id;
        }


        public void OnAvatarFacebookLogInButtonClicked(InventoryAvatarsThumbnail thumbnail, string id,ShopVO vo)
        {
            LogUtil.Log("LoginIn........ The inventory Facebook avatar thumbnail ID is: " + id, "yellow");
            facebookAvatarLogInButtonClicked.Dispatch();
        }

        public void OnAvatarBorderThumbnailButtonClicked(InventoryAvatarsBorderThumbnail thumbnail, string id,ShopVO vo)
        {
            if(currentAvatarsBorderThumbnail != null)
            {
                currentAvatarsBorderThumbnail.tickIcon.SetActive(false);
            }

            currentAvatarsBorderThumbnail = thumbnail;
            currentAvatarsBorderThumbnail.tickIcon.SetActive(true);

            avatarsScreenBorderImage.sprite = avatarBorderThumbsContainer.GetThumb(id).thumbnail;
            vo.inventorySettings.activeAvatarsBorderId = id;
        }

        public void OnChessSkinsThumbnailButtonClicked(InventoryChessSkinsThumbnail thumbnail, SkinShopItem item, ShopVO vo)
        {
            DestroyInfoPanel();

            infoPanelOverlayObject.SetActive(true);

            currentChessSkinsThumbnailClicked = thumbnail;

            if (vo.inventorySettings.activeChessSkinsId != item.id)
            {
                currentInfoEquipPanel = Instantiate<InfoEquipPanel>(infoEquipPanelPrefab);

                currentInfoEquipPanel.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                currentInfoEquipPanel.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);

                currentInfoEquipPanel.nameLabel.text = item.displayName;
                currentInfoEquipPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_INFO_LABEL);
                currentInfoEquipPanel.equipButtonLabel.text = localizationService.Get(LocalizationKey.I_EQUIP_LABEL);

                currentInfoEquipPanel.thumbnailButton.onClick.AddListener(() => DestroyInfoPanel());
                currentInfoEquipPanel.infoButton.onClick.AddListener(() => OnChessSkinsThumbnailInfoButtonClicked(item.id, vo));
                currentInfoEquipPanel.equipButton.onClick.AddListener(() => OnChessSkinsThumbnailEquipButtonClicked(thumbnail, item.id, vo));

                currentInfoEquipPanel.transform.SetParent(infoPanelParent, false);
            }
            else if(vo.inventorySettings.activeChessSkinsId == item.id)
            {
                currentInfoPanel = Instantiate<InfoPanel>(infoPanelPrefab);

                currentInfoPanel.image.sprite = skinThumbsContainer.GetThumb(item.id).thumbnail;
                currentInfoPanel.frame.sprite = spriteCache.GetShopItemsFrame(item.tier);

                currentInfoPanel.nameLabel.text = item.displayName;
                currentInfoPanel.infoButtonLabel.text = localizationService.Get(LocalizationKey.I_INFO_LABEL);

                currentInfoPanel.tickIcon.SetActive(true);

                currentInfoPanel.thumbnailButton.onClick.AddListener(() => DestroyInfoPanel());
                currentInfoPanel.infoButton.onClick.AddListener(() => OnChessSkinsThumbnailInfoButtonClicked(item.id, vo));

                currentInfoPanel.transform.SetParent(infoPanelParent, false);
            }
        }

        public void DestroyInfoPanel()
        {
            currentInfoEquipPanel = null;
            currentInfoPanel = null;
            UnityUtil.DestroyChildren(infoPanelParent);
            infoPanelOverlayObject.SetActive(false);
        }

        public void OnLootBuildButtonClicked(string id, string forgeCardKey, ShopVO vo)
        {
            LogUtil.Log("On Loot Build button clicked Id: " + id, "red");
            lootForgeCardInfoButtonClickedSignal.Dispatch(id, forgeCardKey, vo);
            DestroyInfoPanel();
        }

        public void OnLootInfoButtonClicked(string id, string forgeCardKey, ShopVO vo)
        {
            LogUtil.Log("On Loot Info button clicked boy", "red");
            lootForgeCardInfoButtonClickedSignal.Dispatch(id, forgeCardKey, vo);
            DestroyInfoPanel();
        }

        public void OnLootDismantleButtonClicked(string id, string forgeCardKey, ShopVO vo)
        {
            LogUtil.Log("On Loot dismantle button clicked boy", "red");
            lootForgeCardDismantleButtonClickedSignal.Dispatch(id, forgeCardKey, vo);
            DestroyInfoPanel();
        }

        public void OnChessSkinsThumbnailInfoButtonClicked(string id, ShopVO vo)
        {
            chessSkinsThumbnailInfoButtonClickedSignal.Dispatch(id, vo);
            DestroyInfoPanel();
        }

        public void OnChessSkinsThumbnailEquipButtonClicked(InventoryChessSkinsThumbnail thumbnail, string id,ShopVO vo)
        {
            if (currentChessSkinsThumbnail != null)
            {
                currentChessSkinsThumbnail.tickIcon.SetActive(false);
            }

            currentChessSkinsThumbnail = thumbnail;
            currentChessSkinsThumbnail.tickIcon.SetActive(true);
            vo.inventorySettings.activeChessSkinsId = id;

            DestroyInfoPanel();
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

        private void OnLootButtonClicked()
        {
            lootClickedSignal.Dispatch();
        }

        private void OnCollectionButtonClicked()
        {
            collectionClickedSignal.Dispatch();
        }

        private void OnInfoButtonClicked()
        {
            //pruned
        }

        private void OnAvatarsButtonClicked()
        {
            avatarsButtonClickedSignal.Dispatch();
        }

        private void OnChessSkinsButtonClicked()
        {
            chessSkinsClickedSignal.Dispatch();
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

            avatarsIcon.color = tempColorFullOpacity;
            chessSkinsIcon.color = tempColorFullOpacity;
        }

        private void DisableOverlay()
        {
            overlay.SetActive(false);
        }
    }
}
