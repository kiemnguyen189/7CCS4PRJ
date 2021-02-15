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
        //leaderPos = leader.transform.position;
        //Debug.Log(leader.name);
        //leader = GameObject.Find().GetComponent<Transform>();
        //gameObject.transform.SetParent(leader, false);
        // TODO: Set color of child.

    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 pos = leader.position;
        agent.SetDestination(leader.transform.position);
        //agent.SetDestination(gameObject.transform.parent.transform.position);
    }
}
