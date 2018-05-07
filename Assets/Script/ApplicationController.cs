using System.IO;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    public GameObject cubesParent;
    public Material highlightedMaterial;

    public GameObject currentCube = null;

    private AudioSource _completionSound;

    public string logID;
    public string sceneName;
    public string logPath = "";

    private StreamWriter _sw = null;

    public bool testStarted = false;

    public bool testEnded = false;

    private float _timeOfLastSuccessfulPlacement = 0.0f;

    private int _numberOfMissedPickups = 0;
    private int _numberOfWrongPickups = 0;
    private int _numberOfSuccessfulPickups = 0;

    // Use this for initialization
    private void Start()
    {
        _completionSound = GetComponent<AudioSource>();
        Directory.CreateDirectory("Assets/Logs/Log" + logID);
        logPath = "Assets/Logs/Log" + logID + "/" + sceneName + ".log";
        _sw = new StreamWriter(logPath, true);
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
        if (testEnded && _sw.BaseStream != null)
        {
            WriteLineToLog("Test Concluded. ID: " + logID + ". Scene: " + sceneName + ". Number of correct pickups: " + _numberOfSuccessfulPickups + ". Number of wrong pickups: " + _numberOfWrongPickups + ". Number of missed pickups: " + _numberOfMissedPickups);
            _sw.Close();
            _completionSound.PlayDelayed(1.0f);
        }
    }

    private void OnApplicationQuit()
    {
        if (_sw.BaseStream != null)
        {
            WriteLineToLog("Test Aborted. ID: " + logID + ". Scene: " + sceneName + ". Number of correct pickups: " + _numberOfSuccessfulPickups + ". Number of wrong pickups: " + _numberOfWrongPickups + ". Number of missed pickups: " + _numberOfMissedPickups);
            _sw.Dispose();
        }
    }

    public void DeleteCurrentCube()
    {
        WriteLineToLog("Cube successfully placed in target. " + (Time.time - _timeOfLastSuccessfulPlacement) + "s since last cube successfully placed.");
        _timeOfLastSuccessfulPlacement = Time.time;

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

        currentCube = cubesParent.transform.GetChild(Random.Range(0, cubesParent.transform.childCount)).gameObject;
        currentCube.gameObject.GetComponent<Renderer>().material = highlightedMaterial;
    }

    private void WriteLineToLog(string line)
    {
        if (_sw.BaseStream != null)
        {
            _sw.WriteLine("[" + System.DateTime.Now + "] (" + Time.time + "s): " + line);
            _sw.Flush();
        }
    }

    public void LogMissedObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            _numberOfMissedPickups++;
            WriteLineToLog("User tried to pick up an object but missed. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogWrongObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            _numberOfWrongPickups++;
            WriteLineToLog("User picked up wrong object. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogCorrectObject(float distanceToCorrectObject)
    {
        if (testStarted)
        {
            _numberOfSuccessfulPickups++;
            WriteLineToLog("User picked up the correct object. Distance to center of correct object: " + distanceToCorrectObject);
        }
    }

    public void LogDroppedObject()
    {
        if (testStarted)
            WriteLineToLog("User released an object.");
    }

    public void LogAttemptedDrop()
    {
        if (testStarted)
            WriteLineToLog("User tried to drop an object but was not holding one.");
    }
}