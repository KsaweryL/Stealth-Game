using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;

public class PauseMenu : MonoBehaviour
{
    bool pauseMenuIsoOn;
    //NPC
    NPCMovement[] npcs;
    Dictionary<NPCMovement, float> npcsSpeed;
    //player
    CinemachineFreeLook camCinemachine;
    ThirdPersonMovement player;
    float playerAnimationSpeed;


    // Start is called before the first frame update
    void Start()
    {
        pauseMenuIsoOn = false;
       
    }

    public void FreezeTheGame()
    {
        npcs = GetComponentInParent<Game>().GetNPCmovements();
        npcsSpeed = new Dictionary<NPCMovement, float>();
        player = GetComponentInParent<Game>().GetPlayer();

        //freeze the player
        playerAnimationSpeed = player.GetComponentInChildren<Animator>().speed;
        player.GetComponentInChildren<Animator>().speed = 0;

        Time.timeScale = 0f;

        //cam
        camCinemachine = GetComponentInParent<Game>().GetComponentInChildren<CinemachineFreeLook>();
        camCinemachine.enabled = false;

        //disable mlagent
        GetComponentInParent<Game>().GetPlayer().GetComponent<MLPlayerAgent>().isPauseOn = true;
    }

    void UnfreezeTheGame()
    {
        npcs = GetComponentInParent<Game>().GetNPCmovements();

        //unfreeze the player
        player.GetComponentInChildren<Animator>().speed = playerAnimationSpeed;

        Time.timeScale = 1f;

        //cam
        camCinemachine = GetComponentInParent<Game>().GetComponentInChildren<CinemachineFreeLook>();
        camCinemachine.enabled = true;

        //enable mlagent
        GetComponentInParent<Game>().GetPlayer().GetComponent<MLPlayerAgent>().isPauseOn = false;

    }


    public void UpdatePauseMenu(bool pauseMenuIsOnVariable)
    {
        pauseMenuIsoOn = pauseMenuIsOnVariable;

        Debug.Log(pauseMenuIsoOn);

        if (!pauseMenuIsoOn)
        {
            transform.gameObject.SetActive(false);
            UnfreezeTheGame();
            
        }
        else
        {
            transform.gameObject.SetActive(true);
            FreezeTheGame();
        }
    }
    public void BackToMainMenu()
    {
        GetComponentInParent<Game>().GetPlayer().GetComponent<MLPlayerAgent>().isPauseOn = false;

        SceneManager.LoadScene("Scenes/StartingMenu");
    }
}
