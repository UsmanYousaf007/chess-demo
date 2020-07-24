using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MoveFadeAnim : MonoBehaviour
{
    public GameObject go;
    public Text uiObj;
    public TextMeshProUGUI uiObj1;
    public Transform startPivot;
    public Transform endPivot;

    [Tooltip("Color to fade to")]
    [SerializeField]
    private Color EndColor = Color.white;

    [Tooltip("Color to fade from")]
    [SerializeField]
    private Color StartColor = Color.clear;

    // Start is called before the first frame update
    void OnEnable()
    {
        go.transform.localPosition = new Vector3(startPivot.localPosition.x, startPivot.localPosition.y, startPivot.localPosition.z);
        iTween.MoveTo(go, iTween.Hash("position", endPivot.localPosition, "time", 6f, "islocal", true));
        iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0f, "delay", 0f, "time", 4f, "onupdate", "updateColor"));
    }

    public void updateColor(float val)
    {
        uiObj1.color = ((1f - val) * StartColor) + (val * EndColor);
    }

}
