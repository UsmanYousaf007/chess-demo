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
    public Text strengthText;
	public Image[] barArray;
    public Image panelBg;
    public Text perfectText;
    public Image arrowHead;
    public DrawLine line;
    public Image closeButtonBg;
    public Image closeButton;
    public Transform lineEndPivot;
    public GameObject chessboardBlocker;
    public GameObject UiBlocker;

    private float dotWaitSeconds;
    private Coroutine barAnim = null;
    private Coroutine panelActive;

    //Constants
    const float MIN_WAIT = 0.1f;
    const int LINE_ALPHA_MIN = 0;
    const int LINE_ALPHA_MAX = 204;
    const float START_FADE_AFTER_SECONDS = 4.0f;
    const float FADE_DURATION = 0.1f;
    const float UI_ALPHA_MIN = 0f;
    const float UI_ALPHA_MAX = 1.0f;
    const bool IGNORE_TIMESCALE_WHILE_FADE = false;
    const float RESET_FADE_DURATION = 0f;
    const float PIXELS_TO_MOVE = 150.0f;
    const int START_STRENGTH_TEXT_ANIMATION_FROM = 0;
    const float STRENGTH_TEXT_ANIMATION_DURATION = 1.0f;
    const int MAX_STRENGTH = 10;
    const float ARROW_ALPHA_MAX = 0.8f;

    private StrengthVO strengthVO;

    public void ShowStrengthPanel(StrengthVO strengthVO)
	{

        this.strengthVO = strengthVO;

        perfectText.enabled = false;
        strengthPanel.SetActive(true);
        chessboardBlocker.SetActive(true);
        UiBlocker.SetActive(true);

        //set arrowhead position
        //move it to the root of canvas then calculate screen to canvas postion
        arrowHead.transform.SetParent(this.transform.parent.parent, true);
        arrowHead.transform.SetAsFirstSibling();
        var viewportPoint = Camera.main.WorldToScreenPoint(strengthVO.toPosition);
        arrowHead.rectTransform.position = viewportPoint;
        arrowHead.CrossFadeAlpha(ARROW_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        //set rotation of arrowhead by calculating angle between to and from points
        var angle = Mathf.Atan2(strengthVO.fromPosition.y - strengthVO.toPosition.y, strengthVO.fromPosition.x - strengthVO.toPosition.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        //draw line
        line.Draw(strengthVO.fromPosition, Camera.main.ScreenToWorldPoint(lineEndPivot.position));
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);

        //detect direction of arrow
        var directionVector = new Vector2(strengthVO.toPosition.x < 0 ? 1 : -1, 1 /*localPosTo.y < 0 ? 1 : -1*/);

        //calculate position for strength panel
        strengthVO.toPosition = new Vector3(strengthVO.toPosition.x + (PIXELS_TO_MOVE * directionVector.x),
            strengthVO.toPosition.y + (PIXELS_TO_MOVE * directionVector.y));

        //set position of strength panel
        var strengthPanelRectTransform = strengthPanel.GetComponent<RectTransform>();
        viewportPoint = Camera.main.WorldToScreenPoint(strengthVO.toPosition);
        strengthPanelRectTransform.position = viewportPoint;

		DisableBar();
        int strengthForLabel = Mathf.RoundToInt(strengthVO.strength * 100);
        int strengthForBars = Mathf.FloorToInt(strengthVO.strength * 10);

        //TODO use dotween
        iTween.ValueTo(this.gameObject, iTween.Hash(
            "from", START_STRENGTH_TEXT_ANIMATION_FROM,
            "to", strengthForLabel,
            "time", STRENGTH_TEXT_ANIMATION_DURATION,
            "onupdate", "AnimateStrengthPercentage",
            "onupdatetarget", this.gameObject,
            "oncomplete", "OnCompletePercentageAnimation",
            "oncompletetarget", this.gameObject,
            "oncompleteparams", strengthForBars));

		if (strengthForBars > 0 && strengthForBars <= barArray.Length)
		{
			barAnim = StartCoroutine(Animate(strengthForBars));
		}

		//StartCoroutine(HideStrengthPanel(START_FADE_AFTER_SECONDS));
	}

    private void AnimateStrengthPercentage(int value)
    {
        strengthLabel.text = string.Format("{0}%", value);
    }

    private void OnCompletePercentageAnimation(int strength)
    {
        if (strength == MAX_STRENGTH)
        {
            perfectText.enabled = true;
        }
    }

    public IEnumerator HideStrengthPanel(float t)
	{
		yield return new WaitForSeconds(t);
        //Hide();
        Fade();

        if (barAnim != null)
		{
			StopCoroutine(barAnim);
			barAnim = null;
		}
	}

    public void FadePanel()
    {
        Fade();

        strengthVO.analyticsService.Event(AnalyticsEventId.close_pow_move_meter, strengthVO.analyticsContext);

        if (barAnim != null)
        {
            StopCoroutine(barAnim);
            barAnim = null;
        }
    }

	public void DisableBar()
	{
		for (int i = 0; i < barArray.Length; i++)
		{
			barArray[i].color = Colors.strengthBarDisableColor;
		}
	}

	IEnumerator Animate(int strength)
	{
		float waitTime = Mathf.Max(MIN_WAIT, dotWaitSeconds);

		while (true)
		{
			for (int i = 0; i < strength; i++)
			{
				barArray[i].color = Colors.strengthBarColor[i];
				yield return new WaitForSeconds(waitTime);

			}

			yield return new WaitForSeconds(waitTime);
		}
	}

    public void Hide()
    {
        arrowHead.transform.SetParent(strengthPanel.transform, true);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthPanel.SetActive(false);
        chessboardBlocker.SetActive(false);
        UiBlocker.SetActive(false);
        if (strengthVO.fromIndicator != null)
        {
            strengthVO.fromIndicator.SetActive(false);
            strengthVO.toIndicator.SetActive(false);
        }
        line.Hide();
        CancelInvoke();
        //Reset();
    }

    private void Fade()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButtonBg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButton.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        //perfectText.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);

        Invoke("Hide", FADE_DURATION);
        Invoke("Reset", FADE_DURATION);
    }

    private void Reset()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButtonBg.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButton.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        //perfectText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        line.SetAlpha(LINE_ALPHA_MAX);
    }

}
