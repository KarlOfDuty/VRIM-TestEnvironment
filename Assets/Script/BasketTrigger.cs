using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketTrigger : MonoBehaviour
{
    private AudioSource sound;
    public GameObject applicationController;

    // Use this for initialization
    private void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == applicationController.GetComponent<ApplicationController>().currentCube)
        {
            applicationController.GetComponent<ApplicationController>().DeleteCurrentCube();
            sound.Play();
        }
        else
        {
            collider.gameObject.transform.position = collider.gameObject.GetComponent<CubeRespawnScript>().startPos;
        }
    }
}