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
    
    [Header("Prefabs")]
    public Camera cam;

    [Header("Settings")]
    public bool simStarted = false;
    public bool isPaused = false;
    public float simSpeed = 1.0f;

    // * Initial Simulation User settings.
    [Header("Agent Parameters")]
    public float ratioShoppers;
    public float ratioGroups;
    public float ratioInfected;
    public float infectionChance;
    public int maxGroupSize;
    public float radiusSize;
    public float maxAgentSpeed;

    [Header("Environment Parameters")]
    public DoorwayMode doorMode;

    // * Live Simulation Metrics.
    [Header("Simulation Metrics")]
    public int totalAgents;
    public int totalShoppers;
    public int totalCommuters;
    public int totalGroupShoppers;
    public int totalGroupCommuters;

    public int totalSusceptible;
    public int totalInfected;
    public int totalGroupSusceptible;
    public int totalGroupInfected;

    public static GameObject[] spawners;
    public static GameObject[] buildings;
    public List<Vector3> contactLocations;
    public List<Vector3> infectionLocations;
    public int totalContacts;
    public int infectiousContacts;
    
    
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

    // Iterates the number of Total Contacts by 1.
    public int GetTotalContactsNum() { return totalContacts; }
    public void AddTotalContactNum() { totalContacts += 1; }

    // Iterates the number of Infectious Contacts by 1.
    public int GetInfectiousContactNum() { return infectiousContacts; }
    public void AddInfectiousContactNum() { infectiousContacts += 1; }

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

    // Increases the number of agents of the respective type as well as total
    public void AddTotalAgents(AgentType type, int num) {
        switch (type) {
            case AgentType.Shopper: totalShoppers += num; break;
            case AgentType.Commuter: totalCommuters += num; break;
            case AgentType.GroupShopper: totalGroupShoppers += num; break;
            case AgentType.GroupCommuter: totalGroupCommuters += num; break;
        }
        totalAgents += num;
        // ? Debug.Log("+TOTAL: "+ totalAgents +" || S: "+ totalShoppers +", GS: "+ totalGroupShoppers +" | C: "+ totalCommuters +", GC: "+ totalGroupCommuters);
    }

    // Decreases the number of agents of the respective type as well as total
    public void ReduceTotalAgents(AgentType type, int num) {
        switch (type) {
            case AgentType.Shopper: totalShoppers -= num; break;
            case AgentType.Commuter: totalCommuters -= num; break;
            case AgentType.GroupShopper: totalGroupShoppers -= num; break;
            case AgentType.GroupCommuter: totalGroupCommuters -= num; break;
        }
        totalAgents -= num;
        // ? Debug.Log("-TOTAL: "+ totalAgents +" || S:"+ totalShoppers +", GS:"+ totalGroupShoppers +" | C:"+ totalCommuters +", GC:"+ totalGroupCommuters);
    }


}
