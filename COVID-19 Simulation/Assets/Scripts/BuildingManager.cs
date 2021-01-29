using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingManager : MonoBehaviour
{

    public List<GameObject> tourists = new List<GameObject>();
    public List<BuildingDoor> doors = new List<BuildingDoor>();
    public List<BuildingDoor> temp = new List<BuildingDoor>();

    public BuildingDoor northDoor;
    public BuildingDoor southDoor;
    public BuildingDoor eastDoor;
    public BuildingDoor westDoor;

    public TextMeshPro text;

    public string touristTag = "Tourist";
    public int range = 3;

    public bool showText = true;
    
    // Start is called before the first frame update
    void Start() {
        Random rand = new Random();

        doors.Add(northDoor);
        doors.Add(southDoor);
        doors.Add(eastDoor);
        doors.Add(westDoor);

        Debug.Log(doors[0].transform.position);
        Debug.Log(""+doors[0].doorType);
        
    }

    private void FixedUpdate() {
        if (showText) {
            StartCoroutine(UpdateText());
            text.gameObject.SetActive(true);

            foreach (BuildingDoor door in doors) {
                door.TextToggle(true);
                door.ColorToggle(true);
                //door.gameObject.SetActive(true);
            }

            // northDoor.TextToggle(true);
            // southDoor.TextToggle(true);
            // eastDoor.TextToggle(true);
            // westDoor.TextToggle(true);
        } else {
            text.gameObject.SetActive(false);

            foreach (BuildingDoor door in doors) {
                door.TextToggle(false);
                door.ColorToggle(false);
                //door.gameObject.SetActive(false);
            }

            // northDoor.TextToggle(false);
            // southDoor.TextToggle(false);
            // eastDoor.TextToggle(false);
            // westDoor.TextToggle(false);
        }
    }

    public void AddTourist(GameObject tourist) {
        tourists.Add(tourist);
    }

    public void RemoveTourist(GameObject tourist) {
        // TODO: Add a timer buffer when an agent gets removed so that they don't instantly go back in of they leave via a "Both" door
        tourists.Remove(tourist);
    }    

    public Transform ReturnExitDoor() {
        foreach (BuildingDoor door in doors) {
            if (door.doorType.ToString().Equals(DoorType.Exit.ToString()) || door.doorType.ToString().Equals(DoorType.Both.ToString())){
                temp.Add(door);
            }
        }
        Transform ret = temp[Random.Range(0, temp.Count)].transform;
        temp.Clear();
        return ret;
    }

    IEnumerator UpdateText() {
        text.SetText("" + tourists.Count);
        yield return new WaitForSeconds(1);
    }

}