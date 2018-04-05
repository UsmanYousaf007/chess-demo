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

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class CPUStatsView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        // Scene references
        public Button backButton;

        // View signals
        public Signal backButtonClickedSignal = new Signal();


        public void Init()
        {
//            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        public void CleanUp()
        {
  //          backButton.onClick.RemoveAllListeners();
        }

        public void UpdateView()
        {
            
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }
    }
}
