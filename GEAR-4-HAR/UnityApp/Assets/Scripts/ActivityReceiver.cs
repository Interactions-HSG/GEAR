using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class ActivityReceiver : MonoBehaviour
{

    public GameObject ActivityNotifyContainer;
    public GameObject ActivityText;
    public GameObject SuggestionText;

    public HTTPListener HTTPListener;

    public string tmpActivity = "";
    public float tmpProbability = 0f;
    public bool newActivityArrived = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start activity receiver");
        Debug.Log($"tmpActivity: {tmpActivity}, tmpProbability: {tmpProbability}");
        Debug.Log($"this ActivityReceiver is {this}");
    }

    // Update is called once per frame
    void Update()
    {
        if (HTTPListener.httpNewActivityArrived)
        {
            Debug.Log($"HTTPListener.\ntmpActivity: {HTTPListener.httpTmpActivity}, tmpProbability: { HTTPListener.httpTmpProbability}");
            ReceiveNewActivity(HTTPListener.httpTmpActivity, HTTPListener.httpTmpProbability);
            Debug.Log($"new activity in update loop. newActivityArrived: {HTTPListener.httpNewActivityArrived}");
            HTTPListener.httpNewActivityArrived = false;
            HTTPListener.httpTmpActivity = "";
            HTTPListener.httpTmpProbability = 0f;
        }


        if (newActivityArrived)
        {
            Debug.Log($"tmpActivity: {tmpActivity}, tmpProbability: {tmpProbability}");
            ReceiveNewActivity(tmpActivity, tmpProbability);
            Debug.Log($"new activity in update loop. newActivityArrived: {newActivityArrived}");
            newActivityArrived = false;
            tmpActivity = "";
            tmpProbability = 0f;
        } 
        
    }


    private void ReceiveNewActivity(string activity, float probability)
    {
            Debug.Log($"Displaying new activity: {activity}");
            var probPercent = probability.ToString("P", CultureInfo.InvariantCulture);
            ActivityText.GetComponent<TextMeshPro>().text = $"{activity} ({probPercent}).";
            var suggestion = "";

            switch (activity)
            { 
                case "Reading":
                    suggestion = "Should I translate this text for you?";
                    break;
                case "Inspection":
                    suggestion = "Should I open a technical drawing for you?";
                    break;
                case "Search":
                    suggestion = "Should I open a semantic hypermedia search engine for you?";
                    break;
                default:
                    break;
            }

            SuggestionText.GetComponent<TextMeshPro>().text = $"{suggestion}";
            ActivityNotifyContainer.SetActive(true);
       
    }
}
