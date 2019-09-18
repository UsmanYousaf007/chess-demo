using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.InstantFramework;
using UnityEngine.UI;

public class CoachView : MonoBehaviour
{
    //Model
    [Inject] public IPlayerModel playerModel { get; set; }

    //Visual Properties
    public GameObject coachPanel;
    public Image bg;
    public Image pieceIcon;
    public Text moveText;
    public DrawLine line;
    public Image arrowHead;
    public Text titleText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(Vector3 fromPosition, Vector3 toPostion, string moveFrom, string moveTo, string pieceName)
    {
        coachPanel.SetActive(true);

        //pieceIcon.sprite = SkinContainer.LoadSkin(playerModel.activeSkinId).GetSprite(pieceName);
        moveText.text = string.Format("{0} to {1}", moveFrom, moveTo);
        line.Draw(fromPosition, toPostion);
        line.Fade(0, 255, 0.9f);
        arrowHead.transform.position = toPostion;
        var angle = Mathf.Atan2(fromPosition.y - toPostion.y, fromPosition.x - toPostion.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        Invoke("Fade", 4);
    }

    private void Hide()
    {
        coachPanel.SetActive(false);
    }

    private void Fade()
    {
        bg.CrossFadeAlpha(0, 1, false);
        pieceIcon.CrossFadeAlpha(0, 1, false);
        moveText.CrossFadeAlpha(0, 1, false);
        line.Fade(255, 1, 0.9f);
        arrowHead.CrossFadeAlpha(0, 1, false);
        titleText.CrossFadeAlpha(0, 1, false);

        Invoke("Hide", 1);
        Invoke("Reset", 1);
    }

    private void Reset()
    {
        bg.CrossFadeAlpha(1, 0, false);
        pieceIcon.CrossFadeAlpha(1, 0, false);
        moveText.CrossFadeAlpha(1, 0, false);
        line.SetAlpha(255);
        arrowHead.CrossFadeAlpha(1, 0, false);
        titleText.CrossFadeAlpha(1, 0, false);
    }
}
