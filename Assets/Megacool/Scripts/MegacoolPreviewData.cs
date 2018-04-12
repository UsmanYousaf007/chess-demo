using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegacoolPreviewData {
    public readonly string[] FramePaths;
    public readonly float PlaybackFrameRate = 0.0f;
    public readonly int LastFrameDelay = 0;

    public MegacoolPreviewData(string[] framePaths, float playbackFrameRate, int lastFrameDelay) {
        FramePaths = framePaths;
        PlaybackFrameRate = playbackFrameRate;
        LastFrameDelay = lastFrameDelay;
    }

    public MegacoolPreviewData(Dictionary<string, object> dict) {
        if (dict == null) return;

        object outValue;

        dict.TryGetValue("framePaths", out outValue );
        if (outValue != null) {
            FramePaths = (outValue as IEnumerable).Cast<object>().Select(x => x.ToString()).ToArray();
        }

        dict.TryGetValue("playbackFrameRate", out outValue);
        Single.TryParse(outValue.ToString(), out PlaybackFrameRate);
 
        dict.TryGetValue("lastFrameDelay", out outValue);
        Int32.TryParse (outValue.ToString(), out LastFrameDelay);
    }

    public override string ToString() {
        return string.Format("[MegacoolPreviewInfo: framePaths={0}, playbackFrameRate={1}, lastFrameDelay={2}]", FramePaths, PlaybackFrameRate, LastFrameDelay );
    }
}
