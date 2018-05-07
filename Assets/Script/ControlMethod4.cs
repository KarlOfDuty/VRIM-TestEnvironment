using UnityEngine;

public class ControlMethod4 : MonoBehaviour
{
    public ApplicationController applicationController;

    public GameObject leftControllerObject;
    private SteamVR_TrackedObject _trackedObj1;

    private SteamVR_Controller.Device LeftController
    {
        get { return SteamVR_Controller.Input((int)_trackedObj1.index); }
    }

    public GameObject rightControllerObject;
    private SteamVR_TrackedObject _trackedObj2;

    private SteamVR_Controller.Device RightController
    {
        get { return SteamVR_Controller.Input((int)_trackedObj2.index); }
    }

    public GameObject laserPrefab;
    private GameObject _laser;
    private Vector3 _hitPoint;

    public GameObject heldGameObject;

    private void Start()
    {
        _trackedObj1 = leftControllerObject.GetComponent<SteamVR_TrackedObject>();
        _trackedObj2 = rightControllerObject.GetComponent<SteamVR_TrackedObject>();
        _laser = Instantiate(laserPrefab);
    }

    private void ShowLaser(RaycastHit hit)
    {
        _laser.SetActive(true);
        _laser.transform.position = Vector3.Lerp(transform.position + new Vector3(0.0f, 0.2f, 0.0f), _hitPoint, 0.5f);
        _laser.transform.LookAt(_hitPoint);
        _laser.transform.localScale = new Vector3(_laser.transform.localScale.x, _laser.transform.localScale.y, hit.distance);
    }

    private void GrabObject(GameObject obj)
    {
        heldGameObject = obj;

        FixedJoint joint = AddFixedJoint();
        joint.connectedBody = obj.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 1000;
        fx.breakTorque = 1000;
        return fx;
    }

    private void Update()
    {
        //Drawing laser and picking up object
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            _hitPoint = hit.point;
            ShowLaser(hit);
            if ((LeftController.GetHairTriggerDown() || RightController.GetHairTriggerDown()) && !heldGameObject && hit.collider.gameObject.GetComponent<Rigidbody>())
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
            else if ((LeftController.GetHairTriggerDown() || RightController.GetHairTriggerDown()) && !heldGameObject)
            {
                applicationController.LogMissedObject(-1.0f);
            }
        }

        //Releasing an object
        if ((LeftController.GetHairTriggerUp() || RightController.GetHairTriggerUp()) && heldGameObject)
        {
            applicationController.LogDroppedObject();
            ReleaseObject();
        }
        else if (heldGameObject && !GetComponent<FixedJoint>())
        {
            FixedJoint joint = AddFixedJoint();
            joint.connectedBody = heldGameObject.GetComponent<Rigidbody>();
        }
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            heldGameObject.GetComponent<Rigidbody>().velocity = _laser.GetComponent<Rigidbody>().velocity;
            heldGameObject.GetComponent<Rigidbody>().angularVelocity = _laser.GetComponent<Rigidbody>().angularVelocity;
        }
        heldGameObject = null;
    }
}