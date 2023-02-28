using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**A game state for the pause menu 
Attached to the Canvas*/
public class PauseMenu : GameState 
{
    private GameState previousState; 
    private GameObject PauseMenuPanel;
    void Start()
    {
        this.PauseMenuPanel = GameObject.Find("Canvas").transform.Find("PauseMenuPanel").gameObject;
    }

    // Update is called once per frame
    /**The game is paused via ESC*/
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenuPanel.activeSelf)
            {
                resume();
            }
            else
            {
                previousState = StateManager.OpenState(this);
            }
        }
    }

/**Exists so that the button has a function to call */
    public void resume(){
        StateManager.OpenState(previousState);
    }

    public override void runState(){
        Time.timeScale = 1f;
        this.enabled = true;
        this.PauseMenuPanel.SetActive(true);
    }

    public override void closeState(){
        this.PauseMenuPanel.SetActive(false);
    }
}
