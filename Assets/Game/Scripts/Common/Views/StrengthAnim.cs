using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

public class StrengthAnim : MonoBehaviour
{
	public GameObject strengthPanel;
    public Text strengthLabel;
	public Image[] barArray;
    public Image panelBg;
    public Text perfectText;
    public Image arrowHead;

    private const float MIN_WAIT = 0.1f;
    private float dotWaitSeconds;
	private Coroutine barAnim = null;
	private Coroutine panelActive;
    public DrawLine line;


    public void ShowStrengthPanel(int strength, Vector3 localPosFrom, Vector3 localPosTo)
	{
        perfectText.enabled = false;
        strengthPanel.SetActive(true);

        //draw line
        line.Draw(localPosFrom, localPosTo);

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
        var directionVector = new Vector2(localPosTo.x < 0 ? 1 : -1, localPosTo.y < 0 ? 1 : -1);
        var pixelsToMove = 100f;

        //calculate position for strength panel
        localPosTo = new Vector3(localPosTo.x + (pixelsToMove * directionVector.x), localPosTo.y + (pixelsToMove * directionVector.y));

        //set position of strength panel
        var strengthPanelRectTransform = strengthPanel.GetComponent<RectTransform>();
        viewportPoint = Camera.main.WorldToViewportPoint(localPosTo);
        strengthPanelRectTransform.anchorMin = viewportPoint;
        strengthPanelRectTransform.anchorMax = viewportPoint;

		DisableBar();
		int strengthString = strength * 10;

        iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", strengthString, "time", 1.0f,
            "onupdate", "AnimateStrengthPercentage", "onupdatetarget", this.gameObject,
            "oncomplete", "OnCompletePercentageAnimation", "oncompletetarget", this.gameObject, "oncompleteparams", strength));
		//strengthLabel.text = "Strength " + strengthString.ToString() + "%";

		if (strength > 0 && strength <= barArray.Length)
		{
			barAnim = StartCoroutine(Animate(strength));
		}

		StartCoroutine(HideStrengthPanel(4.0f));
	}

    private void AnimateStrengthPercentage(int value)
    {
        strengthLabel.text = string.Format("Strength {0}%", value);
    }

    private void OnCompletePercentageAnimation(int strength)
    {
        if (strength == 10)
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
        strengthPanel.SetActive(false);
        line.Hide();
        Reset();
    }

    private void Fade()
    {
        panelBg.CrossFadeAlpha(0f, 1f, false);
        strengthLabel.CrossFadeAlpha(0f, 1f, false);
        perfectText.CrossFadeAlpha(0f, 1f, false);
        arrowHead.CrossFadeAlpha(0f, 1f, false);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(0f, 1f, false);
        }

        line.Fade();

        Invoke("Hide", 1f);
    }

    private void Reset()
    {
        panelBg.CrossFadeAlpha(1f, 0f, false);
        strengthLabel.CrossFadeAlpha(1f, 0f, false);
        perfectText.CrossFadeAlpha(1f, 0f, false);
        arrowHead.CrossFadeAlpha(1f, 0f, false);

        foreach (var bar in barArray)
        {
            bar.CrossFadeAlpha(1f, 0f, false);
        }

        line.SetAlpha(255);
        arrowHead.transform.SetParent(strengthPanel.transform, true);
    }

}
