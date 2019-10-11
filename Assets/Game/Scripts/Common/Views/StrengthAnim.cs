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
    public Image closeToolTipImage;
    public Text closeToolTipText;
    public Image stickerBg;
    public Image stickerPieceIcon;
    public Sprite stickerBgWhite;
    public Sprite stickerBgBlack;
    public Transform moveMeterButton;

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
    const float TOOL_TIP_FADE_DURATION = 1.0f;
    const float HIDE_TOOL_TIP_AFTER = 2.7f;
    const float CLOSE_BUTTON_SCALE_DURATION = 1.0f;
    readonly Vector3 CLOSE_BUTTON_SCALE_FINAL = new Vector3(2.0f, 2.0f, 1.0f);

    private StrengthVO strengthVO;

    public void ShowStrengthPanel(StrengthVO strengthVO)
	{

        this.strengthVO = strengthVO;

        perfectText.enabled = false;
        strengthPanel.SetActive(true);
        chessboardBlocker.SetActive(true);
        UiBlocker.SetActive(true);

        moveMeterButton.SetAsFirstSibling();

        //set arrowhead position
        //move it to the root of canvas then calculate screen to canvas postion
        arrowHead.transform.SetParent(this.transform.parent.parent, true);
        var viewportPoint = Camera.main.WorldToScreenPoint(strengthVO.toPosition);
        arrowHead.rectTransform.position = viewportPoint;

        //set rotation of arrowhead by calculating angle between to and from points
        var angle = Mathf.Atan2(strengthVO.fromPosition.y - strengthVO.toPosition.y, strengthVO.fromPosition.x - strengthVO.toPosition.x) * Mathf.Rad2Deg;
        arrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

        //draw line
        line.Draw(strengthVO.fromPosition, Camera.main.ScreenToWorldPoint(lineEndPivot.position));

        var stickerToPosition = viewportPoint;
        var stickerFromPosition = Camera.main.WorldToScreenPoint(strengthVO.fromPosition);
        stickerBg.transform.SetParent(this.transform.parent.parent, true);
        stickerBg.transform.SetAsFirstSibling();
        arrowHead.transform.SetAsFirstSibling();
        stickerBg.sprite = strengthVO.pieceName[0].Equals('W') ? stickerBgBlack : stickerBgWhite;
        stickerBg.rectTransform.position = stickerFromPosition;
        stickerPieceIcon.sprite = SkinContainer.LoadSkin(strengthVO.activeSkinId).GetSprite(strengthVO.pieceName);
        stickerBg.transform.localEulerAngles = new Vector3(0, 0, angle);
        stickerPieceIcon.transform.localEulerAngles = new Vector3(0, 0, angle * -1);

        iTween.MoveTo(stickerBg.gameObject,
            iTween.Hash(
                "position", stickerToPosition,
                "time", 1.0f,
                "easetype", iTween.EaseType.easeOutExpo
                ));

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
        iTween.ValueTo(this.gameObject,
            iTween.Hash(
                "from", START_STRENGTH_TEXT_ANIMATION_FROM,
                "to", strengthForLabel,
                "time", STRENGTH_TEXT_ANIMATION_DURATION,
                "onupdate", "AnimateStrengthPercentage",
                "onupdatetarget", this.gameObject,
                "oncomplete", "OnCompletePercentageAnimation",
                "oncompletetarget", this.gameObject,
                "oncompleteparams", strengthForBars
            ));

		if (strengthForBars > 0 && strengthForBars <= barArray.Length)
		{
			barAnim = StartCoroutine(Animate(strengthForBars));
		}

        FadeIn();

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
        stickerBg.transform.SetParent(strengthPanel.transform, true);

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

        closeToolTipImage.gameObject.SetActive(false);

        if (closeButtonBg.GetComponent<iTween>() != null)
        {
            closeButtonBg.rectTransform.localScale = Vector3.one;
            Destroy(closeButtonBg.GetComponent<iTween>());
        }

        if (stickerBg.GetComponent<iTween>() != null)
        {
            Destroy(stickerBg.GetComponent<iTween>());
        }

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
            bar.GetComponentInChildren<Image>().CrossFadeAlpha(UI_ALPHA_MIN, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        line.Fade(LINE_ALPHA_MAX, LINE_ALPHA_MIN, FADE_DURATION);

        Invoke("Hide", FADE_DURATION);
        //Invoke("Reset", FADE_DURATION);
    }

    private void FadeIn()
    {
        panelBg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButtonBg.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeButton.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthLabel.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        strengthText.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        arrowHead.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        }

        line.SetAlpha(LINE_ALPHA_MIN);
        line.Fade(LINE_ALPHA_MIN, LINE_ALPHA_MAX, FADE_DURATION);
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

    public void ShowToolTip()
    {
        strengthVO.audioService.Play(strengthVO.audioService.sounds.SFX_TOOL_TIP);

        closeToolTipImage.gameObject.SetActive(true);
        closeToolTipImage.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeToolTipText.CrossFadeAlpha(UI_ALPHA_MAX, FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);

        if (closeButtonBg.GetComponent<iTween>() == null)
        {
            //animate close button
            iTween.ScaleTo(closeButtonBg.gameObject,
                iTween.Hash(
                    "scale", CLOSE_BUTTON_SCALE_FINAL,
                    "islocal", true,
                    "time", CLOSE_BUTTON_SCALE_DURATION,
                    "looptype", iTween.LoopType.pingPong
                    ));
        }

        Invoke("FadeToolTip", HIDE_TOOL_TIP_AFTER);
    }

    private void FadeToolTip()
    {
        closeToolTipImage.CrossFadeAlpha(UI_ALPHA_MIN, TOOL_TIP_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
        closeToolTipText.CrossFadeAlpha(UI_ALPHA_MIN, TOOL_TIP_FADE_DURATION, IGNORE_TIMESCALE_WHILE_FADE);
    }
}
