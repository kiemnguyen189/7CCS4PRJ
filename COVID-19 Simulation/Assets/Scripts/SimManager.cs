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

    public float ratioShoppers;
    public float ratioCommuters;    // ! Maybe don't need this if only using ratioShopper
    public float ratioGroupShoppers;
    public float ratioGroupCommuters;
    public DoorwayMode doorMode;
    public int maxGroupSize;
    public float maxAgentSpeed;
    public float radiusSize;

    public static int totalAgents;
    public static int totalShoppers;
    public static int totalCommuters;
    public static int totalGroupShoppers;
    public static int totalGroupCommuters;

    private int totalSusceptible;
    private int totalInfected;
    private int totalGroupSusceptible;
    private int totalGroupInfected;

    public static GameObject[] spawners;
    public static GameObject[] buildings;
    public static Transform[] temp;
    public int contacts;
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        buildings = GameObject.FindGameObjectsWithTag("Building");
        
        ResetMetrics();
        //Debug.Log(maxGroupSize);
        //Debug.Log(doorMode);

    }

    //
    private void ResetMetrics() {
        totalAgents = 0;
        totalShoppers = 0;
        totalCommuters = 0;
    }


    // Update is called once per frame
    void Update()
    {
        // totalAgents = totalAgents;
        // totalShoppers = totalShoppers;
        // totalCommuters = totalCommuters;
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
    public void AddTotalAgents(AgentType type, int num) {
        if (type == AgentType.Shopper) {
            totalShoppers += num;
            totalAgents += num;
        } else if (type == AgentType.GroupShopper) {
            totalGroupShoppers += num;
            totalAgents += num;
        } else if (type == AgentType.Commuter) {
            totalCommuters += num;
            totalAgents += num;
        } else {
            totalGroupCommuters += num;
            totalAgents += num;
        }
        //Debug.Log("+TOTAL: "+ totalAgents +" || S: "+ totalShoppers +", GS: "+ totalGroupShoppers +" | C: "+ totalCommuters +", GC: "+ totalGroupCommuters);

    }

    // Decreases the number of agents of the respective type as well as total
    public void ReduceTotalAgents(AgentType type, int num) {
        // Don't have a negative number of agents in the simulation.
        if (type == AgentType.Shopper) {
            totalShoppers -= num;
            totalAgents -= num;
        } else if (type == AgentType.GroupShopper) {
            totalGroupShoppers -= num;
            totalAgents -= num;
        } else if (type == AgentType.Commuter) {
            totalCommuters -= num;
            totalAgents -= num;
        } else {
            totalGroupCommuters -= num;
            totalAgents -= num;
        }
        //Debug.Log("-TOTAL: "+ totalAgents +" || S:"+ totalShoppers +", GS:"+ totalGroupShoppers +" | C:"+ totalCommuters +", GC:"+ totalGroupCommuters);
        
    }

    // Updates the total numbers to Susceptible and Infectious agents.
    public void UpdateSI() {

    }


}
