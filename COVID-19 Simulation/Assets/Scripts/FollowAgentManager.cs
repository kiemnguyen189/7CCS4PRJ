using UnityEngine;
using UnityEngine.AI;

// Manager class for all human follower agents. Follower agents are sub-agents that follow the main agent around.
public class FollowAgentManager : MonoBehaviour
{

    private SimManager manager;

    public GameObject leader;           // The leading agent.
    public AgentManager leadManager;    // The script component of the leading agent.
    public NavMeshAgent navAgent;       // The navigation mesh agent component that handles the A* pathfinding around the map.
    public AgentType agentType;         // The current agent type.
    private int typeInt;                // Integer representing the agent type.

    public bool isInfected;             // Whether or not this follower agent is infected or not.
    public float timeAlive;             // The total amount of time this agent has spent in the system.

    // Main agent components.
    public Collider coll;
    public Renderer rend;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Leader initialization.
        leader = gameObject.transform.parent.gameObject;
        leadManager = leader.GetComponent<AgentManager>();

        gameObject.tag = leadManager.tag;
        color = leadManager.GetColor();
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
    private void FixedUpdate() {

        timeAlive += Time.deltaTime;
        // If this agent and the lead agent does not have a valid path, disable the collider and rendering components.
        if (navAgent.pathPending && leadManager.navAgent.pathPending) {
            coll.enabled = false;
            rend.enabled = false;
        } else if (!navAgent.pathPending) {
            coll.enabled = true;
            rend.enabled = true;
            navAgent.SetDestination(leader.transform.position);
        }
        // Set the colour of this agent according to its infection state.
        if (!isInfected) {
            color = (Color)manager.GetAgentBlueprint()[typeInt, 2];
            rend.material.color = color;
        } 

        
    }
    
    // Retrieves and sets the colour of this agent.
    public Color GetColor() { return color; }
    public void SetColor(Color col) { rend.material.color = col;}

    // Retrieves the infection state of this agent.
    public bool GetInfection() { return isInfected; }
    // Infects this agent, sets the colour and tracks the infection.
    public void SetInfection(Collision other) { 
        isInfected = true; 
        color = (Color)manager.GetAgentBlueprint()[typeInt, 3];
        rend.material.color = color;
        leadManager.AddGroupInfection();
        TrackInfection(other);
    }


    // Collision detection code to count interactions between different agents.
    // Agents within the same group interacting with each other do not contribute to contact counts.
    // This is to model family and friend groups interactions with other groups.
    private void OnCollisionEnter(Collision other) {
        // * Check if the other object is not the map and not a spawner object.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") & (other.gameObject.name != "Ground"));                         
        if (environmentCheck && (timeAlive >= 0.5f) && (other.gameObject.GetInstanceID() > gameObject.GetInstanceID())) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            Interact(other, leadScript, followScript);
        }
    }

    // The main interaction method between agents in the system.
    public void Interact(Collision other, AgentManager lead, FollowAgentManager follow) {
        // Infection chance.
        bool successful = (Random.Range(0, 100) < manager.GetInfectionChance());
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

    // Tracks the infection interaction that has occurred between this agent and another agent.
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
    }


}
