using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;

public class ProfileDialogView : View
{
    [Header("Player Info")]
    public Image playerProfilePic;
    public Text playerProfileName;
    public Text playerEloLabel;
    public Image playerFlag;

    [Header("Opponent Info")]
    public Image oppProfilePic;
    public Text oppProfileName;
    public Text oppEloLabel;
    public Image oppFlag;

    [Header("Confirm Dialog")]
    public Text confirmLabel;
    public Text yesLabel;
    public Text noLabel;
    public Button yesBtn;
    public Button noBtn;

    [Header("Stats")]
    public Text winsTitle;
    public Text drawsTitle;
    public Text playerWinsLabel;
    public Text playerDrawsLabel;
    public Text opponentWinsLabel;
    public Text opponentDrawsLabel;
    public Text totalGamesLabel;

    [Header("Others")]
    public Text vsLabel;
    public Sprite defaultAvatar;
    public Text blockLabel;
    public Button blockBtn;
    public Button closeBtn;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
