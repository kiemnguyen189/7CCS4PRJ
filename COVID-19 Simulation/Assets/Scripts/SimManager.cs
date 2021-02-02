using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    
    public int totalAgents = 0;
    public int totalTourists = 0;
    public int totalCommuters = 0;
    public int totalSusceptible = 0;
    public int totalInfected = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
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
