using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**A class to Manage the different states of the game, switching between them */
public static class StateManager
{
    private static bool isInitialised;
    private static GameState currentState;

    public static GameState promoteMenu, pauseMenu, Game;

    /**Initialises all the references*/
    public static void Init()
    {
        currentState = GameObject.Find("Board").GetComponent<Game>();
        Game = currentState;
        GameObject canvas = GameObject.Find("Canvas");
        promoteMenu = canvas.GetComponent<PromoteMenu>();
        pauseMenu = canvas.GetComponent<PauseMenu>();
        isInitialised = true;
    }

    /**Opens the specified GameState, returning the previous GameState */
    public static GameState OpenState(GameState state)
    {
        if (!isInitialised) { Init(); }
        GameState oldState = currentState;
        oldState.closeState();
        state.runState();
        currentState = state;
        return oldState;
    }
}
