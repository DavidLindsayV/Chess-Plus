using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boardScript : MonoBehaviour
{
    //The public piece prefabs
    public GameObject pawn;
    public GameObject rook;
    public GameObject bishop;
    public GameObject knight;
    public GameObject queen;
    public GameObject king;
    public GameObject tile;

    //The public materials for each team
    public Material white;
    public Material black;
    public Material highLight;

    //The text for displaying the result of the game on the UI
    public Text gameResultText;

    private GameObject[,] boardArray; //Stores the GameObject chess pieces. Goes from 0-7 for col and row. 
    //Note, it is stored [col, row].
    //Note, 0 on this is col 1. 7 on this is col 8. This is because arrays start from index 0.

    private readonly int boardSize = 8; //The size of the board. For setUpBoard, this must be 8x8 board.
    private bool playerTurn; //stores the current turn. True = player, false = AI
    private List<GameObject> tileList; //stores the moveTiles the player can click on to do a move
    private bool gameOver; //When this is true, the game ends
    private bool gameWon; //Stores, when the game ends, if the player won. True = victory, false = defeat.
    public bool staleMate = false; //When the game ends, if this is true, the game ended in a stalemate/draw.
    private GameObject selected; //The current gameObject (white chess piece) selected by the player
    private bool[,] pieceHasMoved; //Stores which chess pieces have and haven't moved. Is used for Special moves (eg pawns can move 2 squares on their first move)
    private GameObject whiteKing; //stores the black and white kings for easier access
    private GameObject blackKing;
    public bool check; //stores the check and checkMate (as assignned by checkForMate) for whichever team the function was just run for
    public bool checkMate; 
    private List<Move> checkAvoidingMoves; //The list of moves that, if you're in check, get you out of check (you legally have to do one of these moves).

    //Notes to know about the code:
    //The cols and rows go from 1 to 8.
    //TRUE is the player team, and FALSE is the AI team. Always.
    //Currently white is the player and black is the AI.

    //TODO:

    //To consider:
    //Maybe implement getPiece used more instead of referring to boardArray directly?
    //Should I make endGame() a function I call directly instead of calling it during update?
    //Should I be using lists so much? What about sets?


    // Start is called before the first frame update
    void Start()
    {
        //Set up some variables
        gameOver = false;
        playerTurn = true;
        tileList = new List<GameObject>();
        checkAvoidingMoves = new List<Move>();
        boardArray = new GameObject[boardSize, boardSize];
        pieceHasMoved = new bool[boardSize, boardSize];
        //Initialise the chess pieces
        setUpBoard();
        checkForMate(true); //sets up the values of check and checkMate for the first turn, for the user
    }

    //makes the gameObjects of the pieces and fills in boardAray.
    private void setUpBoard()
    {
        //Sets all of pieceHasMoved to false
        for (int col = 1; col <= 8; col++)
            for (int row = 1; row <= 8; row++)
            {
                pieceHasMoved[col - 1, row - 1] = true;
            }
        //make white pieces
        makePiece(rook, 1, 1, true);
        makePiece(knight, 2, 1, true);
        makePiece(bishop, 3, 1, true);
        makePiece(king, 4, 1, true);
        whiteKing = boardArray[3, 0]; //store white king
        makePiece(queen, 5, 1, true);
        makePiece(bishop, 6, 1, true);
        makePiece(knight, 7, 1, true);
        makePiece(rook, 8, 1, true);
        for (int col = 1; col <= 8; col++)
        {
            makePiece(pawn, col, 2, true);
        }

        //make black pieces
        makePiece(rook, 1, 8, false);
        makePiece(knight, 2, 8, false);
        makePiece(bishop, 3, 8, false);
        makePiece(king, 4, 8, false);
        blackKing = boardArray[3, 7]; //store black king
        makePiece(queen, 5, 8, false);
        makePiece(bishop, 6, 8, false);
        makePiece(knight, 7, 8, false);
        makePiece(rook, 8, 8, false);
        for (int col = 1; col <= 8; col++)
        {
            makePiece(pawn, col, 7, false);
        }
    }

    //Creates a piece, given its type (one of the prefabs), col, row and team
    //Fills in boardArray, sets the material, the name, and instantiates it
    private void makePiece(GameObject type, int col, int row, bool team)
    {
        pieceHasMoved[col - 1, row - 1] = false;
        float y = 0;
        if (string.Equals(type.name, "Bishop") || string.Equals(type.name, "Pawn") || string.Equals(type.name, "King"))
        {
            y = 0.15F;
        }
        else
        {
            y = 0.5F;
        }
        boardArray[col - 1, row - 1] = Instantiate(type, new Vector3(colToX(col), y, colToX(row)), Quaternion.Euler(-90, 0, 0));
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            if (playerTurn) //if the game isn't over, go do userTurn or enemyTurn depending on whose turn it is
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
                        if (hit.collider == null) { 
                            deselect();
                            return; 
                        }
                    }
                    //Do the move associated with that moveTile
                    doMove(hit.collider.gameObject.GetComponent<tileScript>().getMove());
                    deselect();
                    checkForMate(false);
                    updateGamestate(true);
                    playerTurn = !playerTurn;
                }
            }
            else
            { //If they click on nothing in particular, deselect the selected chess piece
                deselect();
            }
        }
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
        if(moves.Count == 0) { staleMate = true; } //If there are no valid moves that don't put you in check, then its a stalemate
    }

    //Sees if a certain team is in check, checkmate, or stalemate
    private void checkForMate(bool team)
    {
        GameObject kingPiece = getKing(team);
        Vector2 kingPos = getPos(kingPiece);

        checkAvoidingMoves.Clear();

        checkForStaleMate(team); //sets the value of staleMate

        //See if you're in check. If not, stop there.
        if (!inCheck(team, kingPos)) {
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
            Vector2 from = getPos(move.movedPiece);
            boardArray[(int)from.x - 1, (int)from.y - 1] = null;

            GameObject killedPiece = null;
            if (boardArray[(int)move.to.x - 1, (int)move.to.y - 1] != null)
            {
                killedPiece = boardArray[(int)move.to.x - 1, (int)move.to.y - 1]; //Stores the piece that could be killed when the theoretical move is done
                                                                                  //Don't need to set the piece to unactive: remove the reference to it in boardArray, and it won't have any moves calculated for it
            }
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = move.movedPiece;
            if (move.movedPiece.name.Contains("King")) { kingPos = move.to; }

            if(!inCheck(team, kingPos))
            {
                checkAvoidingMoves.Add(move); //If a move gets you out of check, add it to checkAvoidingMoves and set checkMate to false
                if (checkMate) { checkMate = false; }
            }

            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(kingPiece); }
            boardArray[(int)from.x - 1, (int)from.y - 1] = move.movedPiece;
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = killedPiece;

        }
    }

    //Executes a move 
    private void doMove(Move move)
    {
        boardArray[xToCol(move.movedPiece.transform.position.x) - 1, xToCol(move.movedPiece.transform.position.z) - 1] = null;
        pieceHasMoved[xToCol(move.movedPiece.transform.position.x) - 1, xToCol(move.movedPiece.transform.position.y) - 1] = true;
        if (boardArray[(int)move.to.x - 1, (int)move.to.y - 1] != null) { Destroy(boardArray[(int)move.to.x - 1, (int)move.to.y - 1]); }
        boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = move.movedPiece;
        move.movedPiece.transform.position = new Vector3(colToX(move.to.x), move.movedPiece.transform.position.y, colToX(move.to.y));
    }

    //The AI/Enemy's turn
    private void enemyTurn()
    {
        if (!check)
        {
            List<Move> AIMoves = allMoves(false);
            //Removes all the moves that put the king into check
            removeCheckingMoves(AIMoves, false);

            int index = Random.Range(0, AIMoves.Count);
            doMove(AIMoves[index]); //Does a random move out of all the possible moves (if you're not in check)
        }
        else
        {
            int index = Random.Range(0, checkAvoidingMoves.Count); //If you're in check, it does a random allowed move from checkAvoidingMoves
            doMove(checkAvoidingMoves[index]);
        }

        checkForMate(true); //Checks for checkmate for the white team
        updateGamestate(false);

        playerTurn = !playerTurn;
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
                    moves.Add(new Move(piece, new Vector2(col + a, row + b)));
                }
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
        for(int a = -2; a <= 2; a = a + 4)
        for(int b = -1; b <= 1; b = b + 2)
            {
                if(inBounds(col + a, row + b) && spotNotAlly(piece, col + a, row + b))
                {
                    moves.Add(new Move(piece, new Vector2(col + a, row + b)));
                }
                if (inBounds(col + b, row + a) && spotNotAlly(piece, col + b, row + a))
                {
                    moves.Add(new Move(piece, new Vector2(col + b, row + a)));
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
                    moves.Add(new Move(piece, new Vector2(col + i, row + i)));
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
                    moves.Add(new Move(piece, new Vector2(col - i, row - i)));
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
                    moves.Add(new Move(piece, new Vector2(col - i, row + i)));
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
                    moves.Add(new Move(piece, new Vector2(col + i, row - i)));
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
                    moves.Add(new Move(piece, new Vector2(col + i, row)));
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
                    moves.Add(new Move(piece, new Vector2(col - i, row)));
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
                    moves.Add(new Move(piece, new Vector2(col, row + i)));
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
                    moves.Add(new Move(piece, new Vector2(col, row - i)));
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
        if (piece.name.Contains("white"))
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        if (inBounds(col, row + direction) && boardArray[col - 1, row + direction - 1] == null)
        {
            moves.Add(new Move(piece, new Vector2(col, row + direction)));
            if (!pieceHasMoved[col - 1, row - 1] && inBounds(col, row + 2 * direction) && boardArray[col - 1, row + 2 * direction - 1] == null)
            {
                moves.Add(new Move(piece, new Vector2(col, row + 2 * direction)));
            }
        }
        if (inBounds(col - 1, row + direction) && spotIsEnemy(piece, col - 1, row + direction))
        {
            moves.Add(new Move(piece, new Vector2(col - 1, row + direction)));
        }
        if (inBounds(col + 1, row + direction) && spotIsEnemy(piece, col + 1, row + direction))
        {
            moves.Add(new Move(piece, new Vector2(col + 1, row + direction)));
        }
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

    //Takes a list of moves and removes all the Moves that put the king in check
    private void removeCheckingMoves(List<Move> moves, bool team)
    {
        Vector2 kingPos = getPos(getKing(team));
        //Removes all the moves that cause the player to be in check
        for (int i = 0; i < moves.Count; i++)
        {
            Move move = moves[i];
            Vector2 from = getPos(move.movedPiece);
            //Simulate doing the move
            boardArray[(int)from.x - 1, (int)from.y - 1] = null;
            GameObject killedPiece = null;
            if (boardArray[(int)move.to.x - 1, (int)move.to.y - 1] != null)
            {
                killedPiece = boardArray[(int)move.to.x - 1, (int)move.to.y - 1];
            }
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = move.movedPiece;
            if (move.movedPiece.name.Contains("King")) { kingPos = move.to; }
            if (inCheck(team, kingPos))
            {
                moves.RemoveAt(i); //If the king is in check, remove that move
                i--;
            }
            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(getKing(team)); }
            boardArray[(int)from.x - 1, (int)from.y - 1] = move.movedPiece;
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = killedPiece;
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
