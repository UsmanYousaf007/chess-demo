using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TurboLabz.TLUtils;
using DG.Tweening;
using TurboLabz.InstantGame;
using EasyAlphabetArabic;
using System;
using System.Text;
using System.Text.RegularExpressions;
//using ArabicSupport;

public class ChatBubble : MonoBehaviour 
{
    public RectTransform bg;
    public TMP_Text text;
    public bool flipped; // Set from the scene inspector
    public bool inGameBubble; // Set from the  scene inspector
    public RectTransform container;

    public Sprite bgFullChatOpponent;
    public Sprite bgFullChatPlayer;

    public Image profilePic;
    public Image avatarBg;
    public Image avatarIcon;
    public Text timer;
    public GameObject premiumBorder;

    Image bgImage;
    Coroutine fadeRoutine;
    bool isPlayer;
    LayoutElement layoutElement;

    const float CONTAINER_OFFSET = 65f;
    const float TEXT_WRAP_WIDTH_CHAT_PANEL = 725f;
    const float TEXT_WRAP_WIDTH_IN_GAME = 500f;
    const float TEXT_HEIGHT_IN_GAME = 190f;
    const float TEXT_WIDTH_PADDING = 20f;
    const float TEXT_HEIGHT_PADDING = 20f;

    public void SetText(string newText, bool isPlayer)
    {
        this.isPlayer = isPlayer;

        // Nothing to do if there is no text
        if (newText.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        if (text == null)
        {
            return;
        }

        if (Regex.IsMatch(newText, @"\p{IsArabic}"))
        {
            text.text = EasyArabicCore.CorrectString(newText, 0);
            //text.text = ArabicFixer.Fix(newText);
        }
        else
        {
            text.text = newText;
        }


        if (layoutElement != null)
        {
            //Set the size of the text box
            //TextGenerator textGen = new TextGenerator();
            
            text.autoSizeTextContainer = true;
            Vector2 vector2 = text.GetPreferredValues(newText);
            //TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.rect.size);
            float width = vector2.x;
            float wrapWidth = inGameBubble ? TEXT_WRAP_WIDTH_IN_GAME : TEXT_WRAP_WIDTH_CHAT_PANEL;
            layoutElement.preferredWidth = (width > wrapWidth) ? wrapWidth : width;
            layoutElement.preferredWidth += TEXT_WIDTH_PADDING;

            if (inGameBubble)
            {
                float height = vector2.y;
                layoutElement.preferredHeight = (height > TEXT_HEIGHT_IN_GAME) ? TEXT_HEIGHT_IN_GAME : height;
                layoutElement.preferredHeight += TEXT_HEIGHT_PADDING;
            }
        }

        // Kick off the fade cycle
        if (inGameBubble)
        {
            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            if (isActiveAndEnabled)
            {
                fadeRoutine = StartCoroutine(DoFade());
            }
        }
        text.ForceMeshUpdate();

        if (inGameBubble)
        {
            text.text = text.text.Length > 60 ? text.text.Remove(60) + ".." : text.text;
        }
        //if (text != null)
        //{

        //}


        // Resise the text mesh based on the text
        // text.ForceMeshUpdate(true);
        if (isActiveAndEnabled)
        {
            StartCoroutine(SetBgSizeCR());
        }
    }

    void OnEnable()
    {
        StartCoroutine(SetBgSizeCR());
    }

    IEnumerator SetBgSizeCR()
    {
        yield return null;

        SetBgSize();
    }

    void SetBgSize()
    {
        // Apply the correct bg if this is full chat mode
        if (inGameBubble)
        {
            // Resize the background based on the text mesh
            Vector3 textBoundsSize = text.rectTransform.sizeDelta;

            bg.sizeDelta = new Vector2(
                textBoundsSize.x + 57.65f,
                textBoundsSize.y + 30f);

            // Move the background into the center of the container
            float flipDiv = flipped ? -2f : 2f;
            bg.anchoredPosition = new Vector2(bg.anchoredPosition.x, bg.sizeDelta.y / flipDiv);

        }
        else
        {
            // Resize the background based on the text mesh
            Vector3 textBoundsSize = text.rectTransform.sizeDelta;

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
        layoutElement = text.GetComponent<LayoutElement>();
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
