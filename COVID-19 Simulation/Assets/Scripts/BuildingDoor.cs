using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Enumerated types of doorway nodes.
public enum DoorType {
    Entrance,
    Exit,
    Both,
    None
}

// Manager for each doorway node of a building.
public class BuildingDoor : MonoBehaviour
{
    
    public BuildingManager core;    // Central BuildingManager of the whole building.
    public Material mat;            // Material of the doorway node.
    public TextMeshPro text;        // Label of teh doorway node.
    public DoorType doorType;       // Current enumerated type of the doorway node.
    
    public Renderer rend;           // Renderer.
    public Color color;             // Doorway node colour.
    public int respawnTime = 10;    // Max respawn time of an agent entering a building.

    // Start is called before the first frame update.
    private void Start() {
        rend = GetComponent<Renderer>();
    }

    // Reformats the visual representation of a door.
    public void UpdateFormat() {
        var textRot = transform.rotation.eulerAngles;

        // Initialize the doorway node type.
        switch (doorType) {
            case DoorType.Entrance: SetDoorType("Entrance", new Color(0,1,0,0.5f), new Color(0,1,0,1), ">"); break;
            case DoorType.Exit:     SetDoorType("Exit",     new Color(1,0,0,0.5f), new Color(1,0,0,1), "<"); break;
            case DoorType.Both:     SetDoorType("Both",     new Color(0,0,1,0.5f), new Color(0,0,1,1), "="); break;
            case DoorType.None:     SetDoorType("None",     new Color(0,0,0,0.5f), new Color(0,0,0,1), "x"); break;
        }
        rend.material.color = color;    // Set colour of the doorway node.
    }

    // Sets the formats of a door depending on the chosen door type.
    public void SetDoorType(string tag, Color doorCol, Color textCol, string symbol) {
        gameObject.tag = tag;
        color = doorCol;
        text.faceColor = textCol;
        text.SetText(symbol);
    }

    // Resets the format of a door to the default.
    public void ResetFormat() {
        gameObject.tag = "Untagged";
        rend.material.color = new Color(1,1,1,0.5f);
        text.faceColor = new Color(0,0,0,1);
        text.SetText("o");
    }

    // Toggles the visibility of the directional symbol above a door.
    public void LabelToggle(bool b) {
        if (b) { text.gameObject.SetActive(true); } 
        else { text.gameObject.SetActive(false); }
    }

    // Toggles the visibility of the collider sphere of a door.
    public void ColorToggle(bool b) {
        if (b) { rend.enabled = true; } 
        else { rend.enabled = false; }
    }
    
    // Called when a Shopper collides with Entrance door 'sphere'.
    //private void OnCollisionEnter(Collision other) {
    private void OnTriggerEnter(Collider other) {
        // Check if current door is part of the list of destinations for each agent.
        // * Only checks for the leader agent. Follower agents enter building when leader enters building.
        AgentManager agent = other.GetComponent<AgentManager>();
        // Checks for valid Entry.
        if (agent != null) {
            // * Check if the current destination of the agent is the current door.
            bool targetCheck = (agent.destinations[0] == gameObject.transform);
            // * Check if the current door is not an Exit door.
            bool typeCheck = (doorType != DoorType.Exit);
            // * Check if the agent is of type shopper (group or not).
            bool groupCheck = (agent.agentType == AgentType.Shopper || agent.agentType == AgentType.GroupShopper);
            // * Are all three conditions satisfied?
            if (targetCheck && typeCheck && groupCheck) {
                StartCoroutine(RecreateShopper(other.gameObject));
            }
        }

    }

    // Disables a Shopper for a short amount of time, then recreates it a few seconds later.
    IEnumerator RecreateShopper(GameObject shopper) {
        if (shopper != null) {
            core.AddShopper(shopper);                                           // Add the shopper to the building's internal list of Shoppers
            shopper.GetComponent<AgentManager>().GatherFollowers();             // Gather all followers of the main Shopper agent.
            shopper.gameObject.SetActive(false);                                // Disable the agent.
            yield return new WaitForSeconds(Random.Range(1, respawnTime));      // Keep agent disabled for a set duration.
            Vector3 spawnPos = core.ReturnRandomDoor("Exit").position;          // Reposition the agent to an Exit doorway node.
            spawnPos.y = shopper.transform.position.y;                          // Keep the y position constant.
            shopper.transform.position = spawnPos;                              // Respawn position.
            shopper.gameObject.SetActive(true);                                 // Re-enable the agent.
            shopper.GetComponent<AgentManager>().UpdateDestinations();          // Remove the current doorway node from the agent's list of destinations.
            core.RemoveShopper(shopper);                                        // Remove the Shopper agent from the building's internal list.
        }
        
    }
    

}
