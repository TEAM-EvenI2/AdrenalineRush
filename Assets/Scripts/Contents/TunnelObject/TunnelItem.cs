using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelItem : MonoBehaviour
{
	private Transform rotater;

	private void Awake()
	{
		rotater = transform.GetChild(0);
	}

	public virtual void Setting(Tunnel tunnel, float curveRotation, float ringRotation, float distanceFromCenter)
	{
		transform.SetParent(tunnel.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f); 

        transform.localPosition = tunnel.GetPointOnTorus(curveRotation * Mathf.Deg2Rad, ringRotation * Mathf.Deg2Rad, distanceFromCenter);
	}
}
