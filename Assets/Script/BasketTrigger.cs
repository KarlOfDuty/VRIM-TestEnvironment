using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    private AudioSource _sound;
    public GameObject applicationController;

    // Use this for initialization
    private void Start()
    {
        _sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == applicationController.GetComponent<ApplicationController>().currentCube)
        {
            applicationController.GetComponent<ApplicationController>().DeleteCurrentCube();
            _sound.Play();
        }
        else
        {
            other.gameObject.transform.position = other.gameObject.GetComponent<CubeRespawnScript>().startPos;
        }
    }
}