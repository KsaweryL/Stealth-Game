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

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(Random.Range(-0.296f, 0.318f), 0f, Random.Range(-6.59f, -2.84f));   //with respect to the barrier
        //transform.localPosition = new Vector3(Random.Range(-5.78f, 5.54f), 1.5f, Random.Range(-5.19f, 6.06f));       //whole plane rotation barrier
        //transform.localPosition = new Vector3(Random.Range(-1.57f, 7.74f), 1.5f, Random.Range(-6.72f, -4.08f));
        //transform.localPosition = new Vector3(Random.Range(-3.79f, 4.06f), 1.5f, Random.Range(-3.3f, 4.31f));

        //Debug.Log(GetGamesTransformPosition(transform.position));
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
                ResetPosition();
        }
    }

}
