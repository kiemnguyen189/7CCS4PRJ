using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingDoor : MonoBehaviour
{
    
    public BuildingManager core;
    public Material mat;
    public DoorType doorType;
    Renderer rend;
    public Color color;
    public TextMeshPro text;
    
    private float bufferTime = 0;

    private void Start() {
        Random rand = new Random();
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
        }
        rend.material.color = color;
        
    }
    
    // Called when a Tourist collides with Entrance door 'sphere'.
    private void OnTriggerEnter(Collider other) {
        if (doorType == DoorType.Entrance) {
            StartCoroutine(RecreateTourist(other.gameObject));
        }
    }

    // Called when a Tourist stays inside Both door 'sphere'
    private void OnTriggerStay(Collider other) {
        if (doorType == DoorType.Both) {
            bufferTime += Time.deltaTime;
            if (bufferTime > 5) {
                bufferTime = 0;
                StartCoroutine(RecreateTourist(other.gameObject));
            }
        }
    }

    // Disables a Tourist for a short amount of time, then recreates it a few seconds later 10 units away in x and z direction
    IEnumerator RecreateTourist(GameObject tourist) {
        core.AddTourist(tourist);
        tourist.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        tourist.transform.position = core.ReturnExitDoor().position;
        tourist.gameObject.SetActive(true);
        core.RemoveTourist(tourist.gameObject);
    }
    

}
