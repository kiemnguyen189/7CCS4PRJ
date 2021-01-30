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
    public static float baseBuildingBufferTime = 5;
    public float buildingBufferTimer = 5;

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

    void Destroy()
    {
        Destroy(gameObject);
    }
}
