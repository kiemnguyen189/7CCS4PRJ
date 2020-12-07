using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleMovement : MonoBehaviour
{
    
    public Transform target;
    
    public NavMeshAgent vehicle;

    // Update is called once per frame
    void Update()
    {
        vehicle.SetDestination(target.position);
    }
}
