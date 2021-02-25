using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DoorwayMode {
    OneWay,
    TwoWay,
    Mixed
}

public class SimManager : MonoBehaviour
{

    // * These are the default values unique to each agent type.
    // * [0, 1, 2, 3] = agent types equivalent to enum values.
    // * [X, 0] = Enum name.
    // * [X, 1] = Tag name.
    // * [X, 2] = Default colour value.
    // * [X, 3] = Infected colour value.
    // * [X, 4] = Agent movement speed multiplier.
    private static object[,] agentBlueprint = new object[,] {
        {AgentType.Shopper, "Shopper", new Color(0,1,1,1), new Color(1,0,1,1), 0.5f},
        {AgentType.Commuter, "Commuter", new Color(0,1,0,1), new Color(1,1,0,1), 1.0f},
        {AgentType.GroupShopper, "GroupShopper", new Color(0,0,1,1), new Color(0.5f,0,1,1), 0.5f},
        {AgentType.GroupCommuter, "GroupCommuter", new Color(0,0.5f,0,1), new Color(1,0,0,1), 1.0f}
    };
    
    [Header("Prefabs")]
    public Camera cam;                          // Scene Camera Object

    [Header("Settings")]
    public bool simStarted = false;             // Whether the simulation has been started or not.
    public bool isPaused = false;               // Whether or not the simulation is currently paused.
    public float simSpeed = 1.0f;               // The current "playback speed" of the simulation, Min = 1/8x, Max = 8x.

    // * Initial Simulation User settings.
    [Header("Agent Parameters")]
    public float ratioShoppers;                 // The spawning ratio of Shopper agents. 100 = Only shoppers, 0 = Only Commuters.
    public float ratioGroups;                   // The spawning ratio of Group agents. 100 = Only groups, 0 = Only singles.
    public float ratioInfected;                 // The spawning ratio of Infected agents. 100 = Only infected, 0 = Infection free.
    public float infectionChance;               // The chance of an infected agent infecting another agent upon contact.
    public int maxGroupSize;                    // The maximum spawning group size of agents.
    public float radiusSize;                    // The Social Distancing radius of each agent. 
    public float maxAgentSpeed;                 // The maximum movement speed of the NavMeshAgents.

    [Header("Environment Parameters")]
    public DoorwayMode doorMode;                // The types of Entrances / Exits for each building. Types: [OneWay, TwoWay, Mixed].

    // * Live Simulation Metrics.
    [Header("Simulation Metrics")]
    public int totalAgents;                     // The Total number of agents currently in the simulation run.
    public int totalShoppers;                   // The Total number of Shopper agents currently in the simulation run.
    public int totalCommuters;                  // The Total number of Commuter agents currently in the simulation run.
    public int totalGroupShoppers;              // The Total number of Group Shopper agents currently in the simulation run.
    public int totalGroupCommuters;             // The Total number of Group Commuters agents currently in the simulation run.

    public int totalSusceptible;                // The Total number of Susceptible (Non-Infected) agents currently in the simulation run.
    public int totalInfected;                   // The Total number of Infected agents currently in the simulation run.

    public static GameObject[] spawners;        // A list of all agent spawners in the environment.
    public static GameObject[] buildings;       // A list of all buildings in the environment.
    public List<Vector3> contactLocations;      // A list of all agent contact/interaction locations in the environment.
    public List<Vector3> infectionLocations;    // A list of all infected agent contact and transfers in the environment.
    public int totalContacts;                   // The Total number of contacts in the simulation run.
    public int infectiousContacts;              // The Total number of infectious contacts in the simulation run.
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        buildings = GameObject.FindGameObjectsWithTag("Building");
        contactLocations = new List<Vector3>();
        
        //ResetMetrics();

    }

    // Update is called once per frame
    void Update()
    {
        // totalAgents = totalAgents;
        // totalShoppers = totalShoppers;
        // totalCommuters = totalCommuters;
    }
    
    //
    private void ResetMetrics() {
        totalAgents = 0;
        totalShoppers = 0;
        totalCommuters = 0;
    }

    // * Getter and Setter methods for all simulation parameters and metrics.
    // -----------------------------------------------------------------------------------------------------------------------------

    public object[,] GetAgentBlueprint() { return agentBlueprint; }

    // Returns and Sets whether or not the Simulation has started or not.
    public bool GetSimStarted() { return simStarted; }
    public void SetSimStarted(bool state) { simStarted = state; }

    // Returns and Sets whether or not the Simulation is paused or not.
    public bool GetIsPaused() { return isPaused; }
    public void SetIsPaused(bool state) { isPaused = state; }

    // Returns and Sets the current Speed of the Simulation.
    public float GetSimSpeed() { return simSpeed; }
    public void SetSimSpeed(float speed) { simSpeed = speed; }

    // Returns and Sets the ratio of Shopper agents to include in the simulation, from 0% to 100%.
    public float GetRatioShoppers() { return ratioShoppers; }
    public void SetRatioShoppers(float ratio) { ratioShoppers = ratio; }

    // Returns and Sets the ratio of Group agents to include in the simulation, from 0% to 100%.
    public float GetRatioGroups() { return ratioGroups; }
    public void SetRatioGroups(float ratio) { ratioGroups = ratio; }

    // Returns and Sets the ratio of Infected agents to include in the simulation, from 0% to 100%.
    public float GetRatioInfected() { return ratioInfected; }
    public void SetRatioInfected(float ratio) { ratioInfected = ratio; }

    // Returns and Sets the Infection chance of agent spawning & interaction infections in the simulation, from 0% to 100%.
    public float GetInfectionChance() { return infectionChance; }
    public void SetInfectionChance(float ratio) { infectionChance = ratio; }

    // Returns and Sets the Maximum group size of Group agents in the simulation.
    public int GetMaxGroupSize() { return maxGroupSize; }
    public void SetMaxGroupSize(int size) { maxGroupSize = size; }

    // Returns and Sets the Radius (Social Distance) of agents in the simulation.
    public float GetRadiusSize() { return radiusSize; }
    public void SetRadiusSize(float size) { radiusSize = size; }

    // Returns and Sets the Maximum Movement Speed of agents in the simulation.
    public float GetMaxAgentSpeed() { return maxAgentSpeed; }
    public void SetMaxAgentSpeed(float speed) { maxAgentSpeed = speed; }

    // Returns and Sets the Modes of Entrances and Exits of Buildings in the simulation.
    public DoorwayMode GetDoorMode() { return doorMode; }
    public void SetDoorMode(DoorwayMode mode) { doorMode = mode; }

    // Returns the Total number of agents currently in the simulation.
    public int GetTotalAgents() { return totalAgents; }

    // Returns the Total number of Shopper agents currently in the simulation.
    public int GetTotalShoppers() { return totalShoppers; }

    // Returns the Total number of Commuter agents currently in the simulation.
    public int GetTotalCommuters() { return totalCommuters; }

    // Returns the Total number of Group Shopper agents currently in the simulation.
    public int GetTotalGroupShoppers() { return totalGroupShoppers; }

    // Returns the Total number of Group Commuter agents currently in the simulation.
    public int GetTotalGroupCommuters() { return totalGroupCommuters; }

    // Returns the Total number of Susceptible (Non-Infected) agents currently in the simulation.
    public int GetTotalSusceptible() { return totalSusceptible; }

    // Returns the Total number of Infected agents currently in the simulation.
    public int GetTotalInfected() { return totalInfected; }

    // -----------------------------------------------------------------------------------------------------------------------------


    // Increases the number of agents of the respective type as well as total
    public void AddNumAgents(AgentType type, int num, int infected) {
        //Debug.Log(infected);
        switch (type) {
            case AgentType.Shopper: totalShoppers += num; break;
            case AgentType.Commuter: totalCommuters += num; break;
            case AgentType.GroupShopper: totalGroupShoppers += num; break;
            case AgentType.GroupCommuter: totalGroupCommuters += num; break;
        }
        totalAgents += num;
        if (infected > 0) { 
            totalInfected += infected; 
            totalSusceptible += (num - infected); 
        } else if (infected == 0) { 
            totalSusceptible += num - infected; 
        }

        // ? Debug.Log("+TOTAL: "+ totalAgents +" || S: "+ totalShoppers +", GS: "+ totalGroupShoppers +" | C: "+ totalCommuters +", GC: "+ totalGroupCommuters);
    }

    // Decreases the number of agents of the respective type as well as total
    public void ReduceNumAgents(AgentType type, int num, int infected) {
        //Debug.Log(infected);
        switch (type) {
            case AgentType.Shopper: totalShoppers -= num; break;
            case AgentType.Commuter: totalCommuters -= num; break;
            case AgentType.GroupShopper: totalGroupShoppers -= num; break;
            case AgentType.GroupCommuter: totalGroupCommuters -= num; break;
        }
        totalAgents -= num;
        if (infected > 0) { 
            totalInfected -= infected; 
            totalSusceptible -= (num - infected);
        } else if (infected == 0) { 
            totalSusceptible -= num - infected; 
        }

        // ? Debug.Log("-TOTAL: "+ totalAgents +" || S:"+ totalShoppers +", GS:"+ totalGroupShoppers +" | C:"+ totalCommuters +", GC:"+ totalGroupCommuters);
    }

    // Iterates the number of Total Contacts by 1.
    public int GetTotalContactsNum() { return totalContacts; }
    public void AddTotalContactNum() { totalContacts += 1; }

    // Iterates the number of Infectious Contacts by 1.
    public int GetInfectiousContactNum() { return infectiousContacts; }
    public void AddInfectiousContactNum() { 
        infectiousContacts += 1; 
        totalInfected += 1;
        totalSusceptible -= 1;
    }

    // Returns and Adds a location of Contact to the list of Total Contacts.
    public List<Vector3> GetContactLocations() { return contactLocations; }
    public void AddContactLocations(Vector3 location) { contactLocations.Add(location); }

    // Returns and Adds a location of Infection to the list of Infectious Contacts.
    public List<Vector3> GetInfectionLocations() { return infectionLocations; }
    public void AddInfectionLocations(Vector3 location) { infectionLocations.Add(location); }


    // Returns the Transform of the EndNode unique from the StartNode passed in.
    public Transform SetEndNode(Transform startNode) {
        List<GameObject> tempEnds = new List<GameObject>(spawners);
        foreach (GameObject i in tempEnds) {
            if (i.transform.position == startNode.position) {
                tempEnds.Remove(i);
                break;
            }
        }
        int rand = Random.Range(0, tempEnds.Count);
        return tempEnds[rand].transform;
    }

    // Returns a list of building locations that the agent can visit
    // 
    public List<Transform> SetDestinations(int numDest) {
        List<GameObject> tempBuildings = new List<GameObject>(buildings);
        List<Transform> ret = new List<Transform>();
        for (int i = 0; i <= numDest; i++) {
            // Randomly select a building
            // Only add one entrance per building
            if (tempBuildings.Count > 0) {
                int rand = Random.Range(0, tempBuildings.Count);
                BuildingManager building = tempBuildings[rand].GetComponent<BuildingManager>();
                tempBuildings.Remove(tempBuildings[rand]);
                ret.Add(building.ReturnRandomDoor("Entrance"));
            } else { break; }
        }
        return ret;
    }

    


}
