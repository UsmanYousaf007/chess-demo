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
using System.Collections;

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

        public Signal<string> chatSubmitSignal = new Signal<string>();

        public void InitChat()
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }

        public void OnParentShowChat()
        {
            
        }

        void OnSubmit(string message)
        {
            if (message.Length > 0)
            {
                playerChatBubble.gameObject.SetActive(false);
                playerChatBubble.text.text = message;
                playerChatBubble.gameObject.SetActive(true);
                inputField.text = "";

                chatSubmitSignal.Dispatch(message);
            }
        }
    }
}
