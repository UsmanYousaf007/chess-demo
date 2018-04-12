using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

[AddComponentMenu("Megacool/Gif Preview")]
public class MegacoolGifPreview : MonoBehaviour {

    private GUISystem guiSystem = null;
    private Coroutine _playGifIEnumerator;

    Texture2D previewTexture;

    public void StartPreview(string recordingIdentifier = default(string)) {

        if (guiSystem == null) {
            guiSystem = new GUISystem(gameObject);
        }

        StopPreview();

        MegacoolPreviewData previewData = Megacool.Instance.GetPreviewDataForRecording(recordingIdentifier);
        if (previewData == null || previewData.FramePaths == null || previewData.FramePaths.Length == 0) {
            return;
        }

        guiSystem.ShowPreview();
        _playGifIEnumerator = StartCoroutine(
            PreviewMegacoolGif(previewData.FramePaths, previewData.PlaybackFrameRate, previewData.LastFrameDelay)
        );
    }

    public void StopPreview() {
        if (_playGifIEnumerator != null) {
            StopCoroutine(_playGifIEnumerator);

            guiSystem.HidePreview();

            // Only destroy the preview texture if it has been created, might not be the case if there were no frames
            // in the preview or it was stopped before any frames were loaded.
            if (previewTexture) {
                Destroy(previewTexture);
            }

            _playGifIEnumerator = null;
        }
    }

    public int GetNumberOfFrames(string recordingIdentifier = default(string)) {
        return Megacool.Instance.GetNumberOfFrames(recordingIdentifier);
    }

    private IEnumerator PreviewMegacoolGif(string[] framePaths, float playbackFrameRate, int lastFrameDelay) {
        float updateinterval = 1.0f / playbackFrameRate;
        float _lastFrameDelay = (float)lastFrameDelay / 1000f;
        if (_lastFrameDelay == 0) {
            _lastFrameDelay = updateinterval;
        }

        framePaths = ValidatedFrameList(framePaths);

        byte[] fileData;

        previewTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        guiSystem.SetTexture(previewTexture);

        bool isPlaying = true;

        int totalFrames = framePaths.Length;
        while (isPlaying) {
            for (int i = 0; i < totalFrames; i++) {
                float previewFrameStart = Time.realtimeSinceStartup;

                try {
                    fileData = File.ReadAllBytes(framePaths[i]);
                    if (previewTexture.LoadImage(fileData)) {
                        guiSystem.SetTexture(previewTexture);
                    }
                } catch (System.Exception e) {
                    // Can happen for missing files, files that failed to write completely due to full disk, etc.
                    Debug.Log("Failed to load frame for preview, exception: " + e.Message);
                    continue;
                }

                float waitTime = updateinterval - (Time.realtimeSinceStartup - previewFrameStart);
                if (waitTime > 0.0f && i < totalFrames - 1) {
                    // No matter how small the delay, Unity will always re-execute co-routines on the next frame,
                    // so we don't need to worry about setting the wait timer longer than the frame update time.
#if UNITY_5_4_OR_NEWER
                    WaitForSecondsRealtime waitTimer = new WaitForSecondsRealtime(waitTime);
#else
                    WaitForSeconds waitTimer = new WaitForSeconds(_cachedTime);
#endif
                    yield return waitTimer;
                } else if (i < totalFrames - 1){
                    yield return null;
                }
            }

            // This has to allocated afresh every time to work in the Editor, otherwise the last frame will only have
            // the correct delay on the first playback.
#if UNITY_5_4_OR_NEWER
            WaitForSecondsRealtime waitLastFrame = new WaitForSecondsRealtime(_lastFrameDelay);
#else
            WaitForSeconds waitLastFrame = new WaitForSeconds(_lastFrameDelay);
#endif
            yield return waitLastFrame;
        }
    }

    public string[] ValidatedFrameList(string[] framePaths) {
        var _validFramePaths = new List<string>();

        for (int i=0; i<framePaths.Length; i++) {
            string path = framePaths[i];
            if (File.Exists(path)) {
                _validFramePaths.Add(path);
            }
        }
        return _validFramePaths.ToArray();
    }
}

class GUISystem {

    private GameObject gameObject;
    private Component guiComponent;

    private const string requiredNGUIComponent = "UITexture";
    private const string requiredUGUIComponent = "RawImage";
    string textureProperty;

    public GUISystem(GameObject gameObject) {
        this.gameObject = gameObject;
        AssignGuiFramework();
    }

    private void AssignGuiFramework() {
        var nguiComponent = gameObject.GetComponent(requiredNGUIComponent);
        var uguiComponent = gameObject.GetComponent(requiredUGUIComponent);
        if (uguiComponent != null) {
            textureProperty = "texture";
        } else if (nguiComponent != null) {
            textureProperty = "mainTexture";
        } else {
            string errorMessage = "Missing Required Component ";

            // check if the ngui exists in project
            var nguiAssembly = (from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes ()
                where type.Name == requiredNGUIComponent
                select type).FirstOrDefault ();
            if (nguiAssembly != null) {
                errorMessage += requiredNGUIComponent;
            } else {
                errorMessage += requiredNGUIComponent;
            }
            Debug.LogWarning("Megacool: " + errorMessage);
        }
        guiComponent = nguiComponent ?? uguiComponent;
    }

    public void ShowPreview() {
        if (guiComponent != null) {
            guiComponent.GetType().GetProperty("enabled").SetValue(guiComponent, true, null);
        }
    }

    public void HidePreview() {
        if (guiComponent != null) {
            guiComponent.GetType().GetProperty("enabled").SetValue(guiComponent, false, null);
            guiComponent.GetType().GetProperty(textureProperty).SetValue(guiComponent, null, null);
        }
    }

    public void SetTexture(Texture2D texture) {
        if (guiComponent != null) {
            guiComponent.GetType().GetProperty(textureProperty).SetValue(guiComponent, texture, null);
        }
    }
}
