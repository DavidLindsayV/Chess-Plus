using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**An abstract class for GameStates */
public abstract class GameState: MonoBehaviour{

    /**Start a GameState */
    public abstract void runState();
/**Close a GameState*/ 
    public abstract void closeState();
}