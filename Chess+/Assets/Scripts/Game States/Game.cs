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
    private Piece selectedPiece; //The current Piece selected by the player
    private Card selectedCard; //The current Card selected by the player
    //used for choosing what move tiles can be shown

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
    //TODO format the testing stuff better and tidy up that code
    //TODO improve the user experience with selecting and deselecting cards and pieces - sometimes you get weird results

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
        switch (playerState)
        {
            case PlayerState.moveSelecting:
                userPlaying(); //figure out what was clicked on and deal with it
                break;
            case PlayerState.movePreparing:
                selectedMove.prepareMove(state);
                playerState = PlayerState.moveDoing;
                break;
            case PlayerState.moveDoing:
                doMove(selectedMove);
                playerState = PlayerState.moveSelecting;
                turnOver = true;
                break;
        }
    }

    private void userPlaying()
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
        //If you clicked on a move tile
        if (hit.collider.gameObject.GetComponent<MoveHolder>() != null)
        {
            clickTile(hit.collider.gameObject);
            return;
        }
        //if what you clicked on was above a move tile
        int col = Coordinate.xToCol(hit.transform.gameObject.transform.position.x);
        int row = Coordinate.xToCol(hit.transform.gameObject.transform.position.z);
        Coordinate clickCoord = new Coordinate(col, row);
        LayerMask mask = LayerMask.GetMask("Move tiles");
        RaycastHit hit2;
        if (Physics.Raycast(
            new Vector3(clickCoord.getX(), 0.2F, clickCoord.getZ()),
            Vector3.down,
            out hit2,
            1000,
            mask
        ))
        {
            clickTile(hit2.collider.gameObject);
            return;
        }
        //If you clicked on a piece not above a move tile
        if (hit.collider.gameObject.GetComponent<PieceHolder>() != null)
        {
            Piece p = hit.collider.gameObject.GetComponent<PieceHolder>().getPiece();
            selectPiece(p);
            return;
        }
        deselect();
    }

    private void clickTile(GameObject tile)
    {
        Move move = tile.GetComponent<MoveHolder>().getMove();
        deselect();
        selectedMove = move;
        playerState = PlayerState.movePreparing;
    }

    private void selectPiece(Piece p)
    {
        if (p == selectedPiece) { deselectPiece(); return; }
        deselectPiece();
        selectedPiece = p;
        p.getObject().GetComponent<Renderer>().material = Prefabs.highlight2;
        showValidMoveTiles(p);
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
        deselectCard();
        deselectPiece();
    }

    public void deselectCard()
    {
        selectedCard = null;
        DestroyMoveTiles();
    }

    /**Destroys all move tiles */
    private void DestroyMoveTiles()
    {
        foreach (GameObject tile in tileList)
        {
            Destroy(tile);
        }
    }

    private void deselectPiece()
    {
        if (selectedPiece != null)
        {
            if (selectedPiece.getTeam() == Team.White)
            {
                selectedPiece.getObject().GetComponent<Renderer>().material = Prefabs.white;
            }
            else if (selectedPiece.getTeam() == Team.Black)
            {
                selectedPiece.getObject().GetComponent<Renderer>().material = Prefabs.black;
            }
            selectedPiece = null;
        }
        DestroyMoveTiles();
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

    /**Makes move tiles for all the valid moves of a certain piece, ON ITS OWN with no cards*/
    private void showValidMoveTiles(Piece p)
    {
        List<Move> moves = new List<Move>();
        Team team = state.playersTeam();
        //Get the moves for a piece
        if (selectedPiece.getTeam() == state.playersTeam())
        {
            moves = Processing.validMoves(state, p);
        }
        //Make a tile for each move
        foreach (Move move in moves)
        {
            makeMoveTile(move);
        }
    }

    //Makes move tiles for all the valid moves of a certain Card
    private void showValidMoveTiles(Card c)
    {
        List<Move> moves = new List<Move>();
        //Get the moves from this card that are position-specific
        for (int row = 1; row < state.boardSize; row++)
        {
            for (int col = 1; col < state.boardSize; col++)
            {
                moves.AddRange(c.getCoordSpecificMoves(state, new Coordinate(col, row)));
            }
        }
        moves.AddRange(c.getGeneralMoves(state));

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
        newTile.GetComponent<MoveHolder>().setMove(move);
    }

    public void selectCard(Card c)
    {
        if (selectedCard == c) { return; }
        deselectPiece();
        selectedCard = c;
        showValidMoveTiles(c);
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
