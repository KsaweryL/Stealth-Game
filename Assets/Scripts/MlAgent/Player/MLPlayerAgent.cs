using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPlayerAgent : MonoBehaviour
{
    [SerializeField] private Transform diamondTransform;

    public Vector3 startingPosition;
    public List<Vector3> diamondsPositions;

    Diamond[] allDiamonds;

    void Start()
    {
        //needs to be dynamic
        diamondsPositions = new List<Vector3>();

        allDiamonds = FindObjectsOfType<Diamond>();
        //important! I have an absolute posiotion here, not a local one as in the inspecotr
        for (int diamond = 0; diamond < allDiamonds.Length; diamond++)
            diamondsPositions.Add(allDiamonds[diamond].transform.position);
        
    }

    private void Update()
    {

    }
}
