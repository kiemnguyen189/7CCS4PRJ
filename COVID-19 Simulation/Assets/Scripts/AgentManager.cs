using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentType {
    Tourist,
    Commuter
}

public class AgentManager : MonoBehaviour
{

    public SimManager manager;

    public NavMeshAgent agent;
    public AgentType agentType;
    public Transform startNode;
    public Transform endNode;
    public List<Transform> destinations;

    private static float baseBuildingBufferTime = 2;
    private float buildingBufferTimer = baseBuildingBufferTime;
    private Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start() {

        // TODO: Retrieve spawning probability of Tourists vs Commuters (and groups) from SimManager.
        int chance = Random.Range(0, 100);
        if (chance >= manager.ratioTourists) {
            agentType = AgentType.Tourist;
            gameObject.tag = "Tourist";
            color = new Color(1,0,0,1);
        } else {
            agentType = AgentType.Commuter;
            gameObject.tag = "Commuter";
            color = new Color(0,1,0,1);
        }
        rend = GetComponent<Renderer>();
        rend.material.color = color;

        // TODO: Set the start and end nodes for each agent, assigned randomly.
        // TODO: If the agent is a Tourist, normal randomness.
        // TODO: If the agent is a Commuter, random from set of possible end nodes.


        // TODO: Set all of the buildings an agent would visit, if they are a Tourist agent.
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // TODO: Temporary movement for all agents using mouse clicks
        if (Input.GetMouseButtonDown(0)) {

            Ray ray = manager.GetComponent<SimManager>().cam.ScreenPointToRay(Input.mousePosition);

            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                agent.SetDestination(hit.point);
            }
        }


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


}
