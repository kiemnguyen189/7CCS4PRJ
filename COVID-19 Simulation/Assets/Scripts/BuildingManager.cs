using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    
    public Transform opening1;
    public Transform opening2;
    public Transform opening3;
    public Transform opening4;

    public GameObject northDoor;
    public GameObject[] storedTourists

    public string touristTag = "Tourist";
    public int range = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(opening1.position);
        Debug.Log(opening2.position);
        Debug.Log(opening3.position);
        Debug.Log(opening4.position);
        //InvokeRepeating("UpdateOpening", 0f, 0.5f);
    }

    


}