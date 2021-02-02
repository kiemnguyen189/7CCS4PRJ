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

    public Camera cam;
    public NavMeshAgent agent;
    public AgentType agentType;
    public Transform startNode;
    public Transform endNode;

    private static float baseBuildingBufferTime = 5;
    private float buildingBufferTimer = 5;
    private Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<Renderer>();
        switch (agentType) {
            case AgentType.Tourist: 
                color = new Color(1,0,0,1);
                
                break;
            case AgentType.Commuter:
                color = new Color(0,1,0,1);

                break;
        }
        rend.material.color = color;

        // TODO: Set the start and end nodes for each agent, assigned randomly.
        // TODO: If the agent is a Tourist, normal randomness
        // TODO: If the agent is a Commuter, random from set of possible end nodes.


        // TODO: Set all of the buildings an agent would visit, if they are a Tourist agent.
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // TODO: Temporary movement for all agents using mouse clicks
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
