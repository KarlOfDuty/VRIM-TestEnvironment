using UnityEngine;

public class ControllerStartScript : MonoBehaviour
{
    private SteamVR_TrackedObject _trackedObj;
    public ApplicationController applicationController;

    private void Awake()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)_trackedObj.index); }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            applicationController.StartTest();
            Destroy(gameObject.GetComponent<ControllerStartScript>());
        }
    }
}