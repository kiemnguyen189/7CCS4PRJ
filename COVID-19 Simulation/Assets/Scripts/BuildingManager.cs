using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    public List<GameObject> stored = new List<GameObject>();
    public List<GameObject> doors = new List<GameObject>();

    public GameObject northDoor;
    public GameObject southDoor;
    public GameObject eastDoor;
    public GameObject westDoor;
    

    public string touristTag = "Tourist";
    public int range = 3;
    
    // Start is called before the first frame update
    void Start() {
        doors.Add(northDoor);
        doors.Add(southDoor);
        doors.Add(eastDoor);
        doors.Add(westDoor);

        Debug.Log(doors[0].transform.position);
        /*
        Debug.Log(northDoor.transform.position);
        Debug.Log(southDoor.transform.position);
        Debug.Log(eastDoor.transform.position);
        Debug.Log(westDoor.transform.position);
        */
    }

    public void AddTourist(GameObject tourist) {
        stored.Add(tourist);
    }

    public void RemoveTourist(GameObject tourist) {
        stored.Remove(tourist);
    }    


}