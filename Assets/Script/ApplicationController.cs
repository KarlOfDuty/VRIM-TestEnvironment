using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    public GameObject cubesParent;
    public Material highlightedMaterial;

    public GameObject currentCube = null;

    private AudioSource _completionSound;

    private string _logID;
    private string _sceneName;
    public string logPath = "";

    private StreamWriter _sw = null;

    public bool testStarted = false;

    public bool testEnded = false;

    private float _timeOfLastSuccessfulPlacement = 0.0f;

    private int _numberOfMissedPickups = 0;
    private int _numberOfWrongPickups = 0;
    private int _numberOfSuccessfulPickups = 0;

    private void Awake()
    {
        _logID = "001";
        _sceneName = SceneManager.GetActiveScene().name;
    }

    // Use this for initialization
    private void Start()
    {
        _completionSound = GetComponent<AudioSource>();
        Directory.CreateDirectory("Assets/Logs/Log" + _logID);
        logPath = "Assets/Logs/Log" + _logID + "/" + _sceneName + ".log";
        _sw = new StreamWriter(logPath, true);
        WriteLineToLog("Application Started. ID: " + _logID + ". Scene: " + _sceneName + ".");
    }

    public void StartTest()
    {
        if (!testStarted)
        {
            SetRandomCube();
            WriteLineToLog("Test Started. ID: " + _logID + ". Scene: " + _sceneName + ".");
            testStarted = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (testEnded && _sw.BaseStream != null)
        {
            WriteLineToLog("Test Concluded. ID: " + _logID + ". Scene: " + _sceneName + ". Number of correct pickups: " + _numberOfSuccessfulPickups + ". Number of wrong pickups: " + _numberOfWrongPickups + ". Number of missed pickups: " + _numberOfMissedPickups);
            _sw.Close();
            _completionSound.PlayDelayed(1.0f);
        }
    }

    private void OnApplicationQuit()
    {
        if (_sw.BaseStream != null)
        {
            WriteLineToLog("Test Aborted. ID: " + _logID + ". Scene: " + _sceneName + ". Number of correct pickups: " + _numberOfSuccessfulPickups + ". Number of wrong pickups: " + _numberOfWrongPickups + ". Number of missed pickups: " + _numberOfMissedPickups);
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