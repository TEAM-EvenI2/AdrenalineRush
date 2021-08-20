using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Transform rotater;
    public float distance;


    void Start()
    {
        
    }

    void Update()
    {
        
        transform.position = target.position - transform.forward * distance;
        transform.parent.eulerAngles = rotater.eulerAngles;
    }
}
