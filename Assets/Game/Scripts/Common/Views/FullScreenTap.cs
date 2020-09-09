using UnityEngine;
using UnityEngine.Events;

public class FullScreenTap : MonoBehaviour
{
    public UnityEvent action;
    public float ignoreTouchForSeconds = 0.1f;

    private float timeAtStart;

    // Start is called before the first frame update
    void OnEnable()
    {
        timeAtStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < timeAtStart + ignoreTouchForSeconds)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            action.Invoke();
        }
    }
}
