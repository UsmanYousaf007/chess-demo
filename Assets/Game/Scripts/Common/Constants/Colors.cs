/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:36:46 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace TurboLabz.InstantGame
{
    public static class Colors
    {
        public static readonly Color YELLOW = new Color(235f/255f, 168f/255f, 61f/255f, 1f);
        public static readonly Color RED = new Color(209f/255f, 43f/255f, 43f/255f, 1f);
        public static readonly Color GREEN = new Color(92f/255f, 186f/255f, 45f/255f, 1f);
        public static readonly Color WHITE = new Color(1f, 1f, 1f, 222f/255f);
        public static readonly Color WHITE_150 = new Color(1f, 1f, 1f, 150f/255f);
        public static readonly Color WHITE_100 = new Color(1f, 1f, 1f, 100f / 255f);
        public static readonly Color YELLOW_128 = new Color(235f / 255f, 168f / 255f, 61f / 255f, 128f/255f);
        public static readonly Color RED_128 = new Color(209f / 255f, 43f / 255f, 43f / 255f, 128f/255f);
        public static readonly Color YELLOW_DIM = new Color(223f / 255f, 169f / 255f, 88f / 255f, 1f);
        public static readonly Color RED_DIM = new Color(193f / 255f, 78f / 255f, 76f / 255f, 1f);
        public static readonly Color GREEN_DIM = new Color(148f / 255f, 207f / 255f, 96f / 255f, 1f);
        public static readonly Color BLACK = new Color(0f, 0f, 0f, 1f);
        public static readonly Color BLACK_DIM = new Color(0f, 0f, 0f, 50f/255f);
        public static readonly Color GLASS_GREEN = new Color(198f / 255f, 1f, 74f / 255f);
        public static readonly Color GOLD = new Color(235f / 255f, 168f / 255f, 61f / 255f, 1f);
        public static readonly Color SILVER = new Color(183f / 255f, 183f / 255f, 183f / 255f, 1f);
        public static readonly Color BRONZE = new Color(151f / 255f, 97f / 255f, 65f / 255f, 1f);
        public static readonly Color RED_LIGHT = new Color(212f / 255f, 68f / 255f, 68f / 255f, 1f);
        public static readonly Color GREEN_LIGHT = new Color(126f / 255f, 212f / 255f, 68f / 255f, 1f);


        public static readonly Color DISABLED_WHITE = new Color(1f, 1f, 1f, 50f/255f);
        public static readonly Color TRANSPARENT = new Color(1f, 1f, 1f, 0f);
        public static readonly Color PLAYER_MESSAGE = new Color(215f/255f, 247f/255f, 191f/255f);
        public static readonly Color DISABLED_BUTTON = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);

        public const float DISABLED_TEXT_ALPHA = 128f/255f;
        public const float ENABLED_TEXT_ALPHA = 222f/255f;
        public const float FULL_ALPHA = 1f;

        public const float UI_BLOCKER_INVISIBLE_ALPHA = 1f / 255f;
        public const float UI_BLOCKER_LIGHT_ALPHA = 50f / 255f;
        public const float UI_BLOCKER_DARK_ALPHA = 150f / 255f;

        public const float BLUR_BG_BRIGHTNESS_NORMAL = 150f / 255f;
        public const float BLUR_BG_BRIGHTNESS_DARK = 50f / 255f;

        public static Color ColorAlpha(Color c, float a) 
		{
			return new Color (c.r, c.g, c.b, a);
		}

        public static string GetColorString(string text, Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#");
            sb.Append(ColorUtility.ToHtmlStringRGBA(color));
            sb.Append(">");
            sb.Append(text);
            sb.Append("</color>");
            return sb.ToString();
        }

        public static Color Color(string hexColor)
        {
            if (!hexColor.Contains("#"))
            {
                hexColor = "#" + hexColor;
            }
            if (hexColor.Length <= 7)
            {
                hexColor = hexColor +"FF";
            }

            Color newCol;
            if (ColorUtility.TryParseHtmlString(hexColor, out newCol))
            {
                return newCol;
            }
            return UnityEngine.Color.white;
        }

        public static Color strengthBarDisableColor = Color("3c3c3c");
        public static Color[] strengthBarColor =
                          {
                           Color("df0000"),
                           Color("f21c00"),
                           Color("ff4200"),
                           Color("ff5100"),
                           Color("ff8400"),
                           Color("ffae00"),
                           Color("f2cd00"),
                           Color("95b535"),
                           Color("7be121"),
                           Color("3cd672"),
                          };
    }
}
