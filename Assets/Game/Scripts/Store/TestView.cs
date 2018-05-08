using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestView : MonoBehaviour {

    public Button testButton;

	// Use this for initialization
	void Start () {
//        testButton.onClick.AddListener(testClick);
	}
	
    void testClick()
    {
        Debug.Log("Click");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
