using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMethod1 : MonoBehaviour
{
    public ApplicationController applicationController;

    private SteamVR_TrackedObject trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    public float closestColliderDistance = 5000.0f;
    public Collider closestCollider;
    public SphereCollider thisCollider;
    public GameObject objectInHand;

    private void Awake()
    {
        thisCollider = GetComponent<SphereCollider>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    private void SetCollidingObject(Collider other)
    {
        if (!other.GetComponent<Rigidbody>())
        {
            return;
        }

        if (closestColliderDistance > Vector3.Distance(other.transform.position, thisCollider.transform.TransformPoint(thisCollider.center)))
        {
            closestCollider = other;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (closestCollider == other)
        {
            closestCollider = null;
            closestColliderDistance = 50000.0f;
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
            float distance = Vector3.Distance(thisCollider.transform.TransformPoint(thisCollider.center), applicationController.currentCube.transform.position);

            if (closestCollider)
            {
                if (closestCollider == applicationController.currentCube)
                {
                    applicationController.LogCorrectObject(distance);
                }
                else
                {
                    applicationController.LogWrongObject(distance);
                }
                GrabObject();
            }
            else
            {
                applicationController.LogMissedObject(distance);
            }
        }
        else if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                applicationController.LogDroppedObject();
                ReleaseObject();
            }
        }
    }

    private void GrabObject()
    {
        objectInHand = closestCollider.gameObject;
        closestCollider = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
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
        objectInHand = null;
    }

    private void LateUpdate()
    {
        closestColliderDistance = 50000.0f;
        closestCollider = null;
    }
}