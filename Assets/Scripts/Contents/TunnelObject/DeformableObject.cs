using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableObject : MonoBehaviour
{
    public void Transformation(Tunnel tunnel)
    {

        transform.localScale = new Vector3(transform.localScale.x, tunnel.tunnelRadius * 2, transform.localScale.z);
        transform.localPosition = Vector3.up * -tunnel.tunnelRadius;
    }
}
