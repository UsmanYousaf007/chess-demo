/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public partial class StoreView
    {
        [Header("Purchased Dlg")]

        public GameObject purchasedDlg;
        public Button purchasedDlgOkButton;
        public Text purchasedDlgStoreItemLabel;

        public Signal purchaseDlgOkButtonClickedSignal = new Signal();

        public void InitPurchasedDlg()
        {
            purchasedDlgOkButton.onClick.AddListener(OnPurchasedDlgOkButtonClicked);
        }

        public void CleanupPurchasedDlg()
        {
            purchasedDlgOkButton.onClick.RemoveAllListeners();
        }

        public void UpdatePurchasedDlg(StoreItem item)
        {
            purchasedDlgStoreItemLabel.text = item.displayName;
        }

        public void ShowPurchasedDlg()
        {
            purchasedDlg.SetActive(true);
        }

        public void HidePurchasedDlg()
        {
            purchasedDlg.SetActive(false);
        }

        void OnPurchasedDlgOkButtonClicked()
        {
            purchaseDlgOkButtonClickedSignal.Dispatch();
        }
    }
}
