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
    public TextMeshPro arrow;

    private void Start() {
        rend = GetComponent<Renderer>();
        arrow = GetComponent<TextMeshPro>();
        var textRot = transform.rotation.eulerAngles;
        switch (doorType) {
            case DoorType.Entrance: 
                color = new Color(0,1,0,0.5f);
                arrow.color = new Color(0,1,0,1);
                //textRot.y = 90;
                //arrow.transform.rotation = Quaternion.Euler(textRot);
                break;
            case DoorType.Exit:
                color = new Color(1,0,0,0.5f);
                //textRot.y = 270;
                //arrow.transform.rotation = Quaternion.Euler(textRot);
                break;
            case DoorType.Both: 
                color = new Color(0,0,1,0.5f);
                break;
        }
        rend.material.color = color;
        
    }
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("ENTERED!");
        Debug.Log("" + other.gameObject.name);
        // TODO: Instead of Destroy, store the agents somewhere, whilst keeping agent states consistent.
        switch (doorType) {
            case DoorType.Entrance:
                core.AddTourist(other.gameObject);
                StartCoroutine(RecreateTourist(other.gameObject));
                break;
            case DoorType.Exit:
                break;
            case DoorType.Both:
                core.AddTourist(other.gameObject);
                StartCoroutine(RecreateTourist(other.gameObject));
                break;
        }
        
    }

    // Disables a Tourist for a short amount of time, then recreates it a few seconds later 10 units away in x and z direction
    IEnumerator RecreateTourist(GameObject tourist) {
        tourist.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        // TODO: Change this position to a door with type Exit
        tourist.transform.position = tourist.transform.position + new Vector3(-10, 0, -10);
        tourist.gameObject.SetActive(true);
        core.RemoveTourist(tourist.gameObject);
        
    }
    

}
