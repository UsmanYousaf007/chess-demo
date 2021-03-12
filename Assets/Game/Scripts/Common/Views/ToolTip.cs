using UnityEngine;

public class ToolTip : MonoBehaviour
{
    public bool animate;
    public Vector3 animationStartPoint;
    public Vector3 animationEndPoint;
    public bool autoDisable;
    public float disableAfterSeconds;

    private void OnEnable()
    {
        if (autoDisable)
        {
            Invoke("DisableMe", disableAfterSeconds);
        }

        if (animate)
        {
            transform.localPosition = animationStartPoint;

            iTween.MoveTo(gameObject,
                iTween.Hash(
                    "position",animationEndPoint,
                    "time",0.5f,
                    "islocal",true,
                    "looptype",iTween.LoopType.pingPong,
                    "easetype",iTween.EaseType.easeOutCubic));
        }
    }

    private void DisableMe()
    {
        CancelInvoke();

        if (animate)
        {
            iTween.Stop(gameObject);
        }

        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        DisableMe();
    }

    
}
