// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using UnityEngine;
using System;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.EyeTracking;
using System.Collections.Generic;

/// <summary>
/// This class provides access to the Extended Eye Gaze Tracking API 
/// Values are given in Unity world space or relative to the main camera
/// </summary>
[DisallowMultipleComponent]
public class ExtendedEyeGazeDataProvider : MonoBehaviour
{
    public enum GazeType
    {
        Left,
        Right,
        Combined
    }

    public class GazeReading
    {
        public Vector3 EyePosition;
        public Vector3 GazeDirection;
        public long Timestamp;
        public GazeReading() { }
        public GazeReading(Vector3 position, Vector3 direction)
        {
            EyePosition = position;
            GazeDirection = direction;
        }
    }

    public LogHandler LogHandler;
    public bool DoneReadingCurrentGazeData;

    private Camera _mainCamera;
    private EyeGazeTrackerWatcher _watcher;
    private EyeGazeTracker _eyeGazeTracker;
    private EyeGazeTrackerReading _eyeGazeTrackerReading;
    private System.Numerics.Vector3 _trackerSpaceGazeOrigin;
    private System.Numerics.Vector3 _trackerSpaceGazeDirection;
    private GazeReading _gazeReading = new GazeReading();
    private GazeReading _transformedGazeReading = new GazeReading();
    private bool _gazePermissionEnabled;
    private bool _readingSucceeded;
    private SpatialGraphNode _eyeGazeTrackerNode;
    private Pose _eyeGazeTrackerPose;


    /// <summary>
    /// Get the current reading for the requested GazeType, relative to the main camera
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <returns></returns>
    public GazeReading GetCameraSpaceGazeReading(GazeType gazeType)
    {
        return GetCameraSpaceGazeReading(gazeType, DateTime.UtcNow);
    }

    /// <summary>
    /// Get the reading for the requested GazeType at the given TimeStamp, relative to the main camera
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public GazeReading GetCameraSpaceGazeReading(GazeType gazeType, DateTime timestamp)
    {
        if (GetWorldSpaceGazeReading(gazeType, timestamp) == null)
        {
            return null;
        }

        _transformedGazeReading.EyePosition = _mainCamera.transform.InverseTransformPoint(_gazeReading.EyePosition);
        _transformedGazeReading.GazeDirection = _mainCamera.transform.InverseTransformDirection(_gazeReading.GazeDirection).normalized;

        return _transformedGazeReading;
    }

    /// <summary>
    /// Get the current reading for the requested GazeType
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <returns></returns>
    public GazeReading GetWorldSpaceGazeReading(GazeType gazeType)
    {
        return GetWorldSpaceGazeReading(gazeType, DateTime.UtcNow);
    }



    public List<GazeReading> GetGazeDataFromLastNSeconds(int timespan)
    {
        var gazeList = new List<GazeReading>();
        if (!_gazePermissionEnabled || _eyeGazeTracker == null)
        {
            return gazeList;
        }

        //var log = "-- GetGazeDataFromLastSecond";
        LogHandler.AppendToCurrentLog($"--- GetGazeDataFromLastNSeconds: {timespan}");
        var sampleCounter = 0;
        var sampleCounterMissed= 0;
        var sampleCounterMissedTryLocate = 0;
        var duplicateCounter = 0;
        // Record every 1 second
        var endTime = DateTimeOffset.Now;
        // var startTime = endTime - TimeSpan.FromSeconds(1);
        var currentTime = endTime.AddSeconds(-timespan);

        //log += $"\ndiff to start: {(currentTime - endTime).TotalMilliseconds}";
        LogHandler.AppendToCurrentLog($"diff to start: {(currentTime - endTime).TotalMilliseconds}");


        while (currentTime < endTime)
        {
            // Debug.Log($"in loop at: {currentTime.DateTime.ToString("hh:mm:ss:ffffff")}");
            LogHandler.AppendToCurrentLog($"\nin loop at: {currentTime.DateTime.ToString("hh:mm:ss:ffffff")}");
            /*
            var gazeReading = GetWorldSpaceGazeReading(GazeType.Combined, currentTime.DateTime);
            if (gazeReading != null)
            {
                gazeList.Add(gazeReading);
                sampleCounter++;
                Debug.Log($"\nsample num: {sampleCounter}");
            } else
            {
                sampleCounterMissed++;
                Debug.Log($"\nsample num missed: {sampleCounterMissed}");
            }
            currentTime = currentTime.AddMilliseconds(3);
            */
            //*
            _eyeGazeTrackerReading = _eyeGazeTracker.TryGetReadingAtTimestamp(currentTime.DateTime);
            if (_eyeGazeTrackerReading == null)
            {
                //Debug.LogWarning($"Unable to get eyeGazeTrackerReading at: {currentTime.DateTime.ToLongTimeString()}");
                LogHandler.AppendToCurrentLog($"Unable to get eyeGazeTrackerReading at: {currentTime.DateTime.ToLocalTime().ToLongTimeString()}");

            }
            //Debug.Log($"Able to get eyeGazeTrackerReading at: {currentTime.DateTime.ToString("hh:mm:ss:ffffff")}");
            LogHandler.AppendToCurrentLog($"Able to get eyeGazeTrackerReading at: {currentTime.DateTime.ToLocalTime().ToString("hh:mm:ss:fff")}");
            //Debug.Log($"Able to get  _eyeGazeTrackerReading.Timestamp at: { _eyeGazeTrackerReading.Timestamp.ToString("hh:mm:ss:ffffff")}");
            LogHandler.AppendToCurrentLog($"Able to get  _eyeGazeTrackerReading.Timestamp at: { _eyeGazeTrackerReading.Timestamp.ToLocalTime().ToString("hh:mm:ss:fff")}");
            _readingSucceeded = false;
            // var reading = _eyeGazeTracker.TryGetReadingAfterTimestamp(currentTime.DateTime);
            // currentTime = _eyeGazeTrackerReading.Timestamp;
            
            //Debug.Log($"new currentTime: {currentTime.DateTime.ToString("hh:mm:ss:ffffff")}");
            LogHandler.AppendToCurrentLog($"new currentTime: {currentTime.DateTime.ToLocalTime().ToString("hh:mm:ss:fff")}");

            if (_eyeGazeTrackerReading != null)
            {
                // Debug.Log($"_eyeGazeTrackerReading is not null");
                LogHandler.AppendToCurrentLog($"_eyeGazeTrackerReading is not null");
                var worldReadingSucceded = _eyeGazeTrackerReading.TryGetCombinedEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);

                if (worldReadingSucceded)
                {
                    //Debug.Log($"Able to get TryGetCombinedEyeGazeInTrackerSpace at: {currentTime.DateTime.ToLongTimeString()}");
                    LogHandler.AppendToCurrentLog($"Able to get TryGetCombinedEyeGazeInTrackerSpace at: {currentTime.DateTime.ToLocalTime().ToLongTimeString()}");

                    if (_eyeGazeTrackerNode.TryLocate(_eyeGazeTrackerReading.SystemRelativeTime.Ticks, out _eyeGazeTrackerPose))
                    {
                        //Debug.Log($"TryLocate worked");
                        LogHandler.AppendToCurrentLog($"TryLocate worked");
                        transform.SetPositionAndRotation(_eyeGazeTrackerPose.position, _eyeGazeTrackerPose.rotation);

                        GazeReading _gazeReading = new GazeReading();

                        _gazeReading.EyePosition = transform.TransformPoint(ToUnity(_trackerSpaceGazeOrigin));
                        _gazeReading.GazeDirection = transform.TransformDirection(ToUnity(_trackerSpaceGazeDirection));
                        var date = new DateTime(1970, 1, 1, 0, 0, 0, _eyeGazeTrackerReading.Timestamp.Kind);
                        _gazeReading.Timestamp = System.Convert.ToInt64((_eyeGazeTrackerReading.Timestamp.ToLocalTime() - date).TotalMilliseconds);

                        sampleCounter++;
                        //Debug.Log($"Added new gaze data: {_gazeReading.EyePosition}");
                        LogHandler.AppendToCurrentLog($"new gaze data: {_gazeReading.EyePosition.x}, {_gazeReading.EyePosition.y}, {_gazeReading.EyePosition.z}");
                        //Debug.Log($"\nsample num: {sampleCounter}");
                        LogHandler.AppendToCurrentLog($"sample num: {sampleCounter}");
                        if (gazeList.Count > 1)
                        {
                            var prevGazeDataAdded = gazeList[gazeList.Count - 1];
                            if (prevGazeDataAdded.EyePosition != _gazeReading.EyePosition
                                && prevGazeDataAdded.GazeDirection != _gazeReading.GazeDirection)
                            {
                                gazeList.Add(_gazeReading);
                            }
                            else
                            {
                                duplicateCounter++;
                                //Debug.Log("Not adding new gaze point because it's the same as the previous one");
                                LogHandler.AppendToCurrentLog("Not adding new gaze point because it's the same as the previous one");
                            }
                        } else
                        {
                            gazeList.Add(_gazeReading);
                        }
                          
                    } else
                    {
                        sampleCounterMissedTryLocate++;
                        LogHandler.AppendToCurrentLog($"Unable to get TryLocate at: {_eyeGazeTrackerReading.Timestamp.ToLongTimeString()}");
                    }

                } else
                {
                    //Debug.Log($"Unable to get TryGetCombinedEyeGazeInTrackerSpace at: {currentTime.DateTime.ToLongTimeString()}");
                    LogHandler.AppendToCurrentLog($"Unable to get TryGetCombinedEyeGazeInTrackerSpace at: {currentTime.DateTime.ToLocalTime().ToLongTimeString()}");
                    sampleCounterMissed++;
                }
                
            }
            currentTime = currentTime.AddMilliseconds(1);
        }
        DoneReadingCurrentGazeData = false;
        //log += $"\ncollected samples: {sampleCounter}";
        LogHandler.AppendToCurrentLog($"collected samples: {sampleCounter}");
        //log += $"\ncollected samples missed: {sampleCounterMissed}";
        LogHandler.AppendToCurrentLog($"collected samples missed: {sampleCounterMissed}");
        LogHandler.AppendToCurrentLog($"sampleCounterMissedTryLocate: {sampleCounterMissedTryLocate}");
        LogHandler.AppendToCurrentLog($"duplicateCounter: {duplicateCounter}");
        LogHandler.AppendToCurrentLog($"--- GetGazeDataFromLastNSeconds --- end\n");
        
        //Debug.Log(log);
        return gazeList;


    }



    /// <summary>
    /// Get the reading for the requested GazeType at the given TimeStamp
    /// Will return null if unable to return a valid reading
    /// </summary>
    /// <param name="gazeType"></param>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public GazeReading GetWorldSpaceGazeReading(GazeType gazeType, DateTime timestamp)
    {
        if (!_gazePermissionEnabled || _eyeGazeTracker == null)
        {
            return null;
        }

        _eyeGazeTrackerReading = _eyeGazeTracker.TryGetReadingAtTimestamp(timestamp);
        if (_eyeGazeTrackerReading == null)
        {
            Debug.LogWarning($"Unable to get eyeGazeTrackerReading at: {timestamp.ToLongTimeString()}");
            return null;
        }

        _readingSucceeded = false;
        switch (gazeType)
        {
            case GazeType.Left:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetLeftEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
            case GazeType.Right:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetRightEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
            case GazeType.Combined:
                {
                    _readingSucceeded = _eyeGazeTrackerReading.TryGetCombinedEyeGazeInTrackerSpace(out _trackerSpaceGazeOrigin, out _trackerSpaceGazeDirection);
                    break;
                }
        }
        if (!_readingSucceeded)
        {
            return null;
        }

        // get tracker pose in Unity scene origin space
        if (!_eyeGazeTrackerNode.TryLocate(_eyeGazeTrackerReading.SystemRelativeTime.Ticks, out _eyeGazeTrackerPose))
        {
            return null;
        }


        transform.SetPositionAndRotation(_eyeGazeTrackerPose.position, _eyeGazeTrackerPose.rotation);

        _gazeReading.EyePosition = transform.TransformPoint(ToUnity(_trackerSpaceGazeOrigin));
        _gazeReading.GazeDirection = transform.TransformDirection(ToUnity(_trackerSpaceGazeDirection));
        _gazeReading.Timestamp = ((DateTimeOffset)_eyeGazeTrackerReading.Timestamp).ToUnixTimeSeconds(); ;
        return _gazeReading;
    }

    private async void Start()
    {
        _mainCamera = Camera.main;

        Debug.Log("Initializing ExtendedEyeTracker");
#if ENABLE_WINMD_SUPPORT
        Debug.Log("Triggering eye gaze permission request");
        // This function call may not required if you already use MRTK in your project 
        _gazePermissionEnabled = await AskForEyePosePermission();
#else
        // Always enable when running in editor
        _gazePermissionEnabled = true;
#endif

        if (!_gazePermissionEnabled)
        {
            Debug.LogError("Gaze is disabled");
            return;
        }

        DoneReadingCurrentGazeData = false;

        _watcher = new EyeGazeTrackerWatcher();
        _watcher.EyeGazeTrackerAdded += _watcher_EyeGazeTrackerAdded;
        _watcher.EyeGazeTrackerRemoved += _watcher_EyeGazeTrackerRemoved;
        await _watcher.StartAsync();
    }

    private void _watcher_EyeGazeTrackerRemoved(object sender, EyeGazeTracker e)
    {
        Debug.Log("EyeGazeTracker removed");
        _eyeGazeTracker = null;
    }

    private async void _watcher_EyeGazeTrackerAdded(object sender, EyeGazeTracker e)
    {
        Debug.Log("EyeGazeTracker added");
        try
        {
            await e.OpenAsync(true);
            _eyeGazeTracker = e;
            var supportedFrameRates = _eyeGazeTracker.SupportedTargetFrameRates;
            foreach (var frameRate in supportedFrameRates)
            {
                Debug.Log($"  supportedFrameRate: {frameRate.FramesPerSecond}");
            }

            // Set to highest framerate, it is 90FPS at this time
            _eyeGazeTracker.SetTargetFrameRate(supportedFrameRates[supportedFrameRates.Count - 1]);
            Debug.Log($"selected frameRate: {supportedFrameRates[supportedFrameRates.Count - 1].FramesPerSecond}");
            _eyeGazeTrackerNode = SpatialGraphNode.FromDynamicNodeId(e.TrackerSpaceLocatorNodeId);
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to open EyeGazeTracker\r\n" + ex.ToString());
        }
    }

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Triggers a prompt to let the user decide whether to permit using eye tracking 
    /// </summary>
    private async System.Threading.Tasks.Task<bool> AskForEyePosePermission()
    {
        var accessStatus = await Windows.Perception.People.EyesPose.RequestAccessAsync();
        Debug.Log("Eye gaze access status: " + accessStatus.ToString());
        return accessStatus == Windows.UI.Input.GazeInputAccessStatus.Allowed;
    }
#endif

    private static UnityEngine.Vector3 ToUnity(System.Numerics.Vector3 v) => new UnityEngine.Vector3(v.X, v.Y, -v.Z);
}