
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

    public static Vector3 RotateTranslate(Vector3 origin, Vector3 angle, Vector3 translate)
    {
        Quaternion rotation = Quaternion.Euler(angle);
        Matrix4x4 rMat = Matrix4x4.TRS(translate, rotation, Vector3.one);

        Vector3 rotated = rMat.MultiplyPoint3x4(origin);
        //Vector3 rotated;

        //rotated.x = origin.x * Mathf.Cos(angle.x) * Mathf.Cos(angle.y) +
        //    origin.y * (Mathf.Cos(angle.x) * Mathf.Sin(angle.y) * Mathf.Sin(angle.z) -Mathf.Sin(angle.x) * Mathf.Cos(angle.z)) +
        //    origin.z *(Mathf.Cos(angle.x) * Mathf.Sin(angle.y) * Mathf.Cos(angle.z) + Mathf.Sin(angle.x) * Mathf.Sin(angle.z));
        //rotated.y = origin.x * Mathf.Sin(angle.x) * Mathf.Cos(angle.y) +
        //    origin.y * (Mathf.Sin(angle.x) * Mathf.Sin(angle.y) * Mathf.Sin(angle.z) + Mathf.Cos(angle.x) * Mathf.Cos(angle.z)) +
        //    origin.z * (Mathf.Sin(angle.x) * Mathf.Sin(angle.y) * Mathf.Cos(angle.z) + Mathf.Cos(angle.x) * Mathf.Sin(angle.z));
        //rotated.z = origin.x * (-Mathf.Sin(angle.y)) +
        //    origin.y * (Mathf.Cos(angle.y) * Mathf.Sin(angle.z)) +
        //    origin.z * (Mathf.Cos(angle.y) * Mathf.Cos(angle.z));
        //
        //rotated += translate;
        return rotated;
    }
}
