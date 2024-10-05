using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public bool gameOver;
    public bool playerWon;

    public bool GetGameOver()
    {
        return gameOver;
    }

    public void UpdateGameOver(bool gameOverVariable)
    {
        gameOver = gameOverVariable;
    }
    
    public void UpdatePlayerWon(bool playerWonVariable)
    {
        playerWon = playerWonVariable;

        if (playerWon && gameOver)
        {
            if (GetComponent<Game>().GetIsTrainingOn())
                if (GetComponentInChildren<MLPlayerAgent>())
                    GetComponentInChildren<MLPlayerAgent>().PlayerHasWon();

            if (!GetComponent<Game>().GetIsTrainingOn())
            {
                GetComponent<Game>().GetEndGameMenu().SetActive(true);
                GetComponent<Game>().pauseMenu.GetComponent<PauseMenu>().FreezeTheGame();
                GetComponent<Game>().GetEndGameMenu().GetComponentInChildren<EndGameText>();
                GetComponent<Game>().GetEndGameMenu().GetComponentInChildren<EndGameText>().GetComponent<TextMeshProUGUI>().text = "You won!";
            }
        }

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
            if (GetComponent<Game>().GetIsTrainingOn())
                //when player looses, inform the mlagent component
                if (GetComponentInChildren<MLPlayerAgent>())
                    GetComponentInChildren<MLPlayerAgent>().PlayerHasLost();

            GetComponent<Game>().GetEndGameMenu().SetActive(true);
            GetComponent<Game>().pauseMenu.GetComponent<PauseMenu>().FreezeTheGame();
            GetComponent<Game>().GetEndGameMenu().GetComponentInChildren<EndGameText>();
            GetComponent<Game>().GetEndGameMenu().GetComponentInChildren<EndGameText>().GetComponent<TextMeshProUGUI>().text = "You lost!";
        }
    }
}
