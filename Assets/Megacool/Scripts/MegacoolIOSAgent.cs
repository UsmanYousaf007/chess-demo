#if UNITY_IPHONE || UNITY_IOS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using AOT;

public class MegacoolIOSAgent : MegacoolIAgent {
    [DllImport("__Internal")]
    private static extern void mcl_init_capture(int width, int height, string graphicsDeviceType);

    [DllImport("__Internal")]
    private static extern void mcl_set_capture_texture(IntPtr texturePointer);

    [DllImport("__Internal")]
    private static extern IntPtr mcl_get_unity_render_event_pointer();

    [DllImport("__Internal")]
    private static extern void mcl_set_texture_read_complete_callback([MarshalAs(UnmanagedType.FunctionPtr)] Megacool.TextureReadComplete callbackPointer);

    [DllImport("__Internal")]
    private static extern void startWithAppConfig(string config);

    [DllImport("__Internal")]
    private static extern void startRecording();

    [DllImport("__Internal")]
    private static extern void startRecordingWithConfig(string recordingId, Megacool.Crop crop, int maxFrames, int frameRate, double peakLocation, string overflowStrategy);

    [DllImport("__Internal")]
    private static extern void registerScoreChange(int scoreDelta);

    [DllImport("__Internal")]
    private static extern void captureFrame();

    [DllImport("__Internal")]
    private static extern void captureFrameWithConfig(string recordingId, string overflowStrategy, Megacool.Crop crop, bool forceAdd, int maxFrames, int frameRate);

    [DllImport("__Internal")]
    private static extern void pauseRecording();

    [DllImport("__Internal")]
    private static extern void stopRecording();

    [DllImport("__Internal")]
    private static extern void deleteRecording(string recordingId);

    [DllImport("__Internal")]
    private static extern void deleteShares(IntPtr filter);

    [MonoPInvokeCallback(typeof(Func<MegacoolShare, bool>))]
    private static bool DeleteSharesFilter(Megacool.MegacoolShareData shareData) {
        return deleteSharesFilter(new MegacoolShare(shareData));
    }

    [DllImport("__Internal")]
    private static extern IntPtr getPreviewDataForRecording(string recordingId);

    [DllImport("__Internal")]
    private static extern void getShares();

    [DllImport("__Internal")]
    private static extern void presentShare();

    [DllImport("__Internal")]
    private static extern void mclFree(IntPtr stringPtr);

    [DllImport("__Internal")]
    private static extern void presentShareWithConfig(string recordingId, string lastFrameOverlay, string fallbackImage, string url, string data);

    [DllImport("__Internal")]
    private static extern void presentShareToMessenger();

    [DllImport("__Internal")]
    private static extern void presentShareToMessengerWithConfig(string recordingId, string lastFrameOverlay, string fallbackImage, string url, string data);

    [DllImport("__Internal")]
    private static extern void presentShareToTwitter();

    [DllImport("__Internal")]
    private static extern void presentShareToTwitterWithConfig(string recordingId, string lastFrameOverlay, string fallbackImage, string url, string data);

    [DllImport("__Internal")]
    private static extern void presentShareToMessages();

    [DllImport("__Internal")]
    private static extern void presentShareToMessagesWithConfig(string recordingId, string lastFrameOverlay, string fallbackImage, string url, string data);

    [DllImport("__Internal")]
    private static extern void presentShareToMail();

    [DllImport("__Internal")]
    private static extern void presentShareToMailWithConfig(string recordingId, string lastFrameOverlay, string fallbackImage, string url, string data);

    [DllImport("__Internal")]
    private static extern void setSharingText(string text);

    [DllImport("__Internal")]
    private static extern IntPtr getSharingText();

    [DllImport("__Internal")]
    private static extern void setFrameRate(float frameRate);

    [DllImport("__Internal")]
    private static extern float getFrameRate();

    [DllImport("__Internal")]
    private static extern void setPlaybackFrameRate(float frameRate);

    [DllImport("__Internal")]
    private static extern float getPlaybackFrameRate();

    [DllImport("__Internal")]
    private static extern void setMaxFrames(int maxFrames);

    [DllImport("__Internal")]
    private static extern int getMaxFrames();

    [DllImport("_Internal")]
    private static extern int getNumberOfFrames(string recordingId);

    [DllImport("__Internal")]
    private static extern void setPeakLocation(double peakLocation);

    [DllImport("__Internal")]
    private static extern double getPeakLocation();

    [DllImport("__Internal")]
    private static extern void setLastFrameDelay(int delay);

    [DllImport("__Internal")]
    private static extern int getLastFrameDelay();

    [DllImport("__Internal")]
    private static extern void setLastFrameOverlay(string lastFrameOverlay);

    [DllImport("__Internal")]
    private static extern void setDebugMode(bool debugMode);

    [DllImport("__Internal")]
    private static extern bool getDebugMode();

    [DllImport("__Internal")]
    private static extern void setKeepCompletedRecordings(bool keep);

    [DllImport("__Internal")]
    private static extern void submitDebugDataWithMessage(string message);

    [DllImport("__Internal")]
    private static extern void resetIdentity();

    [DllImport("__Internal")]
    private static extern void setGIFColorTable(int gifColorTable);

    [DllImport("__Internal")]
    private static extern void setMegacoolDidCompleteShareDelegate(IntPtr f);

    [DllImport("__Internal")]
    private static extern void setMegacoolDidDismissShareDelegate(IntPtr f);

    [DllImport("__Internal")]
    private static extern void manualApplicationDidBecomeActive();

    [DllImport("__Internal")]
    private static extern void setOnLinkClickedEventDelegate(IntPtr f);

    private delegate void OnLinkClickedEventDelegate(Megacool.MegacoolLinkClickedEvent e);

    [MonoPInvokeCallback(typeof(OnLinkClickedEventDelegate))]
    private static void OnLinkClickedEvent(Megacool.MegacoolLinkClickedEvent e) {
        if (Megacool.Instance.EventHandler == null) {
            return;
        }
        Megacool.Instance.EventHandler(new MegacoolEvent(e));
    }

    [DllImport("__Internal")]
    private static extern void setOnReceivedShareOpenedEventDelegate(IntPtr f);

    private delegate void OnReceivedShareOpenedEventDelegate(Megacool.MegacoolReceivedShareOpenedEvent e);

    [MonoPInvokeCallback(typeof(OnReceivedShareOpenedEventDelegate))]
    private static void OnReceivedShareOpenedEvent(Megacool.MegacoolReceivedShareOpenedEvent e) {
        if (Megacool.Instance.EventHandler == null) {
            return;
        }
        Megacool.Instance.EventHandler(new MegacoolEvent(e));
    }

    [DllImport("__Internal")]
    private static extern void setOnSentShareOpenedEventDelegate(IntPtr f);

    private delegate void OnSentShareOpenedEventDelegate(Megacool.MegacoolSentShareOpenedEvent e);

    [MonoPInvokeCallback(typeof(OnSentShareOpenedEventDelegate))]
    private static void OnSentShareOpenedEvent(Megacool.MegacoolSentShareOpenedEvent e) {
        if (Megacool.Instance.EventHandler == null) {
            return;
        }
        Megacool.Instance.EventHandler(new MegacoolEvent(e));
    }

    [DllImport("__Internal")]
    private static extern void setOnRetrievedSharesDelegate(IntPtr f);

    private delegate void OnRetrievedSharesDelegate(/*MegacoolShareData[]*/ IntPtr shares, int size);

    [MonoPInvokeCallback(typeof(OnRetrievedSharesDelegate))]
    private static void OnRetrievedShares(IntPtr shares, int size) {
        long longPtr = shares.ToInt64();

        var shs = new List<MegacoolShare>(size);

        for (int i = 0; i < size; i++) {
            IntPtr structPtr = new IntPtr(longPtr);
            Megacool.MegacoolShareData shareData = (Megacool.MegacoolShareData)Marshal.PtrToStructure(structPtr, typeof(Megacool.MegacoolShareData));
            longPtr += Marshal.SizeOf(typeof(Megacool.MegacoolShareData));
            shs.Add(new MegacoolShare(shareData));
        }

        Megacool.Instance.OnSharesRetrieved(shs);
    }

    private static Func<MegacoolShare, bool> deleteSharesFilter = delegate(MegacoolShare arg) {
        return true;
    };

#region Delegates
    private delegate void MegacoolDidCompleteShareDelegate();

    private delegate void MegacoolDidDismissShareDelegate();

    private delegate void EventHandlerDelegate(IntPtr jsonData, int length);

    [MonoPInvokeCallback(typeof(MegacoolDidCompleteShareDelegate))]
    static void DidCompleteShare() {
        Megacool.Instance.CompletedSharing();
    }

    [MonoPInvokeCallback(typeof(MegacoolDidDismissShareDelegate))]
    static void DidDismissShare() {
        Megacool.Instance.DismissedSharing();
    }
#endregion

    // Used to preserve functionality from deprecated ShareConfig lastFrameOverlay
    private void SetLastFrameOverlayFromShareConfig(MegacoolShareConfig config) {
        #pragma warning disable 618
        if (config.LastFrameOverlay != null){
            SetLastFrameOverlay(config.LastFrameOverlay);
        }
        #pragma warning restore 618
    }

    //****************** API Implementation  ******************//
    public void Start(Action<MegacoolEvent> eventHandler) {
        startWithAppConfig(MegacoolConfiguration.Instance.appConfigIos);
        manualApplicationDidBecomeActive();
    }

    public void StartRecording() {
        startRecording();
    }

    public void StartRecording(MegacoolRecordingConfig config) {
        startRecordingWithConfig(config.RecordingId, new Megacool.Crop(new Rect(0,0,0,0)), config.MaxFrames, config.FrameRate, config.PeakLocation, config.OverflowStrategy.ToString());
    }

    public void RegisterScoreChange(int scoreDelta) {
        registerScoreChange(scoreDelta);
    }

    public void CaptureFrame() {
        captureFrame();
    }

    public void CaptureFrame(MegacoolFrameCaptureConfig config) {
        captureFrameWithConfig(config.RecordingId, config.OverflowStrategy.ToString(), new Megacool.Crop(new Rect(0,0,0,0)), config.ForceAdd, config.MaxFrames, config.FrameRate);
    }

    public void SetCaptureMethod(MegacoolCaptureMethod captureMethod, RenderTexture renderTexture) {
        if (captureMethod == MegacoolCaptureMethod.SCREEN){
            mcl_set_capture_texture(IntPtr.Zero);
        } else {
            SignalRenderTexture(renderTexture);
        }
    }

    public void PauseRecording() {
        pauseRecording();
    }

    public void StopRecording() {
        stopRecording();
    }

    public void DeleteRecording(string recordingId) {
        deleteRecording(recordingId);
    }

    public void DeleteShares(Func<MegacoolShare, bool> filter) {
        deleteSharesFilter = filter;
        deleteShares(Marshal.GetFunctionPointerForDelegate(new Func<Megacool.MegacoolShareData, bool>(DeleteSharesFilter)));
    }

    public MegacoolPreviewData GetPreviewDataForRecording(string recordingId) { 
        IntPtr previewInfoJsonPtr = getPreviewDataForRecording(recordingId);
        string previewInfoJson = Marshal.PtrToStringAnsi(previewInfoJsonPtr);
        mclFree(previewInfoJsonPtr);

        if (string.IsNullOrEmpty(previewInfoJson)) {
            return null;
        }

        var dict = MegacoolThirdParty_MiniJSON.Json.Deserialize(previewInfoJson) as Dictionary<string, object>;
        return new MegacoolPreviewData(dict);
    }

    public int GetNumberOfFrames(string recordingId) {
        return getNumberOfFrames(recordingId);
    }

    public void GetShares(Action<List<MegacoolShare>> shares = null, Func<MegacoolShare, bool> filter = null) {
        getShares();
    }

    public void Share() {
        presentShare();
    }

    public void Share(MegacoolShareConfig config) {
        // keep functionality until lastframeOverlay in shareConfig is removed
        SetLastFrameOverlayFromShareConfig(config);
        #pragma warning disable 618
        presentShareWithConfig(config.RecordingId, config.LastFrameOverlay, config.FallbackImage, config.Url.ToString(), config.Data != null ? MegacoolThirdParty_MiniJSON.Json.Serialize(config.Data) : null);
        #pragma warning restore 618
    }

    public void ShareToMessenger() {
        presentShareToMessenger();
    }

    public void ShareToMessenger(MegacoolShareConfig config) {
        // keep functionality until lastframeOverlay in shareConfig is removed
        SetLastFrameOverlayFromShareConfig(config);
        #pragma warning disable 618
        presentShareToMessengerWithConfig(config.RecordingId, config.LastFrameOverlay, config.FallbackImage, config.Url.ToString(), config.Data != null ? MegacoolThirdParty_MiniJSON.Json.Serialize(config.Data) : null);
        #pragma warning restore 618
    }

    public void ShareToTwitter() {
        presentShareToTwitter();
    }

    public void ShareToTwitter(MegacoolShareConfig config) {
        // keep functionality until lastframeOverlay in shareConfig is removed
        SetLastFrameOverlayFromShareConfig(config);
        #pragma warning disable 618
        presentShareToTwitterWithConfig(config.RecordingId, config.LastFrameOverlay, config.FallbackImage, config.Url.ToString(), config.Data != null ? MegacoolThirdParty_MiniJSON.Json.Serialize(config.Data) : null);
        #pragma warning restore 618
    }

    public void ShareToMessages() {
        presentShareToMessages();
    }

    public void ShareToMessages(MegacoolShareConfig config) {
        // keep functionality until lastframeOverlay in shareConfig is removed
        SetLastFrameOverlayFromShareConfig(config);
        #pragma warning disable 618
        presentShareToMessagesWithConfig(config.RecordingId, config.LastFrameOverlay, config.FallbackImage, config.Url.ToString(), config.Data != null ? MegacoolThirdParty_MiniJSON.Json.Serialize(config.Data) : null);
        #pragma warning restore 618
    }

    public void ShareToMail() {
        presentShareToMail();
    }

    public void ShareToMail(MegacoolShareConfig config) {
        // keep functionality until lastframeOverlay in shareConfig is removed
        SetLastFrameOverlayFromShareConfig(config);
        #pragma warning disable 618
        presentShareToMailWithConfig(config.RecordingId, config.LastFrameOverlay, config.FallbackImage, config.Url.ToString(), config.Data != null ? MegacoolThirdParty_MiniJSON.Json.Serialize(config.Data) : null);
        #pragma warning restore 618
    }

    public void SetSharingText(string text) {
        setSharingText(text);
    }

    public string GetSharingText() {
        IntPtr sharingTextPtr = getSharingText();
        string sharingText = Marshal.PtrToStringAnsi(sharingTextPtr);
        mclFree(sharingTextPtr);
        return sharingText;
    }

    public void SetFrameRate(float frameRate) {
        setFrameRate(frameRate);
    }

    public float GetFrameRate() {
        return getFrameRate();
    }

    public void SetPlaybackFrameRate(float frameRate) {
        setPlaybackFrameRate(frameRate);
    }

    public float GetPlaybackFrameRate() {
        return getPlaybackFrameRate();
    }

    public void SetMaxFrames(int maxFrames) {
        setMaxFrames(maxFrames);
    }

    public int GetMaxFrames() {
        return getMaxFrames();
    }

    public void SetPeakLocation(double peakLocation) {
        setPeakLocation(peakLocation);
    }

    public double GetPeakLocation() {
        return getPeakLocation();
    }

    public void SetLastFrameDelay(int delay) {
        setLastFrameDelay(delay);
    }

    public int GetLastFrameDelay() {
        return getLastFrameDelay();
    }

    public void SetLastFrameOverlay(string lastFrameOverlay) {
        String path = Application.streamingAssetsPath + "/" + lastFrameOverlay;
        setLastFrameOverlay(path);
    }

    public void SetDebugMode(bool debugMode) {
        setDebugMode(debugMode);
    }

    public bool GetDebugMode() {
        return getDebugMode();
    }

    public void SetKeepCompletedRecordings(bool keep) {
        setKeepCompletedRecordings(keep);
    }

    public void SubmitDebugData(string message) {
        submitDebugDataWithMessage(message);
    }

    public void ResetIdentity() {
        resetIdentity();
    }

    public void SetGIFColorTable(Megacool.GifColorTableType gifColorTable) {
        int iosValue = 0; // dynamic
        switch (gifColorTable) {
        case Megacool.GifColorTableType.GifColorTableFixed:
            iosValue = 1;
            break;
        case Megacool.GifColorTableType.GifColorTableAnalyzeFirst:
            iosValue = 2;
            break;
        }
        setGIFColorTable(iosValue);
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
        MegacoolDidCompleteShareDelegate didCompleteShareDelegate = new MegacoolDidCompleteShareDelegate(DidCompleteShare);
        MegacoolDidDismissShareDelegate didDismissShareDelegate = new MegacoolDidDismissShareDelegate(DidDismissShare);

        setMegacoolDidCompleteShareDelegate(Marshal.GetFunctionPointerForDelegate(didCompleteShareDelegate));
        setMegacoolDidDismissShareDelegate(Marshal.GetFunctionPointerForDelegate(didDismissShareDelegate));

        OnLinkClickedEventDelegate onLinkClickedEventDelegate = new OnLinkClickedEventDelegate(OnLinkClickedEvent);
        OnReceivedShareOpenedEventDelegate onReceivedShareOpenedEventDelegate = new OnReceivedShareOpenedEventDelegate(OnReceivedShareOpenedEvent);
        OnSentShareOpenedEventDelegate onSentShareOpenedEventDelegate = new OnSentShareOpenedEventDelegate(OnSentShareOpenedEvent);

        setOnLinkClickedEventDelegate(Marshal.GetFunctionPointerForDelegate(onLinkClickedEventDelegate));
        setOnReceivedShareOpenedEventDelegate(Marshal.GetFunctionPointerForDelegate(onReceivedShareOpenedEventDelegate));
        setOnSentShareOpenedEventDelegate(Marshal.GetFunctionPointerForDelegate(onSentShareOpenedEventDelegate));

        setOnRetrievedSharesDelegate(Marshal.GetFunctionPointerForDelegate(new OnRetrievedSharesDelegate(OnRetrievedShares)));
    }

    public void InitializeCapture(float scaleFactor, Megacool.TextureReadComplete callback) {
        int width = (int)(Screen.width / scaleFactor);
        int height = (int)(Screen.height / scaleFactor);
        mcl_init_capture(width, height, SystemInfo.graphicsDeviceType.ToString());
        mcl_set_texture_read_complete_callback(callback);
    }
}
#endif
