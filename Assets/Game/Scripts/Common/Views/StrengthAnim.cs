using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

public class StrengthAnim : MonoBehaviour
{
    //Visual properties
    public GameObject strengthPanel;
    public Text strengthLabel;
	public Image[] barArray;
    public Image panelBg;
    public Text perfectText;
    public Image arrowHead;
    public DrawLine line;

    private float dotWaitSeconds;
    private Coroutine barAnim = null;
    private Coroutine panelActive;

    //Constants
    const float MIN_WAIT = 0.1f;
    const int LINE_ALPHA_MIN = 0;
    const int LINE_ALPHA_MAX = 255;
    const float START_FADE_AFTER_SECONDS = 4.0f;
    const float FADE_DURATION = 1.0f;
    const float UI_ALPHA_MIN = 0f;
    const float UI_ALPHA_MAX = 1.0f;
    const bool IGNORE_TIMESCALE_WHILE_FADE = false;
    const float RESET_FADE_DURATION = 0f;
    const float PIXELS_TO_MOVE = 100.0f;
    const int START_STRENGTH_TEXT_ANIMATION_FROM = 0;
    const float STRENGTH_TEXT_ANIMATION_DURATION = 1.0f;
    const int MAX_STRENGTH = 10;

    public void ShowStrengthPanel(int strength, Vector3 localPosFrom, Vector3 localPosTo)
	{
        perfectText.enabled = false;
        strengthPanel.SetActive(true);

        //draw line
        line.Draw(localPosFrom, localPosTo);
        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);

        //set arrowhead position
        //move it to the root of canvas then calculate screen to canvas postion
        arrowHead.transform.SetParent(this.transform.root.GetChild(0), true);
        var viewportPoint = Camera.main.WorldToViewportPoint(localPosTo);
        arrowHead.rectTransform.anchorMin = viewportPoint;
        arrowHead.rectTransform.anchorMax = viewportPoint;

        //set rotation of arrowhead by calculating angle between to and from points
        var angle = Mathf.Atan2(localPosFrom.y - localPosTo.y, localPosFrom.x - localPosTo.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        //detect direction of arrow
        var directionVector = new Vector2(localPosTo.x < 0 ? 1 : -1, 1 /*localPosTo.y < 0 ? 1 : -1*/);

        //calculate position for strength panel
        localPosTo = new Vector3(localPosTo.x + (PIXELS_TO_MOVE * directionVector.x), localPosTo.y + (PIXELS_TO_MOVE * directionVector.y));

        //set position of strength panel
        var strengthPanelRectTransform = strengthPanel.GetComponent<RectTransform>();
        viewportPoint = Camera.main.WorldToViewportPoint(localPosTo);
        strengthPanelRectTransform.anchorMin = viewportPoint;
        strengthPanelRectTransform.anchorMax = viewportPoint;

		DisableBar();
		int strengthString = strength * 10;

        //TODO use dotween
        iTween.ValueTo(this.gameObject, iTween.Hash(
            "from", START_STRENGTH_TEXT_ANIMATION_FROM,
            "to", strengthString,
            "time", STRENGTH_TEXT_ANIMATION_DURATION,
            "onupdate", "AnimateStrengthPercentage",
            "onupdatetarget", this.gameObject,
            "oncomplete", "OnCompletePercentageAnimation",
            "oncompletetarget", this.gameObject,
            "oncompleteparams", strength));

		if (strength > 0 && strength <= barArray.Length)
		{
			barAnim = StartCoroutine(Animate(strength));
		}

		StartCoroutine(HideStrengthPanel(START_FADE_AFTER_SECONDS));
	}

    private void AnimateStrengthPercentage(int value)
    {
        strengthLabel.text = string.Format("Strength {0}%", value);
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
        strengthPanel.SetActive(false);
        line.Hide();
        CancelInvoke();
        //Reset();
    }

    private void Fade()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
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
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        //perfectText.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(UI_ALPHA_MAX, RESET_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        line.SetAlpha(LINE_ALPHA_MAX);
    }

}
