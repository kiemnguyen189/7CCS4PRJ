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
    
    public Camera cam;

    public float ratioTourists;
    public float ratioCommuters;
    public float ratioGroupTourists;
    public float ratioGroupCommuters;
    public DoorwayMode doorMode;

    private int totalAgents = 0;
    private int totalTourists = 0;
    private int totalCommuters = 0;
    private int totalSusceptible = 0;
    private int totalInfected = 0;
    private int totalGroupTourists = 0;
    private int totalGroupCommuters = 0;
    private int totalGroupSusceptible = 0;
    private int totalGroupInfected = 0;

    public static GameObject[] spawners;
    public static GameObject[] buildings;
    public static Transform[] temp;
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        buildings = GameObject.FindGameObjectsWithTag("Building");
        // //temp = spawners.Select(f => f.transform).ToArray();

        // temp = new Transform[spawners.Length];
        // for (int i = 0; i < spawners.Length; i++) {
        //     temp[i] = spawners[i].transform;
        // }

    }


    // Update is called once per frame
    void Update()
    {
        
    }


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
            } else {
                break;
            }
        }
        return ret;
    }

    // Increases the number of agents of the respective type as well as total
    public void AddTotalAgents(AgentType type) {
        if (type == AgentType.Tourist) {
            totalTourists += 1;
            totalAgents += 1;
        } else {
            totalCommuters += 1;
            totalAgents += 1;
        }
    }

    // Decreases the number of agents of the respective type as well as total
    public void ReduceTotalAgents(AgentType type) {
        // Don't have a negative number of agents in the simulation.
        if (totalAgents > 0 && totalTourists > 0 && totalCommuters > 0) {
            if (type == AgentType.Tourist) {
                totalTourists -= 1;
                totalAgents -= 1;
            } else {
                totalCommuters -= 1;
                totalAgents -= 1;
            }
        }
        
    }

    // Updates the total numbers to Susceptible and Infectious agents.
    public void UpdateSI() {

    }


}
