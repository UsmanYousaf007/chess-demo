using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface MegacoolIAgent {

    void Start(Action<MegacoolEvent> eventHandler);

    void StartRecording();

    void StartRecording(MegacoolRecordingConfig config);

    void RegisterScoreChange(int scoreDelta);

    void CaptureFrame();

    void CaptureFrame(MegacoolFrameCaptureConfig config);

    void SetCaptureMethod(MegacoolCaptureMethod captureMethod, RenderTexture renderTexture);

    void PauseRecording();

    void StopRecording();

    void DeleteRecording(string recordingId);

    void DeleteShares(Func<MegacoolShare, bool> filter);

    MegacoolPreviewData GetPreviewDataForRecording(string recordingId);

    int GetNumberOfFrames(string recordingId);

    void GetShares(Action<List<MegacoolShare>> shares = null, Func<MegacoolShare, bool> filter = null);

    void Share();

    void Share(MegacoolShareConfig config);

    void ShareToMessenger();

    void ShareToMessenger(MegacoolShareConfig config);

    void ShareToTwitter();

    void ShareToTwitter(MegacoolShareConfig config);

    void ShareToMessages();

    void ShareToMessages(MegacoolShareConfig config);

    void ShareToMail();

    void ShareToMail(MegacoolShareConfig config);

    void SetSharingText(string text);

    string GetSharingText();

    void SetFrameRate(float frameRate);

    float GetFrameRate();

    void SetPlaybackFrameRate(float frameRate);

    float GetPlaybackFrameRate();

    void SetMaxFrames(int maxFrames);

    int GetMaxFrames();

    void SetPeakLocation(double peakLocation);

    double GetPeakLocation();

    void SetLastFrameDelay(int delay);

    int GetLastFrameDelay();

    void SetLastFrameOverlay(string lastFrameOverlay);

    void SetDebugMode(bool debugMode);

    bool GetDebugMode();

    void SetKeepCompletedRecordings(bool keep);

    void SubmitDebugData(string message);

    void ResetIdentity();

    void SetGIFColorTable(Megacool.GifColorTableType gifColorTable);

    void SetSharingStrategy(MegacoolSharingStrategy sharingStrategy);

    void SignalRenderTexture(RenderTexture texture);

    void IssuePluginEvent(ref IntPtr nativePluginCallbackPointer, int eventId);

    void InitializeSharingDelegate();

    void InitializeCapture(float scaleFactor, Megacool.TextureReadComplete textureReadCallback);
}
