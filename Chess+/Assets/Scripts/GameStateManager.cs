//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

///** A script to manage the state of the game */
//public class GameStateManager : MonoBehaviour
//{
//    enum GameState { Running, Ended, Paused, Promoting};
//    //Running: the game is running, either AI is playing or waiting for user input
//    //Ended: the game has finished
//    //Paused: the game has paused
//    //Promoting: the game is waiting for user to select what to promote to

//    public Text gameResultText;
//    public GameObject PromotionPanel;
//    public GameObject board;
//    private boardScript boardScriptReference;

//    Stack<GameState> stateStack;

//    // Start is called before the first frame update
//    void Start()
//    {
//        stateStack = new Stack<GameState>();
//        boardScriptReference = board.GetComponent<boardScript>();
//        //deactivate all states
//        setState(GameState.Running);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            if (currentState == GameState.Paused)
//            {
//                setState(pastStates.Pop());
//            }
//            else
//            {
//                setState(GameState.Paused);
//            }
//        }
//    }

//    void setState(GameState newState)
//    {
//        deactivate(this.currentState);
//        pastStates.add(currentState);
//        currentState = newState;
//        activate(newState);
//    }

//    void endState()
//    {

//    }

//    void deactivate(GameState state)
//    {
//        if(state == null) { return;  }
//    }

//    void activate(GameState state)
//    {
//        if(state == null) { return;  }
//    }
//}