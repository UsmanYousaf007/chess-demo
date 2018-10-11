/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TMPro;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Chat")]
        public ChatBubble opponentChatBubble;
        public ChatBubble playerChatBubble;
        public TMP_InputField inputField; 
        public GameObject chatCounter;
        public Text chatCounterLabel;

        public Signal chatButtonClickedSignal = new Signal();

        public void InitChat()
        {
            inputField.onEndEdit.AddListener(EndEdit);
        }

        public void OnParentShowChat()
        {
            
        }

        void EndEdit(string message)
        {
            playerChatBubble.gameObject.SetActive(false);
            playerChatBubble.text.text = message;
            playerChatBubble.gameObject.SetActive(true);
        }
    }
}
