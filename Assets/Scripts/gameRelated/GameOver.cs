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

        if(playerWon && gameOver)
            if (GetComponentInChildren<MLPlayerAgent>())
                GetComponentInChildren<MLPlayerAgent>().PlayerHasWon();
        
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
        if (GetComponentInChildren<ThirdPersonMovement>().GetCurrentHealth() == 0)
        {
            gameOver = true;
            playerWon = false;
            //when player looses, inform the mlagent component
            if(GetComponentInChildren<MLPlayerAgent>())
                GetComponentInChildren<MLPlayerAgent>().PlayerHasLost();
        }
    }
}
