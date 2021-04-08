using UnityEngine;
using UnityEngine.AI;

// Main spawner class for human agents in the system.
// Spawns in agents inside spawner objects at varied intervals throughout a day of a run.
public class Spawner : MonoBehaviour
{

    public SimManager manager;

    public Transform agentPrefab;       // Prefab of an agent to spawn.

    public float timeBetweenWaves;      // Time in seconds between each agent spawn.
    private int hourlyFlow;             // Hourly flow data retrieved from the SimManager. Varies throughout the day.
    private int numSpawners;            // Total number of spawners in the environment.
    private int agentsLeft;             // Agents still needed to spawn in to fill up the hourlyFlow.
    public float countdown;             // Timer countdown for each spawn.

    // Start is called before the first frame update
    void Start() {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();
        countdown = Random.Range(1f, 10f);

    }

    //
    private void FixedUpdate() {

        if (manager.simStarted) {

            // Get the current total hourly pedestrian flow based on the current simulation time.
            // The %24 is used for durations above a day (1440), so indexes stay within 0-23.
            hourlyFlow = manager.GetFlowTimings()[ConvertSimTime(manager.GetSimTime()) % 24];
            numSpawners = manager.GetNumSpawners();
            agentsLeft = hourlyFlow - manager.GetTotalAgents();
            timeBetweenWaves = 60f / ((float)hourlyFlow / numSpawners);
            if (countdown <= 0f && agentsLeft > 0)
            {
                SpawnAgent();
                countdown = timeBetweenWaves;
            }
            countdown -= Time.deltaTime;
        } else { countdown = Random.Range(1f, 10f); }   // Staggered spawning of agents.

    }

    // Spawns a single agent into the system.
    public void SpawnAgent()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2, 1)) {
            Transform agent = Instantiate(agentPrefab, transform.position, transform.rotation);
        } else { Debug.Log("Spawn Failed."); }
        
        
    }

    // Converts the raw simulation time into the relevant hourly index in flowTimings.
    public int ConvertSimTime(float time) {
        return (int)Mathf.Floor((manager.GetFlowTimings().Length / 1440f) * Mathf.Floor(time));
    }


}
