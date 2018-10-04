#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using AOT;

public class MegacoolAndroidAgent : MegacoolIAgent {

    public bool Debug {
        set {
            Android.CallStatic("setDebug", value);
        }
    }

    private AndroidJavaObject CurrentActivity {
        get {
            AndroidJavaClass jclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return jclass.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }

    public MegacoolAndroidAgent() {
    }

    private AndroidJavaObject android;

    private AndroidJavaObject Android {
        get {
            if (android == null) {
                android = new AndroidJavaClass("co.megacool.megacool.Megacool");

                if (Megacool.Debug) {
                    // Ensure debugging is enabled as early as possible
                    Debug = true;
                }
            }
            return android;
        }
    }

    private void ImplementationWarning(string message) {
        UnityEngine.Debug.LogWarning(message + " is not implemented on Android");
    }

    //*******************  Native Libraries  *******************//
    [DllImport("megacool")]
    private static extern void mcl_init_capture(int width, int height, string graphicsDeviceType);

    [DllImport("megacool")]
    private static extern void mcl_set_capture_texture(IntPtr texturePointer);

    [DllImport("megacool")]
    private static extern void mcl_set_texture_read_complete_callback([MarshalAs(UnmanagedType.FunctionPtr)] Megacool.TextureReadComplete callbackPointer);

    [DllImport("megacool")]
    private static extern IntPtr mcl_get_unity_render_event_pointer();

    //****************** API Implementation  ******************//
    public void Start(Action<MegacoolEvent> eventHandler) {
        Android.CallStatic("start", CurrentActivity, MegacoolConfiguration.Instance.appConfigAndroid,
            new OnEventsReceivedListener(eventHandler), "Unity", Application.unityVersion);
        AndroidJavaClass captureMethodEnum = new AndroidJavaClass("co.megacool.megacool.Megacool$CaptureMethod");
        AndroidJavaObject captureMethod = captureMethodEnum.GetStatic<AndroidJavaObject>("OPENGL");
        Android.CallStatic("setCaptureMethod", captureMethod);

        // Since the share listener has to be setup after the singleton on Android, do it here
        Android.CallStatic("setShareListener", new ShareListener(
            () => Megacool.Instance.CompletedSharing(),
            () => Megacool.Instance.PossiblyCompletedSharing(),
            () => Megacool.Instance.DismissedSharing()
        ));
    }

    public void StartRecording() {
        Android.CallStatic("startRecording", null);
    }

    public void StartRecording(MegacoolRecordingConfig config) {
        AndroidJavaObject jConfig = new AndroidJavaObject("co.megacool.megacool.RecordingConfig");

        // We have to use the generic version of Call here since the Java methods are not void, even
        // though we discard the return value
        jConfig.Call<AndroidJavaObject>("id", config.RecordingId);
        jConfig.Call<AndroidJavaObject>("maxFrames", config.MaxFrames);
        jConfig.Call<AndroidJavaObject>("peakLocation", config.PeakLocation);
        jConfig.Call<AndroidJavaObject>("frameRate", config.FrameRate);
        jConfig.Call<AndroidJavaObject>("playbackFrameRate", config.PlaybackFrameRate);
        jConfig.Call<AndroidJavaObject>("lastFrameDelay", config.LastFrameDelay);
        jConfig.Call<AndroidJavaObject>("overflowStrategy", config.OverflowStrategy.ToString());

        Android.CallStatic("startRecording", null, jConfig);
    }

    public void RegisterScoreChange(int scoreDelta) {
        Android.CallStatic("registerScoreChange", scoreDelta);
    }

    public void CaptureFrame() {
        Android.CallStatic("captureFrame", null);
    }

    public void CaptureFrame(MegacoolFrameCaptureConfig config) {
        AndroidJavaObject jConfig = new AndroidJavaObject("co.megacool.megacool.RecordingConfig");

        // We have to use the generic version of Call here since the Java methods are not void, even
        // though we discard the return value
        jConfig.Call<AndroidJavaObject>("id", config.RecordingId);
        jConfig.Call<AndroidJavaObject>("maxFrames", config.MaxFrames);
        jConfig.Call<AndroidJavaObject>("peakLocation", config.PeakLocation);
        jConfig.Call<AndroidJavaObject>("frameRate", config.FrameRate);
        jConfig.Call<AndroidJavaObject>("playbackFrameRate", config.PlaybackFrameRate);
        jConfig.Call<AndroidJavaObject>("lastFrameDelay", config.LastFrameDelay);
        jConfig.Call<AndroidJavaObject>("overflowStrategy", config.OverflowStrategy.ToString());

        Android.CallStatic("captureFrame", null, jConfig);
    }

    public void SetCaptureMethod(MegacoolCaptureMethod captureMethod, RenderTexture renderTexture) {
        if (captureMethod == MegacoolCaptureMethod.SCREEN){
            mcl_set_capture_texture(IntPtr.Zero);
        } else {
            SignalRenderTexture(renderTexture);
        }
    }

    public void PauseRecording() {
        Android.CallStatic("pauseRecording");
    }

    public void StopRecording() {
        Android.CallStatic("stopRecording");
    }

    public void DeleteRecording(string recordingId) {
        Android.CallStatic("deleteRecording", recordingId);
    }

    public MegacoolPreviewData GetPreviewDataForRecording(string recordingId) {
        var jPreviewData = Android.CallStatic<AndroidJavaObject>("getPreviewDataForRecording", recordingId);
        if (jPreviewData == null) {
            return null;
        }

        var jFramePaths = jPreviewData.Call<AndroidJavaObject>("getFramePaths");
        var framePaths = AndroidJNIHelper.ConvertFromJNIArray<string[]>(jFramePaths.GetRawObject());
        var playbackFrameRate = (float)jPreviewData.Call<int>("getPlaybackFrameRate");
        var lastFrameDelayMs = jPreviewData.Call<int>("getLastFrameDelayMs");

        return new MegacoolPreviewData(framePaths, playbackFrameRate, lastFrameDelayMs);
    }

    public int GetNumberOfFrames(string recordingId) {
        return Android.CallStatic<int>("getNumberOfFrames", recordingId);
    }

    public void Share() {
        Android.CallStatic("share", CurrentActivity);
    }

    public void Share(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = ConfigToJavaObject(config);
        Android.CallStatic("share", CurrentActivity, jConfig);
    }

    public void ShareToMessenger() {
        Android.CallStatic("shareToMessenger", CurrentActivity);
    }

    public void ShareToMessenger(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = ConfigToJavaObject(config);
        Android.CallStatic("shareToMessenger", CurrentActivity, jConfig);
    }

    public void ShareToTwitter() {
        Android.CallStatic("shareToTwitter", CurrentActivity);
    }

    public void ShareToTwitter(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = ConfigToJavaObject(config);
        Android.CallStatic("shareToTwitter", CurrentActivity, jConfig);
    }

    public void ShareToMessages() {
        Android.CallStatic("shareToMessages", this.CurrentActivity);
    }

    public void ShareToMessages(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = ConfigToJavaObject(config);
        Android.CallStatic("shareToMessages", CurrentActivity, jConfig);
    }

    public void ShareToMail() {
        Android.CallStatic("shareToMail", CurrentActivity);
    }

    public void ShareToMail(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = ConfigToJavaObject(config);
        Android.CallStatic("shareToMail", CurrentActivity, jConfig);
    }

    private AndroidJavaObject ConfigToJavaObject(MegacoolShareConfig config) {
        AndroidJavaObject jConfig = new AndroidJavaObject("co.megacool.megacool.ShareConfig");
        jConfig.Call<AndroidJavaObject>("recordingId", config.RecordingId);
        jConfig.Call<AndroidJavaObject>("fallbackImageUrl", config.FallbackImage);

        if (config.Url != null) {
            AndroidJavaClass jUriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject jUri = jUriClass.CallStatic<AndroidJavaObject>("parse", config.Url.ToString());
            jConfig.Call<AndroidJavaObject>("url", jUri);
        }

        if (config.Data != null) {
            AndroidJavaObject jData = new AndroidJavaObject("java.util.HashMap");
            foreach(KeyValuePair<string, string> entry in config.Data) {
                jData.Call<AndroidJavaObject>("put", entry.Key, entry.Value);
            }
            jConfig.Call<AndroidJavaObject>("data", jData);
        }
        return jConfig;
    }

    public void GetShares(Action<List<MegacoolShare>> shares, Func<MegacoolShare, bool> filter = null) {
        Android.CallStatic<AndroidJavaObject>("getShares", new ShareCallback(shares), filter != null ? new ShareFilter(filter) : null);
    }

    public void SetSharingText(string text) {
        Android.CallStatic("setSharingText", text);
    }

    public string GetSharingText() {
        return Android.CallStatic<string>("getSharingText");
    }

    public void SetFrameRate(float frameRate) {
        Android.CallStatic("setFrameRate", (int)frameRate);
    }

    public float GetFrameRate() {
        return (float)Android.CallStatic<int>("getFrameRate");
    }

    public void SetPlaybackFrameRate(float frameRate) {
        Android.CallStatic("setPlaybackFrameRate", (int)frameRate);
    }

    public float GetPlaybackFrameRate() {
        return (float)Android.CallStatic<int>("getPlaybackFrameRate");
    }

    public void SetMaxFrames(int maxFrames) {
        Android.CallStatic("setMaxFrames", maxFrames);
    }

    public int GetMaxFrames() {
        return Android.CallStatic<int>("getMaxFrames");
    }

    public void SetPeakLocation(double peakLocation) {
        Android.CallStatic("setPeakLocation", peakLocation);
    }

    public double GetPeakLocation() {
        return Android.CallStatic<double>("getPeakLocation");
    }

    public void SetLastFrameDelay(int delay) {
        Android.CallStatic("setLastFrameDelay", delay);
    }

    public int GetLastFrameDelay() {
        return Android.CallStatic<int>("getLastFrameDelay");
    }

    public void SetLastFrameOverlay(string lastFrameOverlay) {
        Android.CallStatic("setLastFrameOverlay", lastFrameOverlay);
    }

    public void SetDebugMode(bool debugMode) {
        Android.CallStatic("setDebug", debugMode);
    }

    public bool GetDebugMode() {
        ImplementationWarning("getDebugMode");
        return false;
    }

    public void SetKeepCompletedRecordings(bool keep) {
        Android.CallStatic("setKeepCompletedRecordings", keep);
    }

    public void DeleteShares(Func<MegacoolShare, bool> filter) {
        Android.CallStatic("deleteShares", new ShareFilter(filter));
    }

    public void SubmitDebugData(string message) {
        Android.CallStatic("submitDebugData", message);
    }

    public void ResetIdentity() {
        Android.CallStatic("resetIdentity");
    }

    public void SetGIFColorTable(Megacool.GifColorTableType gifColorTable) {
        AndroidJavaClass jGifColorTableClass = new AndroidJavaClass("co.megacool.megacool.GifColorTable");
        AndroidJavaObject jGifColorTable;
        switch (gifColorTable) {
        case Megacool.GifColorTableType.GifColorTableFixed:
            jGifColorTable = jGifColorTableClass.GetStatic<AndroidJavaObject>("FIXED");
            break;
        default:
            // This covers both dynamic and analyzeFirst, the latter is iOS only but largely equivalent
            jGifColorTable = jGifColorTableClass.GetStatic<AndroidJavaObject>("DYNAMIC");
            break;
        }
        Android.CallStatic("setGifColorTable", jGifColorTable);
    }

    public void SetSharingStrategy(MegacoolSharingStrategy sharingStrategy) {
        AndroidJavaClass strategyClass = new AndroidJavaClass("co.megacool.megacool.SharingStrategy");
        AndroidJavaObject strategy;
        if (sharingStrategy == MegacoolSharingStrategy.LINK) {
            strategy = strategyClass.GetStatic<AndroidJavaObject>("LINK");
        } else {
            strategy = strategyClass.GetStatic<AndroidJavaObject>("MEDIA");
        }
        Android.CallStatic("setSharingStrategy", strategy);
    }

    public void SignalRenderTexture(RenderTexture texture) {
        if (!texture) {
            texture = Megacool.Instance.RenderTexture;
            // this automatically does the signalling
            return;
        }
        mcl_set_capture_texture(texture.GetNativeTexturePtr());
    }

    public void IssuePluginEvent(ref IntPtr nativePluginCallbackPointer, int eventId) {
        if (nativePluginCallbackPointer == IntPtr.Zero) {
            nativePluginCallbackPointer = mcl_get_unity_render_event_pointer();
        }
        GL.IssuePluginEvent(nativePluginCallbackPointer, eventId);
    }

    public void InitializeSharingDelegate() {
        // On Android setShareListener has to be called after start(), so we set up the listeners there instead
    }

    public void InitializeCapture(float scaleFactor, Megacool.TextureReadComplete textureReadCompleteCallback) {
        int width = (int)(Screen.width / scaleFactor);
        int height = (int)(Screen.height / scaleFactor);
        mcl_init_capture(width, height, SystemInfo.graphicsDeviceType.ToString());
        mcl_set_texture_read_complete_callback(textureReadCompleteCallback);
    }

    private class ShareListener : AndroidJavaProxy
    {
        private Action shareCompleteHandler;
        private Action sharePossiblyCompleteHandler;
        private Action shareDismissedHandler;

        public ShareListener(
            Action shareCompleteHandler,
            Action sharePossiblyCompleteHandler,
            Action shareDismissedHandler
        ) : base("co.megacool.megacool.Megacool$ShareListener") {
            this.shareCompleteHandler = shareCompleteHandler;
            this.sharePossiblyCompleteHandler = sharePossiblyCompleteHandler;
            this.shareDismissedHandler = shareDismissedHandler;
        }

        void didCompleteShare() {
            shareCompleteHandler();
        }

        void didPossiblyCompleteShare() {
            sharePossiblyCompleteHandler();
        }

        void didDismissShare() {
            shareDismissedHandler();
        }

    }

}

class OnEventsReceivedListener : AndroidJavaProxy {
    private Action<MegacoolEvent> eventHandler;

    public OnEventsReceivedListener(Action<MegacoolEvent> eventHandler) : base("co.megacool.megacool.Megacool$OnEventsReceivedListener") {
        this.eventHandler = eventHandler;
    }

    void onEventsReceived(AndroidJavaObject jEvents) {
        int size = jEvents.Call<int>("size");
        for (int i = 0; i < size; i++) {
            AndroidJavaObject jEvent = jEvents.Call<AndroidJavaObject>("get", i);
            eventHandler(new MegacoolEvent(jEvent));
        }
    }
}

class ShareCallback : AndroidJavaProxy {
    private Action<List<MegacoolShare>> shareHandler;

    public ShareCallback(Action<List<MegacoolShare>> shareHandler) : base("co.megacool.megacool.Megacool$ShareCallback") {
        this.shareHandler = shareHandler;
    }

    void shares(AndroidJavaObject jShares) {
        int size = jShares.Call<int>("size");
        List<MegacoolShare> result = new List<MegacoolShare>(size);
        for (int i = 0; i < size; i++) {
            AndroidJavaObject jShare = jShares.Call<AndroidJavaObject>("get", i);
            result.Add(new MegacoolShare(jShare));
        }
        shareHandler(result);
    }
}

class ShareFilter : AndroidJavaProxy {
    private Func<MegacoolShare, bool> filter;

    public ShareFilter(Func<MegacoolShare, bool> filter) : base("co.megacool.megacool.Megacool$ShareFilter") {
        this.filter = filter;
    }

    bool accept(AndroidJavaObject jShare) {
        return filter(new MegacoolShare(jShare));
    }
}

#endif
