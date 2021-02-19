using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{

    public SimManager manager;

    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        Button start = startButton.GetComponent<Button>();
        start.onClick.AddListener(StartSimulation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSimulation() {
        Debug.Log("TEST BUTTON");
        manager.simStarted = true;
    }
}
