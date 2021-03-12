using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public SimManager manager;

    public List<List<int>> population = new List<List<int>>();
    public List<int> hourlyPop = new List<int>();
    public List<int> cumulativePop = new List<int>();

    public List<List<int>> infectedPop = new List<List<int>>();
    public List<int> hourlyInfected = new List<int>();
    public List<int> cumulativeInfected = new List<int>();

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

        population.Clear();
        hourlyPop.Clear();
        cumulativePop.Clear();

        infectedPop.Clear();
        hourlyInfected.Clear();
        cumulativeInfected.Clear();

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

    //
    public List<int> GetInfections(bool isCumulative, int day) {
        List<int> ret = new List<int>();
        int listNum = 0;
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        if (!isCumulative) { listNum = 0;} 
        else { listNum = 1; }
        // Create temporary sub list out of the total population data to return.
        for (int i = startIndex; i < startIndex+24; i++) { ret.Add(infectedPop[listNum][i]); }
        return ret; 
    }

    //
    public void UpdateInfections(int num) {
        // Update hourly population List.
        cumulativeInfected.Add(num);
        // Update cumulative population List.
        if (hourlyInfected.Count == 0) { hourlyInfected.Add(num); }
        else { hourlyInfected.Add(num - cumulativeInfected[cumulativeInfected.Count-2]); }
        infectedPop.Add(hourlyInfected);
        infectedPop.Add(cumulativeInfected);
    }

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
