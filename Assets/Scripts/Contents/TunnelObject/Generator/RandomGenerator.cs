using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : ItemGenerator
{
	public TunnelItem[] itemPrefabs;

	public float spawnPercent = 0.5f;

	public override void GenerateItems(Tunnel tunnel)
	{
		float angleStep = tunnel.CurveAngle / tunnel.CurveSegmentCount;
		for (int i = 0; i < tunnel.CurveSegmentCount; i++)
		{
			if(Random.Range(0 ,1f) > spawnPercent)
            {
				continue;	
            }
			TunnelItem item = Instantiate(
				itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =
				(Random.Range(0, tunnel.tunnelSegmentCount) + 0.5f) *
				360f / tunnel.tunnelSegmentCount;
			item.Setting(tunnel, i * angleStep, pipeRotation, tunnel.tunnelRadius);
		}
	}
}
