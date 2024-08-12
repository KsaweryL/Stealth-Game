using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    Game game;
    public Vector3 GetGamesTransformPosition(Vector3 position)
    {
        game = GetComponentInParent<Game>();
        return game.transform.InverseTransformPoint(position);
    }
    public void ResetProperties()
    {
        gameObject.SetActive(true);

    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        //it means that if it shoudl be executed if the collided object has the PlayerInventory component
        //aka only player has it
        if(playerInventory != null)
        {
            //Debug.Log("diamond collected");
            playerInventory.DiamondCollected();
            if (!GetComponentInParent<Game>().GetIsTrainingOn())
            {
                gameObject.SetActive(false);
            }
            else
                transform.localPosition = new Vector3(Random.Range(-3.92f, 3.9f), 1.5f, Random.Range(-4.0f, 4.0f));

            Debug.Log(GetGamesTransformPosition(transform.position));
        }
    }

}
