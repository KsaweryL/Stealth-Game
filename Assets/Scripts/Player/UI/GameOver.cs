using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public bool gameOver;
    public bool playerWon;

    public void UpdateGameOver(bool gameOverVariable)
    {
        gameOver = gameOverVariable;
    }
    
    public void UpdatePlayerWon(bool playerWonVariable)
    {
        playerWon = playerWonVariable;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        playerWon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (FindObjectOfType<ThirdPersonMovement>().GetCurrentHealth() == 0)
        {
            gameOver = true;
            playerWon = false;
        }
    }
}
