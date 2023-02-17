using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour //This is for the pause menu
{
    public static bool GamePaused = false;
    public GameObject PauseMenuPanel;
    public GameObject PromotionMenuPanel;
    private bool wasPromotion;
    public GameObject board;
    private Game GameReference; //Used to stop Game from running whilst paused


    void Start()
    {
        GameReference = board.GetComponent<Game>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Resumes the game
    public void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1f;
        PauseMenuPanel.SetActive(false);
        if (wasPromotion) { PromotionMenuPanel.SetActive(true); }
        GameReference.enabled = true;
    }

    //Pauses the game
    private void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        PauseMenuPanel.SetActive(true);
        if (PromotionMenuPanel.activeInHierarchy)
        {
            wasPromotion = true;
            PromotionMenuPanel.SetActive(false);
        }
        GameReference.enabled = false;
    }
}
