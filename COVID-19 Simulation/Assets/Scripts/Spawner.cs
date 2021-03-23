using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{

    public SimManager manager;

    public Transform agentPrefab;

    public bool isDense = false;            // Whether or not a spawner is located in a high traffic area (r.g. main roads/stations).

    public float timeBetweenWaves;
    private int hourlyFlow;
    private int numSpawners;
    public float countdown = 2f;

    //
    void Start() {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();
        numSpawners = manager.GetNumSpawners();

    }

    //
    private void Update() {

        if (manager.simStarted) {

            // Get the current total hourly pedestrian flow based on the current simulation time.
            // The %24 is used for durations above a day (1440), so indexes stay within 0-23.
            hourlyFlow = manager.GetFlowTimings()[ConvertSimTime(manager.GetSimTime()) % 24];
            // ! Change to GetNumSpawners when all spawners placed.
            timeBetweenWaves = 60f / ((float)hourlyFlow / numSpawners);

            if (countdown <= 0f)
            {
                SpawnAgent();
                countdown = timeBetweenWaves;
            }
            countdown -= Time.deltaTime;
            
        }
    }

    //
    public void SpawnAgent()
    {
        // TODO: MavMesh.SamplePosition.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2, 1)) {
            Instantiate(agentPrefab, transform.position, transform.rotation);
        }
        
        
    }

    // Converts the raw simulation time into the relevant hourly index in flowTimings.
    public int ConvertSimTime(float time) {
        return (int)Mathf.Floor((manager.GetFlowTimings().Length / 1440f) * Mathf.Floor(manager.GetSimTime()));
    }


}
