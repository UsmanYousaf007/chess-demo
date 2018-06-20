/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:36:57 UTC+05:00
/// 
/// @description
/// [add_description_here]
/// 

using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class SkinRefs : View 
    {
        private string currentSkinId;

        public void ApplySkin(string skinId)
        {
            if (skinId == currentSkinId) return;
            currentSkinId = skinId;

            SkinContainer container = SkinContainer.LoadSkin(skinId);

            foreach (Transform child in transform)
            {
                Image img = child.GetComponent<Image>();
                img.sprite = container.GetSprite(child.name);
            }
        }
    }
}
