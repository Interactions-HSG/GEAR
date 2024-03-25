using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivityReceiver : MonoBehaviour
{

    public GameObject ActivityNotifyContainer;
    public GameObject ActivityText;
    public GameObject SuggestionText;
    public TextMeshProUGUI DebugText;
    public ScrollRect DebugScrollRect;

    public GameObject ButtonYes;

    
    [Header("Handler")]

    public HTTPListener HTTPListener;
    public LogHandler LogHandler;

    public OCRHandler OCRHandler;
    public InspectionModelHandler InspectionModelHandler;
    public SearchEngineHandler SearchEngineHandler;

    public string tmpActivity = "";
    public float tmpProbability = 0f;
    public bool newActivityArrived = false;

    // Start is called before the first frame update
    void Start()
    {
        LogHandler.AppendToCurrentLog("start activity receiver");
        LogHandler.AppendToCurrentLog($"tmpActivity: {tmpActivity}, tmpProbability: {tmpProbability}");
        LogHandler.AppendToCurrentLog($"this ActivityReceiver is {this}");

        //ButtonYes.GetComponent<Interactable>().OnClick.AddListener(OCRHandler.PrintFeedback);
        ButtonYes.GetComponent<Interactable>().OnClick.AddListener(SearchEngineHandler.ShowFeedback);
    }

    // Update is called once per frame
    void Update()
    {
        if (HTTPListener.httpNewActivityArrived)
        {
            LogHandler.AppendToCurrentLog($"HTTPListener.\ntmpActivity: {HTTPListener.httpTmpActivity}, tmpProbability: { HTTPListener.httpTmpProbability}");
            ReceiveNewActivity(HTTPListener.httpTmpActivity, HTTPListener.httpTmpProbability);
            LogHandler.AppendToCurrentLog($"new activity in update loop. newActivityArrived: {HTTPListener.httpNewActivityArrived}");
            HTTPListener.httpNewActivityArrived = false;
            HTTPListener.httpTmpActivity = "";
            HTTPListener.httpTmpProbability = 0f;
        }


        if (newActivityArrived)
        {
            LogHandler.AppendToCurrentLog($"tmpActivity: {tmpActivity}, tmpProbability: {tmpProbability}");
            ReceiveNewActivity(tmpActivity, tmpProbability);
            LogHandler.AppendToCurrentLog($"new activity in update loop. newActivityArrived: {newActivityArrived}");
            newActivityArrived = false;
            tmpActivity = "";
            tmpProbability = 0f;
        } 
        
    }


    private void ReceiveNewActivity(string activity, float probability)
    {
        LogHandler.AppendToCurrentLog($"Displaying new activity: {activity}");
        DebugText.text += $"\n[{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}]";
        DebugText.text += $"\nDisplaying new activity: {activity}";
        var probPercent = probability.ToString("P", CultureInfo.InvariantCulture);
        ActivityText.GetComponent<TextMeshPro>().text = $"{activity} ({probPercent}).";
        var suggestion = "";

        switch (activity)
        { 
            case "Reading":
                suggestion = "Should I translate this text for you?";
                /*
                    - Once the YES button is clicked
                    - Take a screenshot (Idelall the user alligns the HMDcamera with the text)
                    - DoOCR ()
                    - Receive the text
                    - DoTranslate ()
                    - Receive the translated text
                    - Display the translated text
                */

                ButtonYes.GetComponent<Interactable>().OnClick.AddListener(OCRHandler.PrintFeedback);
                
                break;
            case "Inspection":
                suggestion = "Should I open additional product information for you?";

                    /*
                        - Show the 3D model once the YES button is clicked
                     */
                ButtonYes.GetComponent<Interactable>().OnClick.AddListener(InspectionModelHandler.ShowFeedback);
                                
                break;
            case "Search":
                suggestion = "Should I open a semantic hypermedia search engine for you?";

                ButtonYes.GetComponent<Interactable>().OnClick.AddListener(SearchEngineHandler.ShowFeedback);

                break;
            default:
                break;
        }

        SuggestionText.GetComponent<TextMeshPro>().text = $"{suggestion}";
        ActivityNotifyContainer.SetActive(true);
        LogHandler.WriteCurrentLogToFile();
       
    }
}
