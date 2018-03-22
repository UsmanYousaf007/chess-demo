namespace ChessboardLib
{
    #region Using

    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;

    #endregion

    /// <summary>
    ///   Represents the game of chess over its lfetime. Holds the board, players, turn number and everything related to the chess game in progress.
    /// </summary>
    public static class Game
    {
        #region Constants and Fields

        /// <summary>
        ///   The file name.
        /// </summary>
        private static string saveGameFileName = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref="Game" /> class.
        /// </summary>
        static Game()
        {
            MoveRedoList = new Moves();
            MoveHistory = new Moves();
            FenStartPosition = string.Empty;
            HashTablePawn.Initialise();
            HashTableCheck.Initialise();

            PlayerWhite = new PlayerWhite();
            PlayerBlack = new PlayerBlack();
            PlayerToPlay = PlayerWhite;
            Board.EstablishHashKey();


        }

        #endregion

        #region Delegates

        /// <summary>
        ///   The game event type, raised to the UI when significant game events occur.
        /// </summary>
        public delegate void GameEvent();

        #endregion

        #region Public Events


        /// <summary>
        ///   Riased when the game is saved.
        /// </summary>
        public static event GameEvent GameSaved;

        /// <summary>
        ///   Raised when settings are updated.
        /// </summary>
        public static event GameEvent SettingsUpdated;

        #endregion

        #region Enums

        /// <summary>
        ///   Game stages.
        /// </summary>
        public enum GameStageNames
        {
            /// <summary>
            ///   The opening.
            /// </summary>
            Opening,

            /// <summary>
            ///   The middle.
            /// </summary>
            Middle,

            /// <summary>
            ///   The end.
            /// </summary>
            End
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets the available MegaBytes of free computer memory.
        /// </summary>
        public static uint AvailableMegaBytes
        {
            get
            {
				return 16;
			}
        }

       
        
       
        /// <summary>
        ///   Gets or sets the FEN string for the chess Start Position.
        /// </summary>
        public static string FenStartPosition { private get; set; }

        /// <summary>
        ///   Gets or sets FiftyMoveDrawBase. Appears to be a value set when using a FEN string. Doesn't seem quite right! TODO Invesigate FiftyMoveDrawBase.
        /// </summary>
        public static int FiftyMoveDrawBase { get; set; }

        /// <summary>
        ///   Gets the current game save file name.
        /// </summary>
        public static string FileName
        {
            get
            {
                return saveGameFileName == string.Empty ? "New Game" : saveGameFileName;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether Analyse Mode is active.
        /// </summary>
        public static bool IsInAnalyseMode { get; set; }


        /// <summary>
        ///   Gets the lowest material count for black or white.
        /// </summary>
        public static int LowestMaterialCount
        {
            get
            {
                int intWhiteMaterialCount = PlayerWhite.MaterialCount;
                int intBlackMaterialCount = PlayerBlack.MaterialCount;
                return intWhiteMaterialCount < intBlackMaterialCount ? intWhiteMaterialCount : intBlackMaterialCount;
            }
        }

        /// <summary>
        ///   Gets the largest valid Material Count.
        /// </summary>
        public static int MaxMaterialCount
        {
            get
            {
                return 7;
            }
        }


        /// <summary>
        ///   Gets the currebt move history.
        /// </summary>
        public static Moves MoveHistory { get; private set; }

        /// <summary>
        ///   Gets the current move number.
        /// </summary>
        public static int MoveNo
        {
            get
            {
                return TurnNo >> 1;
            }
        }

        /// <summary>
        ///   Gets the move redo list.
        /// </summary>
        public static Moves MoveRedoList { get; private set; }

        /// <summary>
        ///   Gets black player.
        /// </summary>
        public static Player PlayerBlack { get; private set; }

        /// <summary>
        ///   Gets or sets the player to play.
        /// </summary>
        public static Player PlayerToPlay { get; set; }

        /// <summary>
        ///   Gets white player.
        /// </summary>
        public static Player PlayerWhite { get; private set; }

        /// <summary>
        ///   Gets or sets a value indicating whether to show thinking.
        /// </summary>
        public static bool ShowThinking { get; set; }

        /// <summary>
        ///   Gets current game stage.
        /// </summary>
        public static GameStageNames Stage
        {
            get
            {
                if (LowestMaterialCount >= MaxMaterialCount)
                {
                    return GameStageNames.Opening;
                }

                return LowestMaterialCount <= 3 ? GameStageNames.End : GameStageNames.Middle;
            }
        }

        /// <summary>
        ///   Gets the current turn number.
        /// </summary>
        public static int TurnNo { get; internal set; }

        
        #endregion

        #region Public Methods

        /// <summary>
        ///   Captures all pieces.
        /// </summary>
        public static void CaptureAllPieces()
        {
            PlayerWhite.CaptureAllPieces();
            PlayerBlack.CaptureAllPieces();
        }

        /// <summary>
        ///   Demotes all pieces.
        /// </summary>
        public static void DemoteAllPieces()
        {
            PlayerWhite.DemoteAllPieces();
            PlayerBlack.DemoteAllPieces();
        }

        /// <summary>
        ///   Load a saved game.
        /// </summary>
        /// <param name="fileName"> File name. </param>
        /// <returns> Returns True is game loaded successfully. </returns>
        public static bool Load(string fileName)
        {
            NewInternal();
            saveGameFileName = fileName;
            bool blnSuccess = LoadGame(fileName);

            return blnSuccess;
        }


        /// <summary>
        ///   Make a move.
        /// </summary>
        /// <param name="moveName"> The move name. </param>
        /// <param name="piece"> The piece to move. </param>
        /// <param name="square"> The square to move to. </param>
        public static void MakeAMove(Move.MoveNames moveName, Piece piece, Square square)
        {
            MakeAMoveInternal(moveName, piece, square);
        }

        /// <summary>
        ///   Start a new game.
        /// </summary>
        public static void New()
        {
            New(string.Empty);
        }

        /// <summary>
        ///   Start a new game using a FEN string.
        /// </summary>
        /// <param name="fenString"> The FEN string. </param>
        public static void New(string fenString)
        {
            NewInternal(fenString);
        }


        /// <summary>
        ///   Redo all moves.
        /// </summary>
        public static void RedoAllMoves()
        {
            while (MoveRedoList.Count > 0)
            {
                RedoMoveInternal();
            }
        }

        /// <summary>
        ///   Redo a move.
        /// </summary>
        public static void RedoMove()
        {
            RedoMoveInternal();
        }

        /// <summary>
        ///   Save the game as a file name.
        /// </summary>
        /// <param name="fileName"> The file name. </param>
        public static void Save(string fileName)
        {
 
            SaveGame(fileName);
            saveGameFileName = fileName;

            GameSaved();

        }

        /// <summary>
        ///   Call when settings have been changed in the UI.
        /// </summary>
        public static void SettingsUpdate()
        {
            SettingsUpdated();
        }


        /// <summary>
        ///   Undo all moves.
        /// </summary>
        public static void UndoAllMoves()
        {
            UndoAllMovesInternal();
        }

        /// <summary>
        ///   Undo the last move.
        /// </summary>
        public static void UndoMove()
        {
            UndoMoveInternal();
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Add a move node to the save game XML document.
        /// </summary>
        /// <param name="xmldoc"> Xml document representing the save game file. </param>
        /// <param name="xmlnodeGame"> Parent game xmlnode. </param>
        /// <param name="move"> Move to append to the save game Xml document. </param>
        private static void AddSaveGameNode(XmlDocument xmldoc, XmlElement xmlnodeGame, Move move)
        {
            XmlElement xmlnodeMove = xmldoc.CreateElement("Move");
            xmlnodeGame.AppendChild(xmlnodeMove);
            xmlnodeMove.SetAttribute("MoveNo", move.MoveNo.ToString(CultureInfo.InvariantCulture));
            xmlnodeMove.SetAttribute("Name", move.Name.ToString());
            xmlnodeMove.SetAttribute("From", move.From.Name);
            xmlnodeMove.SetAttribute("To", move.To.Name);
            xmlnodeMove.SetAttribute("SecondsElapsed", Convert.ToInt32(move.TimeStamp.TotalSeconds).ToString(CultureInfo.InvariantCulture));
        }


        /// <summary>
        ///   Load game from the specified file name.
        /// </summary>
        /// <param name="strFileName"> The file name. </param>
        /// <returns> True if load was successful. </returns>
        private static bool LoadGame(string strFileName)
        {
            MoveRedoList.Clear();
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.Load(strFileName);
            }
            catch
            {
                return false;
            }

            XmlElement xmlnodeGame = (XmlElement)xmldoc.SelectSingleNode("/Game");

            if (xmlnodeGame == null)
            {
                return false;
            }

            if (xmlnodeGame.GetAttribute("FEN") != string.Empty)
            {
                NewInternal(xmlnodeGame.GetAttribute("FEN"));
            }
				
            if (xmlnodeGame.GetAttribute("BoardOrientation") != string.Empty)
            {
                Board.Orientation = xmlnodeGame.GetAttribute("BoardOrientation") == "White"
                                        ? Board.OrientationNames.White
                                        : Board.OrientationNames.Black;
            }

            XmlNodeList xmlnodelist = xmldoc.SelectNodes("/Game/Move");

            if (xmlnodelist != null)
            {
                foreach (XmlElement xmlnode in xmlnodelist)
                {
                    Square from;
                    Square to;
                    if (xmlnode.GetAttribute("FromFile") != string.Empty)
                    {
                        from = Board.GetSquare(
                            Convert.ToInt32(xmlnode.GetAttribute("FromFile")),
                            Convert.ToInt32(xmlnode.GetAttribute("FromRank")));
                        to = Board.GetSquare(
                            Convert.ToInt32(xmlnode.GetAttribute("ToFile")),
                            Convert.ToInt32(xmlnode.GetAttribute("ToRank")));
                    }
                    else
                    {
                        from = Board.GetSquare(xmlnode.GetAttribute("From"));
                        to = Board.GetSquare(xmlnode.GetAttribute("To"));
                    }

                    MakeAMoveInternal(Move.MoveNameFromString(xmlnode.GetAttribute("Name")), from.Piece, to);
                    TimeSpan tsnTimeStamp;
                    if (xmlnode.GetAttribute("SecondsElapsed") == string.Empty)
                    {
                        if (MoveHistory.Count <= 2)
                        {
                            tsnTimeStamp = new TimeSpan(0);
                        }
                        else
                        {
                            tsnTimeStamp = MoveHistory.PenultimateForSameSide.TimeStamp + (new TimeSpan(0, 0, 30));
                        }
                    }
                    else
                    {
                        tsnTimeStamp = new TimeSpan(0, 0, int.Parse(xmlnode.GetAttribute("SecondsElapsed")));
                    }

                    MoveHistory.Last.TimeStamp = tsnTimeStamp;
                }

                int intTurnNo = xmlnodeGame.GetAttribute("TurnNo") != string.Empty
                                    ? int.Parse(xmlnodeGame.GetAttribute("TurnNo"))
                                    : xmlnodelist.Count;

                for (int intIndex = xmlnodelist.Count; intIndex > intTurnNo; intIndex--)
                {
                    UndoMoveInternal();
                }
            }

            return true;
        }

        /// <summary>
        ///   Make the specified move. For internal use only.
        /// </summary>
        /// <param name="moveName"> The move name. </param>
        /// <param name="piece"> The piece to move. </param>
        /// <param name="square"> The square to move to. </param>
        private static void MakeAMoveInternal(Move.MoveNames moveName, Piece piece, Square square)
        {
            MoveRedoList.Clear();
            Move move = piece.Move(moveName, square);
            move.EnemyStatus = move.Piece.Player.OpposingPlayer.Status;
            
            PlayerToPlay = PlayerToPlay.OpposingPlayer;
        }

        /// <summary>
        ///   Start a new game. For internal use only.
        /// </summary>
        private static void NewInternal()
        {
            NewInternal(string.Empty);
        }

        /// <summary>
        ///   Start a new game from the specified FEN string position. For internal use only.
        /// </summary>
        /// <param name="fenString"> The str fen. </param>
        private static void NewInternal(string fenString)
        {
            if (fenString == string.Empty)
            {
                fenString = Fen.GameStartPosition;
            }

            Fen.Validate(fenString);

            HashTablePawn.Clear();
            HashTableCheck.Clear();

            UndoAllMovesInternal();
            MoveRedoList.Clear();
            saveGameFileName = string.Empty;
            Fen.SetBoardPosition(fenString);

            Board.StartingHashCodeA = Board.HashCodeA;
            Board.StartingHashCodeB = Board.HashCodeB;
        }

        /// <summary>
        ///   Called when the computer has finished thinking, and is ready to make its move.
        /// </summary>
        /// <exception cref="ApplicationException">Raised when principal variation is empty.</exception>
		/*
		private static void PlayerReadyToMakeMove()
        {
            Move move;
            if (PlayerToPlay.Brain.PrincipalVariation.Count > 0)
            {
                move = PlayerToPlay.Brain.PrincipalVariation[0];
            }
            else
            {
                throw new ApplicationException("Player_ReadToMakeMove: Principal Variation is empty.");
            }

            MakeAMoveInternal(move.Name, move.Piece, move.To);
            SaveBackup();
            SendBoardPositionChangeEvent();
            ResumePondering();
        }
        */

        /// <summary>
        ///   Redo move. For internal use only.
        /// </summary>
        private static void RedoMoveInternal()
        {
            if (MoveRedoList.Count > 0)
            {
                Move moveRedo = MoveRedoList[MoveRedoList.Count - 1];
                moveRedo.Piece.Move(moveRedo.Name, moveRedo.To);
                MoveHistory.Last.TimeStamp = moveRedo.TimeStamp;
                MoveHistory.Last.EnemyStatus = moveRedo.Piece.Player.OpposingPlayer.Status; // 14Mar05 Nimzo
                PlayerToPlay = PlayerToPlay.OpposingPlayer;
                MoveRedoList.RemoveLast();
            }
        }


        /// <summary>
        ///   Save game using the specified file name.
        /// </summary>
        /// <param name="fileName"> The file name. </param>
        private static void SaveGame(string fileName)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlElement xmlnodeGame = xmldoc.CreateElement("Game");

            xmldoc.AppendChild(xmlnodeGame);

            xmlnodeGame.SetAttribute("FEN", FenStartPosition == Fen.GameStartPosition ? string.Empty : FenStartPosition);
            xmlnodeGame.SetAttribute("TurnNo", TurnNo.ToString(CultureInfo.InvariantCulture));
            xmlnodeGame.SetAttribute(
                "BoardOrientation", Board.Orientation == Board.OrientationNames.White ? "White" : "Black");
            xmlnodeGame.SetAttribute("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            foreach (Move move in MoveHistory)
            {
                AddSaveGameNode(xmldoc, xmlnodeGame, move);
            }

            // Redo moves
            for (int intIndex = MoveRedoList.Count - 1; intIndex >= 0; intIndex--)
            {
                AddSaveGameNode(xmldoc, xmlnodeGame, MoveRedoList[intIndex]);
            }

            xmldoc.Save(fileName);
        }

        /// <summary>
        ///   Undo all moves. For internal use pnly.
        /// </summary>
        private static void UndoAllMovesInternal()
        {
            while (MoveHistory.Count > 0)
            {
                UndoMoveInternal();
            }
        }

        /// <summary>
        ///   Undo move. For internal use only.
        /// </summary>
        private static void UndoMoveInternal()
        {
            if (MoveHistory.Count > 0)
            {
                Move moveUndo = MoveHistory.Last;
                MoveRedoList.Add(moveUndo);
                Move.Undo(moveUndo);
                PlayerToPlay = PlayerToPlay.OpposingPlayer;
            
            }
        }

        #endregion
    }
}