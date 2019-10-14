using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

public class StrengthAnim : MonoBehaviour
{
    //Visual properties
    public GameObject strengthPanel;
    public Text strengthLabel;
    public Image perfectIcon;
    public Image filler;
    public Image fillerBg;
    public Image panelBg;
    public Image arrowHead;
    public DrawLine line;
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

    private float timeAtShow = 0;

    //Constants
    const int LINE_ALPHA_MIN = 0;
    const int LINE_ALPHA_MAX = 204;
    const float FADE_DURATION = 0.1f;
    const float UI_ALPHA_MIN = 0f;
    const float UI_ALPHA_MAX = 1.0f;
    const bool IGNORE_TIMESCALE_WHILE_FADE = false;
    const float RESET_FADE_DURATION = 0f;
    const float PIXELS_TO_MOVE = 150.0f;
    const int START_STRENGTH_TEXT_ANIMATION_FROM = 0;
    const float STRENGTH_TEXT_ANIMATION_DURATION = 1.0f;
    const int MAX_STRENGTH = 100;
    const float IGNORE_CLOSE_DURATION = 2.3f;

    private StrengthVO strengthVO;

    public void ShowStrengthPanel(StrengthVO strengthVO)
	{
        timeAtShow = Time.time;

        this.strengthVO = strengthVO;

        perfectIcon.enabled = false;
        strengthLabel.enabled = true;

        strengthPanel.SetActive(true);
        UiBlocker.SetActive(true);
        panelBg.gameObject.SetActive(false);

        moveMeterButton.SetAsFirstSibling();

        var angle = Mathf.Atan2(strengthVO.fromPosition.y - strengthVO.toPosition.y, strengthVO.fromPosition.x - strengthVO.toPosition.x) * Mathf.Rad2Deg;
        var toScreenPosition = Camera.main.WorldToScreenPoint(strengthVO.toPosition);
        var stickerFromPosition = Camera.main.WorldToScreenPoint(strengthVO.fromPosition);

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

        line.Draw(strengthVO.fromPosition, Camera.main.ScreenToWorldPoint(lineEndPivot.position));
        
        arrowHead.transform.SetAsFirstSibling();
        stickerBg.sprite = strengthVO.pieceName[0].Equals('W') ? stickerBgBlack : stickerBgWhite;
        stickerBg.rectTransform.position = stickerFromPosition;
        stickerPieceIcon.sprite = SkinContainer.LoadSkin(strengthVO.activeSkinId).GetSprite(strengthVO.pieceName);

        iTween.MoveTo(stickerBg.gameObject,
            iTween.Hash(
                "position", toScreenPosition,
                "time", 1.0f,
                "easetype", iTween.EaseType.easeOutExpo,
                "oncomplete", "OnCompleteStickerAnimation",
                "oncompletetarget", this.gameObject
                ));

        //detect direction of arrow
        var directionVector = new Vector2(strengthVO.toPosition.x < 0 ? 1 : -1, strengthVO.fromPosition.y > strengthVO.toPosition.y ? 1 : -1);
        Flip(directionVector.x);
        var positionVector = directionVector.y > 0 ? downPosition : upPosition;
        panelBg.rectTransform.position = positionVector;

        FadeIn();

    }

    private void OnCompleteStickerAnimation()
    {

        panelBg.gameObject.SetActive(true);

        int strengthForLabel = Mathf.RoundToInt(strengthVO.strength * 100);

        //TODO use dotween
        iTween.ValueTo(this.gameObject,
            iTween.Hash(
                "from", START_STRENGTH_TEXT_ANIMATION_FROM,
                "to", strengthForLabel,
                "time", STRENGTH_TEXT_ANIMATION_DURATION,
                "onupdate", "AnimateStrengthPercentage",
                "onupdatetarget", this.gameObject,
                "oncomplete", "OnCompletePercentageAnimation",
                "oncompletetarget", this.gameObject,
                "oncompleteparams", strengthForLabel
            ));
    }

    private void Flip(float scale)
    {
        var flipVector = new Vector3(scale, 1, 1);
        panelBg.transform.localScale = flipVector;
        filler.transform.localScale = flipVector;
        fillerBg.transform.localScale = flipVector;
        perfectIcon.transform.localScale = flipVector;
        strengthLabel.transform.localScale = flipVector;
    }

    private void AnimateStrengthPercentage(int value)
    {
        strengthLabel.text = string.Format("{0}%", value);
        filler.fillAmount = (float)value / MAX_STRENGTH;
    }

    private void OnCompletePercentageAnimation(int strength)
    {
        if (strength == MAX_STRENGTH)
        {
            perfectIcon.enabled = true;
            strengthLabel.enabled = false;
        }
    }

    public void FadePanel()
    {
        if (Time.time < timeAtShow + IGNORE_CLOSE_DURATION)
        {
            return;
        }

        Fade();

        strengthVO.analyticsService.Event(AnalyticsEventId.close_pow_move_meter, strengthVO.analyticsContext);

    }

    public void Hide()
    {
        arrowHead.transform.SetParent(strengthPanel.transform, true);
        stickerBg.transform.SetParent(strengthPanel.transform, true);

        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthPanel.SetActive(false);
        UiBlocker.SetActive(false);
        if (strengthVO.fromIndicator != null)
        {
            strengthVO.fromIndicator.SetActive(false);
            strengthVO.toIndicator.SetActive(false);
        }
        line.Hide();

        if (stickerBg.GetComponent<iTween>() != null)
        {
            Destroy(stickerBg.GetComponent<iTween>());
        }

        if (this.gameObject.GetComponent<iTween>() != null)
        {
            Destroy(this.gameObject.GetComponent<iTween>());
        }

        CancelInvoke();
        Reset();
    }

    private void Fade()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        filler.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        fillerBg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        perfectIcon.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);

        Invoke("Hide", FADE_DURATION);
        //Invoke("Reset", FADE_DURATION);
    }

    private void FadeIn()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        filler.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        fillerBg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        perfectIcon.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);
    }

    private void Reset()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        filler.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        fillerBg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        perfectIcon.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        line.SetAlpha(LINE_ALPHA_MAX);
        filler.fillAmount = 0;
        stickerBg.transform.localEulerAngles = Vector3.zero;
        stickerPieceIcon.transform.localEulerAngles = Vector3.zero;
    }
}
