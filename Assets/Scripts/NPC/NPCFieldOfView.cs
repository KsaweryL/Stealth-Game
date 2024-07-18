using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    public bool playerIsHidden;

    [Header("Distance from NPC to Player")]
    public float distnaceToTarget;

    public void UpdateHiddenStatus(bool Hidden)
    {
        playerIsHidden = Hidden;
    }

    private void UpdatePlayerStatus()
    {
        //Debug.Log(playerIsHidden);

            FindObjectOfType<ChasingPlayer>().UpdatePlayerStatus(canSeePlayer);
            FindObjectOfType<NPC2Movement>().UpdatePlayerStatus(canSeePlayer);
        
    }

    public bool GetCanSeePlayer()
    {
        return canSeePlayer;
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            //we only get 1 instance since there is only 1 player
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            //if the player is within our sight
            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                distnaceToTarget = Vector3.Distance(transform.position, target.position);

                //returns false if the ray intersects with the collider (any obstruction) or if distance to target is 0
                if(!Physics.Raycast(transform.position, directionToTarget, distnaceToTarget, obstructionMask) || distnaceToTarget<2)
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            //if it's outside our sight and is not diorectly onto us
            else if(distnaceToTarget >= 2)
                canSeePlayer = false;
        }
        else if(canSeePlayer == true && distnaceToTarget >= 2)
            canSeePlayer= false;
    }

    //looks for the player
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true) {
            yield return wait;
            //check fov only when player is not hidden
            if(!playerIsHidden)
                FieldOfViewCheck();
            UpdatePlayerStatus();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
