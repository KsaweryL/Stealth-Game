using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevel : MonoBehaviour
{
    public void PlayLevel1()
    {
        SceneManager.LoadScene("Scenes/Level1");
    }

    public void PlayLevel2()
    {
        SceneManager.LoadScene("Scenes/Level2");
    }

    public void PlayLevel3()
    {
        SceneManager.LoadScene("Scenes/Level3");
    }


    public void PlayLevel1Spectator()
    {
        SceneManager.LoadScene("Scenes/Mlagents/ScenesForSpectators/Level1-MlAgentsTraining-Spectating");
    }

    public void PlayLevel2Spectator()
    {
        SceneManager.LoadScene("Scenes/Mlagents/ScenesForSpectators/Level2-MlAgentsTraining-Spectating");
    }

    public void PlayLevel3Spectator()
    {
        SceneManager.LoadScene("Scenes/Mlagents/ScenesForSpectators/Level3-MlAgentsTraining-Spectating");
    }



}
