using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour
{
	private Transform rotater;

	private void Awake()
	{
		rotater = transform.GetChild(0);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="map"></param>
	/// <param name="curveRotation"> Degree </param>
	/// <param name="ringRotation"> Degree </param>
	/// <param name="distanceFromCenter"></param>
	public virtual void Setting(MeshWrapper map, float curveRotation, float ringRotation, float distanceFromCenter)
	{
		transform.SetParent(map.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f); 

        transform.localPosition = map.GetPointOnSurface(curveRotation * Mathf.Deg2Rad, ringRotation * Mathf.Deg2Rad, distanceFromCenter);
	}
}
