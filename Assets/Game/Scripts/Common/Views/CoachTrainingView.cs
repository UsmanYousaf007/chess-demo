using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoachTrainingView : MonoBehaviour
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
    public GameObject moveTextPanel;
    public GameObject hand;
    public Button coachButton;
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
        hand.SetActive(false);
        piece3.SetActive(false);
        arrow.SetActive(false);
        moveTextPanel.SetActive(false);
        coachButton.interactable = true;
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
        coachButton.interactable = false;

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
        coachButton.interactable = true;

        iTween.ScaleTo(hand,
            iTween.Hash(
                "scale", Vector3.one,
                "time", 0.3f,
                "oncomplete", "OnCompleteHandScaleAnimation2",
                "oncompletetarget", this.gameObject
                ));
    }

    void OnCompleteHandScaleAnimation2()
    {
        arrow.SetActive(true);
        piece3.SetActive(true);

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
        moveTextPanel.SetActive(true);
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

        onScreenClose.SetActive(false);
        gameObject.SetActive(false);
    }
}
