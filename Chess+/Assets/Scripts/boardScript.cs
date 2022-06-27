using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boardScript : MonoBehaviour
{
    //The public piece prefabs
    public static GameObject pawnPrefab;
    public static GameObject rookPrefab;
    public static GameObject bishopPrefab;
    public static GameObject knightPrefab;
    public static GameObject queenPrefab;
    public static GameObject kingPrefab;
    public static GameObject tilePrefab;

    //The public materials for each team
    public static Material white;
    public static Material black;
    public static Material highLight;

    //The text for displaying the result of the game on the UI
    public Text gameResultText;

    //The size of the board. For setUpBoard, this must be 8x8 board.
    private readonly int boardSize = 8;

    private GameObject[,] gameObjArray; //Stores the GameObject chess pieces. Goes from 0-7 for col and row. 
    //Note, it is stored [col, row].
    //Note, 0 on this is col 1. 7 on this is col 8. This is because arrays start from index 0. So to convert from col, row to this array, use col -1, row - 1
    
    private Piece[,] pieceArray; //Stores the Piece objects

    private Team currentPlayer; //stores the current turn. White = player, Black = AI
    private List<GameObject> tileList; //stores the moveTiles the player can click on to do a move

    enum GameResult { Ongoing, GameWon, GameLost, Stalemate}
    private GameResult gameResult = GameResult.Ongoing;

    private Piece selected; //The current Piece (white chess piece) selected by the player

    private Piece whiteKing; //stores the black and white kings for easier access
    private Piece blackKing;

    public bool check; //stores the check and checkMate (as assignned by checkForMate) for whichever team the function was just run for
    public bool checkMate;
    private List<Move> checkAvoidingMoves; //The list of moves that, if you're in check, get you out of check (you legally have to do one of these moves).
    private bool[,] safeSquares; //Stores whether every square in under attack by an enemy piece (false), or not (true)

    //Castling variables
    private bool bLCastle = true; //Stores whether you can castle in this direction (eg neither king nor rook has moved)
    private bool bRCastle = true; //It's black left, black right, white left and white right
    private bool wLCastle = true;
    private bool wRCastle = true;

    //Promotion variables
    private PromoteMenu promotionMenuReference; //Variables used for Promotion

    //En passant variabes
    Coordinate enPassantPosition;

    public enum AIMode { easy, medium };
    public AIMode AIdifficulty = AIMode.easy;

    private bool turnOver = false; //Stores whether a turn is over. This allows update to go to a third function, endTurn(), before letting the other player go

    //Notes to know about the code:
    //The cols and rows go from 1 to 8.
    //TRUE is the player team, and FALSE is the AI team. Always.
    //Currently white is the player and black is the AI.

    //TODO:
    //replace boardArray with a array of the names, not the gameObjects. Should be less computation to call. Still use an array of gameObjects, though? IDK...
    //Can you make the prefabs finals?
    //ERROR WITH CURRENT CODE
    //If a rook moves or is killed, and when a king moves, it doesn't update the castling variables. IT SHOULD!!!
    //Fix this when you implement boardState() objects

    //To consider:
    //Maybe implement getPiece used more instead of referring to boardArray directly?
    //Should I make endGame() a function I call directly instead of calling it during update?
    //Should I be using lists so much? What about sets?


    // Start is called before the first frame update
    void Start()
    {
        //Set up some variables
        currentPlayer = Team.White;
        tileList = new List<GameObject>();
        checkAvoidingMoves = new List<Move>();

        GameObject Canvas = GameObject.Find("Canvas");
        promotionMenuReference = Canvas.GetComponent<PromotionMenu>(); //Get the promotion menu script

        boardArray = new GameObject[boardSize, boardSize];
        safeSquares = new bool[boardSize, boardSize];

        //Initialise the chess pieces, set up the board with this FEN string
        loadFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        Debug.Log(getFEN());
        checkForMate(true); //sets up the values of check and checkMate for the first turn, for the user
    }

    //Creates a piece, given its type (one of the prefabs), col, row and team
    //Fills in boardArray, sets the material, the name, and instantiates it
    public GameObject makePiece(GameObject type, int col, int row, bool team) //bool hasMoved
    {
        float y = 0;
        if (string.Equals(type.name, "Bishop") || string.Equals(type.name, "Pawn") || string.Equals(type.name, "King"))
        {
            y = 0.15F;
        }
        else
        {
            y = 0.5F;
        }
        Quaternion rotation;
        //The black knights need to be rotated 180 degrees
        if (string.Equals(type.name, "Knight") && !team) { rotation = Quaternion.Euler(-90, 180, 0); } else { rotation = Quaternion.Euler(-90, 0, 0); }
        boardArray[col - 1, row - 1] = Instantiate(type, new Vector3(colToX(col), y, colToX(row)), rotation);
        boardArray[col - 1, row - 1].transform.localScale = new Vector3(35, 35, 35);
        if (team)
        {
            boardArray[col - 1, row - 1].GetComponent<Renderer>().material = white;
            boardArray[col - 1, row - 1].name = "white" + getPiece(col, row).name;
        }
        else
        {
            boardArray[col - 1, row - 1].GetComponent<Renderer>().material = black;
            boardArray[col - 1, row - 1].name = "black" + getPiece(col, row).name;
        }
        return boardArray[col - 1, row - 1];
    }

    //Loads a FEN string to set up the board
    public void loadFEN(string FENstring)
    {
        string[] FENwords = FENstring.Split(' ');
        //Setting the board
        int col = 1;
        int row = 8;
        for (int i = 0; i < FENwords[0].Length; i++)
        {
            char c = FENwords[0][i];
            if (c.Equals('/'))
            {
                row--;
                col = 1;
            }
            else if (char.IsDigit(c))
            {
                col = col + c;
            }
            else
           if (c.Equals(' '))
            {
                break;
            }
            else
            {
                //This will be if it's a character for a piece
                bool team = char.IsUpper(c);
                GameObject prefab;
                c = char.ToUpper(c);
                switch (c)
                {
                    case 'K':
                        prefab = king;
                        break;
                    case 'Q':
                        prefab = queen;
                        break;
                    case 'N':
                        prefab = knight;
                        break;
                    case 'B':
                        prefab = bishop;
                        break;
                    case 'R':
                        prefab = rook;
                        break;
                    case 'P':
                        prefab = pawn;
                        break;
                    default:
                        prefab = pawn; Debug.Log("This should never trigger"); //The default is the pawn. But this should never trigger
                        break;
                }
                makePiece(prefab, col, row, team);
                if (c.Equals('K')) //Stores the white and black kings
                {
                    if (team)
                    {
                        whiteKing = boardArray[col - 1, row - 1];
                    }
                    else
                    {
                        blackKing = boardArray[col - 1, row - 1];
                    }
                }
                col++;
            }
        }

        //The player turn
        playerTurn = FENwords[1].Equals("w");

        //Castling
        if (!FENwords[2].Contains("K")) { wRCastle = true; }
        if (!FENwords[2].Contains("Q")) { wLCastle = true; }
        if (!FENwords[2].Contains("k")) { bLCastle = true; }
        if (!FENwords[2].Contains("q")) { bRCastle = true; }

        //En Passant square
        if (!FENwords[3].Contains("-"))
        {
            char Col = FENwords[3][0];
            char Row = FENwords[3][1];
            if (Col.Equals('a')) { col = 1; } else if (Col.Equals('b')) { col = 2; } else if (Col.Equals('c')) { col = 3; } else if (Col.Equals('d')) { col = 4; } else if (Col.Equals('e')) { col = 5; } else if (Col.Equals('f')) { col = 6; } else if (Col.Equals('g')) { col = 7; } else if (Col.Equals('h')) { col = 8; }
            row = Row - '0'; //Converts char to int
            if (playerTurn) { jumpedPawnBlack = new Vector2(col, row); } else { jumpedPawnWhite = new Vector2(col, row); }
        }

        //Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //Implement fullmoves??? Stores how many turns have elapsed
    }

    //Returns the FEN string for the current state of the board
    public string getFEN()
    {
        string fenString = "";
        //Storing board setup
        for (int row = 8; row >= 1; row--)
        {
            int emptySpaces = 0;
            for (int col = 1; col <= 8; col++)
            {
                if (boardArray[col - 1, row - 1] != null)
                {
                    string name = boardArray[col - 1, row - 1].name;
                    bool team = name.Contains("white");
                    if (team) { name = name.Remove(name.IndexOf("white"), 5); }
                    if (!team) { name = name.Remove(name.IndexOf("black"), 5); }
                    if (name.IndexOf("(Clone)") >= 0) { name = name.Remove(name.IndexOf("(Clone)"), 7); }
                    char fenChar;
                    switch (name)
                    {
                        case "King":
                            fenChar = 'k';
                            break;
                        case "Queen":
                            fenChar = 'q';
                            break;
                        case "Bishop":
                            fenChar = 'b';
                            break;
                        case "Knight":
                            fenChar = 'n';
                            break;
                        case "Rook":
                            fenChar = 'r';
                            break;
                        case "Pawn":
                            fenChar = 'p';
                            break;
                        default:
                            Debug.Log("should be impossible here"); fenChar = 'x';
                            break;
                    }
                    if (team) { fenChar = char.ToUpper(fenChar); }
                    if (emptySpaces != 0) { fenString += emptySpaces; }
                    fenString += char.ToString(fenChar);
                    emptySpaces = 0;
                }
                else
                {
                    emptySpaces++;
                }
            }
            if (emptySpaces != 0) { fenString += emptySpaces; }
            if (row != 1) { fenString += char.ToString('/'); }
        }

        //The player turn
        if (playerTurn) { fenString += " w "; } else { fenString += " b "; }

        //Castling
        if (wRCastle) { fenString += "K"; }
        if (wLCastle) { fenString += "Q"; }
        if (bLCastle) { fenString += "k"; }
        if (bRCastle) { fenString += "q"; }
        fenString += ' ';

        //En Passant square
        Vector2 enemyPawn;
        if (playerTurn) { enemyPawn = jumpedPawnBlack; } else { enemyPawn = jumpedPawnWhite; }
        if (enemyPawn == null) { fenString += "- "; }
        else
        {
            int col = (int)enemyPawn.x;
            switch (col)
            {
                case 1:
                    fenString += 'a';
                    break;
                case 2:
                    fenString += 'b';
                    break;
                case 3:
                    fenString += 'c';
                    break;
                case 4:
                    fenString += 'd';
                    break;
                case 5:
                    fenString += 'e';
                    break;
                case 6:
                    fenString += 'f';
                    break;
                case 7:
                    fenString += 'g';
                    break;
                case 8:
                    fenString += 'h';
                    break;
            }
            fenString += enemyPawn.y + " ";
        }

        //Implement halfmoves??? (50 moves without pawn progression or killing = draw)

        //Implement fullmoves??? Stores how many turns have elapsed

        return fenString;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameResult == GameResult.Ongoing)
        {
            if (turnOver)
            {
                endTurn(); //This is done separately from userTurn and enemyTurn, because this gives other scripts time to do stuffs between updates.
            }
            else if (playerTurn) //if the game isn't over, go do userTurn or enemyTurn depending on whose turn it is
            {
                userTurn();
            }
            else
            {
                enemyTurn();
            }
        }
        else
        {
            endGame(); //If the game is over, call endgame
        }
    }

    //The user's turn.
    private void userTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                //Figure out what the user clicked on
                if (hit.collider.gameObject.name.Contains("white")) //If they clicked on a white piece, show the moves for that chess piece
                {
                    deselect();
                    selected = hit.transform.gameObject;
                    selected.GetComponent<Renderer>().material = highLight;
                    showValidMoves(selected);
                }
                else if (hit.collider.gameObject.name.Contains("moveTile") || hit.collider.gameObject.name.Contains("black"))
                { //If you click on a black piece or a move tile
                    //If you click on the piece you want to kill instead of the move tile, find the move tile directly below it
                    if (hit.collider.gameObject.name.Contains("black"))
                    {
                        GameObject blackPiece = hit.collider.gameObject;
                        Vector2 blackPos = getPos(blackPiece);
                        LayerMask mask = LayerMask.GetMask("Move tiles");
                        Physics.Raycast(new Vector3(colToX(blackPos.x), 0.2F, colToX(blackPos.y)), Vector3.down, out hit, 1000, mask);
                        if (hit.collider == null)
                        {
                            deselect(); //If you clicked on a piece and there was no tile beneath it
                            return;
                        }
                    }
                    //Do the move associated with that moveTile
                    Move move = hit.collider.gameObject.GetComponent<tileScript>().getMove();
                    deselect();
                    doMove(move);
                    turnOver = true;
                }
            }
            else
            { //If they click on nothing in particular, deselect the selected chess piece
                deselect();
            }
        }
    }

    //Ends a players turn
    private void endTurn()
    {
        if (playerTurn) { jumpedPawnBlack = null; } else { jumpedPawnWhite = null; }
        checkForMate(!playerTurn);
        updateGamestate(playerTurn);
        turnOver = false;
        playerTurn = !playerTurn;
        Debug.Log(getFEN());
    }

    //Deselects the selected chess piece, (changes material and removes the move tiles)
    private void deselect()
    {
        if (selected != null)
        {
            if (selected.name.Contains("white"))
            {
                selected.GetComponent<Renderer>().material = white;
            }
            else
            {
                selected.GetComponent<Renderer>().material = black;
            }
            selected = null;
        }
        foreach (GameObject tile in tileList)
        {
            Destroy(tile);
        }
    }

    //Updates the variables of gameOver and such using the results of checkForMate
    private void updateGamestate(bool team)
    {
        if (check)
        {
            Debug.Log(teamName(!team) + " is in check!");
            if (checkMate)
            {
                gameWon = team;
                gameOver = true;
            }
        }
        if (staleMate)
        {
            gameOver = true;
        }
    }

    //Checks if the game is in a stale mate (if the game is not in check but there are no valid moves (that don't put you in check)
    private void checkForStaleMate(bool team)
    {
        List<Move> moves = allMoves(team);
        removeCheckingMoves(moves, team);
        if (moves.Count == 0) { staleMate = true; } //If there are no valid moves that don't put you in check, then its a stalemate
    }

    //Sees if a certain team is in check, checkmate, or stalemate
    private void checkForMate(bool team)
    {
        GameObject kingPiece = getKing(team);
        Vector2 kingPos = getPos(kingPiece);

        checkAvoidingMoves.Clear();

        checkForStaleMate(team); //sets the value of staleMate

        //See if you're in check. If not, stop there.
        updateSafeSquares(team); //updates safe squares once. Uses this to see if the king's in check
        if (safeSquares[(int)kingPos.x - 1, (int)kingPos.y - 1])
        {
            check = false;
            checkMate = false;
            return;
        }
        else
        {
            check = true;
        }

        //If you are in check, check all possible moves and see if you're still in check.
        checkMate = true; //start with assumption of checkmate, try to disprove it
        List<Move> allTeamMoves = allMoves(team);
        foreach (Move move in allTeamMoves) //This goes through all the moves of your team and simulates them, then sees if you're still in check. 
        {                                   //If no move you can do prevents check, then you're in checkmate
            GameObject killedPiece = doMoveState(move);
            if (move.movedPiece.name.Contains("King")) { kingPos = move.to; }
            if (!inCheck(team, kingPos))
            {
                checkAvoidingMoves.Add(move); //If a move gets you out of check, add it to checkAvoidingMoves and set checkMate to false
                if (checkMate) { checkMate = false; }
            }

            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(kingPiece); }
            undoMoveState(move, killedPiece);

        }
    }

    private GameObject nameToPiece(string name)
    {
        if (name.Equals("king"))
        {
            return king;
        }
        if (name.Equals("queen"))
        {
            return queen;
        }
        if (name.Equals("bishop"))
        {
            return bishop;
        }
        if (name.Equals("rook"))
        {
            return rook;
        }
        if (name.Equals("knight"))
        {
            return knight;
        }
        if (name.Equals("pawn"))
        {
            return pawn;
        }
        return null;
    }

    //Executes a move 
    private void doMove(Move move)
    {
        GameObject killedPiece = doMoveState(move); //Updates the boardArray. It is done separately to allow this function to be reused when testing/checking moves
        showMove(move, killedPiece); //Does the rest of the move
    }

    //Moves the GameObjects in boardArray. Is used as part of doMove, and also used to test/check moves (for check and whatnot)
    private GameObject doMoveState(Move move)
    {
        bool team = move.movedPiece.name.Contains("white");
        boardArray[(int)move.from.x - 1, (int)move.from.y - 1] = null;
        GameObject killedPiece = null;
        if (boardArray[(int)move.to.x - 1, (int)move.to.y - 1] != null)
        {
            killedPiece = boardArray[(int)move.to.x - 1, (int)move.to.y - 1]; //Stores the piece that could be killed when the theoretical move is done
                                                                              //Don't need to set the piece to unactive: remove the reference to it in boardArray, and it won't have any moves calculated for it
        }
        boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = move.movedPiece;
        if (move.pawnJump)
        {
            if (team) { jumpedPawnWhite = move.to; } else { jumpedPawnBlack = move.to; }
        }
        if (move.castling)
        {
            if (team)
            {
                wLCastle = false;
                wRCastle = false;
            }
            else
            {
                bLCastle = false;
                bRCastle = false;
            }
            doMoveState(move.castlingMove);
        }
        if (move.promotion)
        {
            GameObject promoted; //For checking moves 1 ahead, use the prefabs as the temporary stored objects
            if (move.promotedTo.Equals("queen"))
            {
                promoted = queen;
            }
            else if (move.promotedTo.Equals("rook"))
            {
                promoted = rook;
            }
            else if (move.promotedTo.Equals("bishop"))
            {
                promoted = bishop;
            }
            else if (move.promotedTo.Equals("knight"))
            {
                promoted = knight;
            }
            else
            {
                Debug.Log("Should be impossible");
                promoted = queen;
            }
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = promoted;
            promoted.name = teamName(move.movedPiece.name.Contains("white")) + move.promotedTo.Substring(0, 1).ToUpper() + move.promotedTo.Substring(1) + "(clone)";
        }
        if (move.enPassant)
        {
            Vector2 enemyPawn;
            if (team) { enemyPawn = jumpedPawnBlack; } else { enemyPawn = jumpedPawnWhite; }
            killedPiece = boardArray[(int)move.to.x - 1, (int)move.from.y - 1];
            boardArray[(int)move.to.x - 1, (int)move.from.y - 1] = null;
        }

        return killedPiece;
    }

    //Undoes the changes to boardArray done by this move. Used when testing moves, to undo them
    private void undoMoveState(Move move, GameObject killedPiece)
    {
        bool team = move.movedPiece.name.Contains("white");
        Vector2 from = move.from;
        boardArray[(int)from.x - 1, (int)from.y - 1] = move.movedPiece;
        boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = killedPiece;
        if (move.pawnJump)
        {
            if (team) { jumpedPawnWhite = null; } else { jumpedPawnBlack = null; }
        }
        if (move.castling) {
            if (team)
            {
                wLCastle = true;
                wRCastle = true;
            }
            else
            {
                bLCastle = true;
                bRCastle = true;
            }
            undoMoveState(move.castlingMove, null);
        }
        //Undoing Promotions is automatically done. You don't need to do anything special to undo a Promotion.
        if (move.enPassant)
        {
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = null;
            boardArray[(int)move.to.x - 1, (int)move.from.y - 1] = killedPiece;
        }
    }

    //Updates the gameobjects (creates, destroys, moves) so the user can see the changes to the chess game
    private void showMove(Move move, GameObject killedPiece)
    {
        if (killedPiece != null) { Destroy(killedPiece); }
        move.movedPiece.transform.position = new Vector3(colToX(move.to.x), move.movedPiece.transform.position.y, colToX(move.to.y));
        if (move.castling)
        {
            showMove(move.castlingMove, null);
        }
        if (move.promotion && playerTurn)
        {
            promotionMenuReference.Run(move); //If its the user's turn, let them choose what to promote to
        }
        if (move.promotion && !playerTurn) //Promotion for enemy AI
        {
            makePiece(nameToPiece(move.promotedTo), (int)move.to.x, (int)move.to.y, false); //The enemy has moved
            Destroy(move.movedPiece);
        }
    }

    //The AI/Enemy's turn
    private void enemyTurn()
    {
        if (!check)
        {
            List<Move> AIMoves = allMoves(false);
            //Removes all the moves that put the king into check
            removeCheckingMoves(AIMoves, false);
            if (easyMode)
            {
                int index = Random.Range(0, AIMoves.Count);
                doMove(AIMoves[index]);
            }
            else
            {
                List<Move> maxPriMoves = getMaxPriMoves(AIMoves, false);
                int index = Random.Range(0, maxPriMoves.Count);
                doMove(maxPriMoves[index]);
            }
        }
        else
        {
            if (easyMode)
            {
                int index = Random.Range(0, checkAvoidingMoves.Count);
                doMove(checkAvoidingMoves[index]);
            }
            else
            {
                List<Move> maxPriMoves = getMaxPriMoves(checkAvoidingMoves, false);
                int index = Random.Range(0, maxPriMoves.Count);
                doMove(maxPriMoves[index]);
            }
        }

        turnOver = true;
    }

    private List<Move> getMaxPriMoves(List<Move> AIMoves, bool team)
    {
        int maxPriority = 0;
        List<Move> maxPriMoves = new List<Move>();
        foreach (Move move in AIMoves)
        {
            int movePri = movePriority(move, team);
            if (movePri == maxPriority) { maxPriMoves.Add(move); }
            if (movePri > maxPriority)
            {
                maxPriority = movePri;
                maxPriMoves.Clear();
                maxPriMoves.Add(move);
            }
        }
        return maxPriMoves;
    }

    //Generates a "priority" for the move: how good it is. BASIC right now
    private int movePriority(Move move, bool team)
    {
        GameObject enemyKing = getKing(!team);
        Vector2 kingPos = getPos(enemyKing);
        GameObject killedPiece = doMoveState(move);
        bool checking = false;
        if (inCheck(!team, kingPos))
        {
            checking = true;
        }
        undoMoveState(move, killedPiece);

        if (checking) { return 10; }
        if (killedPiece != null)
        {
            if (killedPiece.name.Contains("King")) { Debug.Log("This shouldn't be possible. The game should end with checkmate before this happens"); }
            if (killedPiece.name.Contains("Queen")) { return 9; }
            if (killedPiece.name.Contains("Rook") || killedPiece.name.Contains("Bishop")) { return 8; }
            if (killedPiece.name.Contains("Knight")) { return 7; }
            if (killedPiece.name.Contains("Pawn")) { return 6; }
        }
        return 0; //If killedPiece == null and its not checking
    }

    //Returns a list of all of the moves of a certain team
    private List<Move> allMoves(bool team)
    {
        List<Move> allMoves = new List<Move>();
        for (int col = 1; col <= 8; col++)
            for (int row = 1; row <= 8; row++)
            { //Goes through each gameObject in boardArray, and if its in the right team it adds all of its moves to the list its going to return
                if (boardArray[col - 1, row - 1] != null && boardArray[col - 1, row - 1].name.Contains(teamName(team)))
                {
                    allMoves.AddRange(validMoves(boardArray[col - 1, row - 1]));
                }
            }
        return allMoves;
    }

    //Called when the game ends. Does stuff and stops the code from running
    private void endGame()
    { //Displays text depending on the game outcome
        if (staleMate)
        {
            gameResultText.text = "You got a stalemate. Impressive";
        }
        else if (gameWon)
        {
            gameResultText.text = "YOU WON YOU LITTLE BEAUTY!!!";
        }
        else
        {
            gameResultText.text = "YOU LOST!!! HOW DID YOU MANAGE THAT???";
        }
        gameResultText.gameObject.SetActive(true);

        this.enabled = false; //disable the code
    }

    //returns a list of all of the valid moves of a certain object (NOTE: includes moves that put you into check)
    private List<Move> validMoves(GameObject piece)
    {
        List<Move> moves = new List<Move>();
        int col = xToCol(piece.transform.position.x);
        int row = xToCol(piece.transform.position.z);

        bool team = piece.name.Contains("white");
        string name = piece.name;
        if (name.IndexOf("white") >= 0) { name = name.Remove(name.IndexOf("white"), 5); }
        if (name.IndexOf("black") >= 0) { name = name.Remove(name.IndexOf("black"), 5); }
        if (name.IndexOf("(Clone)") >= 0) { name = name.Remove(name.IndexOf("(Clone)"), 7); }
        switch (name)
        { //Calls a function depending on the type of the chess piece
            case "Pawn":
                moves.AddRange(pawnMoves(piece, col, row));
                break;
            case "Rook":
                moves.AddRange(rookMoves(piece, col, row));
                break;
            case "Bishop":
                moves.AddRange(bishopMoves(piece, col, row));
                break;
            case "Knight":
                moves.AddRange(knightMoves(piece, col, row));
                break;
            case "Queen":
                moves.AddRange(queenMoves(piece, col, row));
                break;
            case "King":
                moves.AddRange(kingMoves(piece, col, row));
                break;
        }
        return moves;
    }

    //Returns the moves for a King
    private List<Move> kingMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        for (int a = -1; a <= 1; a++)
            for (int b = -1; b <= 1; b++)
            {
                if ((a != 0 || b != 0) && inBounds(col + a, row + b) && spotNotAlly(piece, col + a, row + b))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + a, row + b)));
                }
            }
        //Check for castling
        bool team = piece.name.Contains("white");
        bool leftCastle;
        bool rightCastle;
        if (team)
        {
            leftCastle = wLCastle;
            rightCastle = wRCastle;
        }
        else
        {
            leftCastle = bLCastle;
            rightCastle = bRCastle;
        }
        if (leftCastle && !check &&
            boardArray[1, row - 1] == null && boardArray[2, row - 1] == null && boardArray[3, row - 1] == null
            && safeSquares[1, row - 1] && safeSquares[2, row - 1])
        {
            Move move = new Move(piece, new Vector2(col, row), new Vector2(col - 2, row));
            move.setCastling(new Move(boardArray[0, row - 1], new Vector2(1, row), new Vector2(col - 1, row)));
            moves.Add(move);
        }
        if (rightCastle && !check
            && boardArray[6, row - 1] == null && boardArray[5, row - 1] == null
            && safeSquares[4, row - 1] && safeSquares[5, row - 1]) //NOTE: I manually entered the numbers for all the squares between king and rook.
        {                                                         //Generalise this?
            Move move = new Move(piece, new Vector2(col, row), new Vector2(col + 2, row));
            move.setCastling(new Move(boardArray[7, row - 1], new Vector2(8, row), new Vector2(col + 1, row)));
            moves.Add(move);
        }
        return moves;
    }

    //Returns the moves for a Queen
    private List<Move> queenMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(bishopMoves(piece, col, row));
        moves.AddRange(rookMoves(piece, col, row));
        return moves;
    }

    //Returns the moves for a Knight
    private List<Move> knightMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        for (int a = -2; a <= 2; a = a + 4)
            for (int b = -1; b <= 1; b = b + 2)
            {
                if (inBounds(col + a, row + b) && spotNotAlly(piece, col + a, row + b))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + a, row + b)));
                }
                if (inBounds(col + b, row + a) && spotNotAlly(piece, col + b, row + a))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + b, row + a)));
                }
            }
        return moves;
    }

    //Returns the moves for a Bishop
    private List<Move> bishopMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col + i, row + i))
            {
                if (spotNotAlly(piece, col + i, row + i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + i, row + i)));
                }
                if (boardArray[col + i - 1, row + i - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col - i, row - i))
            {
                if (spotNotAlly(piece, col - i, row - i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col - i, row - i)));
                }
                if (boardArray[col - i - 1, row - i - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col - i, row + i))
            {
                if (spotNotAlly(piece, col - i, row + i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col - i, row + i)));
                }
                if (boardArray[col - i - 1, row + i - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col + i, row - i))
            {
                if (spotNotAlly(piece, col + i, row - i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + i, row - i)));
                }
                if (boardArray[col + i - 1, row - i - 1] != null) { break; }
            }
        }
        return moves;
    }

    //Returns the moves for a Rook
    private List<Move> rookMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col + i, row))
            {
                if (spotNotAlly(piece, col + i, row))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + i, row)));
                }
                if (boardArray[col + i - 1, row - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col - i, row))
            {
                if (spotNotAlly(piece, col - i, row))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col - i, row)));
                }
                if (boardArray[col - i - 1, row - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col, row + i))
            {
                if (spotNotAlly(piece, col, row + i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col, row + i)));
                }
                if (boardArray[col - 1, row + i - 1] != null) { break; }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            if (inBounds(col, row - i))
            {
                if (spotNotAlly(piece, col, row - i))
                {
                    moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col, row - i)));
                }
                if (boardArray[col - 1, row - i - 1] != null) { break; }
            }
        }
        return moves;
    }

    //Returns the moves for a Pawn
    private List<Move> pawnMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        int direction;
        Vector2 enemyPawn;
        if (piece.name.Contains("white"))
        {
            direction = 1;
            enemyPawn = jumpedPawnBlack;
        }
        else
        {
            direction = -1;
            enemyPawn = jumpedPawnWhite;
        }

        bool promotion = false;
        if ((direction == 1 && row + direction == 8) || (direction == -1 && row + direction == 1)) { promotion = true; }

        if (inBounds(col, row + direction) && boardArray[col - 1, row + direction - 1] == null)
        {
            if (!promotion)
            {
                moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(piece, new Vector2(col, row), new Vector2(col, row + direction)));
            }
            //The "row == 4.5 + 2.5*direction" ensures that the pawn is in its original starting row, hence hasn't moved
            if (row == 4.5 + 2.5 * -direction && inBounds(col, row + 2 * direction) && boardArray[col - 1, row + 2 * direction - 1] == null)
            { //This is for moving 2 spaces forwards. Don't check for promotion here.
                moves.Add((new Move(piece, new Vector2(col, row), new Vector2(col, row + 2 * direction))).setPawnJump());
            }
        }
        //The killing moves
        if (inBounds(col - 1, row + direction) && spotIsEnemy(piece, col - 1, row + direction))
        {
            if (!promotion)
            {
                moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col - 1, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(piece, new Vector2(col, row), new Vector2(col - 1, row + direction)));
            }
        }
        if (inBounds(col + 1, row + direction) && spotIsEnemy(piece, col + 1, row + direction))
        {
            if (!promotion)
            {
                moves.Add(new Move(piece, new Vector2(col, row), new Vector2(col + 1, row + direction)));
            }
            else
            {
                moves.AddRange(promotionMoves(piece, new Vector2(col, row), new Vector2(col + 1, row + direction)));
            }
        }
        if (enemyPawn.y == row && Mathf.Abs(enemyPawn.x - col) == 1)
        {
            moves.Add((new Move(piece, new Vector2(col, row), new Vector2(enemyPawn.x, row + direction))).setEnPassant());
        }
        return moves;
    }

    private List<Move> promotionMoves(GameObject piece, Vector2 from, Vector2 to)
    {
        Move m1 = new Move(piece, from, to);
        Move m2 = new Move(piece, from, to);
        Move m3 = new Move(piece, from, to);
        Move m4 = new Move(piece, from, to);
        m1.setPromotion("queen");
        m2.setPromotion("rook");
        m3.setPromotion("bishop");
        m4.setPromotion("knight");
        List<Move> moves = new List<Move>();
        moves.Add(m1);
        moves.Add(m2);
        moves.Add(m3);
        moves.Add(m4);
        return moves;
    }

    //Checks if a spot in boardArray is NOT an ally with a certain chess piece
    private bool spotNotAlly(GameObject piece, int col, int row)
    {
        if (boardArray[col - 1, row - 1] == null) { return true; }
        return (piece.name.Contains("white") != boardArray[col - 1, row - 1].name.Contains("white"));
    }

    //Checks if a spot in boardArray is of the opposite team of Piece
    private bool spotIsEnemy(GameObject piece, int col, int row)
    {
        if (boardArray[col - 1, row - 1] == null) { return false; }
        return (piece.name.Contains("white") != boardArray[col - 1, row - 1].name.Contains("white"));
    }

    //Used to see if a move puts the king in check. If it does, it is not a valid move.
    //This is more efficient than using updateSafeSquares for checking for a particular spot
    private bool inCheck(bool team, Vector2 kingPos)
    {
        List<Move> allEnemyMoves = allMoves(!team);
        foreach (Move move in allEnemyMoves)
        {
            if (move.to == kingPos)
            {
                return true;
            }
        }
        return false;
    }

    //Updates safeSquares, which holds which squares are under attack and which aren't
    //This is less efficient than inCheck for checking for a particular spot, but provides useful information about other squares THAT CAN BE STORED FOR A LATER DATE
    private void updateSafeSquares(bool team)
    {
        for (int col = 1; col <= boardSize; col++)
            for (int row = 1; row <= boardSize; row++)
            {
                safeSquares[col - 1, row - 1] = true;
            }
        List<Move> allEnemyMoves = allMoves(!team);
        foreach (Move move in allEnemyMoves)
        {
            safeSquares[(int)move.to.x - 1, (int)move.to.y - 1] = false;
        }
    }

    //Takes a list of moves and removes all the Moves that put the king in check
    private void removeCheckingMoves(List<Move> moves, bool team)
    {
        Vector2 kingPos = getPos(getKing(team));
        //Removes all the moves that cause the player to be in check
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            //Simulate doing the move
            GameObject killedPiece = doMoveState(move);
            if (move.movedPiece.name.Contains("King")) { kingPos = move.to; }
            if (inCheck(team, kingPos))
            {
                moves.RemoveAt(i); //If the king is in check, remove that move
                i--;
            }
            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(getKing(team)); }
            undoMoveState(move, killedPiece);
        }
    }

    //Makes move tiles for all the valid moves of a certain piece
    private void showValidMoves(GameObject piece)
    {    //Note: this has the Assumption that Check is the Check for the team Piece belongs to. You cannot showValidMoves for the other team (as Check will be the wrong value)
        if (!check)
        {
            //If you aren't in check, you can do any move (barring the ones that put you in check)
            bool team = piece.name.Contains("white");
            List<Move> moves = validMoves(piece);
            removeCheckingMoves(moves, team);

            //Make a tile for each move
            foreach (Move move in moves)
            {
                GameObject newTile = Instantiate(tile, new Vector3(colToX(move.to.x), 0.16F, colToX(move.to.y)), Quaternion.identity);
                //the 0.16F is only SLIGHTLY above the board so it can be seen
                tileList.Add(newTile);
                newTile.GetComponent<tileScript>().setMove(move);
            }

        }
        else
        {
            //If you are in check, you can only do the moves from checkAvoidingMoves
            foreach (Move move in checkAvoidingMoves)
            {
                if (move.movedPiece == piece)
                {
                    GameObject newTile = Instantiate(tile, new Vector3(colToX(move.to.x), 0.16F, colToX(move.to.y)), Quaternion.identity);
                    tileList.Add(newTile);
                    newTile.GetComponent<tileScript>().setMove(move);
                }
            }
        }
    }

    //Returns whether a col and row are in bounds
    private bool inBounds(int col, int row)
    {
        return (col >= 1 && col <= 8 && row >= 1 && row <= 8);
    }

    //Returns a King for a certain team
    private GameObject getKing(bool team)
    {
        if (team) { return whiteKing; } else { return blackKing; }
    }

    //Converts a x to Col, or a Z to Row.
    private int xToCol(float x)
    {
        x = Mathf.Round(x + 0.5F) - 0.5F;
        return (int)(x + 4.5);
    }

    //Converts a col to X, or row to Z
    //Note: col should really be an int. But taking in a float prevents typecasting.
    private float colToX(float col)
    {
        return (col - 4.5F);
    }

    //returns the Vector2 (col,row) for a chess piece
    private Vector2 getPos(GameObject piece)
    {
        return new Vector2(xToCol(piece.transform.position.x), xToCol(piece.transform.position.z));
    }

    //Returns the chess piece for a certain col and row. SHOULD I USE THIS???
    private GameObject getPiece(int col, int row)
    {
        return boardArray[col - 1, row - 1];
    }

    //Returns white/black depending on the team
    private string teamName(bool team)
    {
        if (team) { return "white"; } else { return "black"; }
    }

}
