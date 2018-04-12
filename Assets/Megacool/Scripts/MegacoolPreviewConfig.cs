using UnityEngine;

public struct MegacoolPreviewConfig {
    public string RecordingId { get; set; }

    public Rect PreviewFrame { get; set; }

    public RectTransform PreviewRectTransform { get; set; }

    public bool IncludeLastFrameOverlay { get; set; }

    public MegacoolPreviewConfig(Rect previewFrame, string recordingId = default(string), bool includeLastFrameOverlay = false) {
        RecordingId = recordingId;
        PreviewFrame = previewFrame;
        PreviewRectTransform = null;
        IncludeLastFrameOverlay = includeLastFrameOverlay;
    }
}
