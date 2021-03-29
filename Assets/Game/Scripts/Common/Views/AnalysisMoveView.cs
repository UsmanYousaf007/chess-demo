using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalysisMoveView : MonoBehaviour
{
    public Text moveNumber;
    public Text move;
    public Image piece;
    public Image moveQuality;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMove(string moveNumber, string move, Sprite moveQuality)
    {
        this.moveNumber.text = moveNumber;
        this.move.text = move;
        this.piece.enabled = false;

        if (moveQuality != null)
        {
            this.moveQuality.enabled = true;
            this.moveQuality.sprite = moveQuality;
        }
        else
        {
            this.moveQuality.enabled = false;
        }
    }
}
