using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleEnd : MonoBehaviour
{

    private void OnTriggerEnter(Collider other) {
        // Check if spawner is the final target of the vehicle;
        VehicleMovement vehicle = other.GetComponent<VehicleMovement>();
        // Checks for valid Entry.
        if (vehicle != null && vehicle.GetTarget() == gameObject.transform) {
            vehicle.Despawn();
        }
    }
}
