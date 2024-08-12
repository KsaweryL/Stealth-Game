using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToDiamondAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRender;

    public Vector3 startingPosition;
    public Vector3 diamondPosition;

    void Start()
    {
        startingPosition = transform.localPosition;
        diamondPosition = targetTransform.localPosition;

    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-1.36f, 6.44f), 9.2f, Random.Range(-6.52f, -1.0f));
        targetTransform.localPosition = new Vector3(Random.Range(-4.36f, 7.02f), 9.2f, Random.Range(-6.52f, -1.0f));

        //this one executes the base method - since nothign is in teh base one, it is not needed
        base.OnEpisodeBegin();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);

        //overrising the method from the parent class
        base.CollectObservations(sensor);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        base.OnActionReceived(actions);
    }

    //Heuristics are mental shortcuts for solving problems in a quick way that delivers a result that is sufficient enough to be useful given time constraints
    //Implement this function to provide custom decision making logic or to support manual control of an agent using keyboard, mouse, game controller input, or a script.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");      //x
        continuousActions[1] = Input.GetAxisRaw("Vertical");        //z

        base.Heuristic(actionsOut);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Diamond>(out Diamond goal))
        {

            Debug.Log("Diamond");
            SetReward(+1f);

            floorMeshRender.material = winMaterial;

            EndEpisode();       //1 run

            

        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);

            floorMeshRender.material = loseMaterial;
            EndEpisode();       //1 run
        }

        //Debug.Log(GetCumulativeReward());
    }

}
