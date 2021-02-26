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
    public Transform hit;
    public Transform infectHit;
    public Transform followerPrefab;
    public NavMeshAgent navAgent;
    private Transform follower;

    [Header("Agent Variables")]
    public AgentType agentType;
    public int typeInt;
    public bool isInfected;
    public int groupSize;
    public int groupInfected;
    public float minSpeed;
    public float maxSpeed;
    public float radius;

    private int infectedChance;
    private int typeChance;
    private int groupChance;

    // Location based variables.
    [Header("Geospatial variables")]
    private Transform startNode;
    private Transform endNode;
    public int maxDestinations = 5;
    public Transform currentDestination;
    public List<Transform> destinations;
    
    
    // Building based variables.
    private static float baseBuildingBufferTime = 2;
    private float buildingBufferTimer = baseBuildingBufferTime;

    // Rendering variables.
    public Renderer rend;
    public Color color;

    // Start is called before the first frame update
    void Start() {

        // * Manager initialization.
        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // * Agent spawning initialization.
        infectedChance = Random.Range(0, 100);
        typeChance = Random.Range(0, 100);
        groupChance = Random.Range(0, 100);
        List<int> types = new List<int>() {0, 1, 2, 3};
        if (!(groupChance < manager.GetRatioGroups())) { 
            types.RemoveRange(2, 2); // Single = [0, 1]
            groupSize = 1;
        } else { 
            types.RemoveRange(0, 2); // Groups = [2, 3]
            groupSize = Random.Range(2, manager.GetMaxGroupSize());
        } 
        if (typeChance < manager.GetRatioShoppers()) { typeInt = types[0]; } 
        else { typeInt = types[1]; }
        groupInfected = 0;
        if (infectedChance < manager.GetRatioInfected()) { 
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
        navAgent.radius = manager.GetRadiusSize() / 2;

        // * Destinations of agent.
        startNode = gameObject.transform;
        destinations = new List<Transform>();
        int numDest = Random.Range(1, maxDestinations);
        if (agentType == AgentType.Shopper || agentType == AgentType.GroupShopper) {
            destinations = manager.SetDestinations(numDest);
        }
        endNode = manager.SetEndNode(startNode);
        destinations.Add(endNode);

        // * SimManager metrics.
        manager.AddNumAgents(agentType, groupSize, groupInfected);


        // * Rendering initialization.
        rend = GetComponent<Renderer>();
        rend.material.color = color;
            
    }

    private void FixedUpdate() {
        
        if (destinations.Count != 0) {
            currentDestination = destinations[0];
            navAgent.SetDestination(currentDestination.position);
        }   

        if (!isInfected) {
            color = (Color)manager.GetAgentBlueprint()[typeInt, 2];
            rend.material.color = color;
        } else {
            color = (Color)manager.GetAgentBlueprint()[typeInt, 3];
            rend.material.color = color;
        }

    }
    
    
    // Update is called once per frame
    void Update()
    {
        // ! Temporary movement for all agents using mouse clicks
        // if (Input.GetMouseButtonDown(0)) {
        //     Ray ray = manager.GetComponent<SimManager>().cam.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit)) {
        //         agent.SetDestination(hit.point);
        //     }
        // }

    }


    //
    IEnumerator SpawnFollowers() {

        float spawnDelay = 0.5f;
        for (int i = 0; i < groupSize-1; i++) {
            // TODO: Edit spawn position so it doesn't spawn inside the parent and bug out.
            Vector3 spawnPos = gameObject.transform.position;
            spawnPos.x += 2;
            follower = Instantiate(followerPrefab, spawnPos, gameObject.transform.rotation);
            follower.transform.parent = gameObject.transform;
            yield return new WaitForSeconds(spawnDelay);
        }

    }

    // Returns an integer corresponding to one of four agent types.
    public int GetTypeInt() { return typeInt; }

    // Returns and sets the colour of an agent.
    public Color GetColor() { return color; }
    public void SetColor(Color col) { rend.material.color = col; }

    // Returns and sets the infection status of an agent.
    public bool GetInfection() { return isInfected; }
    public void SetInfection(bool infection) { isInfected = infection; }

    // Updates the list of destinations each agent has.
    public void UpdateDestinations() { destinations.RemoveAt(0); }

    // 
    public float UpdateBuildingBufferTime() {
        buildingBufferTimer -= Time.deltaTime;
        return buildingBufferTimer;
    }

    //
    public float ResetBuildingBufferTime() {
        buildingBufferTimer = baseBuildingBufferTime;
        return buildingBufferTimer;
    }

    // Detects interactions between agents through collisions.
    // Ensures that only one of the two interacting agents calls the TrackInteraction method.
    // This removes redundant duplicate calls which can affect performance and accuracy of metrics.
    private void OnCollisionEnter(Collision other) {
        // * Do not collide with non agents and within-group agents.
        bool environmentCheck = ((other.gameObject.tag != "Spawner") && (other.gameObject.name != "Map"));
        //if (environmentCheck && !(other.transform.IsChildOf(transform))) {
        if (environmentCheck) {
            AgentManager leadScript = other.collider.GetComponent<AgentManager>();
            FollowAgentManager followScript = other.collider.GetComponent<FollowAgentManager>();
            if (leadScript != null && leadScript.GetInstanceID() > GetInstanceID()) {
                TrackInteraction(other, isInfected, leadScript.isInfected);
                if (isInfected) { leadScript.SetInfection(isInfected); } 
                else if (leadScript.GetInfection()) { SetInfection(leadScript.GetInfection()); }
            } else if (followScript != null && followScript.GetInstanceID() > GetInstanceID()) {
                TrackInteraction(other, isInfected, followScript.isInfected);
                if (isInfected) { followScript.SetInfection(isInfected); } 
                else if (followScript.GetInfection()) { SetInfection(followScript.GetInfection()); }  
            }

        }
        
    }

    // Helper method used in OnCollisionEnter to track interactions between agents.
    public void TrackInteraction(Collision other, bool infected, bool otherInfected) {
        // TODO: Set "hit" spheres inactive until needed to be shown at the end of the simulation.
        // TODO: only change colors of interacting agents if one is infected.
        ContactPoint contact = other.contacts[0];
        Vector3 tempPoint = contact.point;
        tempPoint.y = 10;   // * Keep the same elevation for contact points.
        manager.AddTotalContactNum();
        manager.AddContactLocations(tempPoint);
        //Transform dot = Instantiate(hit, tempPoint, Quaternion.identity);     // TODO: Only show contact points visually at the end of the simulation.
        if ((infected && !otherInfected) ^ (!infected && otherInfected)) {
            tempPoint.y = 20;
            manager.AddInfectiousContactNum();
            manager.AddInfectionLocations(tempPoint);
            
            Transform iDot = Instantiate(infectHit, tempPoint, Quaternion.identity);
        }
        
    }

    

    //
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
