using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TurboLabz.Chess;
using TurboLabz.InstantGame;

public class AnalysisMoveView : MonoBehaviour
{
    [Serializable]
    public class AnalysisMove
    {
        public Text moveNumber;
        public Text move;
        public Image piece;
        public Image moveQuality;
        public Image advantageBg;
        public Image advantageFillerLeft;
        public Image advantageFillerRight;
        public Text advantageLeft;
        public Text advantageRight;
        public Image separator;
        public Text lastMoveText;
    }

    public AnalysisMove normal;
    public AnalysisMove zoomed;
    public Sprite advantageFillerActive;
    public Sprite advantageFillerInactive;
    public Sprite moveQualityInactive;
    public Sprite advantageFillerPartial;

    public bool IsLocked => isLocked;
    public int MoveNumber => moveNumber;

    private bool isLocked;
    private int moveNumber;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMove(int moveNumberInt, string moveNumber, string move, Sprite moveQuality, Sprite piece, float playerAdvantage, bool isLocked, bool isPlayerTurn, bool isLastMove)
    {
        this.isLocked = isLocked;
        this.moveNumber = moveNumberInt;
        SetupMove(normal, moveNumber, move, moveQuality, piece, playerAdvantage, isPlayerTurn, isLastMove);
        SetupMove(zoomed, moveNumber, move, moveQuality, piece, playerAdvantage, isPlayerTurn, isLastMove);
    }

    private void SetupMove(AnalysisMove analysis, string moveNumber, string move, Sprite moveQuality, Sprite piece, float playerAdvantage, bool isPlayerTurn, bool isLastMove)
    {
        analysis.moveNumber.text = moveNumber;
        analysis.move.text = move;
        analysis.piece.sprite = piece;
        analysis.moveQuality.enabled = (moveQuality != null && isPlayerTurn) || isLocked;
        analysis.moveQuality.sprite = isLocked ? moveQualityInactive : moveQuality;
        analysis.advantageBg.sprite = isLocked ? advantageFillerInactive : advantageFillerActive;
        analysis.advantageBg.enabled = !isLastMove;

        analysis.advantageLeft.text = string.Format("{0:0.#}", playerAdvantage);
        analysis.advantageRight.text = string.Format("+{0:0.#}", playerAdvantage);
        analysis.advantageFillerLeft.enabled = analysis.advantageLeft.enabled = playerAdvantage <= -0.1f && !isLocked && !isLastMove;
        analysis.advantageFillerRight.enabled = analysis.advantageRight.enabled = playerAdvantage >= 0.1f && !isLocked && !isLastMove;

        var fillAmount = 91 * (Mathf.Abs(playerAdvantage) / 10);
        analysis.advantageFillerLeft.sprite = analysis.advantageFillerRight.sprite = fillAmount > 82 ? advantageFillerPartial : null;
        ((RectTransform)analysis.advantageFillerLeft.transform).sizeDelta = ((RectTransform)analysis.advantageFillerRight.transform).sizeDelta  = new Vector2(fillAmount, 36);

        analysis.separator.enabled = !isLocked && !isLastMove;
        analysis.lastMoveText.enabled = isLastMove;
    }
}
