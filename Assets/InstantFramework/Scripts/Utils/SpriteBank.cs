/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public static class SpriteBank
    {
        public static SpritesContainer container;

        static SpriteBank()
        {
            container = SpritesContainer.Load("SpriteBank");
        }
    }
}