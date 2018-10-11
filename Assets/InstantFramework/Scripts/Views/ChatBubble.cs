using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TurboLabz.TLUtils;

public class ChatBubble : MonoBehaviour 
{
    public RectTransform bg;
    public TextMeshProUGUI text;
    public bool flipped;

    void OnAwake()
    {
        GetComponent<Button>().onClick.AddListener(OpenChat);
    }

    void OnEnable() 
    {
        // Nothing to do if there is no text
        if (text.text.Length == 0)
        {
            LogUtil.Log("Leaving...", "cyan");
            return;
        }

        // Resise the text mesh based on the text
        text.ForceMeshUpdate();

        // Resize the background based on the text mesh
        Vector3 textBoundsSize = text.textBounds.size;
        bg.sizeDelta = new Vector2(
            textBoundsSize.x + 57.65f, 
            textBoundsSize.y + 41.63f);

        // Move the background into the center of the container
        float flipDiv = flipped ? -2f : 2f;
        bg.anchoredPosition = new Vector2(bg.anchoredPosition.x, bg.sizeDelta.y / flipDiv);

        LogUtil.Log("Howdy...", "cyan");
	}
	
	void OpenChat()
    {
        LogUtil.Log("Opening chat...", "cyan");
    }
}
