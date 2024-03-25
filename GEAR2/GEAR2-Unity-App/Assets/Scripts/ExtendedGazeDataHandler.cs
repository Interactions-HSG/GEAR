using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARETT;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

public class ExtendedGazeDataHandler : MonoBehaviour
{
    private static bool _useExtendedAPI;
    private DateTime _lastTimeStampFromLastUpdate;

    public GazeDataSender GazeDataSender;
    public ExtendedEyeGazeDataProvider ExtendedEyeTrackingDataProvider;

    public TextMeshProUGUI DebugText;
    public ScrollRect DebugScrollRect;
    public LogHandler LogHandler;

    private int _etSampleCounter;
    private int _etSampleCounterAfterGetGaze;

    //public ExtendedEyeGazeDataProvider extendedEyeTrackingDataProvider;
    // Start is called before the first frame update
    void Start()
    {
        _useExtendedAPI = false;
        //  GazeDataSender = new GazeDataSender();
        _lastTimeStampFromLastUpdate = DateTime.UtcNow;
        _etSampleCounter = 0;
        _etSampleCounterAfterGetGaze = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (_useExtendedAPI)
        {

        }
        //*/

    }

    IEnumerator GazeCollector()
    {
        while (_useExtendedAPI)
        {
            /*
            var currentTime = DateTime.UtcNow.ToLocalTime();
            var log = "";
            log += $"\ndiff to last update in ms: {(currentTime - _lastTimeStampFromLastUpdate).TotalMilliseconds}";
            log += $"\nNow: {currentTime.ToString("hh:mm:ss:ffff")}";
            Debug.Log(log);
            GetGazeDataFromExtendedAPI(currentTime);
            _lastTimeStampFromLastUpdate = currentTime;
            //*/
            CollectGazeData();
            yield return new WaitForSeconds(3f);
        }
        yield break;
    }

    public  void CollectGazeData()
    {
        //var log = "";
       //log += "-- Using extended API";
       LogHandler.AppendToCurrentLog("--- Using extended API in CollectGazeData() ");
       var currentTime = DateTime.UtcNow;
        /*
       log += $"\nNow: {currentTime.ToString("hh:mm:ss:ffff")}";
       log += $"\n_lastTimeStampFromLastUpdate: {_lastTimeStampFromLastUpdate.ToLocalTime()}";
       log += $"\ndiff to last update in ms: {(currentTime - _lastTimeStampFromLastUpdate).TotalMilliseconds}";
       log += $"\ndiff to last update in s: {(currentTime - _lastTimeStampFromLastUpdate).TotalSeconds}";
       log += $"\n_lastTimeStampFromLastUpdate.AddSeconds(5): {_lastTimeStampFromLastUpdate.AddSeconds(2).ToLocalTime()}";
        //*/
        
       LogHandler.AppendToCurrentLog($"Now: {currentTime.ToString("hh:mm:ss:ffff")}");
       LogHandler.AppendToCurrentLog($"_lastTimeStampFromLastUpdate: {_lastTimeStampFromLastUpdate.ToLocalTime()}");
       LogHandler.AppendToCurrentLog($"diff to last update in ms: {(currentTime - _lastTimeStampFromLastUpdate).TotalMilliseconds}");
       LogHandler.AppendToCurrentLog($"diff to last update in s: {(currentTime - _lastTimeStampFromLastUpdate).TotalSeconds}");
       LogHandler.AppendToCurrentLog($"_lastTimeStampFromLastUpdate.AddSeconds(5): {_lastTimeStampFromLastUpdate.AddSeconds(2).ToLocalTime()}");

        //Debug.Log(log);
        GetGazeDataFromExtendedAPI(currentTime);
        _etSampleCounter++;

        /*
        // var endTime = DateTimeOffset.Now;
        //var currentTime = timeNow;
        //Debug.Log($"\nendTime: {endTime.ToString("hh:mm:ss:ffff")}");
        // var startTime = endTime - TimeSpan.FromSeconds(1);
        //var lastTime = timeNow.AddSeconds(-1);
        //Debug.Log($"\ncurrentTime: {currentTime.ToString("hh:mm:ss:ffff")}");

        var lastTime = _lastTimeStampFromLastUpdate;

        while (lastTime < currentTime)
        {
            log = "";
            GetGazeDataFromExtendedAPI(currentTime);
            _etSampleCounter++;
            lastTime = lastTime.AddSeconds(0.0003f);
            log += "-- in loop";
            // lastTime = DateTimeOffset.FromUnixTimeSeconds(lastTimeDataReceived).DateTime;
            log += $"\nIncrement currentTime: {lastTime.ToString("hh:mm:ss:ffff")}";
            log += $"\n_etSampleCounter: {_etSampleCounter}";
            Debug.Log(log);

        }
        //*/


        /*
        for (var curTimestamp = timeNow; curTimestamp <= _lastTimeStampFromLastUpdate; curTimestamp = curTimestamp.AddMilliseconds(5))
        {
            GetGazeDataFromExtendedAPI(curTimestamp);
            _etSampleCounter++;
        }
        */
        _lastTimeStampFromLastUpdate = currentTime;
        // Debug.Log(log);
    }



    public void StartGazeDataFromExtendedAPI()
    {
        _useExtendedAPI = true;
        GazeDataSender.CreateEmptyListForNewGazeDataChunk();
        //Debug.Log("-- StartGazeDataFromExtendedAPI");
        LogHandler.AppendToCurrentLog("--- StartGazeDataFromExtendedAPI");
        
       
        DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}]";
        DebugText.text += "\nStarted collecting gaze data with Extended API.";
        // GetGazeDataFromExtendedAPI(DateTime.UtcNow.ToLocalTime());
        StartCoroutine(PushToBottom());
        StartCoroutine(GazeCollector());

    }

    public void StopGazeDataFromExtendedAPI()
    {
        _useExtendedAPI = false;
        //Debug.Log("-- StopGazeDataFromExtendedAPI");
        LogHandler.AppendToCurrentLog("--- StopGazeDataFromExtendedAPI");
        DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}]";
        DebugText.text += "\nStopped collecting gaze data with Extended API.";
        StartCoroutine(PushToBottom());
        StopCoroutine(GazeCollector());
    }

    IEnumerator PushToBottom()
    {
        yield return new WaitForEndOfFrame();
        DebugScrollRect.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)DebugScrollRect.transform);
    }

    private void GetGazeDataFromExtendedAPI(DateTime timestamp)
    {
        try
        {
            LogHandler.AppendToCurrentLog("-- GetGazeDataFromExtendedAPI --");
            if (timestamp == null)
            {
                //Debug.Log("Timestamp is zero");
                LogHandler.AppendToCurrentLog("Timestamp is zero");
                return;
            }
            //var log = "";
            //log += "-- GetGazeDataFromExtendedAPI --";
            
            // ExtendedEyeGazeDataProvider extendedEyeTrackingDataProvider = new ExtendedEyeGazeDataProvider();
            ExtendedEyeTrackingDataProvider.enabled = true;
            //log += $"\nExtendedEyeTrackingDataProvider.enabled: {ExtendedEyeTrackingDataProvider.enabled}";
            LogHandler.AppendToCurrentLog($"\nExtendedEyeTrackingDataProvider.enabled: {ExtendedEyeTrackingDataProvider.enabled}");

            //var leftGazeReadingInWorldSpace = ExtendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
            //var rightGazeReadingInWorldSpace = ExtendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
            // var combinedGazeReadingInWorldSpace = ExtendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);

            //var combinedGazeReadingInCameraSpace = ExtendedEyeTrackingDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);

            var etGazeDataFromLastSecond = ExtendedEyeTrackingDataProvider.GetGazeDataFromLastNSeconds(3);
            //log += $"\netGazeDataFromLastSecond.Count: {etGazeDataFromLastSecond.Count}";
            LogHandler.AppendToCurrentLog($"GetGazeDataFromLastSecond.Count: {etGazeDataFromLastSecond.Count}");

            if (etGazeDataFromLastSecond.Count < 1)
            {
                //log += $"\nno gaze data from last second";
                LogHandler.AppendToCurrentLog($"no gaze data from last second");
                
                //Debug.Log(log);
                return;
            }
            //Debug.Log(log);

            var forEachCounter = 0;

            var t = "";
            foreach (var etGazeData in etGazeDataFromLastSecond)
            {
                if (etGazeData != null)
                {
                    t += $"{etGazeData.Timestamp}: ({etGazeData.EyePosition.x},  {etGazeData.EyePosition.y}, {etGazeData.EyePosition.z})\n";
                }
            }
            LogHandler.AppendToCurrentLog("included gaze samples:");
            LogHandler.AppendToCurrentLog(t);
            //Debug.Log(t);


            //foreach (var etGazeData in etGazeDataFromLastSecond)
            for (int i = 0; i < etGazeDataFromLastSecond.Count; i++)
            {
                var gd = new GazeData();

                var date = new DateTime(1970, 1, 1, 0, 0, 0, timestamp.Kind);

                gd.FrameTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                gd.EyeDataTimestamp = System.Convert.ToInt64((DateTime.UtcNow.ToLocalTime() - date).TotalMilliseconds);


                if (etGazeDataFromLastSecond[i].EyePosition != null)
                {
                    gd.EyeDataTimestamp = etGazeDataFromLastSecond[i].Timestamp;
                    // gd.EyeDataTimestamp = combinedGazeReadingInWorldSpace.Timestamp;
                    //log += $"\nleft pos: {leftGazeReadingInWorldSpace.EyePosition}, ";
                    //log += $"\nright pos: {rightGazeReadingInWorldSpace.EyePosition}, ";
                    var log2 = $"combined pos: ({etGazeDataFromLastSecond[i].EyePosition.x},  {etGazeDataFromLastSecond[i].EyePosition.y}, {etGazeDataFromLastSecond[i].EyePosition.z}) ";
                    LogHandler.AppendToCurrentLog($"combined pos: ({etGazeDataFromLastSecond[i].EyePosition.x},  {etGazeDataFromLastSecond[i].EyePosition.y}, {etGazeDataFromLastSecond[i].EyePosition.z})");

                    //log += $"\ncombined camera pos: {combinedGazeReadingInCameraSpace.EyePosition}, ";




                    LogHandler.AppendToCurrentLog("\nIncoming gaze data is valid");
                    gd.GazeOrigin = etGazeDataFromLastSecond[i].EyePosition;
                    gd.GazeDirection = etGazeDataFromLastSecond[i].GazeDirection;

                    gd.IsCalibrationValid = true;
                    gd.GazeHasValue = true;

                    // from ARETT (DataProvider.cs)
                    // Create a gaze ray based on the gaze data
                    Ray gazeRay = new Ray(gd.GazeOrigin, gd.GazeDirection);

                    ////
                    // The 3D gaze point is the actual position the wearer is looking at.
                    // As everything apart from the eye tracking layers is visible, we have to collide the gaze with every layer except the eye tracking layers

                    // Check if the gaze hits anything that isn't an AOI
                    gd.GazePointHit = Physics.Raycast(gazeRay, out RaycastHit hitInfo, Mathf.Infinity);

                    // If we hit something, write the hit info to the data
                    if (gd.GazePointHit)
                    {
                        //log2 += $"\nhit point: {hitInfo.point}";
                        LogHandler.AppendToCurrentLog($"\nhit point: {hitInfo.point}");
                        // Write all info from the hit to the data object
                        gd.GazePoint = hitInfo.point;
                    }
                    else
                    {
                        gd.GazePoint = new Vector3(0, 0, 0);
                    }
                    //log2 += $"\ncurrent index: {i}";
                    LogHandler.AppendToCurrentLog($"current index: {i}");
                    //Debug.Log(log2);
                    GazeDataSender.HandleNewGazeData(gd, $"Data_from_Extended_ET_API_valid_{i}");
                }
                else
                {
                    //Debug.Log("\nno gaze data available");
                    LogHandler.AppendToCurrentLog("No gaze data available or Incoming gaze data is invalid");
                    //Debug.Log("\nIncoming gaze data is invalid");
                    gd.GazeOrigin = new Vector3(0, 0, 0);
                    gd.GazeDirection = new Vector3(0, 0, 0);
                    gd.IsCalibrationValid = false;
                    gd.GazeHasValue = false;
                    gd.GazePointHit = false;
                    gd.GazePoint = new Vector3(0, 0, 0);
                    GazeDataSender.HandleNewGazeData(gd, "Data_from_Extended_ET_API_invalid");
                }

                
                forEachCounter++;
                //Debug.Log($"\n_forEachCounter: {forEachCounter}");
                LogHandler.AppendToCurrentLog($"\n_forEachCounter: {forEachCounter}");
            }
            //var log3 = "";
            _etSampleCounterAfterGetGaze++;
            //log3 += $"\n_etSampleCounter_: {_etSampleCounter}";
            LogHandler.AppendToCurrentLog($"\n_etSampleCounter_: {_etSampleCounter}");
            //log3 += $"\n_etSampleCounter_aftergetgaze: {_etSampleCounterAfterGetGaze}";
            LogHandler.AppendToCurrentLog($"\n_etSampleCounter_aftergetgaze: {_etSampleCounterAfterGetGaze}");
            //Debug.Log(log3);
            LogHandler.AppendToCurrentLog("-- GetGazeDataFromExtendedAPI -- end --");
        }
        catch (Exception e)
        {
            //Debug.Log($"-- GetGazeDataFromExtendedAPI - Exception occured: \n{e}");
            LogHandler.AppendToCurrentLog($"-- GetGazeDataFromExtendedAPI - Exception occured: \n{e}");
            throw;
        }
       

    }
}
