using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public Image background;
    public Text titleLarge;
    public Text title;
    public Text body;
    public Button closeButton;
    public Button playButton;
    public Text playButtonLabel;
    public Image avatarBg;
    public Image avatarIcon;
    public Image senderPic;
    public Sprite whiteAvatar;
    public Sprite defaultAvatar;
    //public GameObject premiumBorder;
    public Image leagueBorder;

    public Button acceptQuickMatchButton;
    public Text acceptQuickMatchButtonText;

    public Button fullButton;
    public Image icon;
    public Image bgOverlay;
    public GameObject playerObj;
    public Sprite subsriptionAvatar;
    public Sprite leagueAvatar;

    private void OnEnable()
    {
        iTween.MoveFrom(gameObject,
            iTween.Hash(
                "y", 0,
                "time", 0.7f,
                "islocal", true
                ));
    }
}
