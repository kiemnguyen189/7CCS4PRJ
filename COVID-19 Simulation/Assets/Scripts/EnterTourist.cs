using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTourist : MonoBehaviour
{
    
    public BuildingManager core;
    public List<GameObject> storedTourists = new List<GameObject>();
    
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
        tourist.transform.position = tourist.transform.position + new Vector3(-10, 0, -10);
        tourist.gameObject.SetActive(true);
    }
    

}
