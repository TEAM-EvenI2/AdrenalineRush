
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Define;

public class Utils 
{

    public static Vector3 GetViewPortMousePos()
    {
        return Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }
    public static Vector3 SmoothDampEuler(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
    {
        return new Vector3(
          Mathf.SmoothDampAngle(current.x, target.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(current.y, target.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(current.z, target.z, ref currentVelocity.z, smoothTime)
        );
    }


    public static string GetNameFromPath(string path)
    {
        string name = path;
        int index = name.LastIndexOf('/');
        if (index >= 0)
            name = name.Substring(index + 1);

        return name;
    }

    public static Vector3 MinEach(Vector3 cur, Vector3 tar)
    {
        if(cur.x > tar.x)
        {
            cur.x = tar.x;
        }
        if (cur.y > tar.y)
        {
            cur.y = tar.y;
        }
        if (cur.z > tar.z)
        {
            cur.z = tar.z;
        }
        return cur;
    }

    public static Vector3 MaxEach(Vector3 cur, Vector3 tar)
    {
        if (cur.x < tar.x)
        {
            cur.x = tar.x;
        }
        if (cur.y <tar.y)
        {
            cur.y = tar.y;
        }
        if (cur.z < tar.z)
        {
            cur.z = tar.z;
        }
        return cur;
    }
}
