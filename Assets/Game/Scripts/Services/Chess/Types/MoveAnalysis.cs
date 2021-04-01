namespace TurboLabz.Chess
{
    public class MoveAnalysis 
    {
        public ChessMove playerMove;
        public ChessMove bestMove;
        public MoveQuality moveQuality;
        public float strength;
        public bool isPlayerMove;
        public int whiteScore;
        public int blackScore;

        public static MoveQuality MoveQualityToEnum(string quality)
        {
            var rv = MoveQuality.NORMAL;

            switch (quality)
            {
                case "BLUNDER":
                    rv = MoveQuality.BLUNDER;
                    break;

                case "MISTAKE":
                    rv = MoveQuality.MISTAKE;
                    break;

                case "PERFECT":
                    rv = MoveQuality.PERFECT;
                    break;
            }

            return rv;
        }
    }

    public enum MoveQuality
    {
        BLUNDER,
        MISTAKE,
        PERFECT,
        NORMAL
    }

    public class MatchAnalysis
    {
        public int blunders;
        public int perfectMoves;
        public int mistakes;
    }
}
