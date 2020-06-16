using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class FriendBarSimple : MonoBehaviour
{
    public Text profileNameLabel;
    public GameObject thinking;
    public Button unblockButton;
    public Text unblockButtonLabel;
    public GameObject bottomAlphaBg;
    public Mask maskObject;
    public GameObject bgGlow;
    public GameObject bgGlowLastStrip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Friend friend)
    {
        profileNameLabel.text = friend.publicProfile.name;
        thinking.SetActive(false);
        unblockButton.interactable = true;
        unblockButtonLabel.gameObject.SetActive(true);
        unblockButton.onClick.AddListener(OnUnblockButtonPressed);
    }

    public void UpdateMasking(bool isLastCell, bool isLastSection)
    {
        bottomAlphaBg.SetActive(false);
        maskObject.enabled = false;
        bgGlow.SetActive(false);
        bgGlowLastStrip.SetActive(false);
        if (!isLastSection && isLastCell)
        {
            bottomAlphaBg.SetActive(true);
        }
        if (isLastCell)
        {
            maskObject.enabled = true;
            bgGlowLastStrip.SetActive(true);
        }
        else
        {
            bgGlow.SetActive(true);
        }
    }

    private void OnUnblockButtonPressed()
    {
        ShowProcessing(true);
    }

    public void ResetUnblockButton()
    {
        ShowProcessing(false);
    }

    private void ShowProcessing(bool show)
    {
        thinking.SetActive(show);
        unblockButton.interactable = !show;
        unblockButtonLabel.gameObject.SetActive(!show);
    }

    public void RemoveButtonListeners()
    {
        unblockButton.onClick.RemoveAllListeners();
    }
}
