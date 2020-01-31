/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public partial class LobbyView : View
    {
        [Header("Bundles Tab")]
        public GameObject[] galleryBundles;

        public void HideBundles()
        {
            foreach (GameObject child in galleryBundles)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}

