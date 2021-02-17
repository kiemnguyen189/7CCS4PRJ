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
    
    private Renderer rend;
    private Color color;
    private int respawnTime = 10;


    public void UpdateFormat() {
        rend = GetComponent<Renderer>();
        var textRot = transform.rotation.eulerAngles;

        switch (doorType) {
            case DoorType.Entrance: 
                gameObject.tag = "Entrance";
                color = new Color(0,1,0,0.5f);
                text.faceColor = new Color(0,1,0,1);
                textRot.z = 0;
                text.transform.Rotate(0,0,0);
                break;
            case DoorType.Exit:
                gameObject.tag = "Exit";
                color = new Color(1,0,0,0.5f);
                text.faceColor = new Color(1,0,0,1);
                textRot.y = 180;
                text.transform.Rotate(0,0,180);
                break;
            case DoorType.Both: 
                gameObject.tag = "Both";
                color = new Color(0,0,1,0.5f);
                text.faceColor = new Color(0,0,1,1);
                text.SetText("=");
                break;
            case DoorType.None:
                gameObject.tag = "None";
                color = new Color(0,0,0,0.5f);
                text.faceColor = new Color(0,0,0,1);
                text.SetText("x");
                break;
        }
        rend.material.color = color;
    }

    public void TextToggle(bool b) {
        if (b) { text.gameObject.SetActive(true);
        } else { text.gameObject.SetActive(false);
        }
    }

    public void ColorToggle(bool b) {
        if (b) { rend.enabled = true;
        } else { rend.enabled = false;
        }
    }
    
    // Called when a Shopper collides with Entrance door 'sphere'.
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
        shopper.gameObject.SetActive(false);
        int res = Random.Range(1, respawnTime);
        yield return new WaitForSeconds(res);
        shopper.transform.position = core.ReturnRandomDoor("Exit").position;
        shopper.gameObject.SetActive(true);
        shopper.GetComponent<AgentManager>().UpdateDestinations();
        core.RemoveShopper(shopper);
    }
    

}
