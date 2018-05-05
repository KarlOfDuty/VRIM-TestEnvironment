using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMethod3 : MonoBehaviour
{
    public ApplicationController applicationController;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public GameObject laserPrefab;
    private GameObject laser;
    private Vector3 hitPoint;

    public GameObject heldGameObject = null;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        laser = Instantiate(laserPrefab);
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laser.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, 0.5f);
        laser.transform.LookAt(hitPoint);
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, hit.distance);
    }

    private void GrabObject(GameObject obj)
    {
        heldGameObject = obj;

        var joint = AddFixedJoint();
        joint.connectedBody = obj.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    // Update is called once per frame
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100))
        {
            hitPoint = hit.point;
            ShowLaser(hit);
            if (Controller.GetHairTriggerDown() && !heldGameObject && hit.collider.gameObject.GetComponent<Rigidbody>())
            {
                if (applicationController.currentCube == hit.collider.gameObject)
                {
                    applicationController.LogCorrectObject(-1.0f);
                }
                else
                {
                    applicationController.LogWrongObject(-1.0f);
                }
                GrabObject(hit.collider.gameObject);
            }
            else if (Controller.GetHairTriggerDown() && !heldGameObject)
            {
                applicationController.LogMissedObject(-1.0f);
            }
        }
        if (Controller.GetHairTriggerUp() && heldGameObject)
        {
            applicationController.LogDroppedObject();
            ReleaseObject();
        }
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            heldGameObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            heldGameObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        heldGameObject = null;
    }
}