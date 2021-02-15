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

    public GameObject manager;

    // Agent based variables.
    public Transform followerPrefab;
    public NavMeshAgent agent;
    public AgentType agentType;
    public Transform follower;

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

        startNode = gameObject.transform;

        int chance = Random.Range(0, 100);
        int groupChance = Random.Range(0, 100);

        if (chance <= manager.GetComponent<SimManager>().ratioShoppers) {
            if (groupChance <= manager.GetComponent<SimManager>().ratioGroupShoppers) {
                agentType = AgentType.GroupShopper;
                gameObject.tag = "GroupShopper";
                color = new Color(1,0,1,1);

                // TODO: Different number of followers decided randomly based on a range.
                follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                follower.transform.SetParent(gameObject.transform);

            } else {
                agentType = AgentType.Shopper;
                gameObject.tag = "Shopper";
                color = new Color(1,0,0,1);
            }
        } else {
            if (groupChance <= manager.GetComponent<SimManager>().ratioGroupCommuters) {
                agentType = AgentType.GroupCommuter;
                gameObject.tag = "GroupCommuter";
                color = new Color(0,1,1,1);

                // TODO: Different number of followers decided randomly based on a range.
                follower = Instantiate(followerPrefab, gameObject.transform.position, gameObject.transform.rotation);
                follower.transform.SetParent(gameObject.transform);

            } else {
                agentType = AgentType.Commuter;
                gameObject.tag = "Commuter";
                color = new Color(0,1,0,1);
            }
        }


        rend = GetComponent<Renderer>();
        rend.material.color = color;

        destinations = new List<Transform>();
        int numDest = Random.Range(1, maxDestinations);
        if (agentType == AgentType.Shopper || agentType == AgentType.GroupShopper) {
            destinations = manager.GetComponent<SimManager>().SetDestinations(numDest);
        }
        endNode = manager.GetComponent<SimManager>().SetEndNode(startNode);
        destinations.Add(endNode);

        manager.GetComponent<SimManager>().AddTotalAgents(agentType);
        //manager.AddTotalAgents(agentType);
            
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // // ? Temporary movement for all agents using mouse clicks
        // if (Input.GetMouseButtonDown(0)) {
        //     Ray ray = manager.GetComponent<SimManager>().cam.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit)) {
        //         agent.SetDestination(hit.point);
        //     }
        // }

        // TODO: Dynamic destination allocation for each agent.
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
        manager.GetComponent<SimManager>().ReduceTotalAgents(agentType);
        //manager.ReduceTotalAgents(agentType);
        Destroy(gameObject);
    }


}
