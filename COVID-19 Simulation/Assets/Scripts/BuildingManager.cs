using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Manager class for each building.
public class BuildingManager : MonoBehaviour
{

    public SimManager manager;

    public List<GameObject> shoppers = new List<GameObject>();      // Internal list of Shoppers currently inside the building. 
    public List<BuildingDoor> doors = new List<BuildingDoor>();     // List of all 4 doorway nodes if this building.

    public TextMeshPro text;                                        // The central label that shows the capacity of this building.
    public bool showOccupancy = true;                               // Whether or not to show the central capacity label.
    public bool showDoors = true;                                   // Whether or not to render the doorway nodes.
    
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
        CombineInstance[] combine = new CombineInstance[6];     // 5 cuboids + 1 parent mesh.
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
        // Check to see if the SimManager setting is set to show occupancy.
        if (showOccupancy) { text.gameObject.SetActive(true); } 
        else { text.gameObject.SetActive(false); }

        // Check to see if the SimManager setting is set to render doorway nodes.
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

        // If the run has finished, remove all shoppers stored in the list.
        if (!manager.simStarted) {
            shoppers.Clear();
            UpdateText();
        }

    }

    // Sets the doorway policy of the building depending on the Building Entrance Mode parameter set by the user.
    public void SetDoorMode() {
        switch (manager.GetDoorMode()) {
            // Randomly assign either North and South as entrances, or East and West, vice versa for exits.
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
            // Sets all the doors of this building to Two-way.
            case DoorwayMode.TwoWay:
                foreach (BuildingDoor door in doors) {
                    door.doorType = DoorType.Both;
                }
                break;
            // Random assignment of doorway nodes.
            case DoorwayMode.Mixed:
                foreach (BuildingDoor door in doors) {
                    door.doorType = (DoorType)Random.Range(0, doors.Count - 1);
                }
                // Randomly change one of the doors to type Both as assurance.
                doors[Random.Range(0, doors.Count)].doorType = DoorType.Both;
                break;
        }  
        // Recalibrate the doorway nodes.
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