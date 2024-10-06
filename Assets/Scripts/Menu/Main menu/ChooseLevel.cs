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

    public void PlayLevel1WithoutGuards()
    {
        SceneManager.LoadScene("Scenes/Level1WithoutGuards");
    }

    public void PlayLevel1WithoutGuardsSpectator()
    {
        SceneManager.LoadScene("Scenes/Mlagents/ScenesForSpectators/Level1-MlAgentsTraining-Spectating");
    }


}
