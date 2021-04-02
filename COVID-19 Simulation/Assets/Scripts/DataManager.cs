using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public SimManager manager;

    // List of population numbers.
    // Outer list contains 2 elements: HourlyList and Cumulative List.
    // Inner lists contain hourly data. Length is in multiples of 24.
    public List<List<List<int>>> population = new List<List<List<int>>>();

    // List of population demographic numbers.
    // Outermost list contains 4 elements: SingleShopperList, GroupShopperList, SingleCommuterList and GroupCommuterList.
    // Middle lists contains 2 elements: Hourly and Cumulative.
    // Innermost list contains hourly data. Length is in multiples of 24.
    public List<List<List<int>>> demographicPop = new List<List<List<int>>>();

    public List<List<List<int>>> infections = new List<List<List<int>>>();
    public List<List<List<int>>> infectedPop = new List<List<List<int>>>();


    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<SimManager>();
    }

    //
    public void ResetData() {

        population.Clear();
        infections.Clear();
        demographicPop.Clear();
        infectedPop.Clear();

    }

    //
    public List<List<int>> GetPopulation(bool isCumulative, int day) { return GetData(isCumulative, day, population); }
    public void UpdatePopulation(List<int> data, bool isHourly) { UpdateData(data, population, isHourly); }

    //
    public List<List<int>> GetInfections(bool isCumulative, int day) { return GetData(isCumulative, day, infections); }
    public void UpdateInfections(List<int> data, bool isHourly) { UpdateData(data, infections, isHourly); }

    //
    public List<List<int>> GetDemographic(bool isCumulative, int day) { return GetData(isCumulative, day, demographicPop); }
    public void UpdateDemographic(List<int> data, bool isHourly) { UpdateData(data, demographicPop, isHourly); }

    //
    public List<List<int>> GetInfected(bool isCumulative, int day) { return GetData(isCumulative, day, infectedPop); }
    public void UpdateInfected(List<int> data, bool isHourly) { UpdateData(data, infectedPop, isHourly); }

    //
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

    //
    public void UpdateData(List<int> data, List<List<List<int>>> list, bool isHourly) {

        if (list.Count == 0) {
            for (int i = 0; i < data.Count; i++) {
                list.Add(new List<List<int>>() { new List<int>() {data[i]}, new List<int>() {data[i]} });
            }
        } else {
            for (int i = 0; i < data.Count; i++) {
                list[i][0].Add(data[i]);
                if (isHourly) {
                    if ((list[i][1].Count%24) == 0) { list[i][1].Add(data[i]); }
                    else { list[i][1].Add(data[i] + list[i][1][list[i][1].Count-1]); }
                } else {
                    list[i][1].Add(data[i]);
                }
            }
        }

    }


}
