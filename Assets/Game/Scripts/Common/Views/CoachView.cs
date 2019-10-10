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
    public Image icon;
    public Text normalMoveText;
    public Text bestMoveText;
    public GameObject bestMovePanel;
    public GameObject normalMovePanel;
    public GameObject analyzingPanel;
    public Image closeButton;
    public Transform lineEnePivot;
    public GameObject chessboardBlocker;

    //Constants
    const int LINE_ALPHA_MIN = 0;
    const int LINE_ALPHA_MAX = 204;
    const float START_FADE_AFTER_SECONDS = 4.0f;
    const float FADE_DURATION = 0.1f;
    const float UI_ALPHA_MIN = 0f;
    const float UI_ALPHA_MAX = 1.0f;
    const float ARROW_ALPHA_MAX = 0.8f;
    const bool IGNORE_TIMESCALE_WHILE_FADE = false;
    const float RESET_FADE_DURATION = 0f;
    const float ANALYZING_DELAY = 3.0f;

    private float timeAtAnalyzing = 0;

    private CoachVO coachVO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowAnalyzing()
    {
        bestMovePanel.SetActive(false);
        normalMovePanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        arrowHead.gameObject.SetActive(false);
        line.gameObject.SetActive(false);
        bg.gameObject.SetActive(true);
        analyzingPanel.SetActive(true);
        coachPanel.SetActive(true);
        chessboardBlocker.SetActive(true);

        timeAtAnalyzing = Time.time;
    }

    public void Show(CoachVO coachVO)
    {
        this.coachVO = coachVO;
        var timeDiff = Time.time - timeAtAnalyzing;
        Invoke("ShowResult", timeDiff < ANALYZING_DELAY ? ANALYZING_DELAY - timeDiff : 0);
        chessboardBlocker.SetActive(true);
        //Invoke("Fade", START_FADE_AFTER_SECONDS);
    }

    private void ShowResult()
    {
        analyzingPanel.SetActive(false);
        closeButton.gameObject.SetActive(true);
        coachPanel.SetActive(true);
        chessboardBlocker.SetActive(true);
        bg.gameObject.SetActive(true);
        arrowHead.gameObject.SetActive(true);
        line.gameObject.SetActive(true);

        coachVO.audioService.Play(coachVO.audioService.sounds.SFX_HINT);

        bestMovePanel.SetActive(coachVO.isBestMove);
        normalMovePanel.SetActive(!coachVO.isBestMove);

        pieceIcon.sprite = SkinContainer.LoadSkin(coachVO.activeSkinId).GetSprite(coachVO.pieceName);
        moveText.text = string.Format("{0} to {1}", coachVO.moveFrom, coachVO.moveTo);

        var viewportPoint = Camera.main.WorldToScreenPoint(coachVO.toPosition);
        arrowHead.rectTransform.position = viewportPoint;
        arrowHead.CrossFadeAlpha(ARROW_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        var angle = Mathf.Atan2(coachVO.fromPosition.y - coachVO.toPosition.y, coachVO.fromPosition.x - coachVO.toPosition.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        line.Draw(coachVO.fromPosition, Camera.main.ScreenToWorldPoint(lineEnePivot.position));
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);
    }

    public void Hide()
    {
        coachPanel.SetActive(false);
        chessboardBlocker.SetActive(false);
        bg.gameObject.SetActive(false);
        CancelInvoke();
    }

    public void Fade()
    {
        bg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButton.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        icon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        titleText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (coachVO.isBestMove)
        {
            bestMoveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalMoveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            pieceIcon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            moveText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }


        coachVO.analyticsService.Event(AnalyticsEventId.close_pow_coach, coachVO.analyticsContext);

        Invoke("Hide", FADE_DURATION);
        Invoke("Reset", FADE_DURATION);
    }

    private void Reset()
    {
        bg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButton.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        icon.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.SetAlpha(LINE_ALPHA_MAX);
        //arrowHead.CrossFadeAlpha(UIA, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        titleText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (coachVO.isBestMove)
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
