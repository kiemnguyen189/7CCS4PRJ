using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// * Enumeration values for the 4 unique agent types.
public enum AgentType {
    Shopper,
    Commuter,
    GroupShopper,
    GroupCommuter
}

// * General manager class for every main agent (not including follower agents).
public class AgentManager : MonoBehaviour
{
    
    private SimManager manager;

    // Agent based variables.
    [Header("Prefabs")]
    public Transform infectHit;
    public Transform followerPrefab;
    public NavMeshAgent navAgent;

    [Header("Agent Variables")]
    public AgentType agentType;
    public int typeInt;
    public bool isInfected;
    public int groupSize;
    public int groupInfected;
    public float minSpeed;
    public float maxSpeed;
    public float radius;
    public float timeAlive;

    // Location based variables.
    [Header("Geospatial variables")]
    private Vector3 startNode;
    private Transform endNode;
    public Transform currentDestination;
    public List<Transform> destinations;
    
    
    // Building based variables.
    private static float baseBuildingBufferTime = 2;
    private float buildingBufferTimer = baseBuildingBufferTime;

    // Agent Components.
    public Collider coll;
    public Renderer rend;
    public Color color;

    // Start is called before the first frame update
    void Start() {

        timeAlive = 0;

        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Agent spawning initialization.
        List<int> types = new List<int>() {0, 1, 2, 3};
        if (!(Random.Range(0, 100) < manager.GetRatioGroups())) { 
            types.RemoveRange(2, 2); // Single = [0, 1]
            groupSize = 1;
        } else { 
            types.RemoveRange(0, 2); // Groups = [2, 3]
            groupSize = Random.Range(2, manager.GetMaxGroupSize());
        } 
        if (Random.Range(0, 100) < manager.GetRatioTypes()) { typeInt = types[0]; } 
        else { typeInt = types[1]; }
        groupInfected = 0;
        if (Random.Range(0, 100) < manager.GetRatioInfected()) { 
            isInfected = true; 
            groupInfected = groupSize;
        }
        
        agentType = (AgentType)manager.GetAgentBlueprint()[typeInt, 0];
        gameObject.tag = (string)manager.GetAgentBlueprint()[typeInt, 1];
        if (!isInfected) { color = (Color)manager.GetAgentBlueprint()[typeInt, 2]; } 
        else { color = (Color)manager.GetAgentBlueprint()[typeInt, 3]; }
        maxSpeed = manager.GetMaxAgentSpeed() * (float)manager.GetAgentBlueprint()[typeInt, 4];
        minSpeed = maxSpeed / 2;

        // * Spawning of followers if agent is either GroupShopper or GroupCommuter.
        if (groupSize > 1) {
            StartCoroutine(SpawnFollowers());
        }

        // * NavMeshAgent movement stats.
        navAgent.speed = Random.Range(minSpeed, maxSpeed);
        navAgent.angularSpeed = maxSpeed*10;
        navAgent.acceleration = maxSpeed*10;
        NavMesh.avoidancePredictionTime = 0.5f;

        // * Radius initialization.
        radius = manager.GetRadiusSize();
        Vector3 scaleChange = new Vector3(radius, 0, radius);
        gameObject.transform.GetChild(0).localScale += scaleChange;
        navAgent.radius = radius / 2;

        // * Destinations of agent.
        startNode = gameObject.transform.position;
        destinations = new List<Transform>();
        int numDest = Random.Range(1, manager.GetMaxDestinations());
        if (agentType == AgentType.Shopper || agentType == AgentType.GroupShopper) {
            destinations = manager.SetDestinations(numDest);
        }
        // Chooses an End node that is NOT the same as the Start node (param).
        endNode = manager.SetEndNode(startNode);
        destinations.Add(endNode);

        // * SimManager metrics.
        manager.AddNumAgents(agentType, groupSize, groupInfected);

        // * Rendering initialization.
        //rend = GetComponent<Renderer>();
        rend.material.color = color;
            
    }
    
    
    // Update is called once per frame
    void FixedUpdate()
    {

        if (navAgent.pathPending) {
            coll.enabled = false;
            rend.enabled = false;
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
            }
        } else if (!navAgent.pathPending) {
            coll.enabled = true;
            rend.enabled = true;
            foreach (Transform child in transform) {
                child.gameObject.SetActive(true);
            }
        }

        // Destination controller.
        if (destinations.Count != 0) {
            currentDestination = destinations[0];
            navAgent.SetDestination(currentDestination.position);
        }   

        // Colour controller.
        if (!isInfected) {
            color = (Color)manager.GetAgentBlueprint()[typeInt, 2];
            rend.material.color = color;
        } 

        timeAlive += Time.deltaTime;
        // Instance controller.
        if (!manager.simStarted) {
            Destroy(gameObject);
        }

    }


    // Spawn in follower agents if the agent is a Grouped type.
    IEnumerator SpawnFollowers() {

        float spawnDelay = 0.0f;
        int count = 0;
        for (int i = 0; i < groupSize-1; i++) {
            // TODO: Edit spawn position so it doesn't spawn inside the parent and bug out.
            Vector3 spawnPos = gameObject.transform.position;
            spawnPos.x += 2;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPos, out hit, 2, 1)) {
                Transform follower = Instantiate(followerPrefab, spawnPos, gameObject.transform.rotation);
                follower.transform.parent = gameObject.transform;
                count += 1;
            } else { Debug.Log("Follower Failed."); }
            yield return new WaitForSeconds(spawnDelay);
        }
        groupSize = count + 1;

    }

    //
    public void GatherFollowers() {
        foreach (Transform child in transform) {
            if (child.name != "Radius") {
                child.position = transform.position;
            }
        }
    }

    // Returns an integer corresponding to one of four agent types.
    public int GetTypeInt() { return typeInt; }

    // Returns the maximum movement speed of the agent.
    public float GetMaxSpeed() { return maxSpeed; }

    // Returns the Object Avoidance Radius (Social Distancing Radius) of the agent.
    public float GetRadius() { return radius; }

    // Returns and sets the colour of an agent.
    public Color GetColor() { return color; }
    public void SetColor(Color col) { rend.material.color = col; }

    // Returns and sets the infection status of an agent.
    public bool GetInfection() { return isInfected; }
    public void SetInfection(Collision other) { 
        isInfected = true;
        color = (Color)manager.GetAgentBlueprint()[typeInt, 3];
        rend.material.color = color;
        groupInfected += 1;
        TrackInfection(other); 
    }

    //
    public void AddGroupInfection() { groupInfected += 1; }

    // Returns the current destination of the agent.
    public Transform GetCurrentDestination() { return currentDestination; }

    // Updates the list of destinations each agent has.
    public void UpdateDestinations() { destinations.RemoveAt(0); }

    // Updates the buffer time of an agent, used to decide how long an agent should stay in a building.
    public float UpdateBuildingBufferTime() {
        buildingBufferTimer -= Time.deltaTime;
        return buildingBufferTimer;
    }

    // Resets the building buffer time of an agent.
    public float ResetBuildingBufferTime() {
        buildingBufferTimer = baseBuildingBufferTime;
        return buildingBufferTimer;
    }


    // Detects interactions between agents through collisions.
    // Ensures that only one of the two interacting agents calls the TrackInteraction method.
    // This removes redundant duplicate calls which can affect performance and accuracy of metrics.
    private void OnCollisionEnter(Collision other) {
        //Debug.Log("TEST: " + GetInstanceID() + ", " + gameObject.name + "=" + transform.position + " " + other.gameObject.GetInstanceID() + ", " + gameObject.name + "=" + other.transform.position);
        // * Do not collide with non agents and within-group agents.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") && (other.gameObject.tag != "Despawner") && (other.gameObject.name != "Ground"));
        //if (environmentCheck && !(other.transform.IsChildOf(transform))) {
        if (environmentCheck && (timeAlive >= 0.5f) && (other.gameObject.GetInstanceID() > gameObject.GetInstanceID())) {
            // Get the agent collided with. Can either be another leader or follower.
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            // Interact helper method performs the actual interaction behaviour dynamics between the two agents.
            Interact(other, leadScript, followScript);
        }
        
    }

    // Params = other collider.
    public void Interact(Collision other, AgentManager lead, FollowAgentManager follow) {
        // Infection chance.
        bool successful = (Random.Range(0, 100) < manager.GetInfectionChance());
        // If interacting with another lead agent.
        if (lead != null) {
            // If THIS agent is infected and the OTHER lead agent is not and within infection chance, infect OTHER lead agent.
            if ((isInfected && !lead.GetInfection()) && successful) { lead.SetInfection(other); }
            // If the OTHER lead agent is infected and THIS agent is not and within infection chance, infect THIS lead agent.
            else if ((lead.GetInfection() && !isInfected) && successful) { SetInfection(other); }
        } 
        // Else interacting with another follower agent.
        else if (follow != null) {
            // If THIS agent is infected and the OTHER follow agent is not and within infection chance, infect OTHER follower agent.
            if ((isInfected && !follow.GetInfection()) && successful) { follow.SetInfection(other); }
            // If the OTHER follow agent is infected and THIS agent is not and within infection chance, infect THIS lead agent.
            else if ((follow.GetInfection() & !isInfected) & successful) { SetInfection(other); }
        }
    }


    //
    public void TrackInfection(Collision other) {
        Vector3 tempPoint = other.contacts[0].point;
        tempPoint.y = 1;
        // Check if collision point is within a despawn node. If not, count infection.
        // This is due to the fact that there are possibilities that an infection can occur whilst an agent is being despawned.
        // This is known to adversely affect the accuracy of the infection counts.
        // TODO: Check if iDot is within the bounds of any spawner.
        if (!currentDestination.GetComponent<Collider>().bounds.Contains(tempPoint)) {
            manager.AddInfectiousContactNum();
            manager.AddInfectionLocations(tempPoint);
        }
        
    }

    // If the agent is currently inside a building and the sim ahd been stopped, destroy.
    public void OnDisable() {
        if (!manager.simStarted) {
            Destroy(gameObject);
        }
    }
    

    // Despawn the agent.
    public void Despawn() {
        // Recount infection exit infection numbers.
        groupInfected = 0;
        if (isInfected) { groupInfected += 1; }
        for (int i = 1; i < transform.childCount; i++) {
            if (transform.GetChild(i).GetComponent<FollowAgentManager>().GetInfection()) { groupInfected += 1; }
        }
        manager.ReduceNumAgents(agentType, groupSize, groupInfected);
        Destroy(gameObject);
    }


}
