using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class PauseMenu : MonoBehaviour
{
    bool pauseMenuIsoOn;
    //NPC
    NPCMovement[] npcs;
    //player
    CinemachineFreeLook camCinemachine;


    // Start is called before the first frame update
    void Start()
    {
        pauseMenuIsoOn = false;
       
    }

    void FreezeTheGame()
    {
        npcs = GetComponentInParent<Game>().GetNPCmovements();

        for(int npc = 0; npc < npcs.Length; npc++)
        {
            NavMeshAgent agent = npcs[npc].GetNavMeshAgent();
            agent.enabled = false;
        }

        //cam
        camCinemachine = GetComponentInParent<Game>().GetComponentInChildren<CinemachineFreeLook>();
        camCinemachine.enabled = false;


    }

    void UnfreezeTheGame()
    {
        npcs = GetComponentInParent<Game>().GetNPCmovements();

        for (int npc = 0; npc < npcs.Length; npc++)
        {
            NavMeshAgent agent = npcs[npc].GetNavMeshAgent();
            agent.enabled = true;
        }

        //cam
        camCinemachine = GetComponentInParent<Game>().GetComponentInChildren<CinemachineFreeLook>();
        camCinemachine.enabled = true;
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
    public void QuitGame()
    {
        Debug.Log("Exited the game");
        Application.Quit();
    }
}
