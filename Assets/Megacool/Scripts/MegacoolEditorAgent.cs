#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using AOT;

public class MegacoolEditorAgent : MegacoolIAgent {

    MegacoolEditorRecordingManager recordingManager;

    private void ImplementationWarning(string message) {
        Debug.LogWarning("Megacool: " + message + " is not implemented in the Editor");
    }

    //****************** API Implementation  ******************//
    public void Start(Action<MegacoolEvent> eventHandler) {
        recordingManager = new MegacoolEditorRecordingManager();
    }

    public void StartRecording() {
        recordingManager.StartRecording();
    }

    public void StartRecording(MegacoolRecordingConfig config) {
        recordingManager.StartRecording(config);
    }

    public void RegisterScoreChange(int scoreDelta) {
        recordingManager.RegisterScoreChange(scoreDelta);
    }

    public void CaptureFrame() {
        MegacoolEditorRecordingManager.CaptureFrame();
    }

    public void CaptureFrame(MegacoolFrameCaptureConfig config) {
        MegacoolEditorRecordingManager.CaptureFrame(config);
    }

    public void SetCaptureMethod(MegacoolCaptureMethod captureMethod, RenderTexture renderTexture) {
        ImplementationWarning("SetCaptureMethod");
    }

    public void PauseRecording() {
        recordingManager.PauseRecording();
    }

    public void StopRecording() {
        recordingManager.StopRecording();
    }

    public void DeleteRecording(string recordingId) {
        recordingManager.DeleteRecording(recordingId);
    }

    public MegacoolPreviewData GetPreviewDataForRecording(string recordingId) {
        return recordingManager.GetPreviewInfoForRecording(recordingId);
    }

    public int GetNumberOfFrames(string recordingId) {
        return recordingManager.GetNumberOfFrames(recordingId);
    }

    public void Share() {
        ImplementationWarning("Share");
    }

    public void Share(MegacoolShareConfig config) {
        ImplementationWarning("Share");
    }

    public void ShareToMessenger() {
        ImplementationWarning("ShareToMessenger");
    }

    public void ShareToMessenger(MegacoolShareConfig config) {
        ImplementationWarning("ShareToMessenger");
    }

    public void ShareToTwitter() {
        ImplementationWarning("ShareToTwitter");
    }

    public void ShareToTwitter(MegacoolShareConfig config) {
        ImplementationWarning("ShareToTwitter");
    }

    public void ShareToMessages() {
        ImplementationWarning("ShareToMessages");
    }

    public void ShareToMessages(MegacoolShareConfig config) {
        ImplementationWarning("ShareToMessages");
    }

    public void ShareToMail() {
        ImplementationWarning("ShareToMail");
    }

    public void ShareToMail(MegacoolShareConfig config) {
        ImplementationWarning("ShareToMail");
    }

    public void GetShares(Action<List<MegacoolShare>> shares, Func<MegacoolShare, bool> filter = null) {
        ImplementationWarning("GetShares");
    }

    public void SetSharingText(string text) {
        ImplementationWarning("SetSharingText");
    }

    public string GetSharingText() {
        ImplementationWarning("GetSharingText");
        return "";
    }

    public void SetFrameRate(float frameRate) {
        MegacoolConfiguration.Instance.recordingFrameRate = frameRate;
    }

    public float GetFrameRate() {
        return MegacoolConfiguration.Instance.recordingFrameRate;
    }

    public void SetPlaybackFrameRate(float frameRate) {
        MegacoolConfiguration.Instance.playbackFrameRate = frameRate;
    }

    public float GetPlaybackFrameRate() {
        return MegacoolConfiguration.Instance.playbackFrameRate;
    }

    public void SetMaxFrames(int maxFrames) {
        MegacoolConfiguration.Instance.maxFrames = maxFrames;
    }

    public int GetMaxFrames() {
        return MegacoolConfiguration.Instance.maxFrames;
    }

    public void SetPeakLocation(double peakLocation) {
        MegacoolConfiguration.Instance.peakLocation = peakLocation;
    }

    public double GetPeakLocation() {
        return MegacoolConfiguration.Instance.peakLocation;
    }

    public void SetLastFrameDelay(int delay) {
        MegacoolConfiguration.Instance.lastFrameDelay = delay;
    }

    public int GetLastFrameDelay() {
        return MegacoolConfiguration.Instance.lastFrameDelay;
    }

    public void SetLastFrameOverlay(string lastFrameOverlay) {
        ImplementationWarning("SetLastFrameOverlay");
    }

    public void SetDebugMode(bool debugMode) {
        ImplementationWarning("SetDebugMode");
    }

    public bool GetDebugMode() {
        ImplementationWarning("GetDebugMode");
        return false;
    }

    public void SetKeepCompletedRecordings(bool keep) {
        recordingManager.keepCompletedRecordings = keep;
    }

    public void DeleteShares(Func<MegacoolShare, bool> filter) {
        ImplementationWarning("DeleteShares");
    }

    public void SubmitDebugData(string message) {
        ImplementationWarning("SubmitDebugData");
    }

    public void ResetIdentity() {
        ImplementationWarning("ResetIdentity");
    }

    public void SetGIFColorTable(Megacool.GifColorTableType gifColorTable) {
        ImplementationWarning("SetGIFColorTable");
    }

    public void SignalRenderTexture(RenderTexture texture) {
    }

    public void IssuePluginEvent(ref IntPtr nativePluginCallbackPointer, int eventId) {
    }

    public void InitializeSharingDelegate() {
    }

    public void InitializeCapture(float scaleFactor, Megacool.TextureReadComplete textureReadCompleteCallback) {
        recordingManager.SetReadCompleteCallback(textureReadCompleteCallback);
    }
}

public class MegacoolEditorRecordingManager {

    private static readonly MegacoolEditorRecordingManager instance = new MegacoolEditorRecordingManager();
    MegacoolRecordingPersistent persistentRecordings = new MegacoolRecordingPersistent();
    private MegacoolRecording currentRecording;
    private static bool capturingFrame = false;
    public bool keepCompletedRecordings;
    private Megacool.TextureReadComplete ReadCompleteCallback;

    public static MegacoolEditorRecordingManager Instance {
        get {
            return instance;
        }
    }

    public string GetFramesDirectory() {
        return Application.temporaryCachePath + "/frames/";
    }

    public void SetReadCompleteCallback(Megacool.TextureReadComplete callback) {
        Instance.ReadCompleteCallback = callback;
    }

    public string GetRecordingFramesDirectory(string recordingId) {
        string framesDirectory = GetFramesDirectory();
        if (recordingId == default(string)) {
            recordingId = "default";
        }
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(recordingId);
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++) {
              hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        hashString.PadLeft(32, '0');
        return framesDirectory + hashString + "/";
    }

    private void DeleteExistingRecordings() {
        string framesDirectory = GetFramesDirectory();
        if (System.IO.Directory.Exists(framesDirectory)) {
            System.IO.Directory.Delete(framesDirectory, true);
            Instance.persistentRecordings.Clear();
        }
        System.IO.Directory.CreateDirectory(framesDirectory);
    }

    public void StartRecording() {
        MegacoolRecordingConfig config = new MegacoolRecordingConfig();
        config.SetDefaults();
        _StartRecording(config);
    }

    public void StartRecording(MegacoolRecordingConfig config) {
        _StartRecording(config);
    }

    private void _StartRecording(MegacoolRecordingConfig config) {
        if (Instance.currentRecording != null && !Instance.currentRecording.isFinished) {
            Instance.currentRecording = Instance.persistentRecordings.Restore(config.RecordingId);
            return;
        }
        if (!keepCompletedRecordings) {
            DeleteExistingRecordings();
        }
        Instance.currentRecording = new MegacoolRecording(config);
        Instance.currentRecording.Start();
    }

    public void StopRecording() {
        if (Instance.currentRecording != null) {
            Instance.currentRecording.isFinished = true;
        }
    }

    public void PauseRecording() {
        if (Instance.currentRecording != null) {
            Instance.persistentRecordings.Save(Instance.currentRecording);
        }
    }

    public void DeleteRecording(string recordingId) {
        string recordingDirectory = MegacoolEditorRecordingManager.Instance.GetRecordingFramesDirectory(recordingId);
        System.IO.Directory.Delete(recordingDirectory, true);
        Instance.persistentRecordings.Delete(recordingId);
    }

    public void RegisterScoreChange(int scoreDelta) {
        if (Instance.currentRecording != null) {
            Instance.currentRecording.GetOverflowStrategy().RegisterScoreChange(scoreDelta);
        }
    }

    public static void CaptureFrame() {
        if (Instance.currentRecording == null) {
            MegacoolRecordingConfig config = new MegacoolRecordingConfig();
            config.SetDefaults();
            Instance.currentRecording = new MegacoolRecording(config);
            Instance.currentRecording.Start();
        }
        capturingFrame = true;
        Instance.currentRecording.CaptureFrame(false);
    }

    public static void CaptureFrame(MegacoolFrameCaptureConfig config) {
        if (Instance.currentRecording == null) {
            Instance.currentRecording = new MegacoolRecording(config);
            Instance.currentRecording.Start();
        }
        capturingFrame = true;
        bool forceAdd = config.ForceAdd;
        Instance.currentRecording.CaptureFrame(forceAdd);
    }

    public static void SignalEndOfFrame() {
        if (Instance.currentRecording != null && (Megacool.Instance.IsRecording || capturingFrame)) {
            Instance.currentRecording.SignalEndOfFrame();
            capturingFrame = false;
        }
    }

    public void SignalReadComplete() {
        if (Instance.ReadCompleteCallback != null) {
            Instance.ReadCompleteCallback();
        }
    }

    public MegacoolPreviewData GetPreviewInfoForRecording(string recordingId) {
        if (Instance.currentRecording != null) {
            return Instance.currentRecording.GetPreviewInfoForRecording(recordingId);
        }
        return null;
    }

    public int GetNumberOfFrames(string recordingId) {
        if (Instance.currentRecording == null) {
            return 0;
        }
        return Instance.currentRecording.GetNumberOfFrames();
    }

    private class MegacoolRecording {
        public bool isFinished = false;
        private Buffer buffer;
        private string recordingId;
        private int lastFrameDelay;
        private int playbackFrameRate;
        private int maxFrames;
        private double peakLocation;

        public string RecordingId { get { return recordingId; } }
        public int MaxFrames { get { return maxFrames; } }
        public float PeakLocation { get { return (float)peakLocation; } }

        public MegacoolRecording(MegacoolRecordingConfig config) {
            recordingId = config.RecordingId;
            lastFrameDelay = config.LastFrameDelay;
            playbackFrameRate = config.PlaybackFrameRate;
            maxFrames = config.MaxFrames;
            peakLocation = config.PeakLocation;
            string strategy = config.OverflowStrategy.ToString();
            if (strategy == MegacoolOverflowStrategy.LATEST.ToString()) {
                buffer = new CircularBuffer(recordingId, maxFrames);
            } else if (strategy == MegacoolOverflowStrategy.TIMELAPSE.ToString()) {
                buffer = new TimelapseBuffer(recordingId, maxFrames);
            } else if (strategy == MegacoolOverflowStrategy.HIGHLIGHT.ToString()) {
                buffer = new HighlightBuffer(this);
            }
        }

        public MegacoolRecording(MegacoolFrameCaptureConfig config) {
            recordingId = config.RecordingId;
            lastFrameDelay = config.LastFrameDelay;
            playbackFrameRate = config.PlaybackFrameRate;
            maxFrames = config.MaxFrames;
            peakLocation = config.PeakLocation;
            string strategy = config.OverflowStrategy.ToString();
            if (strategy == MegacoolOverflowStrategy.LATEST.ToString()) {
                buffer = new CircularBuffer(recordingId, maxFrames);
            } else if (strategy == MegacoolOverflowStrategy.TIMELAPSE.ToString()) {
                buffer = new TimelapseBuffer(recordingId, maxFrames);
            } else if (strategy == MegacoolOverflowStrategy.HIGHLIGHT.ToString()) {
                buffer = new HighlightBuffer(this);
            }
        }

        public void Start() {
            string recordingDirectory = buffer.GetRecordingDirectory(recordingId);
            if (System.IO.Directory.Exists(recordingDirectory)) {
                System.IO.Directory.Delete(recordingDirectory, true);
            }
            System.IO.Directory.CreateDirectory(recordingDirectory);
        }

        public MegacoolPreviewData GetPreviewInfoForRecording(string recordingId) {
            string recordingDirectory = buffer.GetRecordingDirectory(recordingId);
            if (!System.IO.Directory.Exists(recordingDirectory)){
                return null;
            }

            string[] framePaths = buffer.GetFramePaths();
            return new MegacoolPreviewData(framePaths, playbackFrameRate, lastFrameDelay);
        }

        public int GetNumberOfFrames() {
            return buffer.GetFramePaths().Length;
        }

        public void CaptureFrame(bool forceAdd) {
            if (buffer.ShouldCapture() || forceAdd) {
                buffer.PushFrame();
            }
        }

        public void SignalEndOfFrame() {
            buffer.SignalEndOfFrame();
        }

        public Buffer GetOverflowStrategy() {
            return buffer;
        }
    }

    private class MegacoolRecordingPersistent {
        private Dictionary<string, MegacoolRecording> savedRecordings = new Dictionary<string, MegacoolRecording>();

        public void Save(MegacoolRecording recording) {
            string recordingId = recording.RecordingId;
            if (savedRecordings.ContainsKey(recordingId)) {
                savedRecordings[recordingId] = recording;
            } else {
                savedRecordings.Add(recording.RecordingId, recording);
            }
        }

        public void Delete(string recordingId) {
            savedRecordings.Remove(recordingId);
        }

        public MegacoolRecording Restore(string recordingId) {
            if (recordingId == default(string)) {
                recordingId = "default";
            }
            MegacoolRecording savedRecording = null;
            savedRecordings.TryGetValue(recordingId, out savedRecording);
            return savedRecording;
        }

        public void Clear() {
            savedRecordings.Clear();
        }
    }

    private abstract class Buffer {
        protected string framesDirectory = MegacoolEditorRecordingManager.Instance.GetFramesDirectory();
        protected int width = Screen.width;
        protected int height = Screen.height;
        private Texture2D currentFrame = null;
        private bool pixelsRead;

        public string GetRecordingDirectory(string recordingId) {
            return MegacoolEditorRecordingManager.Instance.GetRecordingFramesDirectory(recordingId);
        }

        public Buffer(){
            int renderTextureWidth = Megacool.Instance.RenderTexture.width;
            int renderTextureHeight = Megacool.Instance.RenderTexture.height;
            currentFrame = new Texture2D(renderTextureWidth, renderTextureHeight, TextureFormat.RGB24, false);
            pixelsRead = false;
        }

        private void ReadPixels() {
            RenderTexture previous = RenderTexture.active;
            RenderTexture active = Megacool.Instance.RenderTexture;
            RenderTexture.active = active;
            currentFrame.ReadPixels(new Rect(0,0, active.width, active.height), 0, 0);
            RenderTexture.active = previous;
            pixelsRead = true;
            MegacoolEditorRecordingManager.Instance.SignalReadComplete();
        }

        public void SignalEndOfFrame() {
            if (pixelsRead == false) {
                ReadPixels();
            }
        }

        protected bool WriteFrameToFile(string directory, string name) {
            if (pixelsRead == true) {
                string frameName = "frame-" + name;
                string frameFileName = directory + frameName + ".png";
                byte[] frameData = currentFrame.EncodeToPNG();
                var newFile = System.IO.File.Create(frameFileName);
                newFile.Write(frameData, 0, frameData.Length);
                newFile.Close();
                pixelsRead = false;
                return true;
            } else {
                return false;
            }
        }

        public virtual bool ShouldCapture(){ return true; }
        public virtual void RegisterScoreChange(int scoreDelta) {
            Debug.LogWarning("Megacool: The current recording does not use the highlight overflow strategy");
        }
        public abstract string[] GetFramePaths();
        public abstract void PushFrame();
    }

    private class CircularBuffer : Buffer {
        private int size;
        private int maxSize;
        private string recordingId;

        public CircularBuffer(string recordingId, int maxSize) {
            this.size = 0;
            this.maxSize = maxSize;
            this.recordingId = recordingId;
        }

        public override void PushFrame() {
            string recordingDirectory = GetRecordingDirectory(recordingId);
            int frameNumber = (size + 1) % maxSize;
            bool frameWritten = WriteFrameToFile(recordingDirectory, frameNumber.ToString());
            if (frameWritten) {
                size++;
            }
        }

        public override string[] GetFramePaths() {
            string recordingDirectory = GetRecordingDirectory(recordingId);
            List<string> framePaths = new List<string>();
            if (size <= maxSize) {
                for (int i = 1; i < size; i++) {
                    string frameNumber = "frame-" + i.ToString();
                    string frameName = recordingDirectory + frameNumber + ".png";
                    framePaths.Add(frameName);
                }
            } else {
                for (int index = size % maxSize, counter = maxSize; counter > 0;
                  index = (index + 1) % maxSize, counter--) {
                      string frameNumber = "frame-" + index.ToString();
                      string frameName = recordingDirectory + frameNumber + ".png";
                      framePaths.Add(frameName);
                  }
            }
            return framePaths.ToArray();
        }
    }

    private class TimelapseBuffer : Buffer {
        private int rate;
        private int maxFrames;
        private int frameNumber;
        private int framesBeforeNextStorage;
        private int framesOnDisk;
        private string recordingId;

        public TimelapseBuffer(string recordingId, int maxSize) {
            this.rate = 1;
            this.maxFrames = maxSize;
            this.frameNumber = -1;
            this.recordingId = recordingId;
        }

        public override bool ShouldCapture() {
            framesBeforeNextStorage -= 1;
            frameNumber++;
            return framesBeforeNextStorage <= 0;
        }

        public override void PushFrame() {
            string recordingDirectory = GetRecordingDirectory(recordingId);
            bool frameWritten = WriteFrameToFile(recordingDirectory, frameNumber.ToString());
            if (frameWritten) {
                framesOnDisk += 1;
                if (framesOnDisk > maxFrames) {
                    rate = (int)Mathf.Min(rate * 2, Mathf.Pow(2, 20));
                    framesOnDisk -= Trim(frameNumber, rate);
                }
                framesBeforeNextStorage = rate - (frameNumber % rate);
            }
        }

        private int Trim(int frameNumber, int rate) {
            string recordingDirectory = GetRecordingDirectory(recordingId);
            int framesDeleted = 0;
            int lastFrameToDelete = frameNumber % rate == 0 ? frameNumber - 1 : frameNumber;
            for (int i = rate / 2; i <= lastFrameToDelete; i += rate) {
                string file = recordingDirectory + "frame-" + i.ToString() + ".png";
                if (System.IO.File.Exists(file)) {
                    System.IO.File.Delete(file);
                    framesDeleted++;
                }
            }
            return framesDeleted;
        }

        private int GetFileNumberFromName(string fileName) {
            int start = fileName.IndexOf("frame-") + "frame-".Length;
            int end = fileName.LastIndexOf(".png");
            string frameNumber = fileName.Substring(start, end - start);
            return int.Parse(frameNumber);
        }

        public override string[] GetFramePaths() {
            string recordingDirectory = GetRecordingDirectory(recordingId);
            string[] info = System.IO.Directory.GetFiles(recordingDirectory, "*.png");
            List<string> files = new List<string>(info);
            files.Sort(
                delegate(string file1, string file2) {
                    int fileNumber1 = GetFileNumberFromName(file1);
                    int fileNumber2 = GetFileNumberFromName(file2);
                    return fileNumber1.CompareTo(fileNumber2);
                }
            );
            return files.ToArray();
        }
    }

    private class HighlightBuffer : Buffer {
        private const float decay = .9f;
        private int maxFrames;
        private int frameNumber;
        private int frameScore;
        private int framesAfterPeak;
        private double maxIntensity;
        private int boringFrameNumber;
        private MegacoolRecording recording;
        private List<int> frameScores;
        private HighlightWindow curHighlight;
        private HighlightWindow curWindow;

        public HighlightBuffer(MegacoolRecording recording) {
            this.maxFrames = recording.MaxFrames;
            this.recording = recording;
            curHighlight = new HighlightWindow(0,0,0);
            curWindow = new HighlightWindow(0,0,0);
            frameScore = 0;
            frameScores = new List<int>();
            maxIntensity = 0;
            boringFrameNumber = 0;
            frameNumber = 0;
            CalculateFramesAfterPeak(recording);
        }

        private void CalculateFramesAfterPeak(MegacoolRecording recording) {
            int framesBeforePeak = (int) Mathf.Ceil(recording.PeakLocation * maxFrames);
            if (framesBeforePeak > maxFrames) {
                framesBeforePeak = maxFrames;
            } else if (framesBeforePeak <= 0) {
                framesBeforePeak = 1;
            }
            framesAfterPeak = maxFrames - framesBeforePeak;
        }

        private bool FrameWithinDeletableBounds(int frameIndex) {
            if (frameIndex < 0) {
                return false;
            }
            if (frameIndex >= curHighlight.start && frameIndex <= curHighlight.end) {
                return false;
            }
            return true;
        }

        private void SetHighlightFromWindow() {
            curHighlight.end = curWindow.end;
            curHighlight.start = curWindow.start;
            curHighlight.score = curWindow.score;
        }

        private void CheckPeak() {
            double curIntensity = CalculateIntensity();
            if (curIntensity >= maxIntensity) {
                maxIntensity = curIntensity;
                boringFrameNumber = 0;
            } else {
                boringFrameNumber++;
            }
        }

        private double CalculateIntensity() {
            double intensity = 0.0;
            for (int i = 0; i < frameScores.Count - 1; i++) {
                intensity += frameScores[i] * Mathf.Pow(decay, (frameScores.Count - i));
            }
            intensity += frameScores[frameScores.Count - 1];
            return intensity;
        }

        private void CheckAddHighlight() {
            if (curWindow.score > curHighlight.score || (curWindow.start > curHighlight.end)) {
                DeleteHighlight();
                SetHighlightFromWindow();
            }
        }

        private void DeleteFrame(int frameIndex) {
            string recordingDirectory = GetRecordingDirectory(recording.RecordingId);
            string frameName = "frame-" + frameIndex.ToString() + ".png";
            if (System.IO.File.Exists(recordingDirectory + frameName)) {
                System.IO.File.Delete(recordingDirectory + frameName);
            }
        }

        private void DeleteHighlight() {
            int stopIndex = Mathf.Min(curHighlight.end, curWindow.start - 1);
            for (int i = curHighlight.start; i <= stopIndex; i++) {
                DeleteFrame(i);
            }
        }

        private void ShiftWindowStart() {
            int oldStartFrame = curWindow.end - maxFrames;
            if (FrameWithinDeletableBounds(oldStartFrame)) {
                DeleteFrame(oldStartFrame);
            }
            if (frameNumber >= maxFrames) {
                int oldStartScore = frameScores[0];
                frameScores.RemoveAt(0);
                curWindow.start = oldStartFrame + 1;
                curWindow.score = curWindow.score - oldStartScore;
            }
        }

        private bool ShiftWindowEnd() {
            string recordingDirectory = GetRecordingDirectory(recording.RecordingId);
            int currentFramescore = frameScore;
            bool frameWritten = WriteFrameToFile(recordingDirectory, frameNumber.ToString());
            if (frameWritten) {
                frameScore = 0;
                frameScores.Add(currentFramescore);
                curWindow.end = frameNumber;
                curWindow.score = curWindow.score + currentFramescore;
            }
            return frameWritten;
        }

        public override void PushFrame() {
            if (ShiftWindowEnd()) {
                ShiftWindowStart();
                CheckPeak();
                if (boringFrameNumber == framesAfterPeak) {
                    CheckAddHighlight();
                }
                frameNumber++;
            }
        }

        public override void RegisterScoreChange(int scoreDelta) {
            frameScore += scoreDelta;
        }

        public override string[] GetFramePaths() {
            if (frameNumber == 0) {
                return (new string[0]);
            }
            CheckAddHighlight();
            int start = curHighlight.start;
            int end = curHighlight.end;
            if (start == -1 && end == -1) {
                start = curWindow.start;
                end = curWindow.end;
            }
            string recordingDirectory = GetRecordingDirectory(recording.RecordingId);
            List<string> framePaths = new List<string>();
            for (int i = start; i <= end; i++) {
                string frameNumber = "frame-" + i.ToString();
                string frameName = recordingDirectory + frameNumber + ".png";
                framePaths.Add(frameName);
            }
            return framePaths.ToArray();
        }
    }

    private class HighlightWindow {
        public int start;
        public int end;
        public int score;

        public HighlightWindow(int start, int end, int score) {
            this.start = start;
            this.end = end;
            this.score = score;
        }
    }
}
#endif
