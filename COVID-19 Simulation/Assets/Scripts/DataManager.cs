﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    
    public SimManager manager;

    public List<int> hourlyPop;
    public List<int> cumulativePop;
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

    // Updates the number of hourly populations.
    public void UpdateHourlyPop(int num) {
        Debug.Log("Hourly: " + num);
        hourlyPop.Add(num);
    }

    // Updates the cumulative number of hourly populations.
    public void UpdateCumulativePop(int num) {
        // Get latest hourly population number.
        if (cumulativePop.Count == 0) {
            cumulativePop.Add(num);
            Debug.Log("Cumulative: " + num);
        } else {
            int newNum = num + cumulativePop[cumulativePop.Count-1];
            Debug.Log("Cumulative: " + newNum);
            // Append total of latest and current population numbers.
            cumulativePop.Add(newNum);
        }
        
    }

}
