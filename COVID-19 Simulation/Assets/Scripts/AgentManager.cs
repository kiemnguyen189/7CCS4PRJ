using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentType {
    Shopper,
    Commuter
}

public class AgentManager : MonoBehaviour
{

    public SimManager manager;

    public NavMeshAgent agent;
    public AgentType agentType;
    public Transform startNode;
    public Transform endNode;
    public int maxDestinations = 5;
    public List<Transform> destinations;
    public Transform currentDestination;
    
    private static float baseBuildingBufferTime = 2;
    private float buildingBufferTimer = baseBuildingBufferTime;
    private Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start() {

        startNode = gameObject.transform;

        // TODO: Retrieve spawning probability of Shoppers vs Commuters (and groups) from SimManager.
        int chance = Random.Range(0, 100);
        if (chance <= manager.ratioShoppers) {
            agentType = AgentType.Shopper;
            gameObject.tag = "Shopper";
            color = new Color(1,0,0,1);
        } else {
            agentType = AgentType.Commuter;
            gameObject.tag = "Commuter";
            color = new Color(0,1,0,1);
        }
        rend = GetComponent<Renderer>();
        rend.material.color = color;

        destinations = new List<Transform>();
        int numDest = Random.Range(1, maxDestinations);
        if (agentType == AgentType.Shopper) {
            destinations = manager.SetDestinations(numDest);
        }
        endNode = manager.SetEndNode(startNode);
        destinations.Add(endNode);
            
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // TODO: Temporary movement for all agents using mouse clicks
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = manager.GetComponent<SimManager>().cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                agent.SetDestination(hit.point);
            }
        }

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
        Destroy(gameObject);
    }


}
