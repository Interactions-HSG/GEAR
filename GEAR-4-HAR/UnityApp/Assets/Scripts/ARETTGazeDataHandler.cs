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

public class ARETTGazeDataHandler : MonoBehaviour
{

    public DataProvider DataProvider;
    private ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();
   

    public GameObject ButtonsContainer;
    public GameObject StartButton;
    public GameObject StopButton;
   
    private GazeDataSender _gazeDataSender;

    // Start is called before the first frame update
    void Start()
    {
        _gazeDataSender = new GazeDataSender();
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
    }

    /// <summary>
    /// Calls the function to start getting data from ARETT.
    /// </summary>
    public void StartCollectingGazeData()
    {
        StartArettData();
        _gazeDataSender.CreateEmptyListForNewGazeDataChunk();
    }

    /// <summary>
    /// Calls the function to stop getting data from ARETT.
    /// </summary>
    public void StopCollectingGazeData()
    {
        StopArettData();
    }


   
    /// <summary>
    /// Starts the Coroutine to get Eye tracking data on the HL2 from ARETT.
    /// </summary>
    public void StartArettData()
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
    public void StopArettData()
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
    public void GetDataFromARETT(GazeData gd)
    {
        HandleDataFromARETT(gd);
    }



    /// <summary>
    /// Handles gaze data from ARETT and prepares it for sending to desktop
    /// </summary>
    /// <param name="gd"></param>
    /// <returns></returns>
    public void HandleDataFromARETT(GazeData gd)
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
        Debug.Log(t);

        _gazeDataSender.HandleNewGazeData(gd);
       
    }

    
}
