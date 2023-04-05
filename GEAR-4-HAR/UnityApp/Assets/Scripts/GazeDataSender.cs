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

public class GazeDataSender : MonoBehaviour
{

    public DataProvider DataProvider;
    private ConcurrentQueue<Action> _mainThreadWorkQueue = new ConcurrentQueue<Action>();
   
    private DateTime _lastSendingTime;
    private string _currentFullCSVpath;
    private int _chunkCounter;

    public GameObject ButtonsContainer;
    public GameObject StartButton;
    public GameObject StopButton;

    private List<string> _currentDataChunk;

    // Start is called before the first frame update
    void Start()
    {
       
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
        CreateEmptyListForNewGazeDataChunk();
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
        _lastSendingTime = DateTime.UtcNow;
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

        Debug.Log($"time diff: {(DateTime.UtcNow - _lastSendingTime)}");

        // we want to send chunks of 10 seconds
        if ((DateTime.UtcNow -_lastSendingTime).TotalSeconds > 10)
        {
            SaveCurrentChunkAsCSV();
            // insert the IP address of the PC where the classifier runs here ↓
            StartCoroutine(SendPostRequestToDesktop("http://0.0.0.0:5555", _currentFullCSVpath));
            CreateEmptyListForNewGazeDataChunk();
            AddLineToCurrentCSV(gd);
            _lastSendingTime = DateTime.UtcNow;
        } else
        {
            AddLineToCurrentCSV(gd);
        }
       
      
        
    }

    /// <summary>
    /// Creates a new empty list for a new gaze data chunk. 
    /// </summary>
    private void CreateEmptyListForNewGazeDataChunk()
    {
        var t = "eyeDataTimestamp,isCalibrationValid,gazeHasValue,gazeOrigin_x,gazeOrigin_y,gazeOrigin_z," +
                "gazeDirection_x,gazeDirection_y,gazeDirection_z,gazePointHit,gazePoint_x,gazePoint_y,gazePoint_z";
        _currentDataChunk = new List<string>();
        _currentDataChunk.Add(t);
    }


    /// <summary>
    /// Saves the current chunk of gaze data (i.e. from the last 10 seconds) as a csv-file on the HoloLens 2
    /// </summary>
    private void SaveCurrentChunkAsCSV()
    {
        var path = Application.persistentDataPath;
        var now = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd_hh-mm-ss");
        var counter = $"{_chunkCounter}";
        counter = counter.PadLeft(3, '0');
        var filepath = $"{path}/chunk_{now}_{counter}.csv";
        _currentFullCSVpath = filepath;
        _chunkCounter++;
        Debug.Log($"Filepath: {filepath}");

        using (StreamWriter writer = new StreamWriter(new FileStream(filepath,
        FileMode.Create, FileAccess.Write)))
        {
            var tt = "";
            Debug.Log($"_currentDataChunk count: {_currentDataChunk.Count}");
            foreach (var line in _currentDataChunk)
            {

                writer.WriteLine(line);
                tt = $"line: {line}\n";

            }
            Debug.Log($"{tt} \nFinished Writing in SaveCurrentChunkAsCSV");
        }
    }

    /// <summary>
    /// Saves the newly arrived gaze data from ARETT in the list for the current chunk.
    /// </summary>
    /// <param name="gd"></param>
    private void AddLineToCurrentCSV(GazeData gd)
    {
        Debug.Log($"appending data: {gd.FrameTimestamp}");


        var t = $"{gd.EyeDataTimestamp},{gd.IsCalibrationValid},{gd.GazeHasValue}," +
                $"{gd.GazeOrigin.x},{gd.GazeOrigin.y},{gd.GazeOrigin.z}," +
                $"{gd.GazeDirection.x},{gd.GazeDirection.y},{gd.GazeDirection.z},{gd.GazePointHit}," +
                $"{gd.GazePoint.x},{gd.GazePoint.y},{gd.GazePoint.z}";
        _currentDataChunk.Add(t);
    }


    /// <summary>
    /// Sends the gaze data chunk to the classifier at the given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filepath"></param>
    /// <returns></returns>
    IEnumerator SendPostRequestToDesktop(string url, string filepath)
    {
        Debug.Log($"Filepath in request: {filepath}");

        WWWForm form = new WWWForm();
        var filename = filepath.Split('/').Last();
        form.AddField("filename", filename);
        form.AddBinaryData("gazedata", File.ReadAllBytes(filepath));
        

        Debug.Log($"url: {url}, formData: {form.ToString()}");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.uploadHandler.contentType = "multipart/form-data";
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return www.SendWebRequest();


            Debug.Log($"www.responseCode: {www.responseCode}, www.result: {www.result}");
            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
   
            {
                Debug.Log(www.error);
                var res = www.GetResponseHeaders();

                foreach (var pair in res)
                {
                    Debug.Log($"Response {pair.Key}: {pair.Value}");
                }
            }
            else
            {
                Debug.Log("PostQuery: worked!");
            }
        }
    }
}
