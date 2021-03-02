using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    
    public SimManager manager;

    public Transform vehiclePrefab;
    public Transform end;
    public Transform path;

    public float timeBetweenWaves = 3f;
    private float countdown = 2f;

    private int waveIndex = 0;

    
    // Start is called before the first frame update
    void Start()
    {

        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>(); 
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {

        if (manager.simStarted && !manager.GetIsPedestrianised()) {

            if (countdown <= 0f)
            {
                StartCoroutine(SpawnWave());
                countdown = timeBetweenWaves;
            }
            countdown -= Time.deltaTime;
        }
    }

    IEnumerator SpawnWave() {
        waveIndex++;
        SpawnVehicle();
        yield return new WaitForSeconds(0.0f);
    }

    //
    public void SpawnVehicle() {
        Transform v = Instantiate(vehiclePrefab, transform.position, transform.rotation);
        v.transform.parent = gameObject.transform;
    }

    public List<Transform> GetRoute() { 
        List<Transform> route = new List<Transform>();
        foreach (Transform child in path) { route.Add(child); }
        route.Add(end);
        return route; 
    }


}
