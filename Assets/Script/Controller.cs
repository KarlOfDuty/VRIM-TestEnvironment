using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject cubesParent;
    public Material highlightedMaterial;

    public GameObject basketTrigger;
    private GameObject currentCube;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DeleteCurrentCube()
    {
        currentCube.transform.parent = null;
        SetRandomCube();
        Destroy(currentCube, 1.0f);
    }

    private bool SetRandomCube()
    {
        if (cubesParent.transform.childCount == 0)
        {
            return false;
        }
        Transform transform = cubesParent.transform.GetChild(Random.Range(0, cubesParent.transform.childCount));
        currentCube = transform.gameObject;
        currentCube.gameObject.GetComponent<Renderer>().material = highlightedMaterial;
        return true;
    }
}