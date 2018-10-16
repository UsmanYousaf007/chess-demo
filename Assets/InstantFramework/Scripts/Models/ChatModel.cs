/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ChatModel : IChatModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        // Listen to signals
        [Inject] public AppEventSignal appEventSignal { get; set; }

        public Dictionary<string, ChatMessages> chatHistory { get; set; }

        const string CHAT_SAVE_FILENAME = "chatSaveFilename";
        const string CHAT_SAVE_KEY = "chatSaveKey";
        const int CHAT_HISTORY_SIZE = 25;

        [PostConstruct]
        public void Load()
        {
            Reset();
            LoadFromFile();

            appEventSignal.AddListener(OnAppEvent);
        }

        private void LoadFromFile()
        {
            if (!localDataService.FileExists(CHAT_SAVE_FILENAME))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(CHAT_SAVE_FILENAME);

                // Read chat here
                if (reader.HasKey(CHAT_SAVE_KEY))
                {
                    Dictionary<string, string> savedChat = reader.ReadDictionary<string, string>(CHAT_SAVE_KEY);
                    foreach (KeyValuePair<string, string> entry in savedChat)
                    {
                        LogUtil.Log("Adding from chat history " + entry.Key + ":" + entry.Value, "red");
                        chatHistory.Add(entry.Key, JsonUtility.FromJson<ChatMessages>(entry.Value));
                    }
                }
                    
                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt chat history! " + e, "red");
                localDataService.DeleteFile(CHAT_SAVE_FILENAME);
                Reset();
            }
        }

        private void SaveToFile()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(CHAT_SAVE_FILENAME);

                // Write chat here
                Dictionary<string, string> savedChat = new Dictionary<string, string>();
                foreach (KeyValuePair<string, ChatMessages> entry in chatHistory)
                {
                    int messageCount = entry.Value.messageList.Count;
                    if (messageCount > CHAT_HISTORY_SIZE)
                    {
                        entry.Value.messageList.RemoveRange(0, messageCount - CHAT_HISTORY_SIZE);
                    }

                    savedChat.Add(entry.Key, JsonUtility.ToJson(entry.Value));
                }

                writer.WriteDictionary<string, string>(CHAT_SAVE_KEY, savedChat);
                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(CHAT_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(CHAT_SAVE_FILENAME);
                }

                LogUtil.Log("Critical error when saving chat history. File deleted. " + e, "red");
            }
        }

        private void Reset()
        {
            chatHistory = new Dictionary<string, ChatMessages>();
        }

        private void OnAppEvent(AppEvent evt)
        {
            if (evt == AppEvent.QUIT || evt == AppEvent.PAUSED)
            {
                SaveToFile();
            }
        }
    }

    [Serializable]
    public class ChatMessages
    {
        public List<ChatMessage> messageList = new List<ChatMessage>();
    }

    [Serializable]
    public struct ChatMessage
    {
        public string senderId;
        public string recipientId;
        public string text;
        public long timestamp;
    }
}
