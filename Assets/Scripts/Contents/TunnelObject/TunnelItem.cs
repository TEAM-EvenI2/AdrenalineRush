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

	public virtual void Setting(Tunnel tunnel, float curveRotation, float ringRotation)
	{
		transform.SetParent(tunnel.transform, false);
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
		rotater.localPosition = new Vector3(0f, tunnel.CurveRadius);
		rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

		// TODO : tunnel의 반지름 크기에 따라서 obstacle 위치, 크기 조정
	}
}
