using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{

    public SimManager manager;

    public List<GameObject> shoppers = new List<GameObject>();
    public List<BuildingDoor> doors = new List<BuildingDoor>();

    public TextMeshPro text;
    public bool showOccupancy = true;
    public bool showDoors = true;
    
    // Start is called before the first frame update
    void Start() {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        // Add all doors to a list
        foreach (Transform child in transform) {
            if (child.tag != "BuildingComponent") {
                doors.Add(child.GetComponent<BuildingDoor>());
            }
        }

        // Combine the meshes of all the component cubes that make up the shape of the building into one mesh.
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[6];
        
        for (int i = 0; i < 6; i++) {
            if (meshFilters[i].transform == transform) { continue; }
            combine[i].mesh = meshFilters[i+9].sharedMesh;
            combine[i].transform = meshFilters[i+9].transform.localToWorldMatrix;
            meshFilters[i+9].gameObject.SetActive(false);
        }

        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        transform.rotation = oldRot;
        transform.position = oldPos;

        transform.gameObject.SetActive(true);
        
    }

    //
    private void FixedUpdate() {

        showOccupancy = manager.GetShowBuildingNum();
        showDoors = manager.GetShowBuildingDoors();

        if (showOccupancy) {
            text.gameObject.SetActive(true);
        } else {
            text.gameObject.SetActive(false);
        }

        // TODO: Maybe don't need to be in FixedUpdate.
        if (showDoors) {
            foreach (BuildingDoor door in doors) {
                door.LabelToggle(true);
                door.ColorToggle(true);
            }
        } else {
            foreach (BuildingDoor door in doors) {
                door.LabelToggle(false);
                door.ColorToggle(false);
            }
        }

        if (!manager.simStarted) {
            shoppers.Clear();
            UpdateText();
        }

    }

    public void SetDoorMode() {
        // TODO: Move to set doormodes in a method, not on Start.
        switch (manager.GetDoorMode()) {
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

    // Resets the configuration of the building (doors, labels, etc.)
    public void ResetBuilding() {
        shoppers.Clear();
        foreach (BuildingDoor door in doors) {
            door.ResetFormat();
        }   
    }

    // Add shopper agents into a building's list of shoppers currently inside.
    public void AddShopper(GameObject shopper) {
        shoppers.Add(shopper);
        foreach (Transform child in shopper.transform) {
            if (shopper.tag == child.tag) {
                shoppers.Add(child.gameObject);
            }
        }
        UpdateText();
    }

    // Remove shopper agents from a building's list of shoppers currently inside.
    public void RemoveShopper(GameObject shopper) {
        shoppers.Remove(shopper);
        foreach (Transform child in shopper.transform) {
            if (shopper.tag == child.tag) {
                shoppers.Remove(child.gameObject);
            }
        }
        UpdateText();
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

    // Updates the text showing how many agents are currently inside the building.
    public void UpdateText() {
        text.SetText("" + shoppers.Count);
    }

}