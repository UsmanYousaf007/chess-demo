/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class SoftReconnectingView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text reconnectingLabel;
        public GameObject framework2DBlocker;

        public void Init()
        {
            reconnectingLabel.text = localizationService.Get(LocalizationKey.SOFT_RECONNECTING);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            framework2DBlocker.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            framework2DBlocker.SetActive(false);
        }
    }
}
