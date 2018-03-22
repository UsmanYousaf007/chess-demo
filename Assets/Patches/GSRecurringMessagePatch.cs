/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-02 11:36:43 UTC+05:00
/// 
/// @description
/// This patch is to ignore recurring messages which are received on the client
/// side due to a bug in GameSparks. Until the bug it resolved this patch has to
/// exist.

using System;
using System.Collections.Generic;

using GameSparks.Api.Messages;

using TurboLabz.Common;

namespace TurboLabz.Patches
{
    // TODO(mubeeniqbal): Clean up this patch after the bug is resolved.
    public static class GSRecurringMessagePatch
    {
        private struct MessageCall
        {
            public string messageId;
            public string callId;
            public long timeTicks;

            public override int GetHashCode()
            {
                return messageId.GetHashCode() ^ callId.GetHashCode();
            }
        }

        // Key: MessageCall hash code
        // Value: MessageCall
        private static IDictionary<int, MessageCall> messageCalls = new Dictionary<int, MessageCall>();
        private static Queue<int> messageCallQueue = new Queue<int>();

        /// <summary>
        /// Checks if the message is recurring.
        /// </summary>
        /// <returns><c>true</c>, if message is recurring, <c>false</c>
        /// otherwise.</returns>
        /// <param name="message">The message object from GameSparks.</param>
        /// <param name="callId">This is the identifier to make the method calls
        /// unique. It is extremely important to make sure that
        /// <paramref name="callId"/> is unique for all the calls to this
        /// method.</param>
        public static bool isMessageRecurring(GSMessage message, string callId)
        {
            // Record time first to be as accurate as possible.
            long timeTicks = DateTime.UtcNow.Ticks;
            string messageId = message.MessageId;

            MessageCall call;
            call.messageId = messageId;
            call.callId = callId;
            call.timeTicks = timeTicks;

            int hashCode = call.GetHashCode();
            bool isRecurring = messageCalls.ContainsKey(hashCode);

            if (isRecurring)
            {
                string log = "Recurring message detected with ID: " + messageId + "\n";
                log += "Time (ticks): " + timeTicks + "\n";
                log += "Last message time (ticks): " + messageCalls[hashCode].timeTicks + "\n";
                log += "Has errors: " + message.HasErrors + "\n";
                log += "Listeners: " + GetListenersString(message) + "\n\n";
                log += message.JSONString;

                LogUtil.LogWarning(log, "orange");
            }

            if (messageCallQueue.Count >= 50)
            {
                int oldHashCode = messageCallQueue.Dequeue();
                messageCalls.Remove(oldHashCode);
            }

            messageCalls[hashCode] = call;
            messageCallQueue.Enqueue(hashCode);

            return isRecurring;
        }

        private static string GetListenersString(GSMessage message)
        {
            Delegate[] invocationList = null;


            if (message is ScriptMessage)
            {
                invocationList = ScriptMessage.Listener.GetInvocationList();
            }
            else if (message is ChallengeStartedMessage)
            {
                invocationList = ChallengeStartedMessage.Listener.GetInvocationList();
            }
            else if (message is ChallengeWonMessage)
            {
                invocationList = ChallengeWonMessage.Listener.GetInvocationList();
            }
            else if (message is ChallengeLostMessage)
            {
                invocationList = ChallengeLostMessage.Listener.GetInvocationList();
            }
            else if (message is ChallengeDrawnMessage)
            {
                invocationList = ChallengeDrawnMessage.Listener.GetInvocationList();
            }
            else if (message is ChallengeTurnTakenMessage)
            {
                invocationList = ChallengeTurnTakenMessage.Listener.GetInvocationList();
            }
            else
            {
                return "Unknown message type";
            }

            string result = "Size: " + invocationList.Length + " | ";

            foreach (Delegate e in invocationList)
            {
                result += e.Target + "." + e.Method + ", ";
            }

            return result;
        }
    }
}
