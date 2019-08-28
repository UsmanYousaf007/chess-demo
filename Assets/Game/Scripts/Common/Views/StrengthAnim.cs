using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

public class StrengthAnim : MonoBehaviour
{
	public GameObject strengthPanel;
    public Text strengthLabel;
	public Image[] barArray;

    private const float MIN_WAIT = 0.1f;
    private float dotWaitSeconds;
	private Coroutine barAnim = null;
	private Coroutine panelActive;
    public DrawLine line;


    public void ShowStrengthPanel(int strength, Vector3 localPosFrom, Vector3 localPosTo)
	{
		strengthPanel.SetActive(true);

        //draw line
        line.Draw(localPosFrom, localPosTo);

        float addPosX = 150;
		float addPosY = 110;

		if (localPosTo.x < 0)
			localPosTo.x = localPosTo.x + addPosX;
		else
			localPosTo.x = localPosTo.x - addPosX;

		localPosTo.y += addPosY;

		strengthPanel.transform.localPosition = localPosTo;


		DisableBar();
		int strengthString = strength * 10;
		strengthLabel.text = "Strength " + strengthString.ToString() + "%";

		if (strength > 0 && strength <= barArray.Length)
		{
			barAnim = StartCoroutine(Animate(strength));
		}

		StartCoroutine(HideStrengthPanel(3.0f));
	}

	public IEnumerator HideStrengthPanel(float t)
	{
		yield return new WaitForSeconds(t);
        Hide();

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
    }

}
