/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
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
		public SkinShopItemPrefab skinShopItemPrefab;
		public GameObject gallery;
		public Button backButton;

		// View signals
		public Signal backButtonClickedSignal = new Signal();
		public Signal<string> skinItemClickedSignal = new Signal<string>();


        public void Init()
        {
			backButton.onClick.AddListener(OnBackButtonClicked);
        }

		public void UpdateView(CPUStoreVO vo)
		{
			PopulateSkins(vo);
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

		private void PopulateSkins(CPUStoreVO vo)
		{
			IStoreSettingsModel storeSettingsModel = vo.storeSettingsModel;

			List<StoreItem> list = storeSettingsModel.lists["Skin"];
			foreach (StoreItem item in list) 
			{
				SkinShopItemPrefab skinThumbnail = Instantiate<SkinShopItemPrefab>(skinShopItemPrefab);
				skinThumbnail.transform.SetParent(gallery.transform, false);

				skinThumbnail.displayName.text = item.displayName;
				//skinThumbnail.thumbnail = ;
				skinThumbnail.price.text = item.currency2Cost.ToString();
				skinThumbnail.button.onClick.AddListener(() => OnSkinItemClicked(item.key));
			}
		}

		private void OnSkinItemClicked(string key)
		{
			skinItemClickedSignal.Dispatch(key);
		}
    }
}
