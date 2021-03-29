using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{

    [Header("Managers")]
    public SimManager manager;
    public DataManager dataManager;

    [Header("Overlay")]
    public Sprite playSprite;
    public Sprite pauseSprite;
    public Image pauseOverlay;

    [Header("Results Overlay")]
    public GameObject dataOverlay;
    public Transform barGraphPrefab;
    public Button hideResultsButton;
    public bool isHidden;
    public Transform resultsView;

    public TMP_Dropdown graphType;
    public TMP_Dropdown currentDay;
    private bool isCumulative = false;
    private int day = 1;

    public Transform populationGraph;
    public Transform infectionsGraph;

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

    public TextMeshProUGUI infectiousContactsGUI;

    [Header("Bottom Bar")]
    public Toggle capacityToggle;
    public Toggle doorToggle;
    public Slider progressBar;

    // Start is called before the first frame update
    void Start()
    {

        manager = GameObject.Find("Manager").GetComponent<SimManager>();
        foreach (Transform img in pauseOverlay.transform) {
            img.GetComponent<Image>().color = new Color(1,1,1,0);
        }

        // List<string> temp = new List<string>() {"temp"};
        // simLength.AddOptions(temp);

        isHidden = true;

        CloseData();
    }

    // Update is called once per frame
    private void Update() {

        float dayTime = manager.GetSimTime()%1440;
        timeText.text = ((int)Mathf.Floor(dayTime/60)).ToString("D2") + ":" + ((int)dayTime%60).ToString("D2") + ":00";
        dayText.text = ((int)Mathf.Ceil(manager.GetSimTime()/1440)).ToString("D2");

        progressBar.value = (manager.GetSimTime() / manager.GetSimDuration()) * 100;

        totalAgentsGUI.text = "" + manager.GetTotalAgents();
        totalShoppersGUI.text = "" + manager.GetTotalShoppers();
        totalSingleShoppersGUI.text = "" + manager.GetTotalSingleShoppers();
        totalGroupShoppersGUI.text = "" + manager.GetTotalGroupShoppers();
        totalCommutersGUI.text = "" + manager.GetTotalCommuters();
        totalSingleCommutersGUI.text = "" + manager.GetTotalSingleCommuters();
        totalGroupCommutersGUI.text = "" + manager.GetTotalGroupCommuters();

        totalSusceptibleGUI.text = "" + manager.GetTotalSusceptible();
        totalInfectedGUI.text = "" + manager.GetTotalInfected();

        infectiousContactsGUI.text = "" + manager.GetInfectiousContactNum();

    }


    // Show the pause overlay on top of the visualizer.
    public void ShowPause(Image img) {
        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        // Change sprite to pause.
        img.transform.GetChild(0).GetComponent<Image>().sprite = pauseSprite;
        // Show pause overlay.
        foreach (Transform subImg in img.transform) {
            subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,0.5f);
        }
    }

    // Show and fade the play overlay on topm of the visualizer.
    IEnumerator ShowPlay(Image img) {
        pauseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        // Change sprite play.
        img.transform.GetChild(0).GetComponent<Image>().sprite = playSprite;
        // Fade the play overlay to transparent.
        for (float i = 0.5f; i >= 0; i -= Time.fixedDeltaTime) {
            // Robustness from spamming.
            if (!manager.GetIsPaused()) {
                foreach (Transform subImg in img.transform) {
                    subImg.GetComponent<Image>().color = new Color(0.2f,0.2f,0.2f,i);
                }
                yield return null;
            }
            
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
        manager.PauseSim();
        // Update play/pause button and overlay state.
        if (!manager.GetIsPaused()) {
            StartCoroutine(ShowPlay(pauseOverlay));
        } else {
            ShowPause(pauseOverlay);
        }
    }

    // Slow Down the simulation.
    public void SlowDown() {
        float speed = manager.GetSimSpeed();
        if (speed > 0.125f) {
            manager.SetSimSpeed(speed / 2.0f);
            if (!manager.GetIsPaused()) { Time.timeScale = manager.GetSimSpeed(); }
            speedText.text = manager.GetSimSpeed() + "x";
        }
    }

    // Speed Up the simulation.
    public void SpeedUp() {
        float speed = manager.GetSimSpeed();
        if (speed < 8.0f) {
            manager.SetSimSpeed(speed * 2.0f);
            if (!manager.GetIsPaused()) { Time.timeScale = manager.GetSimSpeed(); }
            speedText.text = manager.GetSimSpeed() + "x";
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

    // Switches from hourly to cumulative population measurements.
    public void SwitchGraphType() {
        switch (graphType.value) {
            case 0: isCumulative = false; break;
            case 1: isCumulative = true; break;
        }
        switch (currentDay.value) {
            case 0: day = 1; break;
            case 1: day = 2; break;
            case 2: day = 3; break;
            case 3: day = 4; break;
            case 4: day = 5; break;
            case 5: day = 6; break;
            case 6: day = 7; break;
        }
        UpdateAllGraphs();
    }

    //
    public void UpdateAllGraphs() {
        UpdateBarGraph(populationGraph, dataManager.GetPopulation(isCumulative, day), false);
        UpdateBarGraph(infectionsGraph, dataManager.GetInfections(isCumulative, day), false);
    }

    // Updates the bar graph.
    public void UpdateBarGraph(Transform graph, List<int> data, bool percentage) {
        // Get max and min values in the data.
        float max = (float)data[0];
        float min = (float)data[0];
        foreach (int val in data) { 
            if (val > max) { max = val; } 
            else if (val < min) { min = val; }
        }
        // Round the max and min to the place value of the second digit.
        float maxDigits = Mathf.Pow(10f, (Mathf.Floor(Mathf.Log10(max))-1));
        float minDigits = Mathf.Pow(10f, (Mathf.Floor(Mathf.Log10(min))-1));
        max = Mathf.Ceil(max/maxDigits) * maxDigits;
        // If minimum value is only a single digit, set min to 0.
        if (minDigits < 1) { min = 0;}
        else { min = Mathf.Floor(min/minDigits) * minDigits; }
        // Set alignment values for bar positioning.
        float maxHeight = 160;
        float vOffset = 40;
        float hOffset = 55;
        Transform bars = graph.Find("MainBars").GetChild(0);
        for (int j = 0; j < data.Count; j++) {
            float heightValue = ((data[j]-min) / (max-min)) * maxHeight;
            bars.transform.GetChild(j).GetComponent<RectTransform>().sizeDelta = new Vector2(10, heightValue);
            bars.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = new Vector2(hOffset + (j*10), (heightValue/2) + vOffset);
        }
        
        // Change the interval values on the yAxis.
        Transform values = graph.Find("yAxis").Find("Values");
        for (int i = 0; i <= 10; i++) {
            values.GetChild(i).GetComponent<TextMeshProUGUI>().text = (max - (i*((max-min)/10))).ToString();
        }
    }

    // // Updates the bar graph.
    // public void UpdateStackedGraph(Transform graph, List<List<int>> data, bool percentage) {
        
    //     // Iterate through each set of bars.
    //     for (int i = 0; i < data.Count; i++) {
    //         // Get max and min values in the data.
    //         float max = (float)data[i][0];
    //         float min = (float)data[i][0];
    //         foreach (int val in data) { 
    //             if (val > max) { max = val; } 
    //             else if (val < min) { min = val; }
    //         }
    //         // Round the max and min to the place value of the second digit.
    //         float maxDigits = Mathf.Pow(10f, (Mathf.Floor(Mathf.Log10(max))-1));
    //         float minDigits = Mathf.Pow(10f, (Mathf.Floor(Mathf.Log10(min))-1));
    //         max = Mathf.Ceil(max/maxDigits) * maxDigits;
    //         // If minimum value is only a single digit, set min to 0.
    //         if (minDigits < 1) { min = 0;}
    //         else { min = Mathf.Floor(min/minDigits) * minDigits; }
    //         // Set alignment values for bar positioning.
    //         float maxHeight = 160;
    //         float vOffset = 40;
    //         float hOffset = 40;
    //         // Main bar heights.
    //         Transform mainBars = graph.Find("MainBars");
    //         // TODO: Create prefab of set of bars.
    //         Transform bars = Instantiate(barGraphPrefab, mainBars);
    //         //Transform bars = mainBars.GetChild(i);
    //         for (int j = 0; j < data[i].Count; j++) {
    //             float heightValue = ((data[j]-min) / (max-min)) * maxHeight;
    //             bars.transform.GetChild(j).GetComponent<RectTransform>().sizeDelta = new Vector2(10, heightValue);
    //             bars.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = new Vector2(hOffset + (j*10), (heightValue/2) + vOffset);
    //         }
    //     }
    //     // Change the interval values on the yAxis.
    //     Transform values = graph.Find("yAxis").Find("Values");
    //     for (int i = 0; i <= 10; i++) {
    //         values.GetChild(i).GetComponent<TextMeshProUGUI>().text = (max - (i*((max-min)/10))).ToString();
    //     }
    // }

    //
    public void ShowData() {
        isHidden = false;
        dataOverlay.SetActive(true);
        currentDay.options.Clear();
        List<string> temp = new List<string>();
        for (int i = 1; i <= simLength.value+1; i++) {
            temp.Add("Day "+i);
        }
        currentDay.AddOptions(temp);

        UpdateAllGraphs();

    }

    //
    public void HideData() {
        dataOverlay.SetActive(true);
        if (!isHidden) {
            isHidden = true;
            resultsView.gameObject.SetActive(false);
            hideResultsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Results";
        } else {
            isHidden = false;
            resultsView.gameObject.SetActive(true);
            hideResultsButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hide Results";
        }
    }

    //
    public void CloseData() {
        dataOverlay.SetActive(false);
        // TODO: Reset data.
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
