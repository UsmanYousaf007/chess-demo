using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;
using System.Threading;

/// <summary>
/// Attach this script to a camera you want to duplicate the frames from. This does not re-render the frame, but might
/// cause performance degradations on certain GPUs that doesn't implement efficient blitting.
/// </summary>
class MegacoolBlittingCamera : MonoBehaviour {
    private float timeToNextCapture = 0f;
    private const int MCTR = 0x6d637472;


    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        timeToNextCapture -= Time.unscaledDeltaTime;

#if UNITY_EDITOR
        MegacoolEditorRecordingManager.SignalEndOfFrame();
#endif

        if (!(Megacool.Instance.IsRecording && timeToNextCapture <= 0) && !Megacool.Instance.RenderThisFrame) {
            Graphics.Blit(src, dest);
            return;
        }

        if (!Megacool.Instance.TextureReady.WaitOne(0)) {
            Graphics.Blit(src, dest);
            return;
        }

        Megacool.Instance.RenderThisFrame = false;

        Megacool.Instance.RenderTexture.MarkRestoreExpected();
        Graphics.Blit(src, Megacool.Instance.RenderTexture);
        Megacool.Instance.IssuePluginEvent(MCTR);
        timeToNextCapture = 1.0f/Megacool.Instance.FrameRate;

        // This has to happen after the other blit, otherwise you might end up with UI flickering
        Graphics.Blit(src, dest);
#if UNITY_EDITOR
        MegacoolEditorRecordingManager.CaptureFrame();
#endif
    }
}
