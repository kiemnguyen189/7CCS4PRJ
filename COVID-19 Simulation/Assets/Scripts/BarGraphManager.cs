using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarGraphManager : MonoBehaviour
{

    public Image background;
    public TextMeshProUGUI title;
    public Transform bar;
    public Transform xAxis;
    public Transform yAxis;
    public Transform tick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    public void CreateBarGraph(List<int> data, string title) {
        Debug.Log(data.Count + " " + title);
    }

}
