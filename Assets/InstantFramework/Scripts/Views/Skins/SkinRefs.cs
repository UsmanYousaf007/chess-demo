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
using System;

namespace TurboLabz.InstantFramework
{
    public class SkinRefs : View 
    {
        [Inject] public IDownloadablesService downloadablesService { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }

        public Signal refreshSkinLinksSignal = new Signal();

        private string currentSkinId;

        public void LoadSkin(string skinId)
        {
            if (skinId == currentSkinId) 
            {
                return;
            }
            if (skinId == "SkinSlate")
            {
             
                currentSkinId = skinId;

                SkinContainer container = SkinContainer.LoadSkin(skinId);

                foreach (Transform child in transform)
                {
                    Image img = child.GetComponent<Image>();
                    img.sprite = container.GetSprite(child.name);
                }
                refreshSkinLinksSignal.Dispatch();
            }
            else
            {
                
                TLUtils.LogUtil.Log("SkinRefs - requesting download SKN_AMZ", "cyan");
                currentSkinId = skinId;

                downloadablesService.GetDownloadableContent("SKN_AMZ", OnSkinBundleLoaded);
            }
        }

        public void OnSkinBundleLoaded(BackendResult result, AssetBundle bundle)
        {
            TLUtils.LogUtil.Log("SkinRefs - requesting download SKN_AMZ result: " + result.ToString(), "cyan");

            if (result == BackendResult.SUCCESS)
            {
                SkinContainer container2 = bundle.LoadAsset<SkinContainer>("SkinAmazon");

                foreach (Transform child in transform)
                {
                    Image img = child.GetComponent<Image>();
                    img.sprite = container2.GetSprite(child.name);
                }
            }

            TLUtils.LogUtil.Log("SkinRefs - requesting download SKN_AMZ APPLY: ", "cyan");

            refreshSkinLinksSignal.Dispatch();
        }
    }
}
