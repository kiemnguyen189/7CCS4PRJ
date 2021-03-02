using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleMovement : MonoBehaviour
{
    
    public SimManager manager;
    
    public VehicleSpawner spawner;
    public List<Transform> destinations;
    public float speed = 1f;

    private GameObject spawnObject;
    private NavMeshObstacle vehicle;
    private Transform target;
    private Rigidbody rb;

    private void Start() {

        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();
        spawnObject = gameObject.transform.parent.gameObject;
        spawner = spawnObject.GetComponent<VehicleSpawner>();

        vehicle = GetComponent<NavMeshObstacle>();
        rb = GetComponent<Rigidbody>();

        List<Transform> initRoute = spawner.GetRoute();
        destinations = initRoute;

        target = destinations[0];

    }

    // Update is called once per frame
    void Update()
    {

        float step = speed * Time.deltaTime;

        if (Vector3.Distance(target.position, transform.position) < 0.1f) {
            destinations.RemoveAt(0);
            target = destinations[0];
        }
        Quaternion lookRot = Quaternion.LookRotation((target.position - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 5f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        
    }

    private void FixedUpdate() {
        // Instance controller.
        if (!manager.simStarted) {
            Destroy(gameObject);
        }
    }

    public Transform GetTarget() { return target; }

    public void Despawn() {
        Destroy(gameObject);
    }

}
