/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.mediation.impl;
using UnityEngine.UI;
using UnityEngine;

namespace TurboLabz.InstantChess
{
    public class SkinLink : View
    {
        public Image sourceImage;

        Image targetImage;
        SpriteRenderer targetSpriteRenderer;

        public void UpdateSkin()
        {
            if (targetImage != null)
            {
                targetImage.sprite = sourceImage.sprite;
            }
            else if (targetSpriteRenderer != null) 
            {
                targetSpriteRenderer.sprite = sourceImage.sprite;
            }
        }

        void Awake()
        {
            targetImage = gameObject.GetComponent<Image>();
            targetSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            UpdateSkin();
        }
    }
}