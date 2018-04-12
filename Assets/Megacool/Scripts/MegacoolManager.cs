using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class MegacoolManager : MonoBehaviour {
    private const int MCRC = 0x6d637263;
    private Nullable<MegacoolCaptureMethod> previousCaptureMethod = null;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
    private WaitForEndOfFrame endOfFrame;
#if UNITY_5_4_OR_NEWER
    private WaitForSecondsRealtime waitTime;
#else
    private WaitForSeconds waitTime;
#endif
#endif

    public void Awake() {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        endOfFrame = new WaitForEndOfFrame();
#if UNITY_5_4_OR_NEWER
        waitTime = new WaitForSecondsRealtime(1);
#else
        waitTime = new WaitForSeconds(1);
#endif
#endif
    }

    public void Initialize() {
        // Make sure old cameras are cleaned if the capture method changes
        if (previousCaptureMethod != null && previousCaptureMethod != Megacool.Instance.CaptureMethod) {
            RemoveCameras();
        }
        if (Megacool.Instance.CaptureMethod == MegacoolCaptureMethod.BLIT) {
            InitializeBlittingCamera();
        } else if (Megacool.Instance.CaptureMethod == MegacoolCaptureMethod.RENDER){
            InitializeRenderingCamera();
        } else if (Megacool.Instance.CaptureMethod == MegacoolCaptureMethod.SCREEN) {
            RemoveCameras();
        }
        previousCaptureMethod = Megacool.Instance.CaptureMethod;
    }

    private void InitializeRenderingCamera() {
        if (!gameObject.GetComponent<MegacoolRenderingCamera>()) {
            gameObject.AddComponent<MegacoolRenderingCamera>();
        }
    }

    private void InitializeBlittingCamera () {
        if (!gameObject.GetComponent<MegacoolBlittingCamera>()) {
            gameObject.AddComponent<MegacoolBlittingCamera>();
        }
    }

    private void RemoveCameras() {
        MegacoolBlittingCamera blitCamera = gameObject.GetComponent<MegacoolBlittingCamera>() ;
        if (blitCamera) {
            Destroy(blitCamera);
        }
        MegacoolRenderingCamera renderCamera = gameObject.GetComponent<MegacoolRenderingCamera>();
        if (renderCamera) {
            Destroy(renderCamera);
        }
    }


    public IEnumerator Start() {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        while (true) {
            if (Megacool.Instance.CaptureMethod == MegacoolCaptureMethod.SCREEN) {
                yield return endOfFrame;
                Megacool.Instance.IssuePluginEvent(MCRC);
            } else {
                // Wait for a bit longer to prevent re-evaluating this every frame, while still staying fairly
                // responsive to changes
                yield return waitTime;
            }
        }
#else
        yield break;
#endif
    }

}
