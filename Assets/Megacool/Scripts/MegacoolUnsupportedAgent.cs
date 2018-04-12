#if (!UNITY_IPHONE && !UNITY_IOS && !UNITY_ANDROID && !UNITY_EDITOR)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using AOT;

public class MegacoolUnsupportedAgent : MegacoolIAgent {

    private void SupportWarning() {
        Debug.LogWarning("Megacool: Unsupported platform");
    }

    //****************** API Implementation  ******************//
    public void Start(Action<MegacoolEvent> eventHandler) {
        SupportWarning();
    }

    public void StartRecording() {
        SupportWarning();
    }

    public void StartRecording(MegacoolRecordingConfig config) {
        SupportWarning();
    }

    public void RegisterScoreChange(int scoreDelta) {
        SupportWarning();
    }

    public void CaptureFrame() {
        SupportWarning();
    }

    public void CaptureFrame(MegacoolFrameCaptureConfig config) {
        SupportWarning();
    }

    public void SetCaptureMethod(MegacoolCaptureMethod captureMethod, RenderTexture renderTexture) {
        SupportWarning();
    }

    public void PauseRecording() {
        SupportWarning();
    }

    public void StopRecording() {
        SupportWarning();
    }

    public void DeleteRecording(string recordingId) {
        SupportWarning();
    }

    public MegacoolPreviewData GetPreviewDataForRecording(string recordingId) {
        SupportWarning();
        return null;
    }

    public int GetNumberOfFrames(string recordingId) {
        SupportWarning();
        return 0;
    }

    public void Share() {
        SupportWarning();
    }

    public void Share(MegacoolShareConfig config) {
        SupportWarning();
    }

    public void ShareToMessenger() {
        SupportWarning();
    }

    public void ShareToMessenger(MegacoolShareConfig config) {
        SupportWarning();
    }

    public void ShareToTwitter() {
        SupportWarning();
    }

    public void ShareToTwitter(MegacoolShareConfig config) {
        SupportWarning();
    }

    public void ShareToMessages() {
        SupportWarning();
    }

    public void ShareToMessages(MegacoolShareConfig config) {
        SupportWarning();
    }

    public void ShareToMail() {
        SupportWarning();
    }

    public void ShareToMail(MegacoolShareConfig config) {
        SupportWarning();
    }

    public void GetShares(Action<List<MegacoolShare>> shares, Func<MegacoolShare, bool> filter = null) {
        SupportWarning();
    }

    public void SetSharingText(string text) {
        SupportWarning();
    }

    public string GetSharingText() {
        SupportWarning();
        return "";
    }

    public void SetFrameRate(float frameRate) {
        SupportWarning();
    }

    public float GetFrameRate() {
        SupportWarning();
        return 0;
    }

    public void SetPlaybackFrameRate(float frameRate) {
        SupportWarning();
    }

    public float GetPlaybackFrameRate() {
        SupportWarning();
        return 0;
    }

    public void SetMaxFrames(int maxFrames) {
        SupportWarning();
    }

    public int GetMaxFrames() {
        SupportWarning();
        return 0;
    }

    public void SetPeakLocation(double peakLocation) {
        SupportWarning();
    }

    public double GetPeakLocation() {
        SupportWarning();
        return 0;
    }

    public void SetLastFrameDelay(int delay) {
        SupportWarning();
    }

    public int GetLastFrameDelay() {
        SupportWarning();
        return 0;
    }

    public void SetLastFrameOverlay(string lastFrameOverlay) {
        SupportWarning();
    }

    public void SetDebugMode(bool debugMode) {
        SupportWarning();
    }

    public bool GetDebugMode() {
        SupportWarning();
        return false;
    }

    public void SetKeepCompletedRecordings(bool keep) {
        SupportWarning();
    }

    public void DeleteShares(Func<MegacoolShare, bool> filter) {
        SupportWarning();
    }

    public void SubmitDebugData(string message) {
        SupportWarning();
    }

    public void ResetIdentity() {
        SupportWarning();
    }

    public void SetGIFColorTable(Megacool.GifColorTableType gifColorTable) {
        SupportWarning();
    }

    public void SignalRenderTexture(RenderTexture texture) {
    }

    public void IssuePluginEvent(ref IntPtr nativePluginCallbackPointer, int eventId) {
    }

    public void InitializeSharingDelegate() {
    }

    public void InitializeCapture(float scaleFactor, Megacool.TextureReadComplete textureReadCallback) {
    }
}
#endif
