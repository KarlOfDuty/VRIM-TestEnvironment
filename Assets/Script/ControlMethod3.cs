using UnityEngine;

public class ControlMethod3 : MonoBehaviour
{
    public ApplicationController applicationController;

    private SteamVR_TrackedObject _trackedObj;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)_trackedObj.index); }
    }

    public GameObject laserPrefab;
    private GameObject _laser;
    private Vector3 _hitPoint;

    public GameObject heldGameObject;

    private void Awake()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        _laser = Instantiate(laserPrefab);
    }

    private void ShowLaser(RaycastHit hit)
    {
        _laser.SetActive(true);
        _laser.transform.position = Vector3.Lerp(_trackedObj.transform.position, _hitPoint, 0.5f);
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

    // Update is called once per frame
    private void Update()
    {
        //Drawing laser and picking up object
        RaycastHit hit;
        if (Physics.Raycast(_trackedObj.transform.position, transform.forward, out hit, 100))
        {
            _hitPoint = hit.point;
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

        //Releasing an object
        if (Controller.GetHairTriggerUp() && heldGameObject)
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

            heldGameObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            heldGameObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        heldGameObject = null;
    }
}