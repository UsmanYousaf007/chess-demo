using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

/// <summary>
/// Attach this script to a camera you want to record from.
/// </summary>
class MegacoolRenderingCamera : MonoBehaviour {
    private Camera megacoolCamera;
    private GameObject cameraGameObject;
    private Camera cameraCopy;
    private float timeToNextCapture = 0f;
    private const int MCTR = 0x6d637472;

    void Start() {
        megacoolCamera = GetComponent<Camera>();
        // Since we cannot re-render the camera while it's in use (which it usually is during OnPreRender,
        // OnRenderImage and OnPostRender, leaving only OnGUI which usually has some performance hit, we add an extra
        // camera that mirrors the camera we're attached to.
        cameraGameObject = new GameObject();
        cameraCopy = cameraGameObject.AddComponent<Camera>();
        cameraCopy.enabled = false;
    }

    void OnPreRender() {
        timeToNextCapture -= Time.unscaledDeltaTime;

#if UNITY_EDITOR
        MegacoolEditorRecordingManager.SignalEndOfFrame();
#endif

        if (!(Megacool.Instance.IsRecording && timeToNextCapture <= 0) && !Megacool.Instance.RenderThisFrame) {
            return;
        }

        if (!Megacool.Instance.TextureReady.WaitOne(0)) {
            return;
        }

        Megacool.Instance.RenderThisFrame = false;

        cameraCopy.CopyFrom(megacoolCamera);
        cameraCopy.targetTexture = Megacool.Instance.RenderTexture;
        cameraCopy.Render();

        Megacool.Instance.IssuePluginEvent(MCTR);

        timeToNextCapture = 1.0f/Megacool.Instance.FrameRate;
#if UNITY_EDITOR
        MegacoolEditorRecordingManager.CaptureFrame();
#endif
    }
}
