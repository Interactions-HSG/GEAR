using ARETT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GazeDataSender : MonoBehaviour
{

    public DateTime LastSendingTime;
    public LogHandler LogHandler;

    /*
    public bool NewDesktopIPArrived;
    public string TmpDesktopIP;
    public string TmpDesktopPort;
    */
    public string DesktopURL;

    public GameObject DesktopIPText;
    public GameObject Buttons;
    public GameObject WaitingText;
    public HTTPListener HTTPListener;
    public TextMeshProUGUI DebugText;
    public ScrollRect DebugScrollRect;


    private string _currentFullCSVpath;
    private int _chunkCounter;
    private bool _noChunkSentYet;
    private List<string> _currentDataChunk;
    private int _newGazeDataCounter;
    private long _firstTimestampInChunk;

   


    // Start is called before the first frame update
    void Start()
    {
		// CHANGE this to another default value 
        DesktopURL = "http://127.0.0.1:5000"; // default value
        _noChunkSentYet = true;
        LastSendingTime = DateTime.UtcNow;
        _newGazeDataCounter = 0;

        /*
        var path = Application.persistentDataPath;
        System.IO.DirectoryInfo di = new DirectoryInfo(path);
        foreach (FileInfo file in di.EnumerateFiles())
        {
            if (file.Name.StartsWith("chunk"))
            {
                file.Delete();
            }
            
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        if (HTTPListener.NewDesktopIPArrived)
        {
            DesktopURL = $"http://{HTTPListener.TmpDesktopIP}:{HTTPListener.TmpDesktopPort}";
            //LogHandler.AppendToCurrentLog($"GazeDataSender, new DesktopURL: {DesktopURL}");
            LogHandler.AppendToCurrentLog($"GazeDataSender, new DesktopURL: {DesktopURL}");
            DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")}]";
            DebugText.text += $"\nReceived new DesktopURL: {DesktopURL}";
            HTTPListener.NewDesktopIPArrived = false;
            HTTPListener.TmpDesktopIP = "";
            HTTPListener.TmpDesktopPort = "";
            DesktopIPText.GetComponent<TextMeshPro>().text = $"{DesktopURL}";
            Buttons.SetActive(true);
            WaitingText.SetActive(false);
        }

        //DebugText.text += "\n---";
        //StartCoroutine(PushToBottom());
    }

 

    public void HandleNewGazeData(GazeData gd, string eyeTrackingMethod)
    {
        LogHandler.AppendToCurrentLog("-- HandleNewGazeData");
       LogHandler.AppendToCurrentLog($"time diff: {(DateTime.UtcNow.ToLocalTime() - LastSendingTime.ToLocalTime())}");
       
        
        _newGazeDataCounter++;
       LogHandler.AppendToCurrentLog($"_newGazeDataCounter: {_newGazeDataCounter}");
       LogHandler.AppendToCurrentLog($"_currentDataChunk.Count: {_currentDataChunk.Count}");
        // we want to send chunks of 10 seconds

       if ((_firstTimestampInChunk > gd.EyeDataTimestamp) && _currentDataChunk.Count <= 2)
       {
            _firstTimestampInChunk = gd.EyeDataTimestamp;
           LogHandler.AppendToCurrentLog("changed to new _firstTimestampInChunk");
       }
        // send every 5 seconds
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var dateTimeGD = dateTime.AddMilliseconds(gd.EyeDataTimestamp).ToLocalTime();
        var dateTimeFirst = dateTime.AddMilliseconds(_firstTimestampInChunk).ToLocalTime();
        var diff = (dateTimeGD - dateTimeFirst).TotalMilliseconds;
       LogHandler.AppendToCurrentLog($"this data: {gd.EyeDataTimestamp}");
       LogHandler.AppendToCurrentLog($"first data: {_firstTimestampInChunk}");
       LogHandler.AppendToCurrentLog($"time diff to first: {(gd.EyeDataTimestamp - _firstTimestampInChunk)}");
       LogHandler.AppendToCurrentLog($"time diff to first: {diff}");
       LogHandler.AppendToCurrentLog($"diff > 5000: {diff > 5000}");

        if (diff > 5000 && _currentDataChunk.Count > 250)
        //if ((DateTime.UtcNow - LastSendingTime).TotalSeconds > 10 && _currentDataChunk.Count > 10)
        // if (_currentDataChunk.Count > 500)
        {
            SaveCurrentChunkAsCSV();
            StartCoroutine(SendPostRequestToDesktop(DesktopURL, _currentFullCSVpath));
            CreateEmptyListForNewGazeDataChunk();
            AddLineToCurrentCSV(gd, eyeTrackingMethod);
            LastSendingTime = DateTime.UtcNow;
            _noChunkSentYet = false;
            _firstTimestampInChunk = gd.EyeDataTimestamp;
        }
        else
        {
            AddLineToCurrentCSV(gd, eyeTrackingMethod);
        }
    }

    /// <summary>
    /// Creates a new empty list for a new gaze data chunk. 
    /// </summary>
    public void CreateEmptyListForNewGazeDataChunk()
    {
        var t = "eyeDataTimestamp,isCalibrationValid,gazeHasValue,gazeOrigin_x,gazeOrigin_y,gazeOrigin_z," +
                "gazeDirection_x,gazeDirection_y,gazeDirection_z,gazePointHit,gazePoint_x,gazePoint_y,gazePoint_z,note, time_processed";
        _currentDataChunk = new List<string>();
        _currentDataChunk.Add(t);
       LogHandler.AppendToCurrentLog("-- CreateEmptyListForNewGazeDataChunk");
    }


    /// <summary>
    /// Saves the current chunk of gaze data (i.e. from the last 10 seconds) as a csv-file on the HoloLens 2
    /// </summary>
    public void SaveCurrentChunkAsCSV()
    {
        var path = Application.persistentDataPath;
        var now = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd_HH-mm-ss");
        var counter = $"{_chunkCounter}";
        counter = counter.PadLeft(3, '0');
        var filepath = $"{path}/chunk_{now}_{counter}.csv";
        _currentFullCSVpath = filepath;
        _chunkCounter++;
       LogHandler.AppendToCurrentLog($"Filepath: {filepath}");

        using (StreamWriter writer = new StreamWriter(new FileStream(filepath,
        FileMode.Create, FileAccess.Write)))
        {
            var tt = "";
           LogHandler.AppendToCurrentLog($"_currentDataChunk count: {_currentDataChunk.Count}");
            foreach (var line in _currentDataChunk)
            {

                writer.WriteLine(line);
                tt = $"line: {line}\n";

            }
           LogHandler.AppendToCurrentLog($"{tt} \nFinished Writing in SaveCurrentChunkAsCSV");
        }

        LogHandler.WriteCurrentLogToFile();
       
    }

    /// <summary>
    /// Saves the newly arrived gaze data from ARETT in the list for the current chunk.
    /// </summary>
    /// <param name="gd"></param>
    /// <param name="eyeTrackingMethod">The data is coming fromARETT of Extended ET API</param>
    public void AddLineToCurrentCSV(GazeData gd, string eyeTrackingMethod)
    {
       LogHandler.AppendToCurrentLog($"appending data: {gd.FrameTimestamp}, {eyeTrackingMethod}");
        var t = $"{gd.EyeDataTimestamp},{gd.IsCalibrationValid},{gd.GazeHasValue}," +
                $"{gd.GazeOrigin.x},{gd.GazeOrigin.y},{gd.GazeOrigin.z}," +
                $"{gd.GazeDirection.x},{gd.GazeDirection.y},{gd.GazeDirection.z},{gd.GazePointHit}," +
                $"{gd.GazePoint.x},{gd.GazePoint.y},{gd.GazePoint.z},{eyeTrackingMethod},{DateTimeOffset.UtcNow.ToLocalTime().ToUnixTimeMilliseconds()}";
        _currentDataChunk.Add(t);
    }


    IEnumerator PushToBottom()
    {
        yield return new WaitForEndOfFrame();
        DebugScrollRect.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)DebugScrollRect.transform);
    }

    /// <summary>
    /// Sends the gaze data chunk to the classifier at the given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public IEnumerator SendPostRequestToDesktop(string url, string filepath)
    {
        

       LogHandler.AppendToCurrentLog($"Filepath in request: {filepath}");

        WWWForm form = new WWWForm();
        var filename = filepath.Split('/').Last();
        form.AddField("filename", filename);
        form.AddBinaryData("gazedata", File.ReadAllBytes(filepath));

        DebugText.text += $"\n[{DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss:fff")}]";
        DebugText.text += $"\nSent new gaze data to desktop with URL: {url}\nand filename: {filename}";
        StartCoroutine(PushToBottom());

       LogHandler.AppendToCurrentLog($"POST url: {url}, formData: {form.ToString()}");

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.uploadHandler.contentType = "multipart/form-data";
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return www.SendWebRequest();


           LogHandler.AppendToCurrentLog($"www.responseCode: {www.responseCode}, www.result: {www.result}");
            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)

            {
               LogHandler.AppendToCurrentLog(www.error);
                var res = www.GetResponseHeaders();

                foreach (var pair in res)
                {
                   LogHandler.AppendToCurrentLog($"Response {pair.Key}: {pair.Value}");
                }
            }
            else
            {
               LogHandler.AppendToCurrentLog("PostQuery: worked!");
            }
        }
    }

    public void SendTestRequestToDesktop()
    {
        StartCoroutine(SendGetRequestToDesktop(DesktopURL));
    }

    /// <summary>
    /// Sends the gaze data chunk to the classifier at the given url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="filepath"></param>
    /// <returns></returns>
    IEnumerator SendGetRequestToDesktop(string url)
    {

       LogHandler.AppendToCurrentLog($"GET url: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            yield return www.SendWebRequest();


           LogHandler.AppendToCurrentLog($"www.responseCode: {www.responseCode}, www.result: {www.result}");
            if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)

            {
               LogHandler.AppendToCurrentLog(www.error);
                var res = www.GetResponseHeaders();

                foreach (var pair in res)
                {
                   LogHandler.AppendToCurrentLog($"Response {pair.Key}: {pair.Value}");
                }
            }
            else
            {
               LogHandler.AppendToCurrentLog("GetQuery: worked!");
            }
        }
    }
}
