using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterTourist : MonoBehaviour
{
    
    public Transform core;
    
    private void OnTriggerEnter(Collider other) {
        Debug.Log("ENTERED!");
        Debug.Log("" + other.gameObject.name);
        // TODO: Instead of Destroy, store the agents somewhere, whilst keeping agent states consistent.
        
        //Destroy(other.gameObject);
        other.gameObject.SetActive(false);
    }

    

}
