using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : ItemGenerator
{
	public TunnelItem[] itemPrefabs;

	public float spawnPercent = 0.5f;

	public override void GenerateItems(Tunnel pipe)
	{
		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			if(Random.Range(0 ,1f) > spawnPercent)
            {
				continue;
            }
			TunnelItem item = Instantiate(
				itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =
				(Random.Range(0, pipe.tunnelSegmentCount) + 0.5f) *
				360f / pipe.tunnelSegmentCount;
			item.Setting(pipe, i * angleStep, pipeRotation);
		}
	}
}
