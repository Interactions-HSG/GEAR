using ARETT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GazeDataSender : MonoBehaviour
{

    public DateTime LastSendingTime;

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


    private string _currentFullCSVpath;
    private int _chunkCounter;
    private bool _noChunkSentYet;
    private List<string> _currentDataChunk;


    // Start is called before the first frame update
    void Start()
    {
        DesktopURL = "http://10.2.2.172:5000"; // default value
        _noChunkSentYet = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (HTTPListener.NewDesktopIPArrived)
        {
            DesktopURL = $"http://{HTTPListener.TmpDesktopIP}:{HTTPListener.TmpDesktopPort}";
            Debug.Log($"GazeDataSender, new DesktopURL: {DesktopURL}");
            HTTPListener.NewDesktopIPArrived = false;
            HTTPListener.TmpDesktopIP = "";
            HTTPListener.TmpDesktopPort = "";
            DesktopIPText.GetComponent<TextMeshPro>().text = $"{DesktopURL}";
            Buttons.SetActive(true);
            WaitingText.SetActive(false);
        }
    }


    public void HandleNewGazeData(GazeData gd)
    {

        Debug.Log($"time diff: {(DateTime.UtcNow - LastSendingTime)}");
        // we want to send chunks of 10 seconds
        if (_noChunkSentYet || (DateTime.UtcNow - LastSendingTime).TotalSeconds > 10)
        {
            SaveCurrentChunkAsCSV();
            // insert the IP address of the PC where the classifier runs here ↓
            StartCoroutine(SendPostRequestToDesktop(DesktopURL, _currentFullCSVpath));
            CreateEmptyListForNewGazeDataChunk();
            AddLineToCurrentCSV(gd);
            LastSendingTime = DateTime.UtcNow;
            _noChunkSentYet = false;
        }
        else
        {
            AddLineToCurrentCSV(gd);
        }
    }

    /// <summary>
    /// Creates a new empty list for a new gaze data chunk. 
    /// </summary>
    public void CreateEmptyListForNewGazeDataChunk()
    {
        var t = "eyeDataTimestamp,isCalibrationValid,gazeHasValue,gazeOrigin_x,gazeOrigin_y,gazeOrigin_z," +
                "gazeDirection_x,gazeDirection_y,gazeDirection_z,gazePointHit,gazePoint_x,gazePoint_y,gazePoint_z";
        _currentDataChunk = new List<string>();
        _currentDataChunk.Add(t);
    }


    /// <summary>
    /// Saves the current chunk of gaze data (i.e. from the last 10 seconds) as a csv-file on the HoloLens 2
    /// </summary>
    public void SaveCurrentChunkAsCSV()
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
    public void AddLineToCurrentCSV(GazeData gd)
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
    public IEnumerator SendPostRequestToDesktop(string url, string filepath)
    {
        Debug.Log($"Filepath in request: {filepath}");

        WWWForm form = new WWWForm();
        var filename = filepath.Split('/').Last();
        form.AddField("filename", filename);
        form.AddBinaryData("gazedata", File.ReadAllBytes(filepath));


        Debug.Log($"POST url: {url}, formData: {form.ToString()}");

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

        Debug.Log($"GET url: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
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
                Debug.Log("GetQuery: worked!");
            }
        }
    }
}
