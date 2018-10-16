using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using DG.Tweening;
using TurboLabz.InstantGame;

public class ChatBubble : MonoBehaviour 
{
    public RectTransform bg;
    public TextMeshProUGUI text;
    public bool flipped;
    public bool inGameBubble;

    Image bgImage;
    Coroutine fadeRoutine;

    public void SetText(string newText)
    {
        // Nothing to do if there is no text
        if (newText.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        text.text = newText;

        // Kick off the fade cycle
        if (inGameBubble)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = StartCoroutine(DoFade());
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
    }

    void Awake()
    {
        if (inGameBubble)
        {
            GetComponent<Button>().onClick.AddListener(OpenChat);
        }

        bgImage = GetComponent<Image>();
    }
	
    IEnumerator DoFade()
    {
        bgImage.DOFade(0f, 0f);
        text.DOFade(0f, 0f);
        bgImage.DOFade(1f, 0.5f);
        text.DOFade(1f, 0.5f);

        yield return new WaitForSeconds(7f);

        bgImage.DOFade(0f, 0.5f);
        text.DOFade(0f, 0.5f);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            gameObject.SetActive(false);
        }
    }


	void OpenChat()
    {
        
    }
}
