using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAgentManager : MonoBehaviour
{

    private SimManager manager;

    public GameObject leader;
    public AgentManager leadManager;
    public NavMeshAgent navAgent;
    public AgentType agentType;
    private int typeInt;

    public bool isInfected;
    public float timeAlive;

    public Transform hit;
    public Transform infectHit;

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
        // if (leader.tag == "GroupShopper") {
        //     color = new Color(1,0,1,0.5f);
        //     gameObject.tag = "GroupShopper";
        // } else if (leader.tag == "GroupCommuter") {
        //     color = new Color(0,1,1,0.5f);
        //     gameObject.tag = "GroupCommuter";
        // }

        gameObject.tag = leadManager.tag;
        color = leadManager.GetColor();
        //color.a = 0.5f;

        rend = GetComponent<Renderer>();
        rend.material.color = color;
        
        isInfected = leadManager.GetInfection();
        typeInt = leadManager.GetTypeInt();

        // * Radius initialization.
        float radius = leadManager.GetRadius();
        Vector3 scaleChange = new Vector3(radius, 0, radius);
        gameObject.transform.GetChild(0).localScale += scaleChange;

        // * Speed initialization.
        navAgent.speed = leadManager.GetMaxSpeed();
        navAgent.angularSpeed = leadManager.GetMaxSpeed()*10;
        navAgent.acceleration = leadManager.GetMaxSpeed()*10;
        NavMesh.avoidancePredictionTime = 0.5f;
    }

    //
    private void Update() {

        timeAlive += Time.deltaTime;

        if (!isInfected) {
            color = (Color)manager.GetAgentBlueprint()[typeInt, 2];
            rend.material.color = color;
        } 

        navAgent.SetDestination(leader.transform.position);
    }
    
    //
    public Color GetColor() { return color; }
    public void SetColor(Color col) { rend.material.color = col;}

    //
    public bool GetInfection() { return isInfected; }
    public void SetInfection(Collision other) { 
        isInfected = true; 
        color = (Color)manager.GetAgentBlueprint()[typeInt, 3];
        rend.material.color = color;
        leadManager.AddGroupInfection();
        //leadManager.TrackInfection(other);
        TrackInfection(other);
    }


    // Collision detection code to count interactions between different agents.
    // Agents within the same group interacting with each other do not contribute to contact counts.
    // This is to model family and friend groups interactions with other groups.
    private void OnCollisionEnter(Collision other) {
        // TODO: infection only counts.
        // * Check if the other object is not the map and not a spawner object.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") & (other.gameObject.name != "Ground"));  
        // * Check if other object is not the parent of this object.
        bool parentCheck = (transform.IsChildOf(other.transform));                 
        // * Check if siblings have the same parent.                         
        bool siblingCheck = (transform.parent == other.transform.parent);     
        //if (environmentCheck && !parentCheck && !siblingCheck) {                          
        if (environmentCheck && (timeAlive >= 0.5f) && (other.gameObject.GetInstanceID() > gameObject.GetInstanceID())) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            //manager.PauseSim();
            Interact(other, leadScript, followScript);
        }
    }

    // Params = other collider.
    public void Interact(Collision other, AgentManager lead, FollowAgentManager follow) {
        // Infection chance.
        bool successful = (Random.Range(0, 100) < manager.GetInfectionChance());
        // Track contacts with other agents not in the same group.
        //if (!(transform.IsChildOf(other.transform))) { leadManager.TrackContact(other); }
        // If interacting with another lead agent.
        if (lead != null) {
            // If THIS agent is infected and the OTHER lead agent is not and within infection chance, infect OTHER lead agent.
            if ((isInfected && !lead.GetInfection()) && successful) { lead.SetInfection(other); }
            // If the OTHER lead agent is infected and THIS agent is not and within infection chance, infect THIS follower agent.
            else if ((lead.GetInfection() && !isInfected) && successful) { SetInfection(other); }
        } 
        // Else interacting with another follower agent.
        else if (follow != null) {
            // If THIS agent is infected and the OTHER follow agent is not and within infection chance, infect OTHER follower agent.
            if ((isInfected && !follow.GetInfection()) && successful) { follow.SetInfection(other); }
            // If the OTHER follow agent is infected and THIS agent is not and within infection chance, infect THIS follower agent.
            else if ((follow.GetInfection() && !isInfected) && successful) { SetInfection(other); }
        }
    }

    //
    public void TrackInfection(Collision other) {
        Vector3 tempPoint = other.contacts[0].point;
        tempPoint.y = 1;
        // Check if collision point is within a despawn node. If not, count infection.
        // This is due to the fact that there are possibilities that an infection can occur whilst an agent is being despawned.
        // This is known to adversely affect the accuracy of the infection counts.
        if (!leadManager.GetCurrentDestination().GetComponent<Collider>().bounds.Contains(tempPoint)) {
            manager.AddInfectiousContactNum();
            manager.AddInfectionLocations(tempPoint);
        }
        //Transform iDot = Instantiate(infectHit, tempPoint, Quaternion.identity);  // TODO: Only show contact points visually at the end of the simulation.
    }


}
