using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelSystem : MonoBehaviour
{
	public Tunnel tunnelPrefab;

	public int pipeCount;
	public int emptyPipeCount;

	private Tunnel[] tunnel;


	private void Awake()
	{
		tunnel = new Tunnel[pipeCount];
		for (int i = 0; i < tunnel.Length; i++)
		{
			Tunnel pipe = tunnel[i] = Instantiate(tunnelPrefab);
			pipe.transform.SetParent(transform, false);
			pipe.Generate(i > emptyPipeCount);
			if (i > 0)
			{
				pipe.AlignWith(tunnel[i - 1]);
			}
		}
		AlignNextPipeWithOrigin();
	}
	 
	public Tunnel SetupFirstPipe()
	{
		transform.localPosition = new Vector3(0f, -tunnel[1].CurveRadius);
		return tunnel[1];
	}

	public Tunnel SetupNextPipe()
	{
		ShiftPipes();
		AlignNextPipeWithOrigin();
		tunnel[tunnel.Length - 1].Generate();
		tunnel[tunnel.Length - 1].AlignWith(tunnel[tunnel.Length - 2]);
		transform.localPosition = new Vector3(0f, -tunnel[1].CurveRadius);
		return tunnel[1];
	}
	private void ShiftPipes()
	{
		Tunnel temp = tunnel[0];
		for (int i = 1; i < tunnel.Length; i++)
		{
			tunnel[i - 1] = tunnel[i];
		}
		tunnel[tunnel.Length - 1] = temp;
	}
	private void AlignNextPipeWithOrigin()
	{
		Transform transformToAlign = tunnel[1].transform;
		for (int i = 0; i < tunnel.Length; i++)
		{
			if(i != 1)
			tunnel[i].transform.SetParent(transformToAlign);
		}

		transformToAlign.localPosition = Vector3.zero;
		transformToAlign.localRotation = Quaternion.identity;

		for (int i = 0; i < tunnel.Length; i++)
		{
			if (i != 1)
				tunnel[i].transform.SetParent(transform);
		}
	}
}
