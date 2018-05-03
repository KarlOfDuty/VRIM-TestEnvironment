using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMethod2 : MonoBehaviour
{
    public ApplicationController applicationController;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public GameObject objectInHand = null;
    public GameObject recentlyReleasedObject;
    public SphereCollider thisCollider;

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        thisCollider = GetComponent<SphereCollider>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Rigidbody>() || objectInHand || other.gameObject == recentlyReleasedObject)
        {
            return;
        }

        float distance = -1;

        if (applicationController.currentCube)
        {
            distance = Vector3.Distance(thisCollider.transform.TransformPoint(thisCollider.center), applicationController.currentCube.transform.position);
        }

        if (other.gameObject == applicationController.currentCube)
        {
            applicationController.LogCorrectObject(distance);
        }
        else
        {
            applicationController.LogWrongObject(distance);
        }
        objectInHand = other.gameObject;
        objectInHand.transform.position = thisCollider.transform.TransformPoint(thisCollider.center);
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == recentlyReleasedObject)
        {
            recentlyReleasedObject = null;
        }
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void Update()
    {
        if (Controller.GetHairTriggerDown())
        {
            if (objectInHand)
            {
                applicationController.LogDroppedObject();
                ReleaseObject();
            }
            else
            {
                applicationController.LogAttemptedDrop();
            }
        }
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        recentlyReleasedObject = objectInHand;
        objectInHand = null;
    }
}