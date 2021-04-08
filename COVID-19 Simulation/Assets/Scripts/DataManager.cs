using System.Collections.Generic;
using UnityEngine;

// Manager class for storing the metrics of the simulation.
public class DataManager : MonoBehaviour
{
    
    public SimManager manager;

    // Each list contains sub-lists of stacked data, which each contain two further sub-lists of hourly and cumulative data.
    public List<List<List<int>>> population = new List<List<List<int>>>();      // List of population numbers.
    public List<List<List<int>>> demographicPop = new List<List<List<int>>>();  // List of population demographic numbers.
    public List<List<List<int>>> infections = new List<List<List<int>>>();      // List of total number of infections.
    public List<List<List<int>>> infectedPop = new List<List<List<int>>>();     // List of the proportion of susceptible vs infected agents.

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<SimManager>();
    }

    // Resets all of the lists.
    public void ResetData() {

        population.Clear();
        infections.Clear();
        demographicPop.Clear();
        infectedPop.Clear();

    }

    // Gets and updates the population metric.
    // isCumulative controls whether or not to return hourly or cumulative data from the list.
    // day selects the current day to display.
    // isHourly notifies the update on whether or not the data to store is hourly or cumulative.
    public List<List<int>> GetPopulation(bool isCumulative, int day) { return GetData(isCumulative, day, population); }
    public void UpdatePopulation(List<int> data, bool isHourly) { UpdateData(data, population, isHourly); }

    // Gets and updates the infections metric.
    public List<List<int>> GetInfections(bool isCumulative, int day) { return GetData(isCumulative, day, infections); }
    public void UpdateInfections(List<int> data, bool isHourly) { UpdateData(data, infections, isHourly); }

    // Gets and updates the demographic metric.
    public List<List<int>> GetDemographic(bool isCumulative, int day) { return GetData(isCumulative, day, demographicPop); }
    public void UpdateDemographic(List<int> data, bool isHourly) { UpdateData(data, demographicPop, isHourly); }

    // Gets and updates the infected ratio metric.
    public List<List<int>> GetInfected(bool isCumulative, int day) { return GetData(isCumulative, day, infectedPop); }
    public void UpdateInfected(List<int> data, bool isHourly) { UpdateData(data, infectedPop, isHourly); }

    // Retrieves the relevant list from the data list, depending on whether or not the data needed is hourly or cumulative, and the day.
    public List<List<int>> GetData(bool isCumulative, int day, List<List<List<int>>> list) {

        List<List<int>> ret = new List<List<int>>();
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        int listNum = 0;
        if (isCumulative) { listNum = 1;} 
        // Create temporary sub list out of the total population data to return.
        for (int i = 0; i < list.Count; i++) { 
            ret.Add( new List<int>() );
            for (int j = startIndex; j < startIndex+24; j++) { 
                ret[i].Add(list[i][listNum][j]);
            }
        }
        return ret; 
    }

    // Updates the metric lists hourly. Elements are added depending on the isHourly parameter.
    public void UpdateData(List<int> data, List<List<List<int>>> list, bool isHourly) {

        // Called on the first hour of the run when the list is currently empty.
        // This is because cumulative data cannot be added when the list contains nothing.
        if (list.Count == 0) {
            for (int i = 0; i < data.Count; i++) {
                list.Add(new List<List<int>>() { new List<int>() {data[i]}, new List<int>() {data[i]} });
            }
        } else {
            for (int i = 0; i < data.Count; i++) {
                list[i][0].Add(data[i]);    // Add to the hourly list
                if (isHourly) {
                    // If the data passed in is an hourly metric, append to the cumulative list along with the previous element.
                    if ((list[i][1].Count%24) == 0) { list[i][1].Add(data[i]); }
                    else { list[i][1].Add(data[i] + list[i][1][list[i][1].Count-1]); }
                } else {
                    list[i][1].Add(data[i]);    
                }
            }
        }

    }


}
