using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgentManager : MonoBehaviour
{

    public GameObject manager;

    public GameObject leader;
    public NavMeshAgent agent;

    public Vector3 leaderPos;

    //private Transform target;

    private Renderer rend;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        leader = gameObject.transform.parent.gameObject;
        // TODO: Set color of child.
        // TODO: Set tag of child.
        if (leader.tag == "GroupShopper") {
            color = new Color(1,0,1,0.5f);
            gameObject.tag = "GroupShopper";
        } else if (leader.tag == "GroupCommuter") {
            color = new Color(0,1,1,0.5f);
            gameObject.tag = "GroupCommuter";
        }

        rend = GetComponent<Renderer>();
        rend.material.color = color;

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(leader.transform.position);
    }
}
