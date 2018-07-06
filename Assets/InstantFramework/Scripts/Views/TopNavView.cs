/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:54:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class TopNavView : View
    {
        public Button shareButton;
        public Button addBucksButton;
        public Text playerBucks;

        public Signal shareAppButtonClickedSignal = new Signal();
        public Signal addBucksButtonClickedSignal = new Signal();

        public void Init()
        {
            shareButton.onClick.AddListener(OnShareAppButtonClicked);
            addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);
        }

        public void UpdatePlayerBucks(long bucks)
        {
            playerBucks.text = bucks.ToString();
        }

        private void OnShareAppButtonClicked()
        {
            shareAppButtonClickedSignal.Dispatch();
        }

        private void OnAddBucksButtonClicked()
        {
            addBucksButtonClickedSignal.Dispatch();
        }
    }
}
