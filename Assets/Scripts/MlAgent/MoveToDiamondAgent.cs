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

        transform.localPosition = Vector3.zero;
        base.OnEpisodeBegin();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);


        base.CollectObservations(sensor);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.ContinuousActions[0]);

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        base.OnActionReceived(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");      //x
        continuousActions[1] = Input.GetAxisRaw("Vertical");        //z

        base.Heuristic(actionsOut);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
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

        Debug.Log(GetCumulativeReward());
    }

}
