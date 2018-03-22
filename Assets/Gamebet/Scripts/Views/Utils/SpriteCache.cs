/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-10 11:36:10 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class SpriteCache : MonoBehaviour
    {
        [System.Serializable]
        private class _LeagueBadges
        {
            #pragma warning disable 0649

            [SerializeField] private Sprite iron, bronze, silver, gold, crystal,
            onyx, emerald, champion, royal, legend;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> sprites;

            public void Init()
            {
                sprites = new Dictionary<string, Sprite>() {
                    { LocalizationKey.LEAGUE_1, iron },
                    { LocalizationKey.LEAGUE_2, bronze },
                    { LocalizationKey.LEAGUE_3, silver },
                    { LocalizationKey.LEAGUE_4, gold },
                    { LocalizationKey.LEAGUE_5, crystal },
                    { LocalizationKey.LEAGUE_6, onyx },
                    { LocalizationKey.LEAGUE_7, emerald },
                    { LocalizationKey.LEAGUE_8, champion },
                    { LocalizationKey.LEAGUE_9, royal },
                    { LocalizationKey.LEAGUE_10, legend }
                };
            }

            public Sprite Get(string leagueId)
            {
                Assertions.Assert(sprites.ContainsKey(leagueId), "_LeagueBadges doesn't contain a sprite for league ID: " + leagueId);
                return sprites[leagueId];
            }
        }

        [System.Serializable]
        private class _RoomSprites
        {
            #pragma warning disable 0649

            [SerializeField] private Sprite bullet, blitz, classic, allInOne, cuba, england, india, germany,
            spain, france, hungary, ukraine, china, usa, russia;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> sprites;

            public void Init()
            {
                sprites = new Dictionary<string, Sprite>() {

                    { LocalizationKey.BULLET, bullet },
                    { LocalizationKey.BLITZ, blitz },
                    { LocalizationKey.CLASSIC, classic },

                    { LocalizationKey.ALLINONE, allInOne },

                    { LocalizationKey.CUBA, cuba },
                    { LocalizationKey.ENGLAND, england },
                    { LocalizationKey.INDIA, india },
                    { LocalizationKey.GERMANY, germany },
                    { LocalizationKey.SPAIN, spain },
                    { LocalizationKey.FRANCE, france },
                    { LocalizationKey.HUNGARY, hungary },
                    { LocalizationKey.UKRAINE, ukraine },
                    { LocalizationKey.CHINA, china },
                    { LocalizationKey.USA, usa },
                    { LocalizationKey.RUSSIA, russia },
                };
            }

            public Sprite Get(string roomId)
            {
                Assertions.Assert(sprites.ContainsKey(roomId), "_RoomSprites doesn't contain a sprite for room ID: " + roomId);
                return sprites[roomId];
            }
        }

        [System.Serializable]
        private class _RotatingRoomSprites
        {
            #pragma warning disable 0649

            [SerializeField] private Sprite bullet, blitz, classic;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> sprites;

            public void Init()
            {
                sprites = new Dictionary<string, Sprite>() {

                    { LocalizationKey.BULLET, bullet },
                    { LocalizationKey.BLITZ, blitz },
                    { LocalizationKey.CLASSIC, classic },
                };
            }

            public Sprite Get(string roomId)
            {
                Assertions.Assert(sprites.ContainsKey(roomId), "_RoomSprites doesn't contain a sprite for room ID: " + roomId);
                return sprites[roomId];
            }
        }

        [System.Serializable]
        private class _ShopItemsBackground
        {
            #pragma warning disable 0649

            [SerializeField] private Sprite common, rare, epic, legendary;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> sprites;

            public void Init()
            {
                sprites = new Dictionary<string, Sprite>() {

                    { GSBackendKeys.ShopItem.SHOP_ITEM_TIER_COMMON, common },
                    { GSBackendKeys.ShopItem.SHOP_ITEM_TIER_RARE, rare },
                    { GSBackendKeys.ShopItem.SHOP_ITEM_TIER_EPIC, epic },
                    { GSBackendKeys.ShopItem.SHOP_ITEM_TIER_LEGENDARY, legendary },
                };
            }

            public Sprite Get(string id)
            {
                Assertions.Assert(sprites.ContainsKey(id), "Shop items doesn't contain a sprite for room ID: " + id);
                return sprites[id];
            }
        }

        [System.Serializable]
        private class _ShopCurrency
        {
            #pragma warning disable 0649

            [SerializeField] private Sprite bucks, coins;

            #pragma warning restore 0649

            private IDictionary<string, Sprite> sprites;

            public void Init()
            {
                sprites = new Dictionary<string, Sprite>() {

                    { LocalizationKey.BUCKS, bucks },
                    { LocalizationKey.COINS, coins },
                };
            }

            public Sprite Get(string id)
            {
                Assertions.Assert(sprites.ContainsKey(id), "Shop items doesn't contain a sprite for room ID: " + id);
                return sprites[id];
            }
        }

        [SerializeField] private CountryFlagSpriteCache countryFlagSpriteCache;
        [SerializeField] private _LeagueBadges leagueBadges;
        [SerializeField] private _RoomSprites roomFlagsMinor;
        [SerializeField] private _RoomSprites roomFlagsMajor;
        [SerializeField] private _RoomSprites roomBanners;

        [SerializeField] private _RoomSprites roomCardBackgrounds;
        [SerializeField] private _RoomSprites roomCardBanners;
        [SerializeField] private _RoomSprites roomCardInfoBg;

        [SerializeField] private _RotatingRoomSprites rotatingRoomName;
        [SerializeField] private _RotatingRoomSprites rotatingRoomGoalsButtonBg;

        [SerializeField] private _ShopItemsBackground shopItemsFrameBackground;
        [SerializeField] private _ShopItemsBackground shopItemsFrameDoubleBackground;
        [SerializeField] private _ShopItemsBackground shopItemsFrame;
        [SerializeField] private _ShopItemsBackground shopItemsHeadingRightSeperator;
        [SerializeField] private _ShopItemsBackground shopItemsHeadingLeftSeperator;
        [SerializeField] private _ShopCurrency shopCurrencyItemsFrame;
        [SerializeField] private _ShopCurrency shopCurrencyItemsFrameBackground;

        public Sprite GetCountryFlag(string countryId)
        {
            return countryFlagSpriteCache.GetSquare(countryId);
        }

        public Sprite GetLeagueBadge(string leagueId)
        {
            return leagueBadges.Get(leagueId);
        }

        public Sprite GetRoomFlagMinor(string roomId)
        {
            return roomFlagsMinor.Get(roomId);
        }

        public Sprite GetRoomFlagMajor(string roomId)
        {
            return roomFlagsMajor.Get(roomId);
        }

        public Sprite GetRoomBanner(string roomId)
        {
            return roomBanners.Get(roomId);
        }

        public Sprite GetRoomCardBackground(string roomId)
        {
            return roomCardBackgrounds.Get(roomId);
        }

        public Sprite GetRoomCardBanner(string roomId)
        {
            return roomCardBanners.Get(roomId);
        }

        public Sprite GetRoomCardInfoBg(string roomId)
        {
            return roomCardInfoBg.Get(roomId);
        }

        public Sprite GetRotatingRoomName(string roomId)
        {
            return rotatingRoomName.Get(roomId);
        }

        public Sprite GetRotatingRoomGoalsButtonBg(string roomId)
        {
            return rotatingRoomGoalsButtonBg.Get(roomId);
        }

        public Sprite GetShopItemsFrame(string id)
        {
            return shopItemsFrame.Get(id);
        }

        public Sprite GetShopItemsFrameBackground(string id)
        {
            return shopItemsFrameBackground.Get(id);
        }

        public Sprite GetShopItemsFrameDoubleBackground(string id)
        {
            return shopItemsFrameDoubleBackground.Get(id);
        }

        public Sprite GetShopItemsHeadingRightSeperator(string id)
        {
            return shopItemsHeadingRightSeperator.Get(id);
        }

        public Sprite GetShopItemsHeadingLeftSeperator(string id)
        {
            return shopItemsHeadingLeftSeperator.Get(id);
        }

        public Sprite GetShopCurrencyItemsFrame(string id)
        {
            return shopCurrencyItemsFrame.Get(id);
        }

        public Sprite GetShopCurrencyItemsFrameBackground(string id)
        {
            return shopCurrencyItemsFrameBackground.Get(id);
        }

        void Awake()
        {
            leagueBadges.Init();
            roomFlagsMinor.Init();
            roomFlagsMajor.Init();
            roomBanners.Init();

            roomCardBackgrounds.Init();
            roomCardBanners.Init();
            roomCardInfoBg.Init();

            rotatingRoomName.Init();
            rotatingRoomGoalsButtonBg.Init();

            shopItemsFrame.Init();
            shopItemsFrameBackground.Init();
            shopItemsFrameDoubleBackground.Init();
            shopItemsHeadingRightSeperator.Init();
            shopItemsHeadingLeftSeperator.Init();

            shopCurrencyItemsFrame.Init();
            shopCurrencyItemsFrameBackground.Init();
        }
    }
}
