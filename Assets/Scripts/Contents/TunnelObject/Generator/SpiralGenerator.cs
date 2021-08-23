using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralGenerator : ItemGenerator
{
	public TunnelItem[] itemPrefabs;

	public override void GenerateItems(Tunnel tunnel)
	{
		float start = (Random.Range(0, tunnel.tunnelSegmentCount) + 0.5f);
		float direction = Random.value < 0.5f ? 1f : -1f;

		float angleStep = tunnel.CurveAngle / tunnel.CurveSegmentCount;
		for (int i = 0; i < tunnel.CurveSegmentCount; i++)
		{
			TunnelItem item = Instantiate(
				itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =
				(start + i * direction) * 360f / tunnel.tunnelSegmentCount;
			item.Setting(tunnel, i * angleStep, pipeRotation, tunnel.tunnelRadius);
		}
	}
}
