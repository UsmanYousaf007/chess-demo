using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class FriendBarSimple : MonoBehaviour
{
    public Image avatarImage;
    public Image avatarBg;
    public Image avatarIcon;
    public GameObject premiumBorder;
    public Text profileNameLabel;
    public Text eloScoreLabel;
    public GameObject thinking;
    public Image onlineStatus;
    public Sprite online;
    public Sprite offline;
    public Sprite activeStatus;
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
        eloScoreLabel.text = friend.publicProfile.eloScore.ToString();

        if (!friend.publicProfile.isOnline && friend.publicProfile.isActive)
        {
            onlineStatus.sprite = activeStatus;
        }
        else
        {
            onlineStatus.sprite = friend.publicProfile.isOnline ? online : offline;
        }

        avatarIcon.gameObject.SetActive(false);
        avatarBg.gameObject.SetActive(false);

        if (friend.publicProfile.profilePicture != null)
        {
            avatarImage.sprite = friend.publicProfile.profilePicture;
        }
        else
        {
            if (friend.publicProfile.avatarId != null)
            {
                avatarIcon.gameObject.SetActive(true);
                avatarBg.gameObject.SetActive(true);

                avatarBg.color = Colors.Color(friend.publicProfile.avatarBgColorId);
                avatarIcon.sprite = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME).GetSprite(friend.publicProfile.avatarId);
            }
        }

        premiumBorder.SetActive(friend.publicProfile.isSubscriber);
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
        thinking.SetActive(true);
        unblockButton.interactable = false;
        unblockButtonLabel.gameObject.SetActive(false);
    }

    public void UpdateSocialPic(Sprite sprite)
    {
        avatarIcon.gameObject.SetActive(false);
        avatarBg.gameObject.SetActive(false);
        avatarImage.sprite = sprite;
    }

    public void RemoveButtonListeners()
    {
        unblockButton.onClick.RemoveAllListeners();
    }
}
