/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace TurboLabz.InstantFramework
{
    public class ChatModel : IChatModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        // Listen to signals
        [Inject] public SaveToDiskSignal saveToDiskSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set;  }

        public Dictionary<string, bool> hasUnreadMessages { get; set; } = new Dictionary<string, bool>();
        public string lastSavedChatId { get; set; } = "";
        public bool preloadingMessagesCompleted { get; set; } = false;

        Dictionary<string, ChatMessages> chatHistory { get; set; } = new Dictionary<string, ChatMessages>();

        const string CHAT_META_FILE = "chatMetaFile";
        const string CHAT_META_LAST_SAVE_KEY = "chatMetaLastSaveKey";
        const string CHAT_HISTORY_FILE = "chatHistoryFile";
        const string CHAT_HISTORY_SAVE_KEY = "chatHistorySaveKey";
        const int CHAT_HISTORY_SIZE = 50;

        [PostConstruct]
        public void Load()
        {
            saveToDiskSignal.AddListener(SaveChatHistoryFile);
            saveToDiskSignal.AddListener(SaveChatMetaFile);
            LoadMetaFile();
        }

        public void Reset()
        {
            hasUnreadMessages = new Dictionary<string, bool>();
            chatHistory = new Dictionary<string, ChatMessages>();
        }


        public bool AddChat(string opponentId, ChatMessage message, bool isBackupMessage)
        {
            List<ChatMessage> messageList = GetChat(opponentId).messageList;

            if (isBackupMessage && messageList.Count > 0)
            {
                // Leave if our message list already contains the backup message
                for (int i = 0; i < messageList.Count; i++)
                {
                    if (message.guid == messageList[i].guid)
                    {
                        return false;
                    }
                }

                // Looks like we have found a dropped message so insert this to
                // the approriate slot according to timestamp
                int insertIndex = -1;
                for (int i = 0; i < messageList.Count; i++)
                {
                    if (message.timestamp < messageList[i].timestamp)
                    {
                        insertIndex = i;
                        break;
                    }
                }

                if (insertIndex == -1)
                {
                    messageList.Add(message);
                }
                else
                {
                    messageList.Insert(insertIndex, message);
                }

            }
            else
            {
                messageList.Add(message);
            }

            // The last saved chat id must only be that of chats
            // sent by the opponennt
            if (message.senderId != playerModel.id &&
                preloadingMessagesCompleted)
            {
                lastSavedChatId = message.guid;
            }

            return true;
        }

        public ChatMessages GetChat(string opponentId)
        {
            if (chatHistory.ContainsKey(opponentId))
            {
                return chatHistory[opponentId];
            }

            string filename = CHAT_HISTORY_FILE + opponentId;

            if (!localDataService.FileExists(filename))
            {
                chatHistory[opponentId] = new ChatMessages();
                return chatHistory[opponentId];
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(filename);

                // Read chat here
                if (reader.HasKey(CHAT_HISTORY_SAVE_KEY))
                {
                    string savedChat = reader.Read<string>(CHAT_HISTORY_SAVE_KEY);
                    chatHistory.Add(opponentId, JsonUtility.FromJson<ChatMessages>(savedChat));
                }
                    
                reader.Close();
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
            string filename = CHAT_HISTORY_FILE + opponentId;
            localDataService.DeleteFile(filename);
            chatHistory[opponentId] = new ChatMessages();
        }

        void SaveChatHistoryFile()
        {
            // Write chat here
            foreach (KeyValuePair<string, ChatMessages> entry in chatHistory)
            {
                string filename = CHAT_HISTORY_FILE + entry.Key;

                try
                {
                    ILocalDataWriter writer = localDataService.OpenWriter(filename);

                    int messageCount = entry.Value.messageList.Count;
                    if (messageCount > CHAT_HISTORY_SIZE)
                    {
                        entry.Value.messageList.RemoveRange(0, messageCount - CHAT_HISTORY_SIZE);
                    }

                    writer.Write<string>(CHAT_HISTORY_SAVE_KEY, JsonUtility.ToJson(entry.Value));
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

        void SaveChatMetaFile()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(CHAT_META_FILE);
                writer.Write<string>(CHAT_META_LAST_SAVE_KEY, lastSavedChatId);
                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(CHAT_META_FILE))
                {
                    localDataService.DeleteFile(CHAT_META_FILE);
                }
            }
        }

        void LoadMetaFile()
        {
            if (!localDataService.FileExists(CHAT_META_FILE))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(CHAT_META_FILE);

                // Read chat here
                if (reader.HasKey(CHAT_META_LAST_SAVE_KEY))
                {
                    lastSavedChatId = reader.Read<string>(CHAT_META_LAST_SAVE_KEY);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                lastSavedChatId = "";
                localDataService.DeleteFile(CHAT_META_FILE);
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
        public string guid;
    }
}