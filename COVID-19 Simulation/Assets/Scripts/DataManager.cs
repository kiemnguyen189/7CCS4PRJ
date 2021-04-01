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
    public List<List<int>> infections = new List<List<int>>();

    // List of population demographic numbers.
    // Outermost list contains 4 elements: SingleShopperList, GroupShopperList, SingleCommuterList and GroupCommuterList.
    // Middle lists contains 2 elements: Hourly and Cumulative.
    // Innermost list contains hourly data. Length is in multiples of 24.
    public List<List<List<int>>> demographicPop = new List<List<List<int>>>();
    public List<List<List<int>>> infectedPop = new List<List<List<int>>>();

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
        infections.Clear();
        demographicPop.Clear();
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
    // Populations are counted hourly by default in SimManager.
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

    // Returns a 2D list of population demographics.
    // List structure: Outer list contains 4 lists each of size in multiples of 24.
    public List<List<int>> GetDemographic(bool isCumulative, int day) {

        List<List<int>> ret = new List<List<int>>();
        int listNum = 0;
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        if (!isCumulative) { listNum = 0;} 
        else { listNum = 1; }
        // Create temporary sub list out of the total population data to return.
        for (int i = 0; i < 4; i++) { 
            ret.Add( new List<int>() );
            for (int j = startIndex; j < startIndex+24; j++) { 
                ret[i].Add(demographicPop[i][listNum][j]); 
            }
        }
        return ret; 
    }

    //
    public void UpdateDemographic(int sShop, int gShop, int sComm, int gComm) {

        List<int> data = new List<int>() {sShop, gShop, sComm, gComm};
        if (demographicPop.Count == 0) {
            demographicPop.Add(new List<List<int>>() { new List<int>() {sShop}, new List<int>() {sShop} } );    // Single Shoppers
            demographicPop.Add(new List<List<int>>() { new List<int>() {gShop}, new List<int>() {gShop} } );    // Group Shoppers
            demographicPop.Add(new List<List<int>>() { new List<int>() {sComm}, new List<int>() {sComm} } );    // Single Commuters
            demographicPop.Add(new List<List<int>>() { new List<int>() {gComm}, new List<int>() {gComm} } );    // Group Commuters
        } else {
            for (int i = 0; i < demographicPop.Count; i++) {
                demographicPop[i][0].Add(data[i]);
                if ((demographicPop[i][1].Count%24) == 0) { demographicPop[i][1].Add(data[i]); }
                else { demographicPop[i][1].Add(data[i] + demographicPop[i][1][demographicPop[i][1].Count-1]); }
            }
        }
        
    }

    // Returns a 2D list of population demographics.
    // List structure: Outer list contains 4 lists each of size in multiples of 24.
    public List<List<int>> GetInfected(bool isCumulative, int day) {

        List<List<int>> ret = new List<List<int>>();
        int listNum = 0;
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        if (!isCumulative) { listNum = 0;} 
        else { listNum = 1; }
        // Create temporary sub list out of the total population data to return.
        for (int i = 0; i < 2; i++) { 
            ret.Add( new List<int>() );
            for (int j = startIndex; j < startIndex+24; j++) { 
                ret[i].Add(infectedPop[i][listNum][j]); 
            }
        }
        return ret; 
    }

    //
    public void UpdateInfected(int sus, int inf) {

        List<int> data = new List<int>() {sus, inf};
        if (infectedPop.Count == 0) {
            infectedPop.Add(new List<List<int>>() { new List<int>() {sus}, new List<int>() {sus} } );    // Susceptible
            infectedPop.Add(new List<List<int>>() { new List<int>() {inf}, new List<int>() {inf} } );    // Infected
        } else {
            for (int i = 0; i < infectedPop.Count; i++) {
                infectedPop[i][0].Add(data[i]);
                if ((infectedPop[i][1].Count%24) == 0) { infectedPop[i][1].Add(data[i]); }
                else { infectedPop[i][1].Add(data[i] + infectedPop[i][1][infectedPop[i][1].Count-1]); }
            }
        }
        
    }

    // Gets the list of Infection numbers. If isCumulative is true, return cumulative numbers, else return hourly.
    // Returns a list of 24 elements depending on parameters.
    public List<int> GetInfections(bool isCumulative, int day) {
        List<int> ret = new List<int>();
        int listNum = 0;
        int startIndex = ((day*24) - 24);
        // Whether or not to return an hourly or cumulative graph.
        if (!isCumulative) { listNum = 0;} 
        else { listNum = 1; }
        // Create temporary sub list out of the total population data to return.
        for (int i = startIndex; i < startIndex+24; i++) { ret.Add(infections[listNum][i]); }
        return ret; 
    }

    // Updates the number of hourly and cumulative hourly infections.
    // Infections are counted cumulatively by default in SimManager, therefore lists 0 and 1 are reversed.
    public void UpdateInfections(int num) {
        if (infections.Count == 0) {
            infections.Add(new List<int>() {num} );
            infections.Add(new List<int>() {num} );
        } else {
            infections[1].Add(num);
            if (infections[0].Count == 0) { infections[0].Add(num); }
            else { infections[0].Add(num - infections[1][infections[1].Count-2]); }
        }
    }

    

}
