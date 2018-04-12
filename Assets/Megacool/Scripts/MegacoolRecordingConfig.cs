using UnityEngine;

public struct MegacoolRecordingConfig {

    // Cropping will be added in newer versions
    // public Rect Crop { get; set; }

    private MegacoolOverflowStrategy overflowStrategy;

    public MegacoolOverflowStrategy OverflowStrategy {
        get {
            return overflowStrategy = overflowStrategy ?? MegacoolOverflowStrategy.LATEST;
        }
        set {
            overflowStrategy = value;
        }
    }

    public string RecordingId { get; set; }

    public int MaxFrames { get; set; }

    public double PeakLocation { get; set; }

    public int FrameRate { get; set; }

    public int PlaybackFrameRate { get; set; }

    public int LastFrameDelay { get; set; }

    /// <summary>
    /// Configuration options for recording a GIF
    /// </summary>
    /// <param name="overflowStrategy">The MegacoolOverflowStrategy to use while recording.</param>
    /// <param name="recordingId">The recording identifier to assign to the recording.</param>
    /// <param name="maxFrames">The maximum number of frames to store in the recording.</param>
    /// <param name="peakLocation">The percentage within the recording that represents the point of most interest.</param>
    /// <param name="lastFrameDelay">The delay (in milliseconds) to wait on the last frame before replaying. Setting
    /// this to 0 means the last frame will have the same duration as the other frames.</param>
    /// <param name="playbackFrameRate">The framerate at which to replay the recording.</param>
    /// <param name="frameRate">The framerate at which to record the GIF.</param>
    /// </summary>
    public MegacoolRecordingConfig(
//            Rect crop = default(Rect),
            MegacoolOverflowStrategy overflowStrategy = default(MegacoolOverflowStrategy),
            string recordingId = default(string),
            int maxFrames = 0,
            double peakLocation = -42,
            int lastFrameDelay = -42,
            int playbackFrameRate = 0,
            int frameRate = 0) {
        this.overflowStrategy = overflowStrategy;
//        Crop = crop;
        RecordingId = recordingId;
        MaxFrames = maxFrames;
        PeakLocation = peakLocation;
        LastFrameDelay = lastFrameDelay;
        PlaybackFrameRate = playbackFrameRate;
        FrameRate = frameRate;
        OverflowStrategy = overflowStrategy;
    }

    public void SetDefaults() {
        // Load parameters from the global defaults where unset or invalid. Needs to be done outside constructor since
        // constructor might not run if initialized with an object initializer.
        if (MaxFrames <= 0) {
            MaxFrames = Megacool.Instance.MaxFrames;
        }

        if (PeakLocation <= 0d || PeakLocation > 1d) {
            //if set <0d, this ignores valid numbers passed in through the panel because the default null value is 0.0
            //a valid zero value can be set from the config panel and using RecordingConfig by using initializer to
            //create the config
            PeakLocation = (double)Megacool.Instance.PeakLocation;
        }

        if (FrameRate <= 0) {
            FrameRate = (int)Megacool.Instance.FrameRate;
        }

        if (PlaybackFrameRate <= 0) {
            PlaybackFrameRate = (int)Megacool.Instance.PlaybackFrameRate;
        }

        if (LastFrameDelay < 0) {
            LastFrameDelay = Megacool.Instance.LastFrameDelay;
        }

        if (!OverflowStrategy.Equals(MegacoolOverflowStrategy.LATEST)
            && !OverflowStrategy.Equals(MegacoolOverflowStrategy.HIGHLIGHT)
                && !OverflowStrategy.Equals(MegacoolOverflowStrategy.TIMELAPSE)){
                OverflowStrategy = MegacoolOverflowStrategy.LATEST;
        }
    }
}
