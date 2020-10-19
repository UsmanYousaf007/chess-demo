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
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        private const string DEFAULT_SKIN_ID = "SkinSlate";
        public Signal refreshSkinLinksSignal = new Signal();

        private string currentSkinId;
        private string newSkinId;

        public void LoadSkin(string skinId)
        {
            if (skinId != currentSkinId)
            {
                newSkinId = skinId;
                SkinContainer container = SkinContainer.LoadSkin(newSkinId);
                if (container != null)
                {
                    LoadTransform(container);
                    if (currentSkinId != null)
                    {
                        downloadablesModel.PreloadFromCache(currentSkinId, false);
                    }
                    currentSkinId = newSkinId;
                    refreshSkinLinksSignal.Dispatch();
                }

                else
                {
                    downloadablesModel.Get(skinId, OnSkinBundleLoaded, ContentType.Skins);
                }
            }
        }

        public void OnSkinBundleLoaded(BackendResult result, AssetBundle bundle)
        {
            if (result == BackendResult.SUCCESS)
            {
                SkinContainer container = bundle.LoadAsset<SkinContainer>(newSkinId);
                LoadTransform(container);
                if (currentSkinId != null)
                {
                    downloadablesModel.PreloadFromCache(currentSkinId, false);
                }
                downloadablesModel.PreloadFromCache(newSkinId, true);
                currentSkinId = newSkinId;
            }

            else
            {
                SkinContainer container = SkinContainer.LoadSkin(DEFAULT_SKIN_ID);
                LoadTransform(container);
                currentSkinId = DEFAULT_SKIN_ID;
            }

            refreshSkinLinksSignal.Dispatch();
        }

        private void LoadTransform(SkinContainer container)
        {
            foreach (Transform child in transform)
            {
                Image img = child.GetComponent<Image>();
                img.sprite = container.GetSprite(child.name);
            }
        }
    }
}
