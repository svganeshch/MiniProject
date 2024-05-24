using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDebug : MonoBehaviour
{
    string errorText;
    string logFilePath;

    private void Start()
    {
        logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "log.txt");

        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            errorText = $"{System.DateTime.Now} - [{type.ToString()}] - {logString} : {stackTrace}\n\n";

            File.AppendAllText(logFilePath, errorText);
        }
    }

    void OnGUI()
    {
        GUI.color = Color.blue;

        GUIStyle labelStyle = GUI.skin.label;
        labelStyle.fontSize = 16;

        GUI.Label(new Rect(10, 60, Screen.width, Screen.height / 2), errorText, labelStyle);
    }
}
