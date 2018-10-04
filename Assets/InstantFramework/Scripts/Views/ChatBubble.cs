using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatBubble : MonoBehaviour 
{
    public TextMeshProUGUI text;
    public RectTransform bg;

    const float xOffset = 50f;
    const float yOffset = 22f;


	// Use this for initialization
	void Start () 
    {
        text.ForceMeshUpdate();
        Vector3 textBoundsSize = text.textBounds.size;
        bg.sizeDelta = new Vector2(textBoundsSize.x + xOffset, textBoundsSize.y + yOffset);
        GetComponent<RectTransform>().sizeDelta = bg.sizeDelta;
	}
	
	// Update is called once per frame
	void Update () 
    {
        

	}
}
