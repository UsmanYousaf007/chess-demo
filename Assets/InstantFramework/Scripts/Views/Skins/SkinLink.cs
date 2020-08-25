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
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class SkinLink : View
    {
        public Image sourceImage;

        Image targetImage;
        SpriteRenderer targetSpriteRenderer;
        List<SkinLink> clonedSkinLinks = new List<SkinLink>();

        [PostConstruct]
        public void Initialize()
        {
            Setup();
        }

        public void InitPrefabSkin()
        {
            Setup();
            sourceImage = GameObject.FindWithTag(tag.Split('.')[0]).GetComponent<Image>();
            UpdateSkin();
        }

        public void UpdateSkin()
        {
            //Assertions.Assert(sourceImage != null, "Source Image Not Set For Skin Link: " + gameObject.name);

            if (sourceImage == null || sourceImage.sprite == null)
            {
                return;
            }

            // Apply alpha if required
            string[] tokens = sourceImage.sprite.name.Split(',');
            float alpha = 1f;

            if (tokens.Length > 1)
            {
                alpha = float.Parse(tokens[1])/100f;
            }

            // Copy the skin sprite onto the target sprite
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

            foreach(SkinLink link in clonedSkinLinks)
            {
                link.UpdateSkin();
            }
        }

        public void AddClone(SkinLink link)
        {
            clonedSkinLinks.Add(link);
        }

        void Setup()
        {
            targetImage = gameObject.GetComponent<Image>();
            targetSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        
    }
}