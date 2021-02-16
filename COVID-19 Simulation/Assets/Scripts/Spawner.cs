using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform agentPrefab;
    public Transform groupAgentPrefab;
    public Transform spawnPoint;

    public float timeBetweenWaves = 3f;
    private float countdown = 2f;


    private int waveIndex = 0;

    
    void Start() {
        SpawnEnemy();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }

        countdown -= Time.deltaTime;

    }

    IEnumerator SpawnWave()
    {
        waveIndex++;

        for (int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SpawnEnemy()
    {
        Instantiate(agentPrefab, spawnPoint.position, spawnPoint.rotation);
        
    }


    private void OnTriggerEnter(Collider other) {
        // Check if current door is part of the list of destinations for each agent.
        // TODO: Fix collider with followers FollowAgentManager objects.

        AgentManager agent = other.GetComponent<AgentManager>();
        // Checks for valid Entry.
        if (agent != null && agent.currentDestination == gameObject.transform) {
            agent.Despawn();
        }
        

    }

}
