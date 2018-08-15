using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendDialog : MonoBehaviour {

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

	[Header("Others")]
	public Sprite defaultAvatar;
	public Text vsLabel;
	public Text winsLabel;
	public Text drawsLabel;
	public Text wins;
	public Text losses;
	public Text playerDraws;
	public Text oppDraws;
	public Text totalGamesLabel;
	public Text blockLabel;
	public Button blockBtn;
	public Button closeBtn;

}
