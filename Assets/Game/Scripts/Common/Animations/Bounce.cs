using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        iTween.PunchScale(this.gameObject, iTween.Hash("amount", new Vector3(0.15f, 0.15f, 0f), "time", 1f, "loopType", "loop", "delay", 1));
    }

}
