using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDoor : MonoBehaviour
{
    
    public BuildingManager core;
    //enum DoorType {Entrance, Exit, Both};
    public DoorType doorType = DoorType.Entrance;
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("ENTERED!");
        Debug.Log("" + other.gameObject.name);
        // TODO: Instead of Destroy, store the agents somewhere, whilst keeping agent states consistent.
        core.AddTourist(other.gameObject);
        StartCoroutine(RecreateTourist(other.gameObject));
    }

    // Disables a Tourist for a short amount of time, then recreates it a few seconds later 10 units away in x and z direction
    IEnumerator RecreateTourist(GameObject tourist)
    {
        tourist.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        // TODO: Change this position to a door with type Exit
        tourist.transform.position = tourist.transform.position + new Vector3(-10, 0, -10);
        tourist.gameObject.SetActive(true);
        core.RemoveTourist(tourist.gameObject);
        
    }
    

}
