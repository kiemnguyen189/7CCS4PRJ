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
    public bool showText = false;
    
    private Renderer rend;
    private Color color;
    private int respawnTime = 10;

    private void Start() {
        rend = GetComponent<Renderer>();
        var textRot = transform.rotation.eulerAngles;
        switch (doorType) {
            case DoorType.Entrance: 
                color = new Color(0,1,0,0.5f);
                text.faceColor = new Color(0,1,0,1);
                textRot.z = 0;
                text.transform.Rotate(0,0,0);
                break;
            case DoorType.Exit:
                color = new Color(1,0,0,0.5f);
                text.faceColor = new Color(1,0,0,1);
                textRot.y = 180;
                text.transform.Rotate(0,0,180);
                break;
            case DoorType.Both: 
                color = new Color(0,0,1,0.5f);
                text.faceColor = new Color(0,0,1,1);
                text.SetText("=");
                break;
            case DoorType.None:
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
    
    // Called when a Tourist collides with Entrance door 'sphere'.
    private void OnTriggerEnter(Collider other) {
        if (doorType == DoorType.Entrance && other.GetComponent<AgentManager>().agentType == AgentType.Tourist) {
            StartCoroutine(RecreateTourist(other.gameObject));
        }
    }

    // Called when a Tourist stays inside Both door 'sphere'.
    private void OnTriggerStay(Collider other) {
        AgentManager agent = other.GetComponent<AgentManager>();
        if (doorType == DoorType.Both && agent.agentType == AgentType.Tourist) {
            float otherTimer = agent.UpdateBuildingBufferTime();
            if (otherTimer <= 0f) {
                StartCoroutine(RecreateTourist(other.gameObject));
                agent.ResetBuildingBufferTime();
            }
        }
    }

    // Called when a Tourist leaves the Both door 'sphere' before their buildingBufferTimer runs out.
    private void OnTriggerExit(Collider other) {
        AgentManager agent = other.GetComponent<AgentManager>();
        if (doorType == DoorType.Both && agent.agentType == AgentType.Tourist) {
            agent.ResetBuildingBufferTime();
        }
    }

    // Disables a Tourist for a short amount of time, then recreates it a few seconds later 10 units away in x and z direction.
    IEnumerator RecreateTourist(GameObject tourist) {
        core.AddTourist(tourist);
        tourist.gameObject.SetActive(false);
        // TODO: Choose random time to respawn between ranges.
        int res = Random.Range(1, respawnTime);
        yield return new WaitForSeconds(res);
        tourist.transform.position = core.ReturnExitDoor().position;
        tourist.gameObject.SetActive(true);
        core.RemoveTourist(tourist.gameObject);
    }
    

}
