
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

    public class Easing
    {
		public class Quadratic
		{
			public static float In(float k)
			{
				return k * k;
			}

			public static float Out(float k)
			{
				return k * (2f - k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k;
				return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
			}

			/* 
			 * Quadratic.Bezier(k,0) behaves like Quadratic.In(k)
			 * Quadratic.Bezier(k,1) behaves like Quadratic.Out(k)
			 *
			 * If you want to learn more check Alan Wolfe's post about it http://www.demofox.org/bezquad1d.html
			 */
			public static float Bezier(float k, float c)
			{
				return c * 2 * k * (1 - k) + k * k;
			}
		};

		public class Cubic
		{
			public static float In(float k)
			{
				return k * k * k;
			}

			public static float Out(float k)
			{
				return 1f + ((k -= 1f) * k * k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k;
				return 0.5f * ((k -= 2f) * k * k + 2f);
			}
		};

		public class Quartic
		{
			public static float In(float k)
			{
				return k * k * k * k;
			}

			public static float Out(float k)
			{
				return 1f - ((k -= 1f) * k * k * k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k * k;
				return -0.5f * ((k -= 2f) * k * k * k - 2f);
			}
		};

		public class Quintic
		{
			public static float In(float k)
			{
				return k * k * k * k * k;
			}

			public static float Out(float k)
			{
				return 1f + ((k -= 1f) * k * k * k * k);
			}

			public static float InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k;
				return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
			}
		};

		public class Sinusoidal
		{
			public static float In(float k)
			{
				return 1f - Mathf.Cos(k * Mathf.PI / 2f);
			}

			public static float Out(float k)
			{
				return Mathf.Sin(k * Mathf.PI / 2f);
			}

			public static float InOut(float k)
			{
				return 0.5f * (1f - Mathf.Cos(Mathf.PI * k));
			}
		};

		public class Exponential
		{
			public static float In(float k)
			{
				return k == 0f ? 0f : Mathf.Pow(1024f, k - 1f);
			}

			public static float Out(float k)
			{
				return k == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * k);
			}

			public static float InOut(float k)
			{
				if (k == 0f) return 0f;
				if (k == 1f) return 1f;
				if ((k *= 2f) < 1f) return 0.5f * Mathf.Pow(1024f, k - 1f);
				return 0.5f * (-Mathf.Pow(2f, -10f * (k - 1f)) + 2f);
			}
		};
	}


	public static SerVector2 Vector2Ser(Vector2 v)
    {
		return new SerVector2(v.x, v.y);
	}
	public static Vector2 Ser2Vector(SerVector2 v)
	{
		return new Vector2(v.x, v.y);
	}
}
