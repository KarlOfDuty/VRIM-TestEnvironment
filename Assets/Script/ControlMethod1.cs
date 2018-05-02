using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMethod1 : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;

    private GameObject collidingObject;
    private GameObject objectInHand;

    private LinkedList<GameObject> array;

    public ApplicationController applicationController;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;

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
            float distance = Vector3.Distance(GetComponent<SphereCollider>().transform.position, applicationController.currentCube.transform.position);
            //Collider coll = GetComponent<SphereCollider>();
            //coll.bounds
            if (collidingObject)
            {
                if (collidingObject == applicationController.currentCube)
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
}