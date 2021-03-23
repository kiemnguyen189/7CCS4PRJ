using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public SimManager manager;

    // List of population numbers.
    // Outer list contains 2 elements: HourlyList and Cumulative List.
    // Inner lists contain hourly data. Length is in multiples of 24.
    public List<List<int>> population = new List<List<int>>();
    public List<List<int>> infectedPop = new List<List<int>>();

    // List of population demographic numbers.
    // Outermost list contains 4 elements: SingleShopperList, GroupShopperList, SingleCommuterList and GroupCommuterList.
    // Middle lists contains 2 elements: Hourly and Cumulative.
    // Innermost list contains hourly data. Length is in multiples of 24.
    public List<List<List<int>>> demographicPop = new List<List<List<int>>>();

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
        infectedPop.Clear();

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

        if (population.Count == 0) {
            population.Add(new List<int>() {num} );
            population.Add(new List<int>() {num} );
        } else {
            population[0].Add(num);
            if ((population[1].Count%24) == 0) { population[1].Add(num); }
            else { population[1].Add(num + population[1][population[1].Count-1]); }
        }
        
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
        
        if (infectedPop.Count == 0) {
            infectedPop.Add(new List<int>() {num} );
            infectedPop.Add(new List<int>() {num} );
        } else {
            infectedPop[1].Add(num);
            if (infectedPop[0].Count == 0) { infectedPop[0].Add(num); }
            else { infectedPop[0].Add(num - infectedPop[1][infectedPop[1].Count-2]); }
        }
    }

    // //
    // public List<List<int>> GetDemographic(bool isCumulative, int day) {

    //     List<List<int>> ret = new List<List<int>>();
    //     int listNum = 0;
    //     int startIndex = ((day*24) - 24);
    //     // Whether or not to return an hourly or cumulative graph.
    //     if (!isCumulative) { listNum = 0;} 
    //     else { listNum = 1; }
    //     // Create temporary sub list out of the total population data to return.


    //     for (int i = startIndex; i < startIndex+24; i++) { 

    //         ret.Add(demographicPop[listNum][i]); 
    //     }
    //     return ret; 

    // }

    // //
    // public void UpdateDemographic(int sShop, int gShop, int sComm, int gComm) {

    //     List<int> data = new List<int>() {sShop, gShop, sComm, gComm};
        
    //     if (demographicPop.Count == 0) {
    //         demographicPop.Add(new List<int>() {} );    // Single Shoppers
    //         demographicPop.Add(new List<int>() {} );    // Group Shoppers
    //         demographicPop.Add(new List<int>() {} );    // Single Commuters
    //         demographicPop.Add(new List<int>() {} );    // Group Commuters
    //     } else {

    //         for (int i = 0; i < data.Count; i++) {

    //             if (demographicPop[i].Count == 0) {
    //                 demographicPop[i].Add(new List<int>() {data[i]} );
    //                 demographicPop.Add(new List<int>() {data[i]} );
    //             } else {
    //                 demographicPop[1].Add(num);
    //                 if (demographicPop[0].Count == 0) { demographicPop[0].Add(num); }
    //                 else { demographicPop[0].Add(num - demographicPop[1][demographicPop[1].Count-2]); }
    //             }

    //         }
    //     }

        
        
        
    // }

}
