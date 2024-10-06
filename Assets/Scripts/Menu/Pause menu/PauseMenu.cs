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

        for (int npc = 0; npc < npcs.Length; npc++)
        {
            NavMeshAgent agent = npcs[npc].GetNavMeshAgent();
            agent.enabled = false;

            //freeze the animation
            npcsSpeed.Add(npcs[npc], npcs[npc].gameObject.GetComponent<Animator>().speed);
            npcs[npc].gameObject.GetComponent<Animator>().speed = 0;
        }

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

        for (int npc = 0; npc < npcs.Length; npc++)
        {
            NavMeshAgent agent = npcs[npc].GetNavMeshAgent();

            //unfreeze the animation
            npcs[npc].gameObject.GetComponent<Animator>().speed = npcsSpeed[npcs[npc]];

            agent.enabled = true;

            
        }

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
        SceneManager.LoadScene("Scenes/StartingMenu");
    }
}
