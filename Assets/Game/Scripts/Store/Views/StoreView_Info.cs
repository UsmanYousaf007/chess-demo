/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public partial class StoreView
    {
        [Header("Info")]
        public GameObject infoDlg;
        public GameObject uiBlocker;
        public Button closeBtn;
        public Button infoBtn;

        public void InitInfo()
        {
            infoBtn.onClick.AddListener(OnInfoClicked);
            closeBtn.onClick.AddListener(OnInfoCloseClicked);
        }

        void OnInfoClicked()
        {
            infoDlg.SetActive(true);
            uiBlocker.SetActive(true);
        }

        void OnInfoCloseClicked()
        {
            infoDlg.SetActive(false);
            uiBlocker.SetActive(false);
        }
    }
}
