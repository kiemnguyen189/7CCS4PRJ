using UnityEngine;

// Main class for the de-spawner nodes.
public class Despawner : MonoBehaviour
{
    // Called when either an agent or vehicle collides with the de-spawner node.
    private void OnTriggerEnter(Collider other) {
        // Check if spawner is the final target of the vehicle;
        VehicleMovement vehicle = other.GetComponent<VehicleMovement>();
        AgentManager agent = other.GetComponent<AgentManager>();
        // Checks for valid Entry.
        if (vehicle != null && vehicle.GetTarget() == gameObject.transform) {
            vehicle.Despawn();
        }
        else if (agent != null && agent.GetCurrentDestination() == gameObject.transform) {
            agent.Despawn();
        }
    }
}
