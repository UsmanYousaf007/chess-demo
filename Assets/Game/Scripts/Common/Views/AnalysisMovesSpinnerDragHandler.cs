using DG.Tweening;
using Picker;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PickerScrollRect))]
public class AnalysisMovesSpinnerDragHandler : MonoBehaviour, IBeginDragHandler 
{
    public Transform arrowLeft;
    public Transform arrowRight;

    private float arrowLeftOriginalXPosition;
    private float arrowRightOriginalXPosition;
    private PickerScrollRect _picker;

    private void OnEnable()
    {
        _picker = GetComponent<PickerScrollRect>();
        _picker.onSelectItem.AddListener(OnEndDrag);

        arrowLeftOriginalXPosition = arrowLeft.localPosition.x;
        arrowRightOriginalXPosition = arrowRight.localPosition.x;
    }

    private void OnDisable()
    {
        _picker.onSelectItem.RemoveListener(OnEndDrag);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        arrowLeft.DOLocalMoveX(arrowLeftOriginalXPosition - 20, 0.7f);
        arrowRight.DOLocalMoveX(arrowRightOriginalXPosition + 20, 0.7f);
    }

    public void OnEndDrag(GameObject eventData)
    {
        arrowLeft.DOLocalMoveX(arrowLeftOriginalXPosition, 0.7f);
        arrowRight.DOLocalMoveX(arrowRightOriginalXPosition, 0.7f);
    }
}
