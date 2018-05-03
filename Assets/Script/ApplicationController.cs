using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    public GameObject cubesParent;
    public Material highlightedMaterial;

    public GameObject currentCube = null;

    private AudioSource completionSound;

    public string logID;
    public string sceneName;
    public string logPath = "";

    private StreamWriter sw = null;

    public bool testStarted = false;

    public bool testEnded = false;

    private float timeLastSuccessfulPlacement = 0.0f;

    private int numberOfMissedPickups = 0;
    private int numberOfWrongPickups = 0;
    private int numberOfSuccessfulPickups = 0;

    // Use this for initialization
    private void Start()
    {
        completionSound = GetComponent<AudioSource>();
        Directory.CreateDirectory("Assets/Logs/Log" + logID);
        logPath = "Assets/Logs/Log" + logID + "/" + sceneName + ".log";
        sw = new StreamWriter(logPath, true);
        WriteLineToLog("Application Started. ID: " + logID + ". Scene: " + sceneName + ".");
    }

    public void StartTest()
    {
        if (!testStarted)
        {
            SetRandomCube();
            WriteLineToLog("Test Started. ID: " + logID + ". Scene: " + sceneName + ".");
            testStarted = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (testEnded && sw.BaseStream != null)
        {
            WriteLineToLog("Test Concluded. ID: " + logID + ". Scene: " + sceneName + ". Number of correct pickups: " + numberOfSuccessfulPickups + ". Number of wrong pickups: " + numberOfWrongPickups + ". Number of missed pickups: " + numberOfMissedPickups);
            sw.Close();
            completionSound.PlayDelayed(1.0f);
        }
    }

    private void OnApplicationQuit()
    {
        if (sw.BaseStream != null)
        {
            WriteLineToLog("Test Aborted. ID: " + logID + ". Scene: " + sceneName + ". Number of correct pickups: " + numberOfSuccessfulPickups + ". Number of wrong pickups: " + numberOfWrongPickups + ". Number of missed pickups: " + numberOfMissedPickups);
            sw.Dispose();
        }
    }

    public void DeleteCurrentCube()
    {
        WriteLineToLog("Cube successfully placed in target. " + (Time.time - timeLastSuccessfulPlacement) + "s since last cube successfully placed.");
        timeLastSuccessfulPlacement = Time.time;

        currentCube.transform.parent = null;
        Destroy(currentCube, 1.0f);
        currentCube = null;
        SetRandomCube();
    }

    private void SetRandomCube()
    {
        if (cubesParent.transform.childCount == 0)
        {
            testEnded = true;
            return;
        }
        Transform transform = cubesParent.transform.GetChild(Random.Range(0, cubesParent.transform.childCount));
        currentCube = transform.gameObject;
        currentCube.gameObject.GetComponent<Renderer>().material = highlightedMaterial;
    }

    private void WriteLineToLog(string line)
    {
        if (sw.BaseStream != null)
        {
            sw.WriteLine("[" + System.DateTime.Now + "] (" + Time.time + "s): " + line);
            sw.Flush();
        }
    }

    private void WriteLineToLog(string line, bool addEmptyLine)
    {
        if (sw.BaseStream != null)
        {
            sw.WriteLine("[" + System.DateTime.Now + "] (" + Time.time + "s): " + line);
            sw.Flush();
            sw.WriteLine();
        }
    }

    public void LogMissedObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            numberOfMissedPickups++;
            WriteLineToLog("User tried to pick up an object but missed. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogWrongObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            numberOfWrongPickups++;
            WriteLineToLog("User picked up wrong object. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogCorrectObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            numberOfSuccessfulPickups++;
            WriteLineToLog("User picked up the correct object. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogDroppedObject()
    {
        if (testStarted)
        {
            WriteLineToLog("User released an object.");
        }
    }

    public void LogAttemptedDrop()
    {
        if (testStarted)
        {
            WriteLineToLog("User tried to drop an object but was not holding one.");
        }
    }
}