using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using ARETT;
using System.Collections.Concurrent;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.XR;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class ARETTGazeDataHandler : MonoBehaviour
{

    public DataProvider DataProvider;
    private ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();

    public LogHandler LogHandler;

    public GameObject ButtonsContainer;
    public GameObject StartButton;
    public GameObject StopButton;
    public TextMeshProUGUI DebugText;
    public ScrollRect DebugScrollRect;
   
   
    public GazeDataSender GazeDataSender;

    // Start is called before the first frame update
    void Start()
    {
        // GazeDataSender = new GazeDataSender();

        print("DataProvider.EyesApiAvailable: " + DataProvider.EyesApiAvailable);
        print("DataProvider.IsGazeCalibrationValid: " + DataProvider.IsGazeCalibrationValid);
        print("IsEyeTrackingEnabled: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled);
        print("IsEyeCalibrationValid: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeCalibrationValid);
        print("IsEyeTrackingDataValid: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid);

        var txt = "DataProvider.EyesApiAvailable: " + DataProvider.EyesApiAvailable;
        txt += "\nDataProvider.IsGazeCalibrationValid: " + DataProvider.IsGazeCalibrationValid;
        txt += "\nIsEyeTrackingEnabled: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled;
        txt += "\nIsEyeCalibrationValid: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeCalibrationValid;
        txt += "\nIsEyeTrackingDataValid: " + CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid;

        DebugText.text = txt;
    }

    // Update is called once per frame
    void Update()
    {

        // Check if there is something to process
        if (!_mainThreadWorkQueue.IsEmpty)
        {
            // Comment from ARETT:
            // Process all commands which are waiting to be processed
            // Note: This isn't 100% thread save as we could end in a loop when there is still new data coming in.
            //       However, data is added slowly enough so we shouldn't run into issues.
            while (_mainThreadWorkQueue.TryDequeue(out Action action))
            {
                // Invoke the waiting action
                action.Invoke();
            }
        }

        //DebugText.text += "\n+++";
        //StartCoroutine(PushToBottom());

    }

    IEnumerator PushToBottom()
    {
        yield return new WaitForEndOfFrame();
        DebugScrollRect.verticalNormalizedPosition = 0;
        // Canvas.ForceUpdateCanvases();
        //DebugScrollRect.normalizedPosition = new Vector2(0, 0);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)DebugScrollRect.transform);
    }


    /// <summary>
    /// Calls the function to start getting data from ARETT.
    /// </summary>
    public void StartCollectingGazeData()
    {
        StartArettData();
        GazeDataSender.CreateEmptyListForNewGazeDataChunk();
        LogHandler.AppendToCurrentLog("-- StartCollectingGazeData");
        DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}]";
        DebugText.text += "\nStarted collecting gaze data with ARETT.";
        StartCoroutine(PushToBottom());
    }

    /// <summary>
    /// Calls the function to stop getting data from ARETT.
    /// </summary>
    public void StopCollectingGazeData()
    {
        StopArettData();
        LogHandler.AppendToCurrentLog("-- StopCollectingGazeData");
        DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}]";
        DebugText.text += "\nStopped collecting gaze data with ARETT.";
        StartCoroutine(PushToBottom());
    }


   
    /// <summary>
    /// Starts the Coroutine to get Eye tracking data on the HL2 from ARETT.
    /// </summary>
    private void StartArettData()
    {
        StartCoroutine(StartGettingDataCoroutine());
       
    }

    /// <summary>
    /// Subscribes to newDataEvent from ARETT.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGettingDataCoroutine()
    {
        _mainThreadWorkQueue.Enqueue(() =>
        {
            DataProvider.NewDataEvent += HandleDataFromARETT;
        });
        
        print("subscribed to ARETT events");
        yield return null;

    }

    /// <summary>
    /// Unsubscribes from NewDataEvent from ARETT.
    /// </summary>
    private void StopArettData()
    {
        _mainThreadWorkQueue.Enqueue(() =>
        {
            DataProvider.NewDataEvent -= HandleDataFromARETT;
        });

    }


    /// <summary>
    /// Handles what happens with the Gaze data from ARETT.
    /// </summary>
    /// <param name="gd">gaze data from ARETT</param>
    private void GetDataFromARETT(GazeData gd)
    {
        HandleDataFromARETT(gd);
    }



    /// <summary>
    /// Handles gaze data from ARETT and prepares it for sending to desktop
    /// </summary>
    /// <param name="gd"></param>
    /// <returns></returns>
    private void HandleDataFromARETT(GazeData gd)
    {
        // some examplary values from ARETT, for a full ist see: https://github.com/AR-Eye-Tracking-Toolkit/ARETT/wiki/Log-Format
        string t = "received GazeData\n";
        t += "EyeDataRelativeTimestamp:" + gd.EyeDataRelativeTimestamp;
        t += "\ngazedirection: " + gd.GazeDirection;
        t += "\nGazePointWebcam: " + gd.GazePointWebcam;
        t += "\nGazeHasValue: " + gd.GazeHasValue;
        t += "\nGazePoint: " + gd.GazePoint;
        t += "\nGazePointMonoDisplay: " + gd.GazePointMonoDisplay;
        t += "\nGazePointAOIHitPosition: " + gd.GazePointAOIHitPosition;
        t += "\nIsCalibrationValid: " + gd.IsCalibrationValid;

        GazeDataSender.HandleNewGazeData(gd, "Data_from_ARETT");
        LogHandler.AppendToCurrentLog(t);


    }

    
}
