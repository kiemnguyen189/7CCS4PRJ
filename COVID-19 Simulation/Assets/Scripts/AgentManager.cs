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

    public Transform hit;

    // Agent based variables.
    public Transform followerPrefab;
    public NavMeshAgent navAgent;
    public AgentType agentType;
    public Transform follower;
    public int groupSize;
    public float minSpeed;
    public float maxSpeed;
    public float radius;
    public int chance;
    public int groupChance;

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
    public Renderer rend;
    private Color color;

    // TODO: Maybe make a data structure to store varying agent parameters e.g. colour, speed, chances etc.

    // Start is called before the first frame update
    void Start() {

        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Spawning type of agents.
        chance = Random.Range(0, 100);
        groupChance = Random.Range(0, 100);
        groupSize = Random.Range(2, manager.maxGroupSize);
        if (chance < manager.ratioShoppers) {
            // * Shopper agent speeds.
            maxSpeed = manager.maxAgentSpeed * 0.5f;
            minSpeed = maxSpeed / 2;
            if (groupChance < manager.ratioGroupShoppers) {
                // TODO: vvv Can be condensed.
                agentType = AgentType.GroupShopper;
                gameObject.tag = "GroupShopper";
                color = new Color(1,0,1,1);
                for (int i = 0; i < groupSize-1; i++) {
                    follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                    follower.transform.parent = gameObject.transform;
                }
                // TODO: ^^^ Can be condensed.
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
            if (groupChance < manager.ratioGroupCommuters) {
                // TODO: vvv Can be condensed.
                agentType = AgentType.GroupCommuter;
                gameObject.tag = "GroupCommuter";
                color = new Color(0,1,1,1);
                for (int i = 0; i < groupSize-1; i++) {
                    follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                    follower.transform.parent = gameObject.transform;
                }
                // TODO: ^^^ Can be condensed.
            } else {
                agentType = AgentType.Commuter;
                gameObject.tag = "Commuter";
                color = new Color(0,1,0,1);
                groupSize = 1;
            }
        }
        navAgent.speed = Random.Range(minSpeed, maxSpeed);
        navAgent.angularSpeed = maxSpeed*10;
        navAgent.acceleration = maxSpeed*10;

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

        // * Radius initialization.
        radius = manager.radiusSize;
        Vector3 scaleChange = new Vector3(radius, 0, radius);
        gameObject.transform.GetChild(0).localScale += scaleChange;

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
            navAgent.SetDestination(currentDestination.position);
        }   
        
        // TODO: Interaction and Infection checks.

    }

    // Updates the list of destinations each agent has.
    public void UpdateDestinations() {
        destinations.RemoveAt(0);
    }

    //
    public float UpdateBuildingBufferTime() {
        buildingBufferTimer -= Time.deltaTime;
        return buildingBufferTimer;
    }

    //
    public float ResetBuildingBufferTime() {
        buildingBufferTimer = baseBuildingBufferTime;
        return buildingBufferTimer;
    }

    //
    private void OnCollisionEnter(Collision other) {
        // * Do not collide with non agents and within-group agents.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") && (other.gameObject.name != "Map"));
        if (environmentCheck && !(other.transform.IsChildOf(transform))) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            if (leadScript != null && leadScript.GetInstanceID() > GetInstanceID()) {
                TrackInteraction(other);
                rend.material.color = new Color(0, 0, 0, 1);
                other.gameObject.GetComponent<AgentManager>().rend.material.color = new Color(0, 0, 0, 1);
            } else if (followScript != null && followScript.GetInstanceID() > GetInstanceID()) {
                TrackInteraction(other);
                rend.material.color = new Color(0, 0, 0, 0.5f);
                other.gameObject.GetComponent<FollowAgentManager>().rend.material.color = new Color(0, 0, 0, 0.5f);
            }

        }
        
    }

    // Helper method used in OnCollisionEnter to track interactions between agents.
    public void TrackInteraction(Collision other) {
        // TODO: Store world location of interactions.
        // TODO: Set "hit" spheres inactive until needed to be shown at the end of the simulation.
        ContactPoint contact = other.contacts[0];
        Vector3 tempPoint = contact.point;
        tempPoint.y = -10;   // * Keep the same elevation for contact points.
        Instantiate(hit, tempPoint, Quaternion.identity);
        Debug.Log(tempPoint);
        manager.contacts += 1;
    }

    //
    public void Despawn() {
        manager.ReduceTotalAgents(agentType, groupSize);
        Destroy(gameObject);
    }


}
