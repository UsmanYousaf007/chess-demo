using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine.UI;

public class CoachView : MonoBehaviour
{
    [System.Serializable]
    public class MoveDetailsUI
    {
        public Image bg;
        public Image icon;
        public Text text;
    }

    //Visual Properties
    public GameObject coachPanel;
    public Transform parentPanel;
    public MoveDetailsUI normalPanel;
    public MoveDetailsUI bestPanel;
    public DrawLine line;
    public Image arrowHead;
    public Transform lineEndPivot;
    public GameObject UiBlocker;
    public Image stickerBg;
    public Image stickerPieceIcon;
    public Sprite stickerBgWhite;
    public Sprite stickerBgBlack;
    public Transform moveMeterButton;
    public Transform arrowPivot;
    public Transform upPivot;
    public Transform downPivot;

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
    const float TOOL_TIP_FADE_DURATION = 1.0f;
    const float HIDE_TOOL_TIP_AFTER = 2.7f;
    const float CLOSE_BUTTON_SCALE_DURATION = 1.0f;
    const float PIXELS_TO_MOVE = 150.0f;
    const float IGNORE_CLOSE_DURATION = 1.3f;
    readonly Vector3 CLOSE_BUTTON_SCALE = new Vector3(2.0f, 2.0f, 1.0f);

    private float timeAtAnalyzing = 0;
    private float timeAtShow = 0;
    private Vector2 directionVector;

    private CoachVO coachVO;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Show(CoachVO coachVO)
    {
        this.coachVO = coachVO;
        timeAtShow = Time.time;
        ShowResult();
    }

    private void ShowResult()
    {
        coachPanel.SetActive(true);
        UiBlocker.SetActive(true);
        arrowHead.gameObject.SetActive(true);
        line.gameObject.SetActive(true);
        normalPanel.bg.gameObject.SetActive(false);
        bestPanel.bg.gameObject.SetActive(false);

        moveMeterButton.SetAsLastSibling();

        coachVO.audioService.Play(coachVO.audioService.sounds.SFX_HINT);

        var angle = Mathf.Atan2(coachVO.fromPosition.y - coachVO.toPosition.y, coachVO.fromPosition.x - coachVO.toPosition.x) * Mathf.Rad2Deg;
        var toScreenPosition = Camera.main.WorldToScreenPoint(coachVO.toPosition);
        var stickerFromPosition = Camera.main.WorldToScreenPoint(coachVO.fromPosition);

        stickerBg.transform.SetParent(this.transform.parent.parent, true);
        stickerBg.transform.SetAsFirstSibling();
        stickerBg.rectTransform.position = toScreenPosition;

        var upPosition = upPivot.position;
        var downPosition = downPivot.position;

        stickerBg.transform.localEulerAngles = new Vector3(0, 0, angle);
        stickerPieceIcon.transform.localEulerAngles = new Vector3(0, 0, angle * -1);

        arrowHead.transform.SetParent(this.transform.parent.parent, true);
        arrowHead.rectTransform.position = arrowPivot.position;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        line.Draw(coachVO.fromPosition, Camera.main.ScreenToWorldPoint(lineEndPivot.position));

        arrowHead.transform.SetAsFirstSibling();
        stickerBg.sprite = coachVO.pieceName[0].Equals('W') ? stickerBgBlack : stickerBgWhite;
        stickerBg.rectTransform.position = stickerFromPosition;
        stickerPieceIcon.sprite = SkinContainer.LoadSkin(coachVO.activeSkinId).GetSprite(coachVO.pieceName);

        iTween.MoveTo(stickerBg.gameObject,
            iTween.Hash(
                "position", toScreenPosition,
                "time", 1.0f,
                "easetype", iTween.EaseType.easeOutExpo
                ));

        //detect direction of arrow
        directionVector = new Vector2(coachVO.toPosition.x < 0 ? 1 : -1, coachVO.fromPosition.y > coachVO.toPosition.y ? 1 : -1);
        Flip(directionVector.x);
        var positionVector = directionVector.y > 0 ? downPosition : upPosition;
        parentPanel.position = positionVector;

        Invoke("OnCompleteStickerAnimation", 0.8f);

        FadeIn();
    }

    private void OnCompleteStickerAnimation()
    {
        normalPanel.bg.gameObject.SetActive(!coachVO.isBestMove);
        bestPanel.bg.gameObject.SetActive(coachVO.isBestMove);
        parentPanel.localScale = Vector3.zero;

        iTween.ScaleTo(parentPanel.gameObject,
            iTween.Hash(
                "scale", new Vector3(1.1f * directionVector.x, 1.1f, 1.0f),
                "time", 0.3f,
                "islocal", true,
                "oncomplete", "OnCompleteScaleAnimation",
                "oncompletetarget", this.gameObject
                ));
    }

    private void OnCompleteScaleAnimation()
    {
        iTween.ScaleTo(parentPanel.gameObject,
            iTween.Hash(
                "scale", new Vector3(1.0f * directionVector.x, 1.0f, 1.0f),
                "time", 0.3f,
                "islocal", true
                ));
    }

    private void Flip(float scale)
    {
        var vectorToFlip = new Vector3(scale, 1, 1);
        parentPanel.localScale = vectorToFlip;
        if (coachVO.isBestMove)
        {
            bestPanel.icon.transform.localScale = vectorToFlip;
            bestPanel.text.transform.localScale = vectorToFlip;
        }
        else
        {
            normalPanel.icon.transform.localScale = vectorToFlip;
            normalPanel.text.transform.localScale = vectorToFlip;
        }
    }

    public void Hide()
    {
        arrowHead.transform.SetParent(coachPanel.transform, true);
        stickerBg.transform.SetParent(coachPanel.transform, true);

        coachPanel.SetActive(false);
        UiBlocker.SetActive(false);

        if (stickerBg.GetComponent<iTween>() != null)
        {
            Destroy(stickerBg.GetComponent<iTween>());
        }

        if (parentPanel.GetComponent<iTween>() != null)
        {
            Destroy(parentPanel.GetComponent<iTween>());
        }

        CancelInvoke();
        Reset();
    }

    public void Fade()
    {
        if (Time.time < timeAtShow + IGNORE_CLOSE_DURATION)
        {
            return;
        }

        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (coachVO.isBestMove)
        {
            bestPanel.bg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.icon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.text.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalPanel.bg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.icon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.text.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }


        coachVO.analyticsService.Event(AnalyticsEventId.close_pow_coach, coachVO.analyticsContext);

        Invoke("Hide", FADE_DURATION);
        //Invoke("Reset", FADE_DURATION);
    }

    private void FadeIn()
    {
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (coachVO.isBestMove)
        {
            bestPanel.bg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.icon.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.text.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalPanel.bg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.icon.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.text.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
    }

    private void Reset()
    {
        line.SetAlpha(LINE_ALPHA_MAX);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (coachVO.isBestMove)
        {
            bestPanel.bg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.icon.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            bestPanel.text.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }
        else
        {
            normalPanel.bg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.icon.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
            normalPanel.text.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        stickerBg.transform.localEulerAngles = Vector3.zero;
        stickerPieceIcon.transform.localEulerAngles = Vector3.zero;
    }

    
}
