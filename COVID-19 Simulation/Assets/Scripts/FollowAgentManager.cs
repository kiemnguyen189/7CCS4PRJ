using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgentManager : MonoBehaviour
{

    public SimManager manager;

    public GameObject leader;
    public AgentManager leadManager;
    public NavMeshAgent navAgent;

    public Transform hit;

    public Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Leader initialization.
        leader = gameObject.transform.parent.gameObject;
        leadManager = leader.GetComponent<AgentManager>();

        // * Tag and Color initialization.
        if (leader.tag == "GroupShopper") {
            color = new Color(1,0,1,0.5f);
            gameObject.tag = "GroupShopper";
        } else if (leader.tag == "GroupCommuter") {
            color = new Color(0,1,1,0.5f);
            gameObject.tag = "GroupCommuter";
        }
        rend = GetComponent<Renderer>();
        rend.material.color = color;

        // * Radius initialization.
        float radius = leadManager.radius;
        Vector3 scaleChange = new Vector3(radius, 0, radius);
        gameObject.transform.GetChild(0).localScale += scaleChange;

        // * Speed initialization.
        navAgent.speed = leadManager.maxSpeed;
        navAgent.angularSpeed = leadManager.maxSpeed*10;
        navAgent.acceleration = leadManager.maxSpeed*10;
    }

    // Update is called once per frame
    void Update()
    {
        navAgent.SetDestination(leader.transform.position);
        // TODO: Interaction and Infection checks.
    }

    //
    private void OnCollisionEnter(Collision other) {
        bool environmentCheck = ((other.gameObject.tag != "Spawner") && (other.gameObject.name != "Map"));
        if (environmentCheck && !(transform.IsChildOf(other.transform))) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            if (leadScript != null && leadScript.GetInstanceID() > GetInstanceID()) {
                leadManager.TrackInteraction(other);
                rend.material.color = new Color(1, 1, 1, 1);
                other.gameObject.GetComponent<AgentManager>().rend.material.color = new Color(1, 1, 1, 1);
            } else if (followScript != null && followScript.GetInstanceID() > GetInstanceID()) {
                leadManager.TrackInteraction(other);
                rend.material.color = new Color(1, 1, 1, 0.5f);
                other.gameObject.GetComponent<FollowAgentManager>().rend.material.color = new Color(1, 1, 1, 0.5f);
            }

        }
    }


}
