using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public SimManager manager;

    public Transform agentPrefab;
    public Transform spawnPoint;

    public bool isDense = false;            // Whether or not a spawner is located in a high traffic areaa (r.g. main roads/stations).

    public float timeBetweenWaves = 3f;
    private float countdown = 2f;

    //
    void Start() {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();


    }

    //
    private void Update() {

        if (manager.simStarted) {

            // TODO: Spawn rates dependant on simTime.

            // Get the current total hourly pedestrian flow based on the current simulation time.
            // The %24 is used for durations above a day (1440), so indexes stay within 0-23.
            int hourlyFlow = manager.GetFlowTimings()[ConvertSimTime(manager.GetSimTime()) % 24];
            // ! Change to GetNumSpawners when all spawners placed.
            int agentsPerSpawner = (int)Mathf.Floor(hourlyFlow / manager.GetNumSpawners());
            //int agentsPerSpawner = (int)Mathf.Floor(hourlyFlow / 30);
            timeBetweenWaves = 60f / agentsPerSpawner;
            // TODO: Record current stats every hour in sim time.
            

            if (countdown <= 0f)
            {
                SpawnAgent();
                countdown = timeBetweenWaves;
                Debug.Log("HourlyFlow: " + hourlyFlow + " agentsPerSpawner: " + agentsPerSpawner + " timeBetweenWaves: " + timeBetweenWaves);
            }
            countdown -= Time.deltaTime;

            
        }
    }

    //
    public void SpawnAgent()
    {
        // TODO: MavMesh.SamplePosition.
        //NavMeshHit hit;
        //if (NavMesh.SamplePosition(spawnPoint.position, out hit, 2, 1)) {
            Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);
        //}
        
        
    }

    // Converts the raw simulation time into the relevant hourly index in flowTimings.
    public int ConvertSimTime(float time) {
        return (int)Mathf.Floor((manager.GetFlowTimings().Length / 1440f) * Mathf.Floor(manager.GetSimTime()));
    }

    //
    private void OnTriggerEnter(Collider other) {
        // Check if current door is part of the list of destinations for each agent.
        AgentManager agent = other.GetComponent<AgentManager>();
        // Checks for valid Entry.
        if (agent != null && agent.GetCurrentDestination() == gameObject.transform) {
            agent.Despawn();
        }
        

    }

}
