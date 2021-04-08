using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class VehicleSpawner : MonoBehaviour
{
    
    public SimManager manager;

    public Transform vehiclePrefab;     // Prefab of a vehicle to spawn.
    public Transform end;               // De-spawner node.
    public Transform path;              // Path consisting of checkpoints to pass.

    public float maxInterval = 4f;      // Maximum time in seconds between vehicle spawns.
    private float countdown = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>(); 
    }

    private void FixedUpdate() {
        if (manager.simStarted && !manager.GetIsPedestrianised()) {
            if (countdown <= 0f)
            {
                SpawnVehicle();
                countdown = Random.Range(1f, maxInterval);
            }
            countdown -= Time.deltaTime;
        }
    }

    // Spawn a single vehicle.
    public void SpawnVehicle() {
        Transform v = Instantiate(vehiclePrefab, transform.position, transform.rotation);
        v.transform.parent = gameObject.transform;
    }

    // Returns the current route of the vehicle.
    public List<Transform> GetRoute() { 
        List<Transform> route = new List<Transform>();
        foreach (Transform child in path) { route.Add(child); }
        route.Add(end);
        return route; 
    }


}
