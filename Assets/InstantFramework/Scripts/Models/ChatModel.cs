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

        public void AddChat(string opponentId, ChatMessage message)
        {
            if (!chatHistory.ContainsKey(opponentId))
            {
                chatHistory[opponentId] = GetChat(opponentId);
            }


            //Debug.Log("AddingChat:" + message.text);

            List<ChatMessage> messageList = chatHistory[opponentId].messageList;
            foreach (ChatMessage savedMessage in messageList)
            {
                if (message.timestamp == savedMessage.timestamp)
                {
                    //Debug.Log("AddingChat: duplicate detected");
                    return;
                }
            }

            //Debug.Log("AddingChat: Success.");
            chatHistory[opponentId].messageList.Add(message);
        }

        public ChatMessages GetChat(string opponentId)
        {
            if (chatHistory.ContainsKey(opponentId))
            {
                Sort(chatHistory[opponentId]);
                return chatHistory[opponentId];
            }

            string filename = CHAT_SAVE_FILE + opponentId;

            if (!localDataService.FileExists(filename))
            {
                chatHistory[opponentId] = new ChatMessages();
                return chatHistory[opponentId];
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(filename);

                // Read chat here
                if (reader.HasKey(CHAT_SAVE_KEY))
                {
                    string savedChat = reader.Read<string>(CHAT_SAVE_KEY);
                    chatHistory.Add(opponentId, JsonUtility.FromJson<ChatMessages>(savedChat));
                }
                    
                reader.Close();
                Sort(chatHistory[opponentId]);
                return chatHistory[opponentId];
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt chat history! " + e, "red");
                localDataService.DeleteFile(filename);
            }

            chatHistory[opponentId] = new ChatMessages();
            return chatHistory[opponentId];
        }

        public void ClearChat(string opponentId)
        {
            string filename = CHAT_SAVE_FILE + opponentId;
            localDataService.DeleteFile(filename);
            chatHistory[opponentId] = new ChatMessages();
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

        private void Sort(ChatMessages chatMessages)
        {
            chatMessages.messageList.Sort((x, y) => x.timestamp.CompareTo(y.timestamp));
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