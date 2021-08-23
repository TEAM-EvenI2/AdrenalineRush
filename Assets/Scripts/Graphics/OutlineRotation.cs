using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineRotation : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.parent.parent.rotation.x * -1, transform.parent.parent.rotation.y * -1, transform.parent.parent.rotation.z * -1);
    }
}
