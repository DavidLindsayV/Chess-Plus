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

    private GameObject[,] boardArray; //Stores the pieces. Goes from 0-7 for col and row.
    //Note, it is stored [col, row].
    private readonly int boardSize = 8; //The size of the board. For setUpBoard, this must be 8x8 board.
    private bool playerTurn; //stores the current turn. True = player, false = AI
    private List<GameObject> tileList; //stores the tiles the player can click on to do a move
    private bool gameOver; //When this is true, the game ends
    private bool gameWon; //Stores, when the game ends, if the player won. True = victory, false = defeat.
    private GameObject selected;
    private bool[,] pieceHasMoved; //IS THERE A BETTER WAY THAN THIS TO STORE WHICH PIECES HAVE MOVED?
    private GameObject whiteKing; //stores the black and white kings for easier access for checking if the game is over
    private GameObject blackKing;
    public bool check; //stores the check and checkMate (as assignned by checkForMate) for whichever team the function was just run for
    public bool checkMate;
    public bool staleMate = false;
    private List<Move> checkAvoidingMoves;

    //Notes to know about the code:
    //The cols and rows go from 1 to 8.
    //TRUE is the player team, and FALSE is the AI team. Always.

    //REQUEST FOR ADAM:
    //Could you change the models so that when the rotation is Quaternion.identity, that they are already straight up, and the scale of the model is already 1, 1, 1 when sized appropriately
    //and the y is consistent for all of them when just rested on the board

    //TODO:
    //The checkAvoidingMoves isn't working.

    //To consider:
    //Maybe implement getPiece used more instead of referring to boardArray directly?
    //Should I make endGame() a function I call instead of calling it during update?


    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        playerTurn = true;
        tileList = new List<GameObject>();
        checkAvoidingMoves = new List<Move>();
        boardArray = new GameObject[boardSize, boardSize];
        pieceHasMoved = new bool[boardSize, boardSize];
        setUpBoard();
        checkForMate(true); //sets up the values of check and checkMate for the first turn, for the user
    }

    //makes the gameObjects of the pieces and fills in boardAray.
    private void setUpBoard()
    {
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
        whiteKing = boardArray[3, 0];
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
        blackKing = boardArray[3, 7];
        makePiece(queen, 5, 8, false);
        makePiece(bishop, 6, 8, false);
        makePiece(knight, 7, 8, false);
        makePiece(rook, 8, 8, false);
        for (int col = 1; col <= 8; col++)
        {
            makePiece(pawn, col, 7, false);
        }
    }

    //Creates a piece, given its type, col, row and team
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
            if (playerTurn)
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
            endGame();
        }
    }

    private void userTurn()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.name.Contains("white"))
                {
                    deselect();
                    selected = hit.transform.gameObject;
                    selected.GetComponent<Renderer>().material = highLight;
                    showValidMoves(selected);
                }
                else if (hit.collider.gameObject.name.Contains("moveTile") || hit.collider.gameObject.name.Contains("black"))
                {
                    //If you click on the piece you want to kill instead of the move tile
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

                    doMove(hit.collider.gameObject.GetComponent<tileScript>().getMove());
                    deselect();
                    checkForMate(false);
                    if (check)
                    {
                        Debug.Log("Black is in check!");
                        if (checkMate)
                        {
                            gameWon = true;
                            gameOver = true;
                        }
                    }
                    if (staleMate)
                    {
                        gameOver = true;
                    }
                    playerTurn = !playerTurn;
                }
            }
            else
            {
                deselect();
            }
        }
    }

    private void checkForStaleMate(bool team)
    {
        List<Move> moves = allMoves(team);
        removeCheckingMoves(moves, team);
        if(moves.Count == 0) { staleMate = true; }
    }

    //Sees if a certain team has lost due to checkmate
    private void checkForMate(bool team)
    {
        GameObject kingPiece = getKing(team);
        Vector2 kingPos = getPos(kingPiece);

        checkAvoidingMoves.Clear();

        checkForStaleMate(team);

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

        //REMEMBER TO COMMENT THIS, ITS JUST FOR DEBUGGING
        List<Move> enemyMoves = allMoves(!team);
        foreach(Move move in enemyMoves)
        {
            if(move.to == kingPos)
            {
                Debug.Log("In check from " + move.movedPiece.name);
            }
        }

        //If you are in check, check all possible moves and see if you're still in check.
        checkMate = true; //start with assumption of checkmate, try to disprove it
        List<Move> allTeamMoves = allMoves(team);
        foreach (Move move in allTeamMoves)
        {
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
                checkAvoidingMoves.Add(move);
                if (checkMate) { checkMate = false; }
            }

            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(kingPiece); }
            boardArray[(int)from.x - 1, (int)from.y - 1] = move.movedPiece;
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = killedPiece;

        }
    }

    private void doMove(Move move)
    {
        boardArray[xToCol(move.movedPiece.transform.position.x) - 1, xToCol(move.movedPiece.transform.position.z) - 1] = null;
        pieceHasMoved[xToCol(move.movedPiece.transform.position.x) - 1, xToCol(move.movedPiece.transform.position.y) - 1] = true;
        if (boardArray[(int)move.to.x - 1, (int)move.to.y - 1] != null) { Destroy(boardArray[(int)move.to.x - 1, (int)move.to.y - 1]); }
        boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = move.movedPiece;
        move.movedPiece.transform.position = new Vector3(colToX(move.to.x), move.movedPiece.transform.position.y, colToX(move.to.y));
    }

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

    private void enemyTurn()
    {
        if (!check)
        {
            List<Move> AIMoves = allMoves(false);
            //Removes all the moves that put the king into check
            removeCheckingMoves(AIMoves, false);

            int index = Random.Range(0, AIMoves.Count);
            doMove(AIMoves[index]); //Need to fix this. Can't move into check, must get out of checkmate if possible
        }
        else
        {
            int index = Random.Range(0, checkAvoidingMoves.Count);
            doMove(checkAvoidingMoves[index]);
        }

        checkForMate(true);
        if (check)
        {
            Debug.Log("White is in check!");
            if (checkMate)
            {
                gameWon = false;
                gameOver = true;
            }
        }

        playerTurn = !playerTurn;
    }

    private List<Move> allMoves(bool team)
    {
        string teamName;
        if (team) { teamName = "white"; } else { teamName = "black"; }
        List<Move> allMoves = new List<Move>();
        for (int col = 1; col <= 8; col++)
            for (int row = 1; row <= 8; row++)
            {
                if (boardArray[col - 1, row - 1] != null && boardArray[col - 1, row - 1].name.Contains(teamName))
                {
                    allMoves.AddRange(validMoves(boardArray[col - 1, row - 1]));
                }
            }
        return allMoves;
    }

    private void endGame()
    {
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
        //for (int col = 1; col <= 8; col++)
        //    for (int row = 1; row <= 8; row++)
        //   {
        //        Destroy(boardArray[col - 1, row - 1]);
        //    }
        this.enabled = false;
    }

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
        {
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

    private List<Move> queenMoves(GameObject piece, int col, int row)
    {
        List<Move> moves = new List<Move>();
        moves.AddRange(bishopMoves(piece, col, row));
        moves.AddRange(rookMoves(piece, col, row));
        return moves;
    }

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

    private bool spotNotAlly(GameObject piece, int col, int row)
    {
        if (boardArray[col - 1, row - 1] == null) { return true; }
        return (piece.name.Contains("white") != boardArray[col - 1, row - 1].name.Contains("white"));
    }

    private bool spotIsEnemy(GameObject piece, int col, int row)
    {
        if (boardArray[col - 1, row - 1] == null) { return false; }
        return (piece.name.Contains("white") != boardArray[col - 1, row - 1].name.Contains("white"));
    }

    //Used under validMoves to see if a move puts the king in check. If it does, it is not a valid move.
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
                moves.RemoveAt(i);
                i--;
            }
            if (move.movedPiece.name.Contains("King")) { kingPos = getPos(getKing(team)); }
            boardArray[(int)from.x - 1, (int)from.y - 1] = move.movedPiece;
            boardArray[(int)move.to.x - 1, (int)move.to.y - 1] = killedPiece;
        }
    }

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

    private bool inBounds(int col, int row)
    {
        return (col >= 1 && col <= 8 && row >= 1 && row <= 8);
    }

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

    private Vector2 getPos(GameObject piece)
    {
        return new Vector2(xToCol(piece.transform.position.x), xToCol(piece.transform.position.z));
    }

    private GameObject getPiece(int col, int row)
    {
        return boardArray[col - 1, row - 1];
    }
}
