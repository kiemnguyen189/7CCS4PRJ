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

    public void PauseSimulation() {
        //float curTimeScale = manager.simSpeed;
        if (manager.isPaused) {
            manager.isPaused = false;
            Time.timeScale = manager.simSpeed;
        } else {
            manager.isPaused = true;
            Time.timeScale = 0f;
        }
    }

    public void SpeedUp() {
        if (Time.timeScale < 8.0f) {
            Time.timeScale = Time.timeScale * 2.0f;
            manager.simSpeed *= 2.0f;
        }
    }

    public void SlowDown() {
        if (Time.timeScale > 0.125f) {
            Time.timeScale = Time.timeScale / 2.0f;
            manager.simSpeed /= 2.0f;
        }
    }

}
