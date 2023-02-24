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
    private GameObject selectedPiece; //The current Piece selected by the player
    private GameObject selectedCard; //The current Card selected by the player
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
    //TODO allow the player to physically play cards
    //TODO make the enemy cards not show
    //TODO make cards disappear when played by user
    //TODO make the Hand part of the FEN string
    //TODO check you can en-passant and castle (manually test it)
    //TODO also format the testing stuff better and tidy up that code
    //TODO playing cards doesn't remove them from your hand
    //TODO figure out how to do both Click_Piece -> Play_Card  moves  as well as Click_Piece -> Click_Card -> Choose_One_Of_Move_Options and Click_Piece -> Choose_Move_Option  and  Click_Card -> Click_Move_Option

    //TODO specifics:
    // - make selection when you click on Nothing deselects all, when you click on a 
    //new Piece it changes selection to that new piece without affecting any selected cards
    //You can change selection of Cards/Pieces without affecting the other
    //And clicking on a selected thing deselects it
    //And make sure makeMoveTiles updates properly when BOTH Piece and Card are specified

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
                userClicked(hit); //figure out what was clicked on and deal with it
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

    private void userClicked(RaycastHit hit)
    {
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

        //If you clicked on a card
        if (hit.collider.gameObject.GetComponent<CardHolder>() != null)
        {
            select(hit.collider.gameObject);
            return;
        }
        //If you clicked on a piece not above a move tile
        if (hit.collider.gameObject.GetComponent<PieceHolder>() != null)
        { //TODO make all the piece gameobjects have a script referring to their Piece like Cards and moveTile
            Piece p = hit.collider.gameObject.GetComponent<PieceHolder>().getPiece();
            select(hit.collider.gameObject);
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

    private void select(GameObject obj)
    {
        if (obj.GetComponent<CardHolder>() != null)
        {
            deselectCard();
            if (obj == selectedCard) { return; }
            selectedCard = obj;
            Card c = obj.GetComponent<CardHolder>().getCard();
            state.getHand(state.playersTeam()).highlight(c);
            //If the Card has no moves to be played on your selected piece, then deselect the piece
            if (selectedPiece != null && selectedCard.GetComponent<CardHolder>().getCard().getPieceSpecificMoves(state, state.playersTeam(), selectedPiece.GetComponent<PieceHolder>().getPiece()).Count == 0)
            {
                deselectPiece();
            }
        }
        else
        {
            deselectPiece();
            if (obj == selectedPiece) { return; }
            selectedPiece = obj;
            if (selectedCard != null && selectedCard.GetComponent<CardHolder>().getCard().getPieceSpecificMoves(state, state.playersTeam(), selectedPiece.GetComponent<PieceHolder>().getPiece()).Count == 0)
            {
                deselectCard();
            }
            state.getHand(state.playersTeam()).showCardOptions(state, selectedPiece.GetComponent<PieceHolder>().getPiece()); //show what cards are potentially valid for playing
        }
        obj.GetComponent<Renderer>().material = Prefabs.highlight2;
        showValidMoveTiles(); //TODO: fix the problem of playing cards that need to be played on pieces - currently you can't do it
    }

    //Ends a player or AI turn

    private void endTurn()
    {
        Processing.updateGameResult(state, state.currentTeam().nextTeam());
        turnOver = false;
        state.setTeam(state.currentTeam().nextTeam());
        state.Draw(state.currentTeam());
        Messages.Log(MessageType.BoardState, state.ToString());
    }

    //Deselects the selected chess piece, (changes material and removes the move tiles)
    private void deselect()
    {
        deselectCard();
        deselectPiece();
    }

    private void deselectCard()
    {
        state.getHand(state.playersTeam()).dehighlight(); //dehighlight all cards in your hand
        selectedCard = null;
        foreach (GameObject tile in tileList)
        {
            Destroy(tile);
        }
    }

    private void deselectPiece()
    {
        if (selectedPiece != null)
        {
            Piece p = selectedPiece.GetComponent<PieceHolder>().getPiece();
            if (p.getTeam() == Team.White)
            {
                selectedPiece.GetComponent<Renderer>().material = Prefabs.white;
            }
            else if (p.getTeam() == Team.Black)
            {
                selectedPiece.GetComponent<Renderer>().material = Prefabs.black;
            }
            selectedPiece = null;
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
    private void showValidMoveTiles()
    {
        List<Move> moves = null;
        Team team = state.playersTeam();
        if (selectedPiece != null && selectedCard != null)
        {
            Piece p = selectedPiece.GetComponent<PieceHolder>().getPiece();
            Card c = selectedCard.GetComponent<CardHolder>().getCard();
            moves = Processing.validMoves(state, p, c, team);
        }
        else if (selectedPiece != null)
        {
            Piece p = selectedPiece.GetComponent<PieceHolder>().getPiece();
            moves = Processing.validMoves(state, p);
        }
        else if (selectedCard != null)
        {
            moves = Processing.validMoves(state, selectedCard.GetComponent<CardHolder>().getCard(), team);
        }
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
