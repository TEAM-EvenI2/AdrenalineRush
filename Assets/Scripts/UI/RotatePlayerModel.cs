using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerModel : MonoBehaviour
{
    public float rotateSpeedX = 0.1f;
    public float rotateSpeedY = 0.1f;
    public float rotateSpeedZ = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(rotateSpeedX, rotateSpeedY, rotateSpeedZ));
    }
}
