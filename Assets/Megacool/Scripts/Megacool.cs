using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using AOT;
using System.Collections.Generic;
using System.Threading;


public enum MegacoolCaptureMethod {
    BLIT,
    SCREEN,
    RENDER,
}


/// <summary>
/// Not all iOS sharing channels in the native share modal support both links and GIFs. The SharingStrategy
/// sets what should be prioritized. This will only affect the `Share` method, not the custom channels like
/// `ShareToMessenger`, ShareToMessages`,`ShareToTwitter` and `ShareToEmail`.
/// </summary>
public enum MegacoolSharingStrategy {
    /// <summary>
    /// Prioritize GIFs (this is the default).
    /// </summary>
    GIF,

    /// <summary>
    /// Prioritize links. This setting currently only affects WhatsApp.
    /// </summary>
    LINK,
}


public sealed class Megacool {

    [StructLayout(LayoutKind.Sequential)]
    public struct MegacoolLinkClickedEvent {
        public int isFirstSession;
        public string userId;
        public string shareId;
        public string url;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MegacoolReceivedShareOpenedEvent {
        public string userId;
        public string shareId;
        public int state;
        public double createdAt;
        public double updatedAt;
        public IntPtr dataBytes;
        public int dataLength;
        public string url;
        public int isFirstSession;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MegacoolSentShareOpenedEvent {
        public string userId;
        public string shareId;
        public int state;
        public double createdAt;
        public double updatedAt;
        public string receiverUserId;
        public string url;
        public int isFirstSession;
        public IntPtr eventDataBytes;
        public int eventDataLength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MegacoolShareData {
        public string userId;
        public string shareId;
        public int state;
        public double createdAt;
        public double updatedAt;
        public IntPtr dataBytes;
        public int dataLength;
    }


    /// <summary>
    /// How the colors in the GIF should be computed.
    /// </summary>
    public enum GifColorTableType {
        /// <summary>
        /// A fixed set of colors is used. This is very fast, but sacrifices quality for nuanced colors and gradients.
        /// </summary>
        GifColorTableFixed,

        /// <summary>
        /// Analyze the frames first. This algorithm is largely equivalent to dynamic, but uses a bit more memory.
        /// Which is faster depends on workload.
        /// </summary>
        /// <description>
        /// This is only available on iOS, on Android this is the same as dynamic.
        /// </description>
        GifColorTableAnalyzeFirst,

        /// <summary>
        /// A subset of the frames is analyzed first. This is the default and yields a good balance between quality and
        /// speed.
        /// </summary>
        GifColorTableDynamic,
    }

#pragma warning disable 0414
    [StructLayout(LayoutKind.Sequential)]
    public struct Crop {
        float x;
        float y;
        float width;
        float height;

        public Crop(Rect rect) {
            this.x = rect.x;
            this.y = rect.y;
            this.width = rect.width;
            this.height = rect.height;
        }
    }
#pragma warning restore 0414

#region Platform Agent
    private MegacoolIAgent _platformAgent;
#endregion

#region Instance

    private static readonly Megacool instance = new Megacool();

    private Megacool() {
#if UNITY_EDITOR
  _platformAgent = new MegacoolEditorAgent();
#elif (UNITY_IPHONE || UNITY_IOS)
  _platformAgent = new MegacoolIOSAgent();
#elif UNITY_ANDROID
  _platformAgent = new MegacoolAndroidAgent ();
#else
  _platformAgent = new MegacoolUnsupportedAgent();
#endif
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static Megacool Instance {
        get {
            return instance;
        }
    }

#endregion

#region Delegates

    private delegate void EventHandlerDelegate(IntPtr jsonData, int length);

    public static Action<List<MegacoolEvent>> OnMegacoolEvents = delegate {};

    public static Action<MegacoolEvent> OnReceivedShareOpened = delegate {};

    public static Action<MegacoolEvent> OnLinkClicked = delegate {};

    public static Action<MegacoolEvent> OnSentShareOpened = delegate {};

    /// <summary>
    /// Callback when a user has completed a share. On Android this is only available for API level 22+.
    /// </summary>
    /// <example>
    /// Megacool.Instance.CompletedSharing += () => {
    ///     Debug.Log("User completed sharing");
    /// }
    /// </example>
    public Action CompletedSharing = delegate {
    };

    /// <summary>
    /// Callback when a user has aborted (dismissed) a share. On Android this is only available for API level 22+.
    /// </summary>
    /// <example>
    /// Megacool.Instance.DismissedSharing += () => {
    ///     Debug.Log("User dismissed sharing");
    /// }
    /// </example>
    public Action DismissedSharing = delegate {
    };

    /// <summary>
    /// Callback when a user either aborted or completed a share, but we can't know which.
    /// </summary>
    /// <description>
    /// This is only called on Android, when we cannot tell whether the share actually completed or not.
    /// </description>
    public Action PossiblyCompletedSharing = delegate {
    };

    public Action<List<MegacoolShare>> OnSharesRetrieved = delegate {
    };

#endregion

#region Properties

    private const int MCRS = 0x6d637273;
    private IntPtr nativePluginCallbackPointer;

    /// <summary>
    /// Set the text to be shared of different channels.
    /// </summary>
    /// <remarks>
    /// The text should be set before Share() is called.
    /// </remarks>
    /// <value>The sharing text.</value>
    public string SharingText {
        set {
            _platformAgent.SetSharingText(value);
        }
        get {
            return _platformAgent.GetSharingText();
        }
    }

    /// <summary>
    /// Set number of frames per second to record.
    /// </summary>
    /// <remarks>
    /// Default is 10 frames / second. The GIF will be recorded with this frame rate.
    /// </remarks>
    /// <value>The frame rate.</value>
    public float FrameRate {
        set {
            _platformAgent.SetFrameRate(value);
        }
        get {
            return _platformAgent.GetFrameRate();
        }
    }

    /// <summary>
    /// Set number of frames per second to play.
    /// </summary>
    /// <remarks>
    /// Default is 10 frames / second. The GIF will be exported with this frame rate.
    /// </remarks>
    /// <value>The playback frame rate.</value>
    public float PlaybackFrameRate {
        set {
            _platformAgent.SetPlaybackFrameRate(value);
        }
        get {
          return _platformAgent.GetPlaybackFrameRate();
        }
    }

    /// <summary>
    /// Max number of frames on the buffer.
    /// </summary>
    /// <remarks>
    /// Default is 50 frames.
    /// </remarks>
    /// <value>Max frames.</value>
    public int MaxFrames {
        set {
            _platformAgent.SetMaxFrames(value);
        }
        get {
            return _platformAgent.GetMaxFrames();
        }
    }

    /// <summary>
    /// Location in recording where max number of points should occur
    /// </summary>
    /// <remarks>
    /// Default is 0.7 (70% of the way through the recording)
    /// </remarks>
    /// <value>Peak location.</value>
    public double PeakLocation {
        set {
            _platformAgent.SetPeakLocation(value);
        }
        get {
            return _platformAgent.GetPeakLocation();
        }
    }

    /// <summary>
    /// Set a delay (in seconds) on the last frame in the animation.
    /// </summary>
    /// <remarks>
    /// Default is 1 second
    /// </remarks>
    /// <value>The last frame delay.</value>
    public int LastFrameDelay {
        set {
            _platformAgent.SetLastFrameDelay(value);
        }
        get {
            return _platformAgent.GetLastFrameDelay();
        }
    }

    /// <summary>
    /// Overlay an image over the last frame of the GIF.
    /// </summary>
    /// <remarks>
    /// Default is none. The path should be relative to the StreamingAssets directory.
    ///
    /// To show the overlay on previews as well you need to set includeLastFrameOverlay=true
    /// on the PreviewConfig.
    /// </remarks>
    /// <value>The path to the last frame overlay</value>
    public string LastFrameOverlay {
        set {
            _platformAgent.SetLastFrameOverlay(value);
        }
    }

    /// <summary>
    /// Set the type of GIF color table to use. Default is fixed 256 colors.
    /// </summary>
    /// <remarks>
    /// It's recommended to test all to see which gives the best result. It depends on the color usage in the app.
    /// </remarks>
    /// <value>The gif color table type</value>
    public GifColorTableType GifColorTable {
        set {
            _platformAgent.SetGIFColorTable(value);
        }
    }

    /// <summary>
    /// Set whether to prioritize GIFs or link when sharing to channels that support either but not both.
    /// </summary>
    /// <value>The sharing strategy to use</value>
    public MegacoolSharingStrategy SharingStrategy {
        set {
            _platformAgent.SetSharingStrategy(value);
        }
    }

    /// <summary>
    /// Turn on / off debug mode. In debug mode calls to the SDK are stored and can be submitted to the core developers using SubmitDebugData later.
    /// </summary>
    /// <value><c>true</c> if debug mode; otherwise, <c>false</c>.</value>
    public static bool Debug {
        set {
            MegacoolConfiguration.Instance.debugMode = value;
            Instance._platformAgent.SetDebugMode(value);
        }
        get {
            return MegacoolConfiguration.Instance.debugMode;
        }
    }

    /// <summary>
    /// Whether to keep completed recordings around.
    /// </summary>
    /// <description>
    /// The default is false, which means that all completed recordings will be deleted
    /// whenever a new recording is started with either <c>captureFrame</c> or <c>startRecording</c>.
    /// Setting this to <c>true</c> means we will never delete a completed recording, which is what you want if you want to
    /// enable player to browse previous GIFs they've created. A completed recording will still be
    /// overwritten if a new recording is started with the same <c>recordingId</c>
    /// </description>
    /// <value><c>true</c> to keep completed recordings; otherwise, <c>false</c>.</value>
    public bool KeepCompletedRecordings {
        set {
            _platformAgent.SetKeepCompletedRecordings(value);
        }
    }

    public Action<MegacoolEvent> EventHandler { get; private set; }

    private float ScaleFactor = getDefaultScaleFactor();

    private static float getDefaultScaleFactor() {
        // Default to half of the screen size to strike a balance between quality and memory
        // usage/performance, while ensuring at least 200x200 for compatibility with Facebook
        int shortestEdge = Math.Min(Screen.width, Screen.height);
        int targetShortestEdge = Math.Max(shortestEdge / 2, 200);

        // Screen sizes bigger than 1500px (like iPad mini 3) will be divided by 4
        if (shortestEdge > 1500) {
            targetShortestEdge = shortestEdge / 4;
        }
        return (float)shortestEdge / targetShortestEdge;
    }

    private RenderTexture renderTexture;

    public RenderTexture RenderTexture {
        get {
            if (!renderTexture) {
                int width = (int)(Screen.width / ScaleFactor);
                int height = (int)(Screen.height / ScaleFactor);

                renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                renderTexture.filterMode = FilterMode.Point;
            }
            if (!renderTexture.IsCreated()) {
                // The texture can become lost on level reloads, ensure it's recreated
                renderTexture.Create();

                if (CaptureMethod != MegacoolCaptureMethod.SCREEN) {
                    _platformAgent.SignalRenderTexture(renderTexture);
                }
            }

            return renderTexture;
        }
    }

    private MegacoolCaptureMethod captureMethod = MegacoolCaptureMethod.SCREEN;

    /// <summary>
    /// Set how frames should be captured.
    /// </summary>
    /// <value>The capture method.</value>
    public MegacoolCaptureMethod CaptureMethod {
        get {
            // SCREEN is only compatible with OpenGL ES 3 or newer, fall back to blitting if unsupported.
            if (captureMethod == MegacoolCaptureMethod.SCREEN &&
                    SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3) {
                return MegacoolCaptureMethod.BLIT;
            }
            return captureMethod;
        }
        set {
            captureMethod = value;
            if (!hasStarted) {
                // Only communicate changes if it's already set, otherwise it'll be initialized with the correct method.
                return;
            }
            _platformAgent.SetCaptureMethod(CaptureMethod, renderTexture);
        }
    }


    private bool _isRecording = false;
    public bool IsRecording {
        get {
            return _isRecording;
        }
    }

#endregion

#region Functionality

    private void InitializeSharingDelegate() {
        _platformAgent.InitializeSharingDelegate();
    }

    private void SetupDefaultConfiguration() {
        SharingText = MegacoolConfiguration.Instance.sharingText;
        FrameRate = MegacoolConfiguration.Instance.recordingFrameRate;
        PlaybackFrameRate = MegacoolConfiguration.Instance.playbackFrameRate;
        MaxFrames = MegacoolConfiguration.Instance.maxFrames;
        PeakLocation = MegacoolConfiguration.Instance.peakLocation;
        LastFrameDelay = MegacoolConfiguration.Instance.lastFrameDelay;
        GifColorTable = MegacoolConfiguration.Instance.gifColorTable;
    }

    private void CreateMainThreadAction(MegacoolEvent megacoolEvent) {

        // Call the appropriate delegate
        switch (megacoolEvent.Type) {

        // This device has received a share to the app, including a share object
        case MegacoolEvent.MegacoolEventType.ReceivedShareOpened:
            OnReceivedShareOpened(megacoolEvent);
            break;

            // The app has been opened from a link click, send the user instantly to
            // the right scene if the URL path exists
        case MegacoolEvent.MegacoolEventType.LinkClicked:
            OnLinkClicked(megacoolEvent);
            break;

            // A Friend has received a share from your device
        case MegacoolEvent.MegacoolEventType.SentShareOpened:
            OnSentShareOpened(megacoolEvent);
            break;
        }

        // Call an umbrella delegate to handle all the events
        List<MegacoolEvent> allCurrentEvents = new List<MegacoolEvent> ();
        allCurrentEvents.Add(megacoolEvent);
        OnMegacoolEvents(allCurrentEvents);
        allCurrentEvents.Clear ();
    }

    private bool hasStarted = false;

    /// <summary>
    /// Deprecated initialization of SDK with an event handler
    /// </summary>
    [System.Obsolete("Use Start() and add callbacks to their respective delegates. See https://docs.megacool.co/customize")]
    public void Start(Action<MegacoolEvent> eventHandler) {
        _Start(eventHandler);
    }


    /// <summary>
    /// Initialize the SDK.
    /// </summary>
    public void Start() {
         // Create a main thread action for every asynchronous callback
        Action<MegacoolEvent> eventHandler = ((MegacoolEvent e) => Megacool.Instance.CreateMainThreadAction (e));
        _Start(eventHandler);
    }

    // Temporary extracted method to initialize SDK until deprecated Start is removed
    private void _Start(Action<MegacoolEvent> eventHandler){
        if (hasStarted) {
            // Allowing multiple initializations would make it hard to maintain both thread-safety and performance
            // of the underlying capture code, and doesn't have any good use case for allowing it, thus ignoring.
            UnityEngine.Debug.Log("Megacool: Skipping duplicate init");
            return;
        }
        hasStarted = true;

        // Set debugging first so that it can be enabled before initializing the native SDK
        Debug = MegacoolConfiguration.Instance.debugMode;

        EventHandler = eventHandler;

        // Delegates must be initialized before start() since start() might trigger the event callbacks.
        InitializeSharingDelegate();

        _platformAgent.Start(eventHandler);

        SetupDefaultConfiguration();

        _platformAgent.InitializeCapture(ScaleFactor, TextureReadCompleteCallback);
        IssuePluginEvent(MCRS);
        _platformAgent.SignalRenderTexture(renderTexture);
    }

    public void IssuePluginEvent(int eventId) {
        _platformAgent.IssuePluginEvent(ref nativePluginCallbackPointer, eventId);
    }

    /// <summary>
    /// Start recording a GIF
    /// </summary>
    /// <remarks>
    /// This will keep a buffer of 50 frames (default). The frames are overwritten until <c>StopRecording</c> gets called.
    /// </remarks>
    public void StartRecording() {
        InitializeManager();
        _platformAgent.StartRecording();
        _isRecording = true;
        SafeReleaseTextureReady();
    }

    private void SafeReleaseTextureReady () {
        // Release the TextureReady without throwing if already at max capacity.
        try {
            TextureReady.Release();
#pragma warning disable 0168
        } catch (SemaphoreFullException e) {
#pragma warning restore 0168
            // Ignore
        }
    }

    /// <summary>
    /// Start customized GIF recording.
    /// </summary>
    /// <remarks>
    /// This will keep a buffer of 50 frames (default). The frames are overwritten until <c>StopRecording</c> gets called.
    /// </remarks>
    /// <param name="config">Config to customize the recording.</param>
    public void StartRecording(MegacoolRecordingConfig config) {
        config.SetDefaults();
        InitializeManager();
        _platformAgent.StartRecording(config);
        _isRecording = true;
        SafeReleaseTextureReady();
    }

    private void InitializeManager() {
        MegacoolManager manager = null;
        foreach (Camera cam in Camera.allCameras) {
            MegacoolManager foundManager = cam.GetComponent<MegacoolManager>();
            if (foundManager) {
                manager = foundManager;
                break;
            }
        }
        if (!manager) {
            Camera mainCamera = Camera.main;
            if (!mainCamera) {
                UnityEngine.Debug.Log("No MegacoolManager already in the scene and no main camera to attach to, " +
                    "either attach it manually to a camera or tag one of the cameras as MainCamera");
                return;
            }
            mainCamera.gameObject.AddComponent<MegacoolManager>();
            manager = mainCamera.GetComponent<MegacoolManager>();
        }
        // Doing an explicit initialize ensures that if the capture method was customized the changes
        // are respected even if the manager was explicitly added to a camera and thus awoke before the
        // capture method was set.
        manager.Initialize();
    }

    /// <summary>
    /// Note an event for highlight recording
    /// </summary>
    /// <remarks>
    /// For highlight recording use only. Calling this function when something interesting occurs means that the end
    /// recording will focus on the stretch of gameplay with the most interesting events logged.
    /// </remarks>
    public void RegisterScoreChange() {
        RegisterScoreChange(1);
    }

    /// <summary>
    /// Note a change in score for highlight recording
    /// </summary>
    /// <remarks>
    /// For highlight recording use only. Calling this function when the score changes means that the end recording
    /// will focus on the stretch of gameplay with the most frequent / highest value events logged.
    /// </remarks>
    public void RegisterScoreChange(int scoreDelta) {
        _platformAgent.RegisterScoreChange(scoreDelta);
    }

    // Indicates whether this frame should be rendered. Used by the custom cameras to detect when CaptureFrame
    // has been called.
    public bool RenderThisFrame = false;

    // Protects access to the render texture. The blit/render cameras wait for this before writing to the texture, and
    // the native library posts to it once a read is finished.
    public Semaphore TextureReady = new Semaphore(1, 1);

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void TextureReadComplete ();

    [MonoPInvokeCallback(typeof(TextureReadComplete))]
    private static void TextureReadCompleteCallback () {
        Megacool.Instance.SafeReleaseTextureReady();
    }


    /// <summary>
    /// Capture a single frame.
    /// </summary>
    /// <remarks>
    /// Capture a single frame to the buffer. The buffer size is 50 frames (default) and the oldest frames will be deleted if the method gets called more than 50 times.
    /// The total number of frames can be customized by setting the <c>MaxFrames</c> property.
    /// </remarks>
    public void CaptureFrame() {
        InitializeManager();
        RenderThisFrame = true;
        _platformAgent.CaptureFrame();
    }

    public void CaptureFrame(MegacoolFrameCaptureConfig config) {
        config.SetDefaults();
        InitializeManager();
        RenderThisFrame = true;
        _platformAgent.CaptureFrame(config);
    }

    public void PauseRecording() {
        _platformAgent.PauseRecording();
        _isRecording = false;
    }

    /// <summary>
    /// Stops the recording.
    /// </summary>
    public void StopRecording() {
        _platformAgent.StopRecording();
        _isRecording = false;
    }

    /// <summary>
    /// Delete a recording
    /// </summary>
    /// <description>
    /// Will remove any frames of the recording in memory and on disk. Both completed and incomplete
    /// recordings will take space on disk, thus particularly if you're using <c>KeepCompletedRecordings = true</c> you might want
    /// to provide an interface to your users for removing recordings they don't care about anymore to free up space for new recordings.
    /// </description>
    /// <param name="recordingId">Recording identifier.</param>
    public void DeleteRecording(string recordingId) {
        _platformAgent.DeleteRecording(recordingId);
    }

    public MegacoolPreviewData GetPreviewDataForRecording(string recordingId) {
        return _platformAgent.GetPreviewDataForRecording(recordingId);
    }

    public int GetNumberOfFrames(string recordingId) {
        return _platformAgent.GetNumberOfFrames(recordingId);
    }

    public void setDefaultLastFrameOverlay(string filename){
        _platformAgent.SetLastFrameOverlay(filename);
    }

    public void GetShares(Action<List<MegacoolShare>> shares) {
        OnSharesRetrieved = shares;
        _platformAgent.GetShares(shares);
    }

    public void DeleteShares(Func<MegacoolShare, bool> filter) {
        _platformAgent.DeleteShares(filter);
    }

    /// <summary>
    /// Share this instance.
    /// </summary>
    public void Share() {
        _platformAgent.Share();
    }

    public void Share(MegacoolShareConfig config) {
        _platformAgent.Share(config);
    }

    public void ShareToMessenger() {
        _platformAgent.ShareToMessenger();
    }

    public void ShareToMessenger(MegacoolShareConfig config) {
        _platformAgent.ShareToMessenger(config);
    }

    public void ShareToTwitter() {
        _platformAgent.ShareToTwitter();
    }

    public void ShareToTwitter(MegacoolShareConfig config) {
        _platformAgent.ShareToTwitter(config);
    }

    public void ShareToMessages() {
        _platformAgent.ShareToMessages();
    }

    public void ShareToMessages(MegacoolShareConfig config) {
        _platformAgent.ShareToMessages(config);
    }

    public void ShareToMail() {
        _platformAgent.ShareToMail();
    }

    public void ShareToMail(MegacoolShareConfig config) {
        _platformAgent.ShareToMail(config);
    }

    public void SubmitDebugData(string message) {
        _platformAgent.SubmitDebugData(message);
    }

    public void ResetIdentity() {
        _platformAgent.ResetIdentity();
    }


#endregion
}
