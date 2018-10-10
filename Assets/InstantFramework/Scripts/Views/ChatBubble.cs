using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour 
{
    public TextMeshProUGUI text;
    public RectTransform textRectTransform;
    public RectTransform bg;

	// Use this for initialization
	void OnEnable() 
    {
        text.ForceMeshUpdate();
        Vector3 textBoundsSize = text.textBounds.size;
        bg.sizeDelta = new Vector2(
            textBoundsSize.x + 57.65f, 
            textBoundsSize.y + 41.63f);
        //GetComponent<RectTransform>().sizeDelta = bg.sizeDelta;

        Debug.Log("TEXT BOUNDS = " + textBoundsSize);

	}
	
	// Update is called once per frame
	void Update() 
    {
        

	}
}
