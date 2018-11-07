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

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public Dictionary<string, bool> hasUnreadMessages { get; set; } = new Dictionary<string, bool>();
        Dictionary<string, ChatMessages> chatHistory { get; set; } = new Dictionary<string, ChatMessages>();

        const string CHAT_META_FILE = "chatMetaFile";
        const string CHAT_LAST_READ_TIMESTAMP = "chatLastReadTimestamp";
        const string CHAT_SAVE_FILE = "chatSaveFile";
        const string CHAT_SAVE_KEY = "chatSaveKey";
        const int CHAT_HISTORY_SIZE = 25;

        [PostConstruct]
        public void Load()
        {
            appEventSignal.AddListener(OnAppEvent);
        }

        public void AddChat(string playerId, ChatMessage message)
        {
            if (!chatHistory.ContainsKey(playerId))
            {
                chatHistory[playerId] = GetChat(playerId);
            }

            chatHistory[playerId].messageList.Add(message);
        }

        public ChatMessages GetChat(string playerId)
        {
            if (chatHistory.ContainsKey(playerId))
            {
                return chatHistory[playerId];
            }

            string filename = CHAT_SAVE_FILE + playerId;

            if (!localDataService.FileExists(filename))
            {
                chatHistory[playerId] = new ChatMessages();
                return chatHistory[playerId];
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(filename);

                // Read chat here
                if (reader.HasKey(CHAT_SAVE_KEY))
                {
                    string savedChat = reader.Read<string>(CHAT_SAVE_KEY);
                    chatHistory.Add(playerId, JsonUtility.FromJson<ChatMessages>(savedChat));
                }
                    
                reader.Close();
                return chatHistory[playerId];
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt chat history! " + e, "red");
                localDataService.DeleteFile(filename);
            }

            chatHistory[playerId] = new ChatMessages();
            return chatHistory[playerId];
        }

        public void ClearChat(string playerId)
        {
            string filename = CHAT_SAVE_FILE + playerId;
            localDataService.DeleteFile(filename);
            chatHistory[playerId] = new ChatMessages();
        }

        private void SaveToFile()
        {
            // Write chat here
            foreach (KeyValuePair<string, ChatMessages> entry in chatHistory)
            {
                string filename = CHAT_SAVE_FILE + entry.Key;

                try
                {
                    ILocalDataWriter writer = localDataService.OpenWriter(filename);

                    int messageCount = entry.Value.messageList.Count;
                    if (messageCount > CHAT_HISTORY_SIZE)
                    {
                        entry.Value.messageList.RemoveRange(0, messageCount - CHAT_HISTORY_SIZE);
                    }

                    writer.Write<string>(CHAT_SAVE_KEY, JsonUtility.ToJson(entry.Value));
                    writer.Close();
                }
                catch (Exception e)
                {
                    if (localDataService.FileExists(filename))
                    {
                        localDataService.DeleteFile(filename);
                    }

                    LogUtil.Log("Critical error when saving chat history. File deleted. " + e, "red");
                }
            }
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