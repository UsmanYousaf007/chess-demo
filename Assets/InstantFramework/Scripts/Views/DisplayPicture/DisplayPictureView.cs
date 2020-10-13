using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

public class DisplayPictureView : MonoBehaviour
{
    public Sprite defaultAvatar;
    public Sprite whiteAvatar;
    public Image profilePic;
    public Image avatarBG;
    public Image avatarIcon;
    public Image leagueBorder;

    //public GameObject noProfilePicBorder;
    //public GameObject hasProfilePicBorder;
    //public GameObject premiumBorder;

    public Image onlineStatus;
    public Sprite online;
    public Sprite offline;
    public Sprite activeStatus;

    public Button displayPictureButton;

    public string playerId;
    private SpritesContainer defaultAvatarContainer;

    public void UpdateView(PublicProfile vo)
    {
        playerId = vo.playerId;
        SetProfilePic(vo) ;
    }

    private void SetProfilePic(PublicProfile vo)
    {
        //noProfilePicBorder.SetActive(false);
        //hasProfilePicBorder.SetActive(false);
        avatarBG.gameObject.SetActive(false);
        avatarIcon.gameObject.SetActive(false);
        ShowPremiumBorder(vo.isSubscriber);

        this.leagueBorder.gameObject.SetActive(vo.leagueBorder != null);
        this.leagueBorder.sprite = vo.leagueBorder;
        this.leagueBorder.SetNativeSize();

        if (vo.profilePicture != null)
        {
            profilePic.sprite = vo.profilePicture;
            //hasProfilePicBorder.SetActive(true);
        }
        else
        {
            profilePic.sprite = defaultAvatar;

            if (vo.avatarId != null)
            {
                defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
                Sprite newSprite = defaultAvatarContainer.GetSprite(vo.avatarId);
                if (newSprite != null)
                {
                    avatarIcon.gameObject.SetActive(true);
                    avatarBG.gameObject.SetActive(true);
                    avatarIcon.sprite = newSprite;
                    avatarBG.sprite = whiteAvatar;
                    avatarBG.color = Colors.Color(vo.avatarBgColorId);
                }
            }

            //noProfilePicBorder.SetActive(true);
        }

        UpdateOnlineStatus(vo.isOnline, vo.isActive);
    }
    
    public void ShowPremiumBorder(bool show)
    {
        //premiumBorder.SetActive(show);
    }

    public void UpdateOnlineStatus(bool isOnline, bool isActive)
    {
        onlineStatus.sprite = isOnline ? online : activeStatus;
        if (!isOnline && isActive)
        {
            onlineStatus.sprite = activeStatus;
        }
        else
        {
            onlineStatus.sprite = isOnline ? online : offline;
        }
    }

    public void SetProfilePicture(Sprite picture)
    {
        profilePic.sprite = picture;
        //hasProfilePicBorder.SetActive(true);
        //noProfilePicBorder.SetActive(false);
        avatarBG.gameObject.SetActive(false);
        avatarIcon.gameObject.SetActive(false);
    }
}