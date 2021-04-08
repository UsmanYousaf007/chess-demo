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

    private float arrowLeftOriginalXPosition;
    private float arrowRightOriginalXPosition;

    private void OnEnable()
    {
        arrowLeftOriginalXPosition = arrowLeft.localPosition.x;
        arrowRightOriginalXPosition = arrowRight.localPosition.x;
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
}
