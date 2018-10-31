﻿using System.Collections;
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
    public bool flipped; // Set from the scene inspector
    public bool inGameBubble; // Set from the  scene inspector
    public RectTransform container;

    public Sprite bgFullChatOpponent;
    public Sprite bgFullChatPlayer;

    public Image profilePic;
    public Text timer;

    Image bgImage;
    Coroutine fadeRoutine;

    const float CONTAINER_OFFSET = 65f;

    public void SetText(string newText, bool isPlayer)
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
        text.ForceMeshUpdate(true);

        // Apply the correct bg if this is full chat mode
        if (inGameBubble)
        {
            // Resize the background based on the text mesh
            Vector3 textBoundsSize = text.textBounds.size;

            bg.sizeDelta = new Vector2(
                textBoundsSize.x + 57.65f, 
                textBoundsSize.y + 41.63f);

            // Move the background into the center of the container
            float flipDiv = flipped ? -2f : 2f;
            bg.anchoredPosition = new Vector2(bg.anchoredPosition.x, bg.sizeDelta.y / flipDiv);

        }
        else
        {
            // Resize the background based on the text mesh
            Vector3 textBoundsSize = text.textBounds.size;

            bg.sizeDelta = new Vector2(
                textBoundsSize.x + 65f, 
                textBoundsSize.y + 30f);

            bgImage.sprite = (isPlayer) ? bgFullChatPlayer : bgFullChatOpponent;

            // Move the background to the top of the container
            bg.anchoredPosition = new Vector2(bg.anchoredPosition.x, -1 * bg.sizeDelta.y);

            // Set the size of container to that the scroll view can stack elements correctly
            container.sizeDelta = new Vector2(container.sizeDelta.x, bg.sizeDelta.y + CONTAINER_OFFSET);

        }
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
