using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapItem : MonoBehaviour
{

	/// <summary>
	/// 
	/// </summary>
	/// <param name="mw"></param>
	/// <param name="curverPecent"> Degree percent</param>
	/// <param name="ringPercent"> Degree percent</param>
	/// <param name="distanceFromCenter"></param>
	public virtual void Setting(MapMeshWrapper mw, float curverPecent, float ringPercent, float distanceFromCenter)
	{
		float curveArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;
		float curveRotation = curverPecent * mw.curveAngle;
		float ringRotation = ringPercent * 360;

		transform.SetParent(mw.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
		transform.GetChild(0).localRotation = Quaternion.Euler(ringRotation, 0f, 0f); 

        transform.localPosition = mw.GetPointOnSurface(curveRotation * Mathf.Deg2Rad, ringRotation * Mathf.Deg2Rad, distanceFromCenter);
	}
}
