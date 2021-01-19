using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    
    public Transform opening1;
    public Transform opening2;
    public Transform opening3;
    public Transform opening4;

    public int radius;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(opening1.position);
        Debug.Log(opening2.position);
        Debug.Log(opening3.position);
        Debug.Log(opening4.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
