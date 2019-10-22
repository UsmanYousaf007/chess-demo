using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StrengthTrainingView : MonoBehaviour
{
    [System.Serializable]
    public class Pivots
    {
        public Transform from;
        public Transform to;
    }

    public GameObject piece1;
    public GameObject piece2;
    public GameObject piece3;
    public GameObject arrow;
    public GameObject strengthBar;
    public GameObject hand;
    public Button strengthButton;
    public Image filler;
    public Text strengthText;
    public Image perfectIcon;
    public Pivots piece1Pivots;
    public Pivots piece2Pivots;
    public Pivots handPivots;
    public GameObject onScreenClose;

    private Coroutine animationRoutine;

    // Start is called before the first frame update
    void OnEnable()
    {
        animationRoutine = StartCoroutine(AnimateInLoop());
        Setup();
        onScreenClose.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AnimateInLoop()
    {
        yield return new WaitForSeconds(0.7f);
        while (true)
        {
            Animate();
            yield return new WaitForSeconds(5.0f);
        }
    }

    void Setup()
    {
        piece1.transform.position = piece1Pivots.from.position;
        piece2.transform.position = piece2Pivots.from.position;
        hand.transform.position = handPivots.from.position;
        piece1.SetActive(true);
        piece2.SetActive(false);
        hand.SetActive(false);
        piece3.SetActive(false);
        arrow.SetActive(false);
        strengthBar.SetActive(false);
        filler.fillAmount = 0.0f;
        strengthText.text = "0%";
        strengthText.enabled = true;
        perfectIcon.enabled = false;
    }

    void Animate()
    {
        Setup();

        iTween.MoveTo(piece1,
            iTween.Hash(
                "position", piece1Pivots.to.position,
                "time", 0.4f,
                "oncomplete", "OnCompletePiece1Animation",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompletePiece1Animation()
    {
        hand.SetActive(true);

        iTween.MoveTo(hand,
            iTween.Hash(
                "position", handPivots.to.position,
                "time", 1f,
                "oncomplete", "OnCompleteHandAnimation",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompleteHandAnimation()
    {
        iTween.ScaleTo(hand,
            iTween.Hash(
                "scale", new Vector3(0.9f, 0.9f, 1.0f),
                "time", 0.3f,
                "oncomplete", "OnCompleteHandScaleAnimation1",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompleteHandScaleAnimation1()
    {
        strengthButton.interactable = false;

        iTween.ScaleTo(hand,
            iTween.Hash(
                "scale", new Vector3(1.1f, 1.1f, 1.0f),
                "time", 0.3f,
                "oncomplete", "OnCompleteHandScaleAnimation2",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompleteHandScaleAnimation2()
    {
        strengthButton.interactable = true;
        piece1.SetActive(false);
        piece2.SetActive(true);
        arrow.SetActive(true);
        piece3.SetActive(true);

        iTween.ScaleTo(hand,
            iTween.Hash(
                "scale", Vector3.one,
                "time", 0.3f
                ));

        iTween.MoveTo(piece2,
            iTween.Hash(
                "position", piece2Pivots.to.position,
                "time", 1f,
                "oncomplete", "OnCompletePiece2Animation",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompletePiece2Animation()
    {
        strengthBar.SetActive(true);

        iTween.ValueTo(strengthBar,
            iTween.Hash(
                "from", 0,
                "to", 100,
                "time", 1.0f,
                "onupdate", "AnimateStrengthPercentage",
                "onupdatetarget", this.gameObject,
                "oncomplete", "OnCompletePercentageAnimation",
                "oncompletetarget", this.gameObject
                ));
    }

    void AnimateStrengthPercentage(int value)
    {
        strengthText.text = string.Format("{0}%", value);
        filler.fillAmount = (float)value / 100;
    }

    private void OnCompletePercentageAnimation()
    {
        perfectIcon.enabled = true;
        strengthText.enabled = false;
    }

    public void Close()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

        if (piece1.GetComponent<iTween>() != null)
        {
            Destroy(piece1.GetComponent<iTween>());
        }

        if (piece2.GetComponent<iTween>() != null)
        {
            Destroy(piece2.GetComponent<iTween>());
        }

        if (hand.GetComponent<iTween>() != null)
        {
            Destroy(hand.GetComponent<iTween>());
        }

        if (strengthBar.GetComponent<iTween>() != null)
        {
            Destroy(strengthBar.GetComponent<iTween>());
        }

        onScreenClose.SetActive(false);
        gameObject.SetActive(false);
    }
}
