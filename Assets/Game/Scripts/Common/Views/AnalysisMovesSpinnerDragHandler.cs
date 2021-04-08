using DG.Tweening;
using Picker;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnalysisMovesSpinnerDragHandler : MonoBehaviour
{
    public Transform arrowLeft;
    public Transform arrowRight;

    [HideInInspector] public IAudioService audioService;

    private float arrowLeftOriginalXPosition = 0;
    private float arrowRightOriginalXPosition = 0;

    private void OnEnable()
    {
        if (arrowLeftOriginalXPosition == 0)
        {
            arrowLeftOriginalXPosition = arrowLeft.localPosition.x;
            arrowRightOriginalXPosition = arrowRight.localPosition.x;
        }
        else
        {
            SetOriginalLocalPositionX(arrowLeft, arrowLeftOriginalXPosition);
            SetOriginalLocalPositionX(arrowRight, arrowRightOriginalXPosition);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.name.Equals("Trigger"))
        {
            arrowLeft.DOLocalMoveX(arrowLeftOriginalXPosition, 0.1f);
            arrowRight.DOLocalMoveX(arrowRightOriginalXPosition, 0.1f);
            audioService.Play(audioService.sounds.SFX_PLACE_PIECE);
#if UNITY_IOS && !UNITY_EDITOR
            IOSNative.StartHapticFeedback(HapticFeedbackTypes.LIGHT);
#endif
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        if (collision.name.Equals("Trigger"))
        {
            arrowLeft.DOLocalMoveX(arrowLeftOriginalXPosition - 20, 0.1f);
            arrowRight.DOLocalMoveX(arrowRightOriginalXPosition + 20, 0.1f);
        }
    }

    private void SetOriginalLocalPositionX(Transform obj, float originalXPosition)
    {
        var pos = obj.localPosition;
        pos.x = originalXPosition;
        obj.localPosition = pos;
    }
}
