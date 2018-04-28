using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRespawnScript : MonoBehaviour
{
    public Vector3 startPos;

    // Use this for initialization
    private void Start()
    {
        startPos = transform.position;
    }
}