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

        public GameObject chatDlgBg;
        public Button maximizeChatDlgBtn;
        //public Button minimizeChatDlgBtnX;
        public Button minimizeChatDlgBtn;

        public Signal<string> chatSubmitSignal = new Signal<string>();
        public Signal openChatDlgSignal = new Signal();
        public Signal closeChatDlgSignal = new Signal();

        public void InitChat()
        {
            inputField.onSubmit.AddListener(OnSubmit);
            maximizeChatDlgBtn.onClick.AddListener(OnOpenChatDlg);
          //  minimizeChatDlgBtnX.onClick.AddListener(OnCloseChatDlg);
            minimizeChatDlgBtn.onClick.AddListener(OnCloseChatDlg);
        }

        public void OnParentShowChat()
        {
            
        }

        public void ShowChatDlg()
        {
            chatDlgBg.SetActive(true);
            minimizeChatDlgBtn.gameObject.SetActive(true);
            maximizeChatDlgBtn.gameObject.SetActive(false);
        }

        public void HideChatDlg()
        {
            chatDlgBg.SetActive(false);
            minimizeChatDlgBtn.gameObject.SetActive(false);
            maximizeChatDlgBtn.gameObject.SetActive(true);
        }

        public void OnReceive(string message)
        {
            if (message.Length > 0)
            {
                opponentChatBubble.gameObject.SetActive(true);
                opponentChatBubble.text.text = message;
                opponentChatBubble.Refresh();
            }
        }

        void OnSubmit(string message)
        {
            if (message.Length > 0)
            {
                playerChatBubble.gameObject.SetActive(true);
                playerChatBubble.text.text = message;
                playerChatBubble.Refresh();

                inputField.text = "";

                chatSubmitSignal.Dispatch(message);
            }
        }

        void OnOpenChatDlg()
        {
            openChatDlgSignal.Dispatch();
        }

        void OnCloseChatDlg()
        {
            closeChatDlgSignal.Dispatch();
        }
    }
}
