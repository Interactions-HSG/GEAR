using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

public class LogHandler : MonoBehaviour
{

    private StringBuilder currentLog;

    // Start is called before the first frame update
    void Start()
    {
        currentLog = new StringBuilder();
        AppendToCurrentLog("test start");
        WriteCurrentLogToFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AppendToCurrentLog(string text)
    {
        var now = DateTime.Now.ToString("HH:mm:ss:fff");
        var t = $"[{now}]: {text}";
        currentLog.AppendLine(t);
    }


    public void WriteCurrentLogToFile()
    {
        var content = currentLog.ToString();
        
        var dirPath = UnityEngine.Application.persistentDataPath + "/GEAR_log";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var now = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
        var filePath = $"{dirPath}/{now}_log.txt";
        using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (var writer = new StreamWriter(file, Encoding.UTF8))
            {
                writer.Write(content);
            }
        }
        Debug.Log($"Wrote log file to {filePath}");
        currentLog = new StringBuilder();
    }
}
