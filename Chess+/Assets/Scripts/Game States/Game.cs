using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Game : GameState
{
    //The text for displaying the result of the game on the UI
    public Text gameResultText;

    private List<GameObject> tileList; //stores the moveTiles the player can click on to do a move

    private enum PlayerState { moveSelecting, movePreparing, moveDoing }; //stores the state of the player

    private PlayerState playerState = PlayerState.moveSelecting;
    private Move selectedMove; //the selected move
    private Piece selected; //The current Piece (white chess piece) selected by the player

    public BoardState state; //this boardState stores the current state of the board

    public enum AIMode
    {
        easy,
        medium
    };

    public AIMode AIdifficulty = AIMode.easy;

    private bool enemyRunning = false; //stores whether the enemyTurn async method is running

    private bool turnOver = false; //Stores whether a turn is over. This allows update to go to a third function, endTurn(), before letting the other player go

    private System.Random random = new System.Random();
    //TODO update comments/documentation in all files
    //TODO make more tests using runMoves under swen221 (as it is the better runner of tests)
    //TODO make card-holding and card-playing mechanics
    //TODO make the Hand part of the FEN string
    //also format the testing stuff better and tidy up that code



    // Start is called before the first frame update
    void Start()
    {
        //Set up some variables
        tileList = new List<GameObject>();
        GameObject Canvas = GameObject.Find("Canvas");
        StateManager.Init();
        //Initialise the chess pieces, set up the board with this FEN string
        state = new BoardState("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - O");
    }

    // Update is called once per frame 
    void Update()
    {
        if (state.getGameResult() != BoardState.GameResult.Ongoing)
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
            if (!enemyRunning) { enemyTurn(); }
        }
    }

    //The user's turn.
    private void userTurn()
    {
        if (playerState == PlayerState.moveSelecting)
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
                    Coordinate blackPosCoord = new Coordinate(col, row);
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
                Move move = hit.collider.gameObject.GetComponent<TileScript>().getMove();
                deselect();
                selectedMove = move;
                playerState = PlayerState.movePreparing;
            }
        }
        else if (playerState == PlayerState.movePreparing){
            selectedMove.prepareMove(state);
            playerState = PlayerState.moveDoing;
        }
        else if (playerState == PlayerState.moveDoing){
            doMove(selectedMove);
            playerState = PlayerState.moveSelecting;
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

    /**Executes a move. Updates the internal state as well as the external view*/
    private void doMove(Move move)
    {
        Piece killedPiece = move.doMoveState(state); //Updates the boardState.
        //It is done separately to allow this function to be reused when testing/checking moves
        move.showMove(state, killedPiece); //Does the rest of the move, updating the GameObject placements
    }

    //The AI/Enemy's turn
    private async void enemyTurn()
    {
        enemyRunning = true;
        Move move = null;
        await Task.Run(() =>
        {
            //This creates a separate thread that doesn't execute the code below
            //until this task of selecting moves has finished
            //Allowing Update to still run while the enemy is taking its time to think
            List<Move> AIMoves = Processing.allValidMoves(state, state.enemysTeam());
            if (AIdifficulty == AIMode.easy)
            {
                int index = this.random.Next(0, AIMoves.Count);
                move = AIMoves[index];
            }
            else
            {
                List<Move> maxPriMoves = AI.getMaxPriMoves(state, AIMoves, state.enemysTeam());
                int index = this.random.Next(0, maxPriMoves.Count);
                move = maxPriMoves[index];
            }
        });
        doMove(move);
        turnOver = true;
        enemyRunning = false;
    }

    //Called when the game ends. Does stuff and stops the code from running
    private void endGame()
    { //Displays text depending on the game outcome
        if (state.getGameResult() == BoardState.GameResult.Stalemate)
        {
            gameResultText.text = "You got a stalemate. Impressive";
        }
        else if (state.getGameResult() == BoardState.GameResult.GameWon)
        {
            gameResultText.text = "YOU WON YOU LITTLE BEAUTY!!!";
        }
        else if (state.getGameResult() == BoardState.GameResult.GameLost)
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
            new Vector3(move.moveTilePos().getX(), 0.16F, move.moveTilePos().getZ()),
            Quaternion.identity
        );
        tileList.Add(newTile);
        newTile.GetComponent<TileScript>().setMove(move);
    }

    public override void runState()
    {
        Time.timeScale = 1f;
        this.enabled = true;
    }

    public override void closeState()
    {
        this.enabled = false;
    }
}
