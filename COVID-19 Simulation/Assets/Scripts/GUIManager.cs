using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public SimManager manager;

    // * Sim Settings
    [Header("Top Bar")]
    public Button slowDownButton;
    public Button pauseButton;
    public Button startButton;
    public Button speedUpButton;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public Slider progressBar;
    
    // * Parameters
    [Header("Parameters")]
    public Slider ratioTypesSlider;
    public TextMeshProUGUI ratioTypesText;
    public Slider ratioGroupsSlider;
    public TextMeshProUGUI ratioGroupsText;
    public Slider ratioInfectedSlider;
    public TextMeshProUGUI ratioInfectedText;
    public Slider infectionChanceSlider;
    public TextMeshProUGUI infectionChanceText;
    public Slider maxGroupSizeSlider;
    public TextMeshProUGUI maxGroupSizeText;

    public TMP_Dropdown socialDistanceRadius;
    public TMP_Dropdown buildingEntranceMode;

    public Toggle capacityToggle;
    public Toggle doorToggle;

    // * Metrics
    [Header("Simulation Metrics Text")]
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


    }

    // Update is called once per frame
    private void Update() {

        float dayTime = manager.GetSimTime()%1440;
        timeText.text = ((int)Mathf.Floor(dayTime/60)).ToString("D2") + ":" + ((int)dayTime%60).ToString("D2") + ":00";
        dayText.text = ((int)Mathf.Ceil(manager.GetSimTime()/1440)).ToString("D2");

    }

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

    // Start/Stop the simulation.
    public void StartStopSimulation() {

        if (!manager.simStarted) {
            manager.simStarted = true;
            foreach (GameObject building in manager.GetBuildings()) {
                building.GetComponent<BuildingManager>().SetDoorMode();
            }
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            startButton.GetComponent<Image>().color = new Color(1,0,0,1);

        } else {
            manager.simStarted = false;
            manager.ResetMetrics();
            manager.ResetTime();
            foreach (GameObject building in manager.GetBuildings()) {
                building.GetComponent<BuildingManager>().ResetBuilding();
            }
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
            startButton.GetComponent<Image>().color = new Color(0,1,0,1);

        }
    }
    
    // Pause the simulation.
    public void PauseSimulation() {
        if (manager.GetIsPaused()) {
            manager.SetIsPaused(false);
            Time.timeScale = manager.simSpeed;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        } else {
            manager.SetIsPaused(true);
            Time.timeScale = 0f;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        }
    }

    // Speed Up the simulation.
    public void SpeedUp() {
        if (Time.timeScale < 8.0f) {
            Time.timeScale = Time.timeScale * 2.0f;
            manager.simSpeed *= 2.0f;
        }
    }

    // Slow Down the simulation.
    public void SlowDown() {
        if (Time.timeScale > 0.125f) {
            Time.timeScale = Time.timeScale / 2.0f;
            manager.simSpeed /= 2.0f;
        }
    }

    // Matches the shown text value of Agent Type chances to the Slider value.
    public void MatchInputsRatioTypes() { 
        ratioTypesText.text = ratioTypesSlider.value + "%"; 
        manager.SetRatioTypes(ratioTypesSlider.value);
    }

    // Matches the shown text value of Group Spawn chances to the Slider value.
    public void MatchInputsRatioGroups() { 
        ratioGroupsText.text = ratioGroupsSlider.value + "%"; 
        manager.SetRatioGroups(ratioGroupsSlider.value);
    }

    // Matches the shown text value of Infected Spawn chances to the Slider value.
    public void MatchInputsRatioInfected() { 
        ratioInfectedText.text = ratioInfectedSlider.value + "%"; 
        manager.SetRatioInfected(ratioInfectedSlider.value);
    }

    // Matches the shown text value of Infection Chances to the Slider value.
    public void MatchInputsInfectionChance() { 
        infectionChanceText.text = infectionChanceSlider.value + "%"; 
        manager.SetInfectionChance(infectionChanceSlider.value);
    }

    // Matches the shown text value of Max Group Sizes to the Slider value.
    public void MatchInputsGroupSize() { 
        maxGroupSizeText.text = maxGroupSizeSlider.value + ""; 
        manager.SetMaxGroupSize((int)maxGroupSizeSlider.value);
    }

    // Matches the shown text value of Max Group Sizes to the Slider value.
    public void SetSocialDistanceRadius() { 
        switch (socialDistanceRadius.value) {
            case 0: manager.SetRadiusSize(0.5f); break;
            case 1: manager.SetRadiusSize(2f); break;
            case 2: manager.SetRadiusSize(4f); break;
        }
    }

    // Matches the shown text value of Max Group Sizes to the Slider value.
    public void SetBuildingEntranceMode() { 
        switch (buildingEntranceMode.value) {
            case 0: manager.SetDoorMode(DoorwayMode.OneWay); break;
            case 1: manager.SetDoorMode(DoorwayMode.TwoWay); break;
            case 2: manager.SetDoorMode(DoorwayMode.Mixed); break;
        }
    }

    // Toggle to control whether or not to show the capacities of each building.
    public void ToggleBuildingCapacity() {
        if (manager.GetShowBuildingNum()) {
            manager.SetShowBuildingNum(false);
            capacityToggle.GetComponentInChildren<Image>().color = new Color(1,0,0,1);
        } else {
            manager.SetShowBuildingNum(true);
            capacityToggle.GetComponentInChildren<Image>().color = new Color(0,1,0,1);
        }
        
    }

    // Toggle to control whether or not to show the visual representation of doors.
    public void ToggleDoorVisibility() {
        if (manager.GetShowBuildingDoors()) {
            manager.SetShowBuildingDoors(false);
            doorToggle.GetComponentInChildren<Image>().color = new Color(1,0,0,1);
        } else {
            manager.SetShowBuildingDoors(true);
            doorToggle.GetComponentInChildren<Image>().color = new Color(0,1,0,1);
        }
    }

}
