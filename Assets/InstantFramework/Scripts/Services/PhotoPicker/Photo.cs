/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;

namespace TurboLabz.InstantFramework
{ 
    public class PhotoVO
    {
        public Sprite sprite { get; set; }
        public byte[] stream { get; set; }

        public PhotoVO(Sprite photoSprite, byte[] photoStream)
        {
            this.sprite = photoSprite;
            this.stream = photoStream;
        }
    }
}
