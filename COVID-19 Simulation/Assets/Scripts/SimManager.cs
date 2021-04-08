using System;
using System.Collections;
using System.Collections.Generic;
using Random=UnityEngine.Random;
using UnityEngine;

// Enumerated types of building doorway policies.
public enum DoorwayMode {
    OneWay,
    TwoWay,
    Mixed
}

// Main manager class of the simulation.
public class SimManager : MonoBehaviour
{

    public GUIManager guiManager;
    public DataManager dataManager;

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

    // An array of 24 integers, each representing the amount of total pedestrian flow per hour in a day.
    private static int[] flowTimings = new int[] {
        100, 125, 150, 170, 200, 225,           // 00:00 to 05:00
        250, 275, 300, 350, 400, 450,           // 06:00 to 11:00
        500, 600, 700, 800, 1000, 1000,         // 12:00 to 17:00
        900, 700, 500, 350, 250, 150            // 18:00 to 23:00
    };

    public static GameObject[] spawners;        
    public static GameObject[] despawners;      
    public static GameObject[] buildings;       
    
    [Header("Prefabs")]
    public Camera cam;                          
    public Transform dots;                      // Placeholder object to hold all red infection dots on the map at the end of a run.
    public Transform infectHit;                 // Prefab of a red dot marker for infection locations.

    [Header("Settings")]
    public float simTime;                       
    public float countdown = 60f;               // Countdown until data is recorded (records every hour in sim time).
    public bool simStarted = false;             
    public bool simFinished = false;            
    public bool isPaused = false;               
    public bool recorded = false;               // Has data been recorded in the dataManager yet.
    public float simSpeed = 1.0f;               // The current "playback speed" of the simulation, Min = 1/8x, Max = 8x.
    public float simDuration = 1440f;           // The duration of the simulation in raw time. 1440 seconds in real time, equating to 1 day in sim time.
    public bool showBuildingNum = true;         
    public bool showBuildingDoors = true;       

    // * Initial Simulation User settings.
    [Header("Agent Parameters")]
    public float ratioTypes;                    // The spawning ratio of Shopper agents. 100 = Only shoppers, 0 = Only Commuters.
    public float ratioGroups;                   // The spawning ratio of Group agents. 100 = Only groups, 0 = Only singles.
    public float ratioInfected;                 // The spawning ratio of Infected agents. 100 = Only infected, 0 = Infection free.
    public float infectionChance;               // The chance of an infected agent infecting another agent upon contact.
    public int maxGroupSize;                    // The maximum spawning group size of agents.
    public float radiusSize;                    // The Social Distancing radius of each agent. 

    [Header("Private Params")]
    public float maxAgentSpeed;                 
    public int maxDestinations;                 

    [Header("Environment Parameters")]
    public DoorwayMode doorMode;                // The types of Entrances / Exits for each building. Types: [OneWay, TwoWay, Mixed].
    public bool isPedestrianised;               

    // * Live Simulation Metrics.
    [Header("Simulation Metrics")]
    public int totalAgents;                     
    public int totalShoppers;                   
    public int totalSingleShoppers;             
    public int totalGroupShoppers;              
    public int totalCommuters;                  
    public int totalSingleCommuters;            
    public int totalGroupCommuters;             

    public int totalSusceptible;                
    public int totalInfected;                   
    
    public List<Vector3> infectionLocations;    // A list of all infected agent contact and transfers in the environment.
    public int infectiousContacts;              // The Total number of infectious contacts in the simulation run.
    
    
    // Start is called before the first frame update
    void Start()
    {

        guiManager = GameObject.Find("Manager").GetComponent<GUIManager>();
        dataManager = GameObject.Find("Manager").GetComponent<DataManager>();

        // Retrieve environment objects.
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        despawners = GameObject.FindGameObjectsWithTag("Despawner");
        buildings = GameObject.FindGameObjectsWithTag("Building");
        
    }

    // Update is called once per frame.
    void Update()
    {
        // Current time in the simulation.
        if (simStarted && !isPaused) {
            simTime += Time.deltaTime;
            countdown -= Time.deltaTime;
        } 

        // Record data every minute (hour in simulation time).
        if (countdown <= 0f) { 
            countdown = 60f;
            RecordData();   
        }

        // If a day has passed, reset the number of infectious contacts.
        if (Mathf.Floor(simTime) % 1440f == 0f) {
            infectiousContacts = 0;
        }  

        // If simulation reached sim length.
        if (simTime > simDuration) {
            RecordData(); 
            simFinished = true;
            // Show metrics when finished.
            guiManager.StartStopSimulation();
            
        }
        
    }

    // Intermediate method to update data structures in DataManager.
    public void RecordData() {
        dataManager.UpdatePopulation(new List<int>() {totalAgents}, true);
        dataManager.UpdateInfections(new List<int>() {infectiousContacts}, false);
        dataManager.UpdateDemographic(new List<int>() {totalSingleShoppers, totalGroupShoppers, totalSingleCommuters, totalGroupCommuters}, true);
        dataManager.UpdateInfected(new List<int>() {totalSusceptible, totalInfected}, true); 
    }

    // Starts a simulation run.
    public void StartSim() {
        simStarted = true;
        simFinished = false;
        guiManager.CloseData();
        dataManager.ResetData();
        RemoveInfectiousContacts();
        ResetMetrics();
    }

    // Stops a simulation run.
    // ! Note: This stop the simulation mid-run and will not display data.
    public void StopSim() {
        simStarted = false;
        if (simFinished) {
            guiManager.ShowData();
            ShowInfectiousContacts();
        }
        ResetMetrics();
        ResetTime();
        
    }

    // Pause and un-pause the simulation.
    public void PauseSim() {
        if (isPaused) {
            isPaused = false;
            Time.timeScale = simSpeed;
        } else {
            isPaused = true;
            Time.timeScale = 0f;
        }
    }

    // Resets the Date and Time of the simulation.
    public void ResetTime() {
        simTime = 0;
    }
    
    // Resets all of the metrics collected.
    public void ResetMetrics() {

        totalAgents = 0;
        totalShoppers = 0;
        totalSingleShoppers = 0;
        totalGroupShoppers = 0;
        totalCommuters = 0;
        totalSingleCommuters = 0;
        totalGroupCommuters = 0;

        totalSusceptible = 0;
        totalInfected = 0;

        infectiousContacts = 0;

        infectionLocations.Clear();

    }

    // * Getter and Setter methods for all simulation parameters and metrics.
    // -----------------------------------------------------------------------------------------------------------------------------

    // Returns the blueprints for creating each agent.
    public object[,] GetAgentBlueprint() { return agentBlueprint; }

    // Returns the hourly pedestrian flow timings in a day.
    public int[] GetFlowTimings() { return flowTimings; }

    // Returns whether or not to show building capacity.
    public bool GetShowBuildingNum() { return showBuildingNum; }
    public void SetShowBuildingNum(bool mode) { showBuildingNum = mode; }

    // Returns whether or not to show building doors.
    public bool GetShowBuildingDoors() { return showBuildingDoors; }
    public void SetShowBuildingDoors(bool mode) { showBuildingDoors = mode; }

    // Returns and Sets the run duration of the simulation.
    public float GetSimDuration() { return simDuration; }
    public void SetSimDuration(float duration) { simDuration = duration*1440; }

    // Returns the and converts the current time in the simulation.
    public float GetSimTime() { return simTime; }

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
    public float GetRatioTypes() { return ratioTypes; }
    public void SetRatioTypes(float ratio) { ratioTypes = ratio; }

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

    // Returns
    public int GetMaxDestinations() { return maxDestinations; }
    public void SetMaxDestinations(int value) { maxDestinations = value; }

    // Returns and Sets the Modes of Entrances and Exits of Buildings in the simulation.
    public DoorwayMode GetDoorMode() { return doorMode; }
    public void SetDoorMode(DoorwayMode mode) { doorMode = mode; }

    //  Returns and Sets whether or not the environment should be pedestrianised.
    public bool GetIsPedestrianised() { return isPedestrianised; }
    public void SetIsPedestrianised(bool mode) { isPedestrianised = mode; }

    // ! --------------------------------------------------------------------------------------------------------------

    // Returns the Total number of agents currently in the simulation.
    public int GetTotalAgents() { return totalAgents; }

    // Returns the Total number of Shopper agents currently in the simulation.
    public int GetTotalShoppers() { return totalShoppers; }
    // Returns the Total number of Group Shopper agents currently in the simulation.
    public int GetTotalSingleShoppers() { return totalSingleShoppers; }
    // Returns the Total number of Group Shopper agents currently in the simulation.
    public int GetTotalGroupShoppers() { return totalGroupShoppers; }

    // Returns the Total number of Commuter agents currently in the simulation.
    public int GetTotalCommuters() { return totalCommuters; }
    // Returns the Total number of Group Commuter agents currently in the simulation.
    public int GetTotalSingleCommuters() { return totalSingleCommuters; }
    // Returns the Total number of Group Commuter agents currently in the simulation.
    public int GetTotalGroupCommuters() { return totalGroupCommuters; }

    // Returns the Total number of Susceptible (Non-Infected) agents currently in the simulation.
    public int GetTotalSusceptible() { return totalSusceptible; }
    // Returns the Total number of Infected agents currently in the simulation.
    public int GetTotalInfected() { return totalInfected; }

    // Retrieves an array of all the spawners in the system.
    public GameObject[] GetSpawners() { return spawners; }

    // Retrieves an array of all the buildings in the system
    public GameObject[] GetBuildings() { return buildings; }

    // -----------------------------------------------------------------------------------------------------------------------------

    // Slow Down the simulation.
    public void SlowDown() {
        float speed = simSpeed;
        if (speed > 0.125f) {
            simSpeed = speed / 2.0f;
            if (!isPaused) { Time.timeScale = simSpeed; }
        }
    }

    // Speed Up the simulation.
    public void SpeedUp() {
        float speed = simSpeed;
        if (speed < 8.0f) {
            simSpeed = speed * 2.0f;
            if (!isPaused) { Time.timeScale = simSpeed; }
        }
    }

    // Increases the number of agents of the respective type as well as total
    public void AddNumAgents(AgentType type, int num, int infected) {
        switch (type) {
            case AgentType.Shopper: totalSingleShoppers += num; totalShoppers += num; break;
            case AgentType.GroupShopper: totalGroupShoppers += num; totalShoppers += num; break;
            case AgentType.Commuter: totalSingleCommuters += num; totalCommuters += num; break;
            case AgentType.GroupCommuter: totalGroupCommuters += num; totalCommuters += num; break;
        }
        totalAgents += num;
        totalInfected += infected; 
        totalSusceptible += (num - infected); 
    }

    // Decreases the number of agents of the respective type as well as total
    public void ReduceNumAgents(AgentType type, int num, int infected) {
        switch (type) {
            case AgentType.Shopper: totalSingleShoppers -= num; totalShoppers -= num; break;
            case AgentType.GroupShopper: totalGroupShoppers -= num; totalShoppers -= num; break;
            case AgentType.Commuter: totalSingleCommuters -= num; totalCommuters -= num; break;
            case AgentType.GroupCommuter: totalGroupCommuters -= num; totalCommuters -= num;  break;
        }
        totalAgents -= num;
        if ((totalInfected - infected) < 0) { totalInfected = 0; } 
        else { totalInfected -= infected; }
        if ((totalSusceptible - (num - infected)) < 0) { totalSusceptible = 0; }
        else { totalSusceptible -= (num - infected); }
    }

    // Iterates the number of Infectious Contacts by 1.
    public int GetInfectiousContactNum() { return infectiousContacts; }
    public void AddInfectiousContactNum() {
        infectiousContacts += 1; 
        if (!(totalInfected + 1 > totalAgents) && !(totalSusceptible - 1 < 0)) {
            totalInfected += 1;
            totalSusceptible -= 1;
        }
    }

    // Returns the total number of spawners in the environment.
    public int GetNumSpawners() { return spawners.Length; }

    // Returns and Adds a location of Infection to the list of Infectious Contacts.
    public List<Vector3> GetInfectionLocations() { return infectionLocations; }
    public void AddInfectionLocations(Vector3 location) { infectionLocations.Add(location); }

    // Shows all of the infectious contact locations.
    public void ShowInfectiousContacts() {
        foreach (Vector3 dot in infectionLocations) {
            Transform d = Instantiate(infectHit, dot, Quaternion.identity);
            d.transform.parent = dots.transform;
        }
    }

    // Removes all of the red dots showing the infection locations off of the map.
    public void RemoveInfectiousContacts() {
        foreach (Transform child in dots) {
            Destroy(child.gameObject);
        }
    }

    // Returns the Transform of the EndNode unique from the StartNode passed in.
    public Transform SetEndNode(Vector3 startNode) {
        List<GameObject> starts = new List<GameObject>(spawners);
        List<GameObject> ends = new List<GameObject>(despawners);
        Vector3 par = new Vector3(0,0,0);
        foreach (GameObject i in starts) {
            if (startNode == i.transform.position) {
                par = i.transform.parent.transform.position;
            }
        }
        foreach (GameObject j in ends) {
            if (j.transform.parent.transform.position == par) {
                ends.Remove(j);
                break;
            }
        }
        int rand = Random.Range(0, ends.Count);
        return ends[rand].transform;
    }

    // Returns a list of building locations that the agent can visit
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
