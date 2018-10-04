using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Json = MegacoolThirdParty_MiniJSON.Json;

public class MegacoolShare {

    public enum MegacoolShareState {
        Sent = 0,
        Clicked,
        Opened,
        Installed
    }

    public MegacoolReferralCode ReferralCode { get; private set; }

    public MegacoolShareState State { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public Dictionary<string, string> Data { get; private set; }

    public Uri Url { get; private set; }

    readonly Encoding _encoding = Encoding.UTF8;
    readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private DateTime FromUnixTime(double unixTime) {
        return _epoch.AddSeconds(unixTime);
    }

    public MegacoolShare(AndroidJavaObject jShare) {
        try {
            ReferralCode = new MegacoolReferralCode(jShare.Call<AndroidJavaObject>("getReferralCode"));
        } catch (Exception) {}
        string jState = jShare.Call<AndroidJavaObject> ("getState").Call<string> ("toString");
        if (jState == "SENT") {
            State = MegacoolShareState.Sent;
        } else if (jState == "CLICKED") {
            State = MegacoolShareState.Clicked;
        } else if (jState == "OPENED") {
            State = MegacoolShareState.Opened;
        } else {
            State = MegacoolShareState.Installed;
        }
        try {
            long jTime = jShare.Call<AndroidJavaObject>("getCreatedAt").Call<long>("getTime");
            CreatedAt = _epoch.AddMilliseconds(jTime).ToLocalTime();
        } catch (Exception) {
        }
        try {
            long jTime = jShare.Call<AndroidJavaObject>("getUpdatedAt").Call<long>("getTime");
            UpdatedAt = _epoch.AddMilliseconds(jTime).ToLocalTime();
        } catch (Exception) {
        }
        try {
            Url = new Uri(jShare.Call<AndroidJavaObject>("getUrl").Call<string>("toString"), UriKind.Relative);
        } catch (Exception) {
        }
        Data = new Dictionary<string, string>();
        try {
            AndroidJavaObject jEntrySet = jShare.Call<AndroidJavaObject>("getData").Call<AndroidJavaObject>("entrySet");
            AndroidJavaObject jEntrySetIterator = jEntrySet.Call<AndroidJavaObject>("iterator");
            while (jEntrySetIterator.Call<bool>("hasNext")) {
                AndroidJavaObject jEntry = jEntrySetIterator.Call<AndroidJavaObject>("next");
                Data.Add(jEntry.Call<string>("getKey"), jEntry.Call<string>("getValue"));
            }
        } catch (Exception) {
        }
    }

    public MegacoolShare(Megacool.MegacoolReceivedShareOpenedEvent e) {
        ReferralCode = new MegacoolReferralCode(e.userId, e.shareId);
        State = (MegacoolShareState)e.state;
        CreatedAt = FromUnixTime(e.createdAt).ToLocalTime();
        UpdatedAt = FromUnixTime(e.updatedAt).ToLocalTime();

        if (e.url != null) {
            Url = new Uri(e.url, UriKind.Relative);
        }

        if (e.dataLength > 0) {
            byte[] bytes = new byte[e.dataLength];
            Marshal.Copy(e.dataBytes, bytes, 0, e.dataLength);

            DeserializeDataObject(bytes);
        } else {
            Data = new Dictionary<string, string>();
        }
    }

    public MegacoolShare(Megacool.MegacoolSentShareOpenedEvent e) {
        ReferralCode = new MegacoolReferralCode(e.userId, e.shareId);
        State = (MegacoolShareState)e.state;
        CreatedAt = FromUnixTime(e.createdAt).ToLocalTime();
        UpdatedAt = FromUnixTime(e.updatedAt).ToLocalTime();

        if (e.url != null) {
            Url = new Uri(e.url, UriKind.Relative);
        }

        Data = new Dictionary<string, string>() {
            { "receiverUserId" , e.receiverUserId }
        };
    }

    public MegacoolShare(Megacool.MegacoolShareData share) {
        ReferralCode = new MegacoolReferralCode(share.userId, share.shareId);
        State = (MegacoolShareState)share.state;
        CreatedAt = FromUnixTime(share.createdAt).ToLocalTime();
        UpdatedAt = FromUnixTime(share.updatedAt).ToLocalTime();

        if (share.dataLength > 0) {
            byte[] bytes = new byte[share.dataLength];
            Marshal.Copy(share.dataBytes, bytes, 0, share.dataLength);

            DeserializeDataObject(bytes);
        } else {
            Data = new Dictionary<string, string>();
        }

    }

    private void DeserializeDataObject(byte[] bytes) {
        var m_Data = Json.Deserialize(_encoding.GetString(bytes)) as Dictionary<string, object>;

        if (m_Data != null && m_Data.Count > 0) {
            Data = new Dictionary<string, string>(m_Data.Count);
            foreach (var data in m_Data) {
                Data.Add(data.Key, data.Value.ToString());
            }
        }
    }

    public override string ToString() {
        return string.Format("[MegacoolShare: ReferralCode={0}, State={1}, CreatedAt={2}, UpdatedAt={3}, Data={4}, Url={5}]", ReferralCode, State, CreatedAt, UpdatedAt, Data, Url);
    }
}
