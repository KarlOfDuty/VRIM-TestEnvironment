using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerStartScript : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    public ApplicationController applicationController;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Controller.GetHairTriggerDown())
        {
            applicationController.StartTest();
            Destroy(gameObject.GetComponent<ControllerStartScript>());
        }
    }
}