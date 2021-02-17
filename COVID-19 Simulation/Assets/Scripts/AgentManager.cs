using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentType {
    Shopper,
    Commuter,
    GroupShopper,
    GroupCommuter
}

public class AgentManager : MonoBehaviour
{

    private SimManager manager;

    // Agent based variables.
    public Transform followerPrefab;
    public NavMeshAgent agent;
    public AgentType agentType;
    public Transform follower;
    public int groupSize;
    public float minSpeed;
    public float maxSpeed;

    // Location based variables.
    private Transform startNode;
    private Transform endNode;
    public int maxDestinations = 5;
    public List<Transform> destinations;
    public Transform currentDestination;
    
    // Building based variables.
    private static float baseBuildingBufferTime = 2;
    private float buildingBufferTimer = baseBuildingBufferTime;

    // Rendering variables.
    private Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start() {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Spawning type of agents.
        int chance = Random.Range(0, 100);
        int groupChance = Random.Range(0, 100);
        groupSize = Random.Range(2, manager.maxGroupSize);

        if (chance <= manager.ratioShoppers) {
            // * Shopper agent speeds.
            maxSpeed = manager.maxAgentSpeed * 0.5f;
            minSpeed = maxSpeed / 2;
            if (groupChance <= manager.ratioGroupShoppers) {
                agentType = AgentType.GroupShopper;
                gameObject.tag = "GroupShopper";
                color = new Color(1,0,1,1);
                // Spawn followers if of type group.
                for (int i = 0; i < groupSize-1; i++) {
                    follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                    follower.transform.parent = gameObject.transform;
                }
            } else {
                agentType = AgentType.Shopper;
                gameObject.tag = "Shopper";
                color = new Color(1,0,0,1);
                groupSize = 1;
            }
        } else {
            // * Commuter agent speeds.
            maxSpeed = manager.maxAgentSpeed;
            minSpeed = maxSpeed / 2;
            if (groupChance <= manager.ratioGroupCommuters) {
                agentType = AgentType.GroupCommuter;
                gameObject.tag = "GroupCommuter";
                color = new Color(0,1,1,1);
                // Spawn followers if of type group.
                for (int i = 0; i < groupSize-1; i++) {
                    follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                    follower.transform.parent = gameObject.transform;
                }
            } else {
                agentType = AgentType.Commuter;
                gameObject.tag = "Commuter";
                color = new Color(0,1,0,1);
                groupSize = 1;
            }
        }
        agent.speed = Random.Range(minSpeed, maxSpeed);
        agent.angularSpeed = maxSpeed*10;
        agent.acceleration = maxSpeed*10;

        rend = GetComponent<Renderer>();
        rend.material.color = color;

        // * Destinations of agent.
        startNode = gameObject.transform;
        destinations = new List<Transform>();
        int numDest = Random.Range(1, maxDestinations);
        if (agentType == AgentType.Shopper || agentType == AgentType.GroupShopper) {
            destinations = manager.SetDestinations(numDest);
        }
        endNode = manager.SetEndNode(startNode);
        destinations.Add(endNode);

        // * SimManager metrics.
        manager.AddTotalAgents(agentType, groupSize);
            
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // ! Temporary movement for all agents using mouse clicks
        // if (Input.GetMouseButtonDown(0)) {
        //     Ray ray = manager.GetComponent<SimManager>().cam.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit)) {
        //         agent.SetDestination(hit.point);
        //     }
        // }

        if (destinations.Count != 0) {
            currentDestination = destinations[0];
            agent.SetDestination(currentDestination.position);
        }   
        

    }

    // Updates the list of destinations each agent has.
    public void UpdateDestinations() {
        destinations.RemoveAt(0);
    }


    public float UpdateBuildingBufferTime() {
        buildingBufferTimer -= Time.deltaTime;
        return buildingBufferTimer;
    }

    public float ResetBuildingBufferTime() {
        buildingBufferTimer = baseBuildingBufferTime;
        return buildingBufferTimer;
    }

    //public 
    public void Despawn() {
        manager.ReduceTotalAgents(agentType, groupSize);
        Destroy(gameObject);
    }


}
