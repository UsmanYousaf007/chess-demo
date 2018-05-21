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
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class SkinLink : View
    {
        public Image sourceImage;

        Image targetImage;
        SpriteRenderer targetSpriteRenderer;

        public void UpdateSkin()
        {
            if (gameObject.name == "TagStandardBtn")
            {
                sourceImage = GameObject.FindWithTag("StandardBtn").GetComponent<Image>();
            }

            string[] tokens = sourceImage.sprite.name.Split(',');
            float alpha = 1f;

            if (tokens.Length > 1)
            {
                alpha = float.Parse(tokens[1])/100f;
            }

            if (targetImage != null)
            {
                targetImage.sprite = sourceImage.sprite;

                Color tmp = targetImage.color;
                tmp.a = alpha;
                targetImage.color = tmp;
            }
            else if (targetSpriteRenderer != null) 
            {
                targetSpriteRenderer.sprite = sourceImage.sprite;
            }
        }

        protected override void Awake()
        {
            targetImage = gameObject.GetComponent<Image>();
            targetSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        protected override void OnEnable()
        {
            UpdateSkin();
        }
    }
}