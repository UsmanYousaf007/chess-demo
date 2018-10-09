/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Chat")]
        public Button chatButton;
        public GameObject chatCounter;
        public Text chatCounterLabel;

        public Signal chatButtonClickedSignal = new Signal();

        public void InitChat()
        {
            chatButton.onClick.AddListener(OnChatButtonClicked);
        }

        public void OnParentShowChat()
        {
            
        }

        public void EnableGameChat(bool enable)
        {
            /*
            RectTransform wifiIcon = warning.GetComponent<RectTransform>();

            if (enable)
            {
                chatButton.gameObject.SetActive(true);
                wifiIcon.anchoredPosition = new Vector2(-171f, 0f);
            }
            else
            {
                chatButton.gameObject.SetActive(false);
                wifiIcon.anchoredPosition = new Vector2(-64, 0f);
            }
            */
        }

        void OnChatButtonClicked()
        {
            chatButtonClickedSignal.Dispatch();
        }
    }
}
