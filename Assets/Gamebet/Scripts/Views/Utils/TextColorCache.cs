/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Irtaza Mumtaz <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-18 10:17:45 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TurboLabz.Common;


namespace TurboLabz.Gamebet
{
    public class TextColorCache : MonoBehaviour
    {
        [System.Serializable]
        private class _RotatingRoomsTextColor
        {
            #pragma warning disable 0649

            [SerializeField] private Color bullet, blitz, classic;

            #pragma warning restore 0649

            private IDictionary<string, Color> colors;

            public void Init()
            {
                colors = new Dictionary<string, Color>() {
                    { LocalizationKey.BULLET, bullet },
                    { LocalizationKey.BLITZ, blitz },
                    { LocalizationKey.CLASSIC, classic }
                };
            }

            public Color Get(string id)
            {
                Assertions.Assert(colors.ContainsKey(id), "_LeagueBadges doesn't contain a sprite for league ID: " + id);
                return colors[id];
            }
        }

        [System.Serializable]
        private class _ShopItems
        {
            #pragma warning disable 0649

            [SerializeField] private Color common, rare, epic, legendary;

            #pragma warning restore 0649

            private IDictionary<string, Color> colors;

            public void Init()
            {
                colors = new Dictionary<string, Color>() {
                    
                    { LocalizationKey.COMMON, common },
                    { LocalizationKey.RARE, rare },
                    { LocalizationKey.EPIC, epic },
                    { LocalizationKey.LEGENDARY, legendary },
                };
            }

            public Color Get(string id)
            {
                Assertions.Assert(colors.ContainsKey(id), "Shop text heading doesn't contain a sprite for league ID: " + id);
                return colors[id];
            }
        }

        [SerializeField] private _RotatingRoomsTextColor rotatingRoomsTextColor;

        [SerializeField] private _ShopItems shopItemsHeadingSeperatorColor;

        public Color GetProgressFieldColor(string id)
        {
            return rotatingRoomsTextColor.Get(id);
        }

        public Color GetShopItemsHeadingSeperatorColor(string roomId)
        {
            return shopItemsHeadingSeperatorColor.Get(roomId);
        }

        void Awake()
        {
            rotatingRoomsTextColor.Init();

            shopItemsHeadingSeperatorColor.Init();
        }
    }
}
