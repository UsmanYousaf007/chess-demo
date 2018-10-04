using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class MegacoolEvent {

    public enum MegacoolEventType {
        ReceivedShareOpened = 0,
        SentShareOpened,
        LinkClicked
    }

    public MegacoolShare Share { get; private set; }

    public MegacoolEventType Type { get; private set; }

    public bool FirstSession { get; private set; }

    public Dictionary<string, object> Data { get; private set; }

    public MegacoolEvent(AndroidJavaObject jEvent) {
        try {
            Share = new MegacoolShare(jEvent.Call<AndroidJavaObject>("getShare"));
        } catch (Exception) {}
        string jType = jEvent.Get<AndroidJavaObject>("type").Call<string>("name");
        if (jType == "LINK_CLICKED") {
            Type = MegacoolEventType.LinkClicked;
        } else if (jType == "SENT_SHARE_OPENED") {
            Type = MegacoolEventType.SentShareOpened;
        } else {
            Type = MegacoolEventType.ReceivedShareOpened;
        }
        FirstSession = jEvent.Call<bool>("isFirstSession");
        Data = new Dictionary<string, object>();
        try {
            AndroidJavaObject jData = jEvent.Call<AndroidJavaObject>("getData");
            try {
                AndroidJavaObject jUrl = jData.Call<AndroidJavaObject>("get", "url");
                Data.Add("url", jUrl.Call<string>("toString"));
            } catch (Exception) {}
            try {
                AndroidJavaObject jReferralCode = jData.Call<AndroidJavaObject>("get", "referralCode");
                Data.Add("referralCode", new MegacoolReferralCode(jReferralCode));
            } catch (Exception) {}
            try {
                AndroidJavaObject jReceiverUserId = jData.Call<AndroidJavaObject>("get", "receiverUserId");
                Data.Add("receiverUserId", jReceiverUserId.Call<string>("toString"));
            } catch (Exception) {}
            try {
                AndroidJavaObject jSenderUserId = jData.Call<AndroidJavaObject>("get", "senderUserId");
                Data.Add("senderUserId", jSenderUserId.Call<string>("toString"));
            } catch (Exception) {}
        } catch (Exception) {}
    }

    public MegacoolEvent(Megacool.MegacoolLinkClickedEvent e) {
        Share = null;
        Type = MegacoolEventType.LinkClicked;
        FirstSession = e.isFirstSession != 0;
        Data = new Dictionary<string, object> {
            { "referralCode",  new MegacoolReferralCode(e.userId, e.shareId) },
            { "url", e.url }
        };
    }

    public MegacoolEvent(Megacool.MegacoolReceivedShareOpenedEvent e) {
        Share = new MegacoolShare(e);
        Type = MegacoolEventType.ReceivedShareOpened;
        FirstSession = e.isFirstSession != 0;
    }

    public MegacoolEvent(Megacool.MegacoolSentShareOpenedEvent e) {
        Share = new MegacoolShare(e);
        Type = MegacoolEventType.SentShareOpened;
        FirstSession = e.isFirstSession != 0;

        if (e.eventDataLength > 0) {
            byte[] bytes = new byte[e.eventDataLength];
            Marshal.Copy(e.eventDataBytes, bytes, 0, e.eventDataLength);

            Data = MegacoolThirdParty_MiniJSON.Json.Deserialize(System.Text.Encoding.UTF8.GetString(bytes)) as Dictionary<string, object>;
        } else {
            Data = new Dictionary<string, object>();
        }
    }

    public override string ToString() {
        return string.Format("[MegacoolEvent: Share={0}, Type={1}, FirstSession={2}, Data={3}]", Share, Type, FirstSession, Data);
    }
}
