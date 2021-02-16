using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{

    public GameObject manager;

    public List<GameObject> shoppers = new List<GameObject>();
    public List<BuildingDoor> doors = new List<BuildingDoor>();

    public TextMeshPro text;
    public bool showText = true;
    
    // Start is called before the first frame update
    void Start() {

        // TODO: Set manager instance.

        // Add all doors to a list
        foreach (Transform child in transform) {
            if (child.tag != "BuildingComponent") {
                doors.Add(child.GetComponent<BuildingDoor>());
            }
        }

        switch (manager.GetComponent<SimManager>().doorMode) {
            case DoorwayMode.OneWay:
                int rand = Random.Range(0, 2);
                if (rand == 0) {
                    // Entrance NS, Exit EW
                    doors[0].doorType = DoorType.Entrance;
                    doors[1].doorType = DoorType.Entrance;
                    doors[2].doorType = DoorType.Exit;
                    doors[3].doorType = DoorType.Exit;
                } else {
                    // Entrance EW, Exit NS
                    doors[0].doorType = DoorType.Exit;
                    doors[1].doorType = DoorType.Exit;
                    doors[2].doorType = DoorType.Entrance;
                    doors[3].doorType = DoorType.Entrance;
                }
                break;
            case DoorwayMode.TwoWay:
                foreach (BuildingDoor door in doors) {
                    door.doorType = DoorType.Both;
                }
                break;
            case DoorwayMode.Mixed:
                foreach (BuildingDoor door in doors) {
                    door.doorType = (DoorType)Random.Range(0, doors.Count - 1);
                }
                // Randomly change one of the doors to type Both as assurance.
                doors[Random.Range(0, doors.Count)].doorType = DoorType.Both;
                break;
        }  

        foreach (BuildingDoor door in doors) {
            door.UpdateFormat();
        }       
        
    }

    private void FixedUpdate() {
        if (showText) {
            StartCoroutine(UpdateText());
            text.gameObject.SetActive(true);
            foreach (BuildingDoor door in doors) {
                door.TextToggle(true);
                door.ColorToggle(true);
            }
        } else {
            text.gameObject.SetActive(false);
            foreach (BuildingDoor door in doors) {
                door.TextToggle(false);
                door.ColorToggle(false);
            }
        }
    }

    public void AddShopper(GameObject shopper) {
        // TODO: Include number of agents in a group.
        shoppers.Add(shopper);
    }

    public void RemoveShopper(GameObject shopper) {
        // TODO: Include number of agents in a group.
        shoppers.Remove(shopper);
    }


    // Returns a random door, depending or the type parameter. Will either return Entrance/Both or Exit/Both.
    public Transform ReturnRandomDoor(string type) {
        List<BuildingDoor> temp = new List<BuildingDoor>();
        foreach (BuildingDoor door in doors) {
            if (door.doorType.ToString().Equals(type) || door.doorType.ToString().Equals(DoorType.Both.ToString())){
                temp.Add(door);
            }
        }
        return temp[Random.Range(0, temp.Count)].transform;
    }

    IEnumerator UpdateText() {
        text.SetText("" + shoppers.Count);
        yield return new WaitForSeconds(1);
    }

}