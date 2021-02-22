using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public SimManager manager;

    public Transform agentPrefab;
    public Transform spawnPoint;

    public float timeBetweenWaves = 3f;
    private float countdown = 2f;

    private int waveIndex = 0;

    
    void Start() {
        // TODO: Only start spawning if start button is pressed.
        //SpawnEnemy();
        manager = GameObject.Find("Manager").GetComponent<SimManager>();
    }
    
    
    // // Update is called once per frame
    // void Update()
    // {
    //     if (manager.simStarted) {

    //         if (countdown <= 0f)
    //         {
    //             StartCoroutine(SpawnWave());
    //             countdown = timeBetweenWaves;
    //         }
    //         countdown -= Time.deltaTime;
    //     }
        

    // }

    private void FixedUpdate() {
        if (manager.simStarted) {

            if (countdown <= 0f)
            {
                StartCoroutine(SpawnWave());
                countdown = timeBetweenWaves;
            }
            countdown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave()
    {
        // TODO: Spawning rate based on initial user inputs.
        // TODO: I.e. maximum number of agents, spawning based on peak times, etc.
        waveIndex++;

        SpawnEnemy();
        yield return new WaitForSeconds(0.0f);

    //     for (int i = 0; i < waveIndex; i++)
    //     {
    //         SpawnEnemy();
    //         yield return new WaitForSeconds(0.5f);
    //     }
    }

    public void SpawnEnemy()
    {
        Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);
        
    }


    private void OnTriggerEnter(Collider other) {
        // Check if current door is part of the list of destinations for each agent.
        AgentManager agent = other.GetComponent<AgentManager>();
        // Checks for valid Entry.
        if (agent != null && agent.currentDestination == gameObject.transform) {
            agent.Despawn();
        }
        

    }

}
