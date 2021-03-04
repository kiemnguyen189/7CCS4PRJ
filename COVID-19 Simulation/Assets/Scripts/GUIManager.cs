using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    public SimManager manager;

    public Image playOverlay;
    public Image pauseOverlay;

    // * Sim Settings
    [Header("Top Bar")]
    public Button slowDownButton;
    public Button pauseButton;
    public Button startButton;
    public Button speedUpButton;
    public TextMeshProUGUI speedText;
    public TMP_Dropdown simLength;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public Button quitButton;
    
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
    public TMP_Dropdown pedestrianisedMode;

    
    // * Metrics
    [Header("Simulation Metrics Text")]
    public TextMeshProUGUI totalAgentsGUI;
    public TextMeshProUGUI totalShoppersGUI;
    public TextMeshProUGUI totalSingleShoppersGUI;
    public TextMeshProUGUI totalGroupShoppersGUI;
    public TextMeshProUGUI totalCommutersGUI;
    public TextMeshProUGUI totalSingleCommutersGUI;
    public TextMeshProUGUI totalGroupCommutersGUI;

    public TextMeshProUGUI totalSusceptibleGUI;
    public TextMeshProUGUI totalInfectedGUI;

    public TextMeshProUGUI totalContactsGUI;
    public TextMeshProUGUI infectiousContactsGUI;

    [Header("Bottom Bar")]
    public Toggle capacityToggle;
    public Toggle doorToggle;
    public Slider progressBar;

    // Start is called before the first frame update
    void Start()
    {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();
        //playOverlay.color = new Color(1,1,1,0);
        foreach (Transform img in playOverlay.transform) {
            img.GetComponent<Image>().color = new Color(1,1,1,0);
        }
        //pauseOverlay.color = new Color(1,1,1,0);
        foreach (Transform img in pauseOverlay.transform) {
            img.GetComponent<Image>().color = new Color(1,1,1,0);
        }
    }

    // Update is called once per frame
    private void Update() {

        float dayTime = manager.GetSimTime()%1440;
        timeText.text = ((int)Mathf.Floor(dayTime/60)).ToString("D2") + ":" + ((int)dayTime%60).ToString("D2") + ":00";
        dayText.text = ((int)Mathf.Ceil(manager.GetSimTime()/1440)).ToString("D2");

        float maxDuration = manager.GetSimDuration();
        progressBar.value = (manager.GetSimTime() / manager.GetSimDuration()) * 100;

    }

    void FixedUpdate()
    {
        totalAgentsGUI.text = "" + manager.GetTotalAgents();
        totalShoppersGUI.text = "" + manager.GetTotalShoppers();
        totalSingleShoppersGUI.text = "" + manager.GetTotalSingleShoppers();
        totalGroupShoppersGUI.text = "" + manager.GetTotalGroupShoppers();
        totalCommutersGUI.text = "" + manager.GetTotalCommuters();
        totalSingleCommutersGUI.text = "" + manager.GetTotalSingleCommuters();
        totalGroupCommutersGUI.text = "" + manager.GetTotalGroupCommuters();

        totalSusceptibleGUI.text = "" + manager.GetTotalSusceptible();
        totalInfectedGUI.text = "" + manager.GetTotalInfected();

        totalContactsGUI.text = "" + manager.GetTotalContactsNum();
        infectiousContactsGUI.text = "" + manager.GetInfectiousContactNum();


    }

    // Show the pause overlay on top of the visualizer.
    public void ShowPause(Image img) {
        foreach (Transform subImg in img.transform) {
            subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,0.5f);
        }
    }

    // Show and fade the play overlay on topm of the visualizer.
    IEnumerator ShowPlay(Image play, Image pause) {
        // Set the pause overlay alpha to 0.
        foreach (Transform subImg in pause.transform) {
            subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,0f);
        }
        // Show the play overlay.
        foreach (Transform subImg in play.transform) {
            subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,0.2f);
        }
        // Fade the play overlay to transparent.
        for (float i = 0.5f; i >= 0; i -= Time.deltaTime) {
            foreach (Transform subImg in play.transform) {
                subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,i);
            }
            yield return null;
        }
    }

    

    // Start/Stop the simulation.
    public void StartStopSimulation() {

        if (!manager.simStarted) {
            manager.StartSim();
            foreach (GameObject building in manager.GetBuildings()) {
                building.GetComponent<BuildingManager>().SetDoorMode();
            }
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            startButton.GetComponent<Image>().color = new Color(1,0,0,1);   // Set to RED

        } else {
            if (manager.isPaused) { PauseSimulation(); }
            manager.StopSim();
            foreach (GameObject building in manager.GetBuildings()) {
                building.GetComponent<BuildingManager>().ResetBuilding();
            }
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
            startButton.GetComponent<Image>().color = new Color(0,1,0,1);   // Set to GREEN

        }
    }
    
    // Pause the simulation.
    public void PauseSimulation() {
        if (manager.GetIsPaused()) {
            manager.SetIsPaused(false);
            Time.timeScale = manager.simSpeed;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
            StartCoroutine(ShowPlay(playOverlay, pauseOverlay));
        } else {
            manager.SetIsPaused(true);
            Time.timeScale = 0f;
            pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
            ShowPause(pauseOverlay);
        }
    }

    // Slow Down the simulation.
    public void SlowDown() {
        if (Time.timeScale > 0.125f) {
            Time.timeScale = Time.timeScale / 2.0f;
            manager.simSpeed /= 2.0f;
            speedText.text = manager.simSpeed + "x";
        }
    }

    // Speed Up the simulation.
    public void SpeedUp() {
        if (Time.timeScale < 8.0f) {
            Time.timeScale = Time.timeScale * 2.0f;
            manager.simSpeed *= 2.0f;
            speedText.text = manager.simSpeed + "x";
        }
    }

    // Sets the run duration of the simulation.
    public void SetRunLength() { 
        switch (simLength.value) {
            case 0: manager.SetSimDuration(1f); break;
            case 1: manager.SetSimDuration(2f); break;
            case 2: manager.SetSimDuration(3f); break;
            case 3: manager.SetSimDuration(4f); break;
            case 4: manager.SetSimDuration(5f); break;
            case 5: manager.SetSimDuration(6f); break;
            case 6: manager.SetSimDuration(7f); break;
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

    // Sets the social distancing radius depending on user input.
    public void SetSocialDistanceRadius() { 
        switch (socialDistanceRadius.value) {
            case 0: manager.SetRadiusSize(0.5f); break;
            case 1: manager.SetRadiusSize(2f); break;
            case 2: manager.SetRadiusSize(4f); break;
        }
    }

    // Sets the building entrance mode depending on user input.
    public void SetBuildingEntranceMode() { 
        switch (buildingEntranceMode.value) {
            case 0: manager.SetDoorMode(DoorwayMode.OneWay); break;
            case 1: manager.SetDoorMode(DoorwayMode.TwoWay); break;
            case 2: manager.SetDoorMode(DoorwayMode.Mixed); break;
        }
    }

    // Sets whether or not the environment should be pedestrianised depending on user input.
    public void SetPedestianised() { 
        switch (pedestrianisedMode.value) {
            case 0: manager.SetIsPedestrianised(false); break;
            case 1: manager.SetIsPedestrianised(true); break;
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

    // Quits the program.
    public void Quit() {
        Application.Quit();
    }

}
