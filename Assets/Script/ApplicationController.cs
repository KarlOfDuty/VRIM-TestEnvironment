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
    private float lastCubeRemoved = 0.0f;

    // Use this for initialization
    private void Start()
    {
        completionSound = GetComponent<AudioSource>();
        Directory.CreateDirectory("Assets/Logs/Log" + logID);
        logPath = "Assets/Logs/Log" + logID + "/" + sceneName + ".log";
        sw = new StreamWriter(logPath, true);
        sw.WriteLine("Application Started. ID: " + logID + ". Scene: " + sceneName + ". Start time: " + System.DateTime.Now);
        sw.Flush();
    }

    public void StartTest()
    {
        if (!testStarted)
        {
            SetRandomCube();
            sw.WriteLine("Test Started. ID: " + logID + ". Scene: " + sceneName + ". " + Time.time + "s after application start.");
            sw.Flush();
            lastCubeRemoved = Time.time;
            testStarted = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (testEnded && sw.BaseStream != null)
        {
            sw.WriteLine("Test Concluded. ID: " + logID + ". Scene: " + sceneName + ". End time: " + System.DateTime.Now);
            sw.Close();
            completionSound.PlayDelayed(1.0f);
        }
    }

    private void OnApplicationQuit()
    {
        if (sw.BaseStream != null)
        {
            sw.WriteLine("Test Aborted. ID: " + logID + ". Scene: " + sceneName + ". End time: " + System.DateTime.Now);
            sw.Dispose();
        }
    }

    public void DeleteCurrentCube()
    {
        sw.WriteLine();
        sw.WriteLine("[" + System.DateTime.Now + "]: Cube successfully placed. ");
        sw.WriteLine(Time.time + "s after application start.");
        sw.WriteLine((Time.time - lastCubeRemoved) + "s since last cube placed.");
        lastCubeRemoved = Time.time;
        sw.Flush();
        if (currentCube != null)
        {
            currentCube.transform.parent = null;
            Destroy(currentCube, 1.0f);
            currentCube = null;
        }
        else
        {
            print("CurrentCube was unexpectedly null on deletion.");
        }
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
}