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

namespace TurboLabz.InstantGame
{
    public static class Colors
    {
        public static readonly Color YELLOW = new Color(235f/255f, 168f/255f, 61f/255f, 1f);
        public static readonly Color RED = new Color(209f/255f, 43f/255f, 43f/255f, 1f);
        public static readonly Color GREEN = new Color(92f/255f, 186f/255f, 45f/255f, 1f);
        public static readonly Color WHITE = new Color(1f, 1f, 1f, 222f/255f);
        public static readonly Color DISABLED_WHITE = new Color(1f, 1f, 1f, 50f/255f);
        public const float DISABLED_TEXT_ALPHA = 128f/255f;
        public const float ENABLED_TEXT_ALPHA = 222f/255f;

		public static Color ColorAlpha(Color c, float a) 
		{
			return new Color (c.a, c.g, c.b, a);
		}

        /*
         *         private readonly Color redColor = new Color(0.82f, 0.18f, 0.18f);
        private readonly Color yellowColor = new Color(0.98f, 0.66f, 0.15f);
        private readonly Color greenColor = new Color(0.04f, 0.6f, 0.4f);*/
    }
}
