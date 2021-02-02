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

    private GameObject[] spawners;
    private GameObject[] buildings;
    
    
    // Start is called before the first frame update
    void Start()
    {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        buildings = GameObject.FindGameObjectsWithTag("Building");
        //Debug.Log("" + spawners.Length);
        //Debug.Log("" + spawners[1].name);
    }

    // Update is called once per frame
    void Update()
    {
        
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
