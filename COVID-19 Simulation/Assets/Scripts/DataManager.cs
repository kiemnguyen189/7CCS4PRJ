using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public SimManager manager;
    public Transform barGraphPrefab;

    public List<List<int>> population = new List<List<int>>();
    public List<int> hourlyPop = new List<int>();
    public List<int> cumulativePop = new List<int>();

    public List<List<int>> infectedPop = new List<List<int>>();

    

    public List<int> hourlyInfected;
    public List<int> cumulativeInfected;
    public List<List<int>> demographicPop;
    public List<int> proportionInfectedHourly;
    public List<int> proportionInfectedCumulative;


    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<SimManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetData() {
        hourlyPop.Clear();
        cumulativePop.Clear();
    }

    // Gets the list of population numbers. If isCumulative is true, return cumulative numbers, else return hourly.
    // Returns a list of 24 elements depending on parameters.
    public List<int> GetPopulation(bool isCumulative, int day) { 
        List<int> ret = new List<int>();
        int listNum = 0;
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        if (!isCumulative) { listNum = 0;} 
        else { listNum = 1; }
        // Create temporary sub list out of the total population data to return.
        for (int i = startIndex; i < startIndex+24; i++) { ret.Add(population[listNum][i]); }
        return ret; 
    }

    // Updates the number of hourly and cumulative hourly populations.
    public void UpdatePopulation(int num) {
        // Update hourly population List.
        hourlyPop.Add(num);
        // Update cumulative population List.
        if ((cumulativePop.Count%24) == 0) { cumulativePop.Add(num); }
        else { cumulativePop.Add(num + cumulativePop[cumulativePop.Count-1]); }
        population.Add(hourlyPop);
        population.Add(cumulativePop);
    }

    // // Updates the number of hourly populations.
    // public List<int> GetHourlyPop() { return hourlyPop; }
    // public void UpdateHourlyPop(int num) {
    //     hourlyPop.Add(num);
    // }

    // // Updates the cumulative number of hourly populations.
    // public List<int> GetCumulativePop() { return cumulativePop; }
    // public void UpdateCumulativePop(int num) {
    //     // Get latest hourly population number.
    //     if (cumulativePop.Count == 0) {
    //         cumulativePop.Add(num);
    //     } else {
    //         int newNum = num + cumulativePop[cumulativePop.Count-1];
    //         // Append total of latest and current population numbers.
    //         cumulativePop.Add(newNum);
    //     }
        
    // }

    // Updates the number of hourly infected populations.
    public List<int> GetHourlyInfected() { return hourlyInfected; }
    public void UpdateHourlyInfected(int num) {
        hourlyInfected.Add(num);
    }

    // Updates the cumulative number of hourly infected populations.
    public List<int> GetCumulativeInfected() { return cumulativeInfected; }
    public void UpdateCumulativeInfected(int num) {
        // Get latest hourly infected population number.
        if (cumulativeInfected.Count == 0) {
            cumulativeInfected.Add(num);
        } else {
            int newNum = num + cumulativeInfected[cumulativeInfected.Count-1];
            // Append total of latest and current infected numbers.
            cumulativeInfected.Add(newNum);
        }
        
    }

}
