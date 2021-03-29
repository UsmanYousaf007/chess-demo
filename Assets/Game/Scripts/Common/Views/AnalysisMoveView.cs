using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnalysisMoveView : MonoBehaviour
{
    [Serializable]
    public class AnalysisMove
    {
        public Text moveNumber;
        public Text move;
        public Image piece;
        public Image moveQuality;
    }

    public AnalysisMove normal;
    public AnalysisMove zoomed;

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
        SetupMove(normal, moveNumber, move, moveQuality);
        SetupMove(zoomed, moveNumber, move, moveQuality);
    }

    private void SetupMove(AnalysisMove analysis, string moveNumber, string move, Sprite moveQuality)
    {
        analysis.moveNumber.text = moveNumber;
        analysis.move.text = move;
        analysis.piece.enabled = false;

        if (moveQuality != null)
        {
            analysis.moveQuality.enabled = true;
            analysis.moveQuality.sprite = moveQuality;
        }
        else
        {
            analysis.moveQuality.enabled = false;
        }
    }
}
