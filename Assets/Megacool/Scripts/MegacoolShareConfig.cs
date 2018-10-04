using System;
using System.Collections.Generic;
using UnityEngine;

public struct MegacoolShareConfig {
    public string RecordingId { get; set; }

    public string FallbackImage { get; set; }

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
}
