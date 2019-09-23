using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine.UI;

public class CoachView : MonoBehaviour
{ 
    //Visual Properties
    public GameObject coachPanel;
    public Image bg;
    public Image pieceIcon;
    public Text moveText;
    public DrawLine line;
    public Image arrowHead;
    public Text titleText;
    public Text normalMoveText;
    public Text bestMoveText;
    public GameObject bestMovePanel;
    public GameObject normalMovePanel;

    //Constants
    const int LINE_ALPHA_MIN = 0;
    const int LINE_ALPHA_MAX = 255;
    const float START_FADE_AFTER_SECONDS = 4.0f;
    const float FADE_DURATION = 1.0f;
    const float UI_ALPHA_MIN = 0f;
    const float UI_ALPHA_MAX = 1.0f;
    const bool IGNORE_TIMESCALE_WHILE_FADE = false;
    const float RESET_FADE_DURATION = 0f;

    private bool isBestMoveMade = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Show(Vector3 fromPosition, Vector3 toPostion, string moveFrom, string moveTo, string pieceName, string activeSkinId, bool isBestMove)
    {
        isBestMoveMade = isBestMove;
        coachPanel.SetActive(true);
        bg.gameObject.SetActive(true);
        bestMovePanel.SetActive(isBestMove);
        normalMovePanel.SetActive(!isBestMove);
        pieceIcon.sprite = SkinContainer.LoadSkin(activeSkinId).GetSprite(pieceName);
        moveText.text = string.Format("{0} to {1}", moveFrom, moveTo);
        line.Draw(fromPosition, toPostion);
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);
        arrowHead.transform.SetParent(this.transform.root.GetChild(0), true);
        var viewportPoint = Camera.main.WorldToViewportPoint(toPostion);
        arrowHead.rectTransform.anchorMin = viewportPoint;
        arrowHead.rectTransform.anchorMax = viewportPoint;
        var angle = Mathf.Atan2(fromPosition.y - toPostion.y, fromPosition.x - toPostion.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        Invoke("Fade", START_FADE_AFTER_SECONDS);
    }

    public void Hide()
    {
        coachPanel.SetActive(false);
        bg.gameObject.SetActive(false);
        arrowHead.transform.SetParent(coachPanel.transform, true);
        CancelInvoke();
    }

    private void Fade()
    {
        bg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        titleText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (isBestMoveMade)
        {
            bestMoveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalMoveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            pieceIcon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            moveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        Invoke("Hide", FADE_DURATION);
        Invoke("Reset", FADE_DURATION);
    }

    private void Reset()
    {
        bg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);        
        line.SetAlpha(LINE_ALPHA_MAX);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        titleText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (isBestMoveMade)
        {
            bestMoveText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalMoveText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            pieceIcon.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            moveText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
    }
}
