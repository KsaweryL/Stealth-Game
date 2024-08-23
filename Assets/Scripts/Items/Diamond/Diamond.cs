using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    Game game;
    int testNr;
    bool isTrainingOn;

    private void UpdateTrainingRelatedData()
    {
        game = GetComponentInParent<Game>();
        testNr = game.GetTestNr();
        isTrainingOn = game.GetIsTrainingOn();

    }
    private void Start()
    {
        testNr = 0;
    }
    public Vector3 GetGamesTransformPosition(Vector3 position)
    {
        game = GetComponentInParent<Game>();
        return game.transform.InverseTransformPoint(position);
    }
    public void ResetProperties()
    {
        gameObject.SetActive(true);

    }

    public void ResetPosition()
    {
        if (testNr == 0)
        {
            game = GetComponentInParent<Game>();
            testNr = game.GetTestNr();
        }

        //2nd test
        if (testNr == 2)
            transform.localPosition = new Vector3(Random.Range(-0.296f, 0.318f), 0f, Random.Range(-6.59f, -2.84f));   //with respect to the barrier
        //transform.localPosition = new Vector3(Random.Range(-5.78f, 5.54f), 1.5f, Random.Range(-5.19f, 6.06f));       //whole plane rotation barrier
        //transform.localPosition = new Vector3(Random.Range(-1.57f, 7.74f), 1.5f, Random.Range(-6.72f, -4.08f));
        //transform.localPosition = new Vector3(Random.Range(-3.79f, 4.06f), 1.5f, Random.Range(-3.3f, 4.31f));

        //Debug.Log(GetGamesTransformPosition(transform.position));
        //3rd test
        else if (testNr == 3)
            transform.localPosition = new Vector3(Random.Range(-8.52f, 7.76f), 1f, Random.Range(-10.88f, 7.09f));
        else if(testNr == 4)
        {
            PlayerSpawnPoint[] playerSpawnPoints = GetComponentInParent<Game>().GetPlayerSpawningPoints();

            MLPlayerAgent mlagent = GetComponentInParent<Game>().GetPlayer().GetComponent<MLPlayerAgent>();
            int randomIndexSpawn = mlagent.GetRandomIndexSPawn();

            while (randomIndexSpawn == mlagent.GetRandomIndexSPawn())
            {
                randomIndexSpawn = Random.Range(0, playerSpawnPoints.Length);
            }

            //initially there was "randomIndexSpawn"
            transform.localPosition = GetGamesTransformPosition(playerSpawnPoints[randomIndexSpawn].transform.position);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UpdateTrainingRelatedData();

        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        //it means that if it shoudl be executed if the collided object has the PlayerInventory component
        //aka only player has it
        if(playerInventory != null)
        {
            //Debug.Log("diamond collected");
            playerInventory.DiamondCollected();
            if (!isTrainingOn)
            {
                gameObject.SetActive(false);
            }
            else
                ResetPosition();
        }

    }

}
