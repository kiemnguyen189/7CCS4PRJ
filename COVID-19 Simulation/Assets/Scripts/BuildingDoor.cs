using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DoorType {
    Entrance,
    Exit,
    Both,
    None
}

public class BuildingDoor : MonoBehaviour
{
    
    public BuildingManager core;
    public Material mat;
    public TextMeshPro text;
    public DoorType doorType;
    
    public Renderer rend;
    public Color color;
    public int respawnTime = 10;


    private void Start() {
        rend = GetComponent<Renderer>();
    }

    // Reformats the visual representation of a door.
    public void UpdateFormat() {
        var textRot = transform.rotation.eulerAngles;

        switch (doorType) {
            case DoorType.Entrance: SetDoorType("Entrance", new Color(0,1,0,0.5f), new Color(0,1,0,1), ">"); break;
            case DoorType.Exit:     SetDoorType("Exit",     new Color(1,0,0,0.5f), new Color(1,0,0,1), "<"); break;
            case DoorType.Both:     SetDoorType("Both",     new Color(0,0,1,0.5f), new Color(0,0,1,1), "="); break;
            case DoorType.None:     SetDoorType("None",     new Color(0,0,0,0.5f), new Color(0,0,0,1), "x"); break;
        }
        rend.material.color = color;
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

    // ! Don't need OnTriggerStay for buffering due to entering checks.
    // // Called when a Shopper stays inside Both door 'sphere'.
    // private void OnTriggerStay(Collider other) {
    //     AgentManager agent = other.GetComponent<AgentManager>();
    //     // TODO: Check if the current door is at the top of the list of Directions.
    //     // ? agent.destinations.Contains(gameObject.transform)
    //     if (agent.destinations[0] == gameObject.transform && doorType == DoorType.Both && agent.agentType == AgentType.Shopper) {
    //         float otherTimer = agent.UpdateBuildingBufferTime();
    //         if (otherTimer <= 0f) {
    //             StartCoroutine(RecreateShopper(other.gameObject));
    //             agent.ResetBuildingBufferTime();
    //         }
    //     }
    // }

    // Called when a Shopper leaves the Both door 'sphere' before their buildingBufferTimer runs out.
    private void OnTriggerExit(Collider other) {
        AgentManager agent = other.GetComponent<AgentManager>();
        if (agent != null && doorType == DoorType.Both && agent.agentType == AgentType.Shopper) {
            agent.ResetBuildingBufferTime();
        }
    }

    // Disables a Shopper for a short amount of time, then recreates it a few seconds later 10 units away in x and z direction.
    IEnumerator RecreateShopper(GameObject shopper) {
        core.AddShopper(shopper);
        //core.UpdateText();
        shopper.GetComponent<AgentManager>().GatherFollowers();
        shopper.gameObject.SetActive(false);
        yield return new WaitForSeconds(Random.Range(1, respawnTime));
        Vector3 spawnPos = core.ReturnRandomDoor("Exit").position;
        spawnPos.y = shopper.transform.position.y;
        shopper.transform.position = spawnPos;
        shopper.gameObject.SetActive(true);
        shopper.GetComponent<AgentManager>().UpdateDestinations();
        core.RemoveShopper(shopper);
        //core.UpdateText();
    }
    

}
