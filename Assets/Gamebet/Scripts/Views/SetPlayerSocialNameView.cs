/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-08 11:50:21 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class SetPlayerSocialNameView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public NameOption nameOptionPrefab;

        public GameObject nameOptionsParent;
        public Text selectNameLabel;

        public Signal<string> nameOptionButtonClickedSignal = new Signal<string>();

        public void Init()
        {
        }

        public void UpdateView(SetPlayerSocialNameVO vo)
        {
            selectNameLabel.text = localizationService.Get(LocalizationKey.SPSN_HEADING_LABEL);

            string[] nameOptions = vo.nameOptions;

            // Destroy all old name option objects if there are any.
            // Leave the first object as it is the title label.
            UnityUtil.DestroyChildren(nameOptionsParent.transform, 1);

            for (int i = 0; i < nameOptions.Length; ++i)
            {
                string name = nameOptions[i];

                NameOption nameOption = Instantiate<NameOption>(nameOptionPrefab);
                nameOption.nameLabel.text = name;

                nameOption.button.onClick.AddListener(() => { OnNameOptionButtonClicked(name); });

                // It is very important that worldPositionStays is set to false
                // otherwise the object will have incorrect size on screen
                // sizes/resolutions other than the one it is originally
                // designed for.
                nameOption.transform.SetParent(nameOptionsParent.transform, false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnNameOptionButtonClicked(string name)
        {
            nameOptionButtonClickedSignal.Dispatch(name);
        }
    }
}
