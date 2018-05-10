/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
	class GooglePurchaseData 
	{
		public string inAppPurchaseData;
		public string inAppDataSignature;

		[System.Serializable]
		private class GooglePurchaseReceipt 
		{
			public string Payload = null;
		}

		[System.Serializable]
		private class GooglePurchasePayload 
		{
			public string json = null;
			public string signature = null;
		}

		public GooglePurchaseData(string receipt)
		{
			try
			{
				GooglePurchaseReceipt receiptDeserialized = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
				GooglePurchasePayload payloadDeserialized = JsonUtility.FromJson<GooglePurchasePayload>(receiptDeserialized.Payload);

				inAppPurchaseData = payloadDeserialized.json;
				inAppDataSignature = payloadDeserialized.signature;

			}
			catch (System.Exception e)
			{
				TLUtils.LogUtil.Log("GooglePurchaseData expection:" + e, "red");

				inAppPurchaseData = "";
				inAppDataSignature = "";
			}
		}
	}
}

