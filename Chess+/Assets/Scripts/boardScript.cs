using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class boardScript : MonoBehaviour
{
    //The text for displaying the result of the game on the UI
    public Text gameResultText;

    private List<GameObject> tileList; //stores the moveTiles the player can click on to do a move

    private Piece selected; //The current Piece (white chess piece) selected by the player

    public boardState state; //this boardState stores the current state of the board

    //Promotion variables
    private PromoteMenu promotionMenuReference; //Variables used for Promotion

    public enum AIMode
    {
        easy,
        medium
    };

    public AIMode AIdifficulty = AIMode.easy;

    private bool turnOver = false; //Stores whether a turn is over. This allows update to go to a third function, endTurn(), before letting the other player go

    //TODO improve menu/screen management (promotion, pausing, gameplay) (maybe using GameStateManager)
    //TODO make automated testing cover piece movements
    //TODO change heirarchy to make all the pieces children of something so u can minimise them
    //TODO rename boardScript to something more descriptive of what it does


    // Start is called before the first frame update
    void Start()
    {
        //Set up some variables
        tileList = new List<GameObject>();

        GameObject Canvas = GameObject.Find("Canvas");
        promotionMenuReference = Canvas.GetComponent<PromoteMenu>(); //Get the promotion menu script

        //Initialise the chess pieces, set up the board with this FEN string
        state = new boardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - O");
    }

    // Update is called once per frame 
    void Update()
    {
        if (state.getGameResult() != boardState.GameResult.Ongoing)
        {
            endGame(); //If the game is over, call endgame
            return;
        }
        else if (turnOver)  
        {
            endTurn(); //Either player or enemy has finished their turn.
        }
        else if (state.currentTeam() == state.playersTeam()) //if the game isn't over, go do userTurn or enemyTurn depending on whose turn it is
        {
            userTurn();  
        }  
        else
        {
            enemyTurn(); //TODO change enemyTurn into a coroutine so frames can still happen when enemy is having its turn
        }
    }

    //The user's turn.
    private void userTurn()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1000))
        {
            //If nothing is hit, deselect
            deselect();
            return;
        }
        //Figure out what the user clicked on
        if (hit.collider.gameObject.name.Contains(Team.White.ToString())) //If they clicked on a white piece, show the moves for that chess piece
        {
            deselect();
            int col = Coordinate.xToCol(hit.transform.gameObject.transform.position.x);
            int row = Coordinate.xToCol(hit.transform.gameObject.transform.position.z);
            selected = state.getPiece(new Coordinate(col, row));
            selected.getObject().GetComponent<Renderer>().material = Prefabs.highLight;
            showValidMoveTiles(selected);
        }
        else if (
            hit.collider.gameObject.name.Contains("moveTile")
            || hit.collider.gameObject.name.Contains(Team.Black.ToString())
        )
        { //If you click on a black piece or a move tile
            //If you click on the piece you want to kill instead of the move tile, find the move tile directly below it
            if (hit.collider.gameObject.name.Contains(Team.Black.ToString()))
            {
            int col = Coordinate.xToCol(hit.transform.gameObject.transform.position.x);
            int row = Coordinate.xToCol(hit.transform.gameObject.transform.position.z);
            Coordinate blackPosCoord = new Coordinate(col,row);
                LayerMask mask = LayerMask.GetMask("Move tiles");
                Physics.Raycast(
                    new Vector3(blackPosCoord.getX(), 0.2F, blackPosCoord.getZ()),
                    Vector3.down,
                    out hit,
                    1000,
                    mask
                );
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

    //Ends a player or AI turn
    private void endTurn()
    {
        Processing.updateGameResult(state, state.currentTeam().nextTeam());
        turnOver = false;
        state.setTeam(state.currentTeam().nextTeam());
        Messages.Log(MessageType.BoardState, state.ToString());
    }

    //Deselects the selected chess piece, (changes material and removes the move tiles)
    private void deselect()
    {
        if (selected != null)
        {
            if (selected.getTeam() == Team.White)
            {
                selected.getObject().GetComponent<Renderer>().material = Prefabs.white;
            }
            else
            {
                selected.getObject().GetComponent<Renderer>().material = Prefabs.black;
            }
            selected = null;
        }
        foreach (GameObject tile in tileList)
        {
            Destroy(tile);
        }
    }

    //Executes a move
    private void doMove(Move move)
    {
        Piece killedPiece = move.doMoveState(state); //Updates the boardState.
        //It is done separately to allow this function to be reused when testing/checking moves
        showMove(move, killedPiece); //Does the rest of the move, updating the GameObject placements
    }

    //Updates the gameobjects (creates, destroys, moves) so the user can see the changes to the chess game
    private void showMove(Move move, Piece killedPiece)
    {
        if (killedPiece != null)
        {
            killedPiece.destroy();
        }
        move.getPiece().getObject().transform.position = new Vector3(
            move.getTo().getX(),
            move.getPiece().getObject().transform.position.y,
            move.getTo().getZ()
        );
        if (move is CastlingMove)
        {
            showMove(((CastlingMove)move).rookMove, null);
        }
        if (move is PromoteMove)
        {
            if (state.currentTeam() == state.playersTeam())
            {
                promotionMenuReference.Run((PromoteMove)move); //If its the user's turn, let them choose what to promote to
            }
            else //Promotion for enemy AI
            {
                move.getPiece().destroy();
                ((PromoteMove)move).makePromotedPiece(); //Allow the new piece replacing the pawn to appear
            }
        }
    }

    //The AI/Enemy's turn
    private void enemyTurn()
    { 
        List<Move> AIMoves = Processing.allValidMoves(state, state.enemysTeam());
        if (AIdifficulty == AIMode.easy)
        {
            int index = Random.Range(0, AIMoves.Count);
            doMove(AIMoves[index]);
        }
        else
        {
            List<Move> maxPriMoves = AI.getMaxPriMoves(state, AIMoves, state.enemysTeam());
            int index = Random.Range(0, maxPriMoves.Count);
            doMove(maxPriMoves[index]);
        }
        turnOver = true;
    }

    //Called when the game ends. Does stuff and stops the code from running
    private void endGame()
    { //Displays text depending on the game outcome
        if (state.getGameResult() == boardState.GameResult.Stalemate)
        {
            gameResultText.text = "You got a stalemate. Impressive";
        }
        else if (state.getGameResult() == boardState.GameResult.GameWon)
        {
            gameResultText.text = "YOU WON YOU LITTLE BEAUTY!!!";
        }
        else if (state.getGameResult() == boardState.GameResult.GameLost)
        {
            gameResultText.text = "YOU LOST!!! HOW DID YOU MANAGE THAT???";
        }
        gameResultText.gameObject.SetActive(true);

        this.enabled = false; //disable the code
    }

    //Makes move tiles for all the valid moves of a certain piece
    private void showValidMoveTiles(Piece piece)
    {
        //If you aren't in check, you can do any move (barring the ones that put you in check)
        Team team = piece.getTeam();
        List<Move> moves = Processing.validMoves(state, piece);

        //Make a tile for each move
        foreach (Move move in moves)
        {
            makeMoveTile(move);
        }
    }

    /**Makes move tiles*/
    private void makeMoveTile(Move move)
    {
        GameObject newTile = Instantiate(
            Prefabs.tilePrefab,
            new Vector3(move.getTo().getX(), 0.16F, move.getTo().getZ()),
            Quaternion.identity
        );
        tileList.Add(newTile);
        newTile.GetComponent<tileScript>().setMove(move);
    }
}
