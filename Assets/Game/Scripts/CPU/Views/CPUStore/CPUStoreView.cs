/// @license #LICENSE# <#LICENSE_URL#>
/// @copyright Copyright (C) #COMPANY# #YEAR# - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public class CPUStoreView : View
    {
		public Button backButton;

		// View signals
		public Signal backButtonClickedSignal = new Signal();


        public void Init()
        {
            // Initialise the view here.
            // Call Init() in mediator's OnRegister()
            // after adding all the listeners.
			backButton.onClick.AddListener(OnBackButtonClicked);
        }

		public void UpdateView(CPUStoreVO vo)
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

		private void OnBackButtonClicked()
		{
			backButtonClickedSignal.Dispatch();
		}


    }
}
