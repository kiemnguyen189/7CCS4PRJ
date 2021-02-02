using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{

    public SimManager manager;

    public List<GameObject> tourists = new List<GameObject>();
    public List<BuildingDoor> doors = new List<BuildingDoor>();
    public List<BuildingDoor> exits = new List<BuildingDoor>();
    public List<BuildingDoor> entrances = new List<BuildingDoor>();

    public TextMeshPro text;

    public string touristTag = "Tourist";
    public int range = 3;

    public bool showText = true;
    
    // Start is called before the first frame update
    void Start() {

        // Add all doors to a list
        foreach (Transform child in transform) {
            if (child.tag != "BuildingComponent") {
                doors.Add(child.GetComponent<BuildingDoor>());
            }
        }

        Debug.Log(manager.doorMode);

        switch (manager.doorMode) {
            case DoorwayMode.OneWay:
                Debug.Log("ONEWAY");
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
                Debug.Log("TWOWAY");
                foreach (BuildingDoor door in doors) {
                    door.doorType = DoorType.Both;
                }
                break;
            case DoorwayMode.Mixed:
                Debug.Log("MIXED");
                foreach (BuildingDoor door in doors) {
                    door.doorType = (DoorType)Random.Range(0, 3);
                }
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

    public void AddTourist(GameObject tourist) {
        tourists.Add(tourist);
    }

    public void RemoveTourist(GameObject tourist) {
        tourists.Remove(tourist);
    }    

    public Transform ReturnExitDoor() {
        List<BuildingDoor> temp = new List<BuildingDoor>();
        foreach (BuildingDoor door in doors) {
            if (door.doorType.ToString().Equals(DoorType.Exit.ToString()) || door.doorType.ToString().Equals(DoorType.Both.ToString())){
                temp.Add(door);
            }
        }
        Transform ret = temp[Random.Range(0, temp.Count)].transform;
        //temp.Clear();
        return ret;
    }

    IEnumerator UpdateText() {
        text.SetText("" + tourists.Count);
        yield return new WaitForSeconds(1);
    }

}