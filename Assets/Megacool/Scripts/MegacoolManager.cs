using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class MegacoolManager : MonoBehaviour {
    private const int MCRC = 0x6d637263;
    private Coroutine writeCoroutine = null;
    private Nullable<MegacoolCaptureMethod> previousCaptureMethod = null;
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
    private WaitForEndOfFrame endOfFrame;
#endif

    public void Awake() {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        endOfFrame = new WaitForEndOfFrame();
#endif
    }

    public void StartWrites() {
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

        StopWrites();
        if (Megacool.Instance.CaptureMethod == MegacoolCaptureMethod.SCREEN) {
            writeCoroutine = StartCoroutine(StartWriteCoroutine());
        }
    }

    public void StopWrites() {
        if (writeCoroutine != null) {
            StopCoroutine(writeCoroutine);
            writeCoroutine = null;
        }
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


    public IEnumerator StartWriteCoroutine() {
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        while (true) {
            yield return endOfFrame;
            Megacool.Instance.IssuePluginEvent(MCRC);
        }
#else
        yield break;
#endif
    }

}
