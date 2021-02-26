using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public SimManager manager;

    // * Sim Settings
    public Button startButton;

    // * Parameters


    // * Metrics
    public TextMeshProUGUI totalAgentsGUI;
    public TextMeshProUGUI totalShoppersGUI;
    public TextMeshProUGUI totalCommutersGUI;
    public TextMeshProUGUI totalGroupShoppersGUI;
    public TextMeshProUGUI totalGroupCommutersGUI;

    public TextMeshProUGUI totalSusceptibleGUI;
    public TextMeshProUGUI totalInfectedGUI;

    public TextMeshProUGUI totalContactsGUI;
    public TextMeshProUGUI infectiousContactsGUI;

    // Start is called before the first frame update
    void Start()
    {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();

        Button start = startButton.GetComponent<Button>();
        start.onClick.AddListener(StartSimulation);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        totalAgentsGUI.text = "" + manager.GetTotalAgents();
        totalShoppersGUI.text = "" + manager.GetTotalShoppers();
        totalCommutersGUI.text = "" + manager.GetTotalCommuters();
        totalGroupShoppersGUI.text = "" + manager.GetTotalGroupShoppers();
        totalGroupCommutersGUI.text = "" + manager.GetTotalGroupCommuters();

        totalSusceptibleGUI.text = "" + manager.GetTotalSusceptible();
        totalInfectedGUI.text = "" + manager.GetTotalInfected();

        totalContactsGUI.text = "" + manager.GetTotalContactsNum();
        infectiousContactsGUI.text = "" + manager.GetInfectiousContactNum();
    }

    public void StartSimulation() {
        Debug.Log("TEST BUTTON");
        manager.simStarted = true;
    }

    public void PauseSimulation() {
        //float curTimeScale = manager.simSpeed;
        if (manager.GetIsPaused()) {
            manager.SetIsPaused(false);
            Time.timeScale = manager.simSpeed;
        } else {
            manager.SetIsPaused(true);
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
