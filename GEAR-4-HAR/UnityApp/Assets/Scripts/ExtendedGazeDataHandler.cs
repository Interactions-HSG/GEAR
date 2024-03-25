using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARETT;

public class ExtendedGazeDataHandler : MonoBehaviour
{
    private bool _useExtendedAPI;
    private DateTime _lastTimeStampFromLastUpdate;

    public GazeDataSender GazeDataSender;
    
    //public ExtendedEyeGazeDataProvider extendedEyeTrackingDataProvider;
    // Start is called before the first frame update
    void Start()
    {
        _useExtendedAPI = false;
        GazeDataSender = new GazeDataSender();
    }

    // Update is called once per frame
    void Update()
    {
        if (_useExtendedAPI)
        {
            var timeNow = DateTime.Now;
            Debug.Log($"Now: {timeNow}");

            // GetGazeDataFromExtendedAPI(timestamp);

            for (var curTimestamp = timeNow; curTimestamp <= _lastTimeStampFromLastUpdate; curTimestamp = curTimestamp.AddMilliseconds(5))
            {
                GetGazeDataFromExtendedAPI(curTimestamp);
            }
            _lastTimeStampFromLastUpdate = timeNow;

        }
    }

    public void StartGazeDataFromExtendedAPI()
    {
        _useExtendedAPI = true;
        GazeDataSender.CreateEmptyListForNewGazeDataChunk();
    }

    public void StopGazeDataFromExtendedAPI()
    {
        _useExtendedAPI = false;
    }

    private void GetGazeDataFromExtendedAPI(DateTime timestamp)
    {
        Debug.Log("-- GetGazeDataFromExtendedAPI --");
        ExtendedEyeGazeDataProvider extendedEyeTrackingDataProvider = new ExtendedEyeGazeDataProvider();

        var leftGazeReadingInWorldSpace = extendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Left, timestamp);
        var rightGazeReadingInWorldSpace = extendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Right, timestamp);
        var combinedGazeReadingInWorldSpace = extendedEyeTrackingDataProvider.GetWorldSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);

        var combinedGazeReadingInCameraSpace = extendedEyeTrackingDataProvider.GetCameraSpaceGazeReading(ExtendedEyeGazeDataProvider.GazeType.Combined, timestamp);

        var log = $"left: {leftGazeReadingInWorldSpace}";
        log += $"right: {rightGazeReadingInWorldSpace}";
        log += $"combined: {combinedGazeReadingInWorldSpace}";
        log += $"combined camera: {combinedGazeReadingInCameraSpace}";

        Debug.Log(log);

        var gd = new GazeData();
        var date = new DateTime(1970, 1, 1, 0, 0, 0, timestamp.Kind);
        gd.FrameTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        gd.EyeDataTimestamp = System.Convert.ToInt64((timestamp - date).TotalMilliseconds);

      

        gd.GazeOrigin = combinedGazeReadingInWorldSpace.EyePosition;
        gd.GazeDirection = combinedGazeReadingInWorldSpace.GazeDirection;

        if (gd.GazeOrigin != new Vector3(0, 0, 0) || gd.GazeDirection != new Vector3(0, 0, 0))
        {
            gd.IsCalibrationValid = true;
            gd.GazeHasValue = true;
        }
        else
        {
            gd.IsCalibrationValid = false;
            gd.GazeHasValue = false;
        }

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
            // Write all info from the hit to the data object
            gd.GazePoint = hitInfo.point;
        } else
        {
            gd.GazePoint = new Vector3(0, 0, 0);
        }
        
        GazeDataSender.HandleNewGazeData(gd);
    }
}
