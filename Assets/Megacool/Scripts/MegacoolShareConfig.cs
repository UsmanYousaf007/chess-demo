using System;
using System.Collections.Generic;
using UnityEngine;

public struct MegacoolShareConfig {

    private static string dataPath = Application.streamingAssetsPath + "/";

    public string RecordingId { get; set; }


    private String lastFrameOverlay;

    [System.Obsolete("Use Megacool.SetLastFrameOverlay(string filename)")]
    public string LastFrameOverlay {
        get {
            return lastFrameOverlay;
        }
        set {
            lastFrameOverlay = (dataPath + value);
        }
    }

    private String fallbackImage;

    public string FallbackImage {
        get {
            return fallbackImage;
        }
        set {
#if UNITY_IOS && !UNITY_EDITOR
            fallbackImage = dataPath + value;
#else
            fallbackImage = value;
#endif
        }
    }

    /// <summary>
    /// Set extra share data that will be present on the received MegacoolShare.
    /// </summary>
    public Dictionary<string, string> Data;
    public string DataSerialized() {
        if (Data != null) {
            return MegacoolThirdParty_MiniJSON.Json.Serialize(Data);
        }
        return null;
    }

    /// <summary>
    /// Customize the link shared.
    /// </summary>
    /// <description>
    /// Note that only the path and query parameters are actually included in the shared link, thus this should be a
    /// relative URL of the form "/some/path?key=value".
    /// </description>
    public Uri Url;
    public string UrlString() {
        if (Url != null) {
            return Url.ToString();
        }
        return null;
    }

    /// <summary>
    /// Deprecated way to customize the Url and Data on the share.
    /// </summary>
    /// <value>The share.</value>
    [System.Obsolete("Set .Url and .Data directly on the ShareConfig instead")]
    public MegacoolShare Share {
        set {
            if (value != null) {
                Url = value.Url;
                Data = value.Data;
            }
        }
    }
}
