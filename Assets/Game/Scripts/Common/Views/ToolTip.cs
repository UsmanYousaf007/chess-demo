using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public bool animate;
    public Vector3 animationStartPoint;
    public Vector3 animationEndPoint;
    public bool useDisplacement;
    public Vector3 displacement;
    public bool autoDisable;
    public float disableAfterSeconds;

    [HideInInspector] public bool hiddenByClick;

    private void OnEnable()
    {
        if (autoDisable)
        {
            Invoke("DisableMe", disableAfterSeconds);
        }

        if (animate)
        {
            if (useDisplacement)
            {
                iTween.MoveBy(gameObject,
                    iTween.Hash(
                        "amount", displacement,
                        "time", 0.5f,
                        "islocal", true,
                        "looptype", iTween.LoopType.pingPong,
                        "easetype", iTween.EaseType.easeOutCubic));
            }
            else
            {
                transform.localPosition = animationStartPoint;

                iTween.MoveTo(gameObject,
                    iTween.Hash(
                        "position", animationEndPoint,
                        "time", 0.5f,
                        "islocal", true,
                        "looptype", iTween.LoopType.pingPong,
                        "easetype", iTween.EaseType.easeOutCubic));
            }
        }
    }

    private void DisableMe()
    {
        CancelInvoke();
        StopAnimation();
        //gameObject.SetActive(false);
    }

    public void OnClick()
    {
        hiddenByClick = true;
        DisableMe();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    private void StopAnimation()
    {
        if (animate)
        {
            iTween.Stop(gameObject);
        }
    }
}
