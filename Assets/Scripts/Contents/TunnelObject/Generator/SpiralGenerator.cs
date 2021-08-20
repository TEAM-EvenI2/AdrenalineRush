using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralGenerator : ItemGenerator
{
	public TunnelItem[] itemPrefabs;

	public override void GenerateItems(Tunnel pipe)
	{
		float start = (Random.Range(0, pipe.tunnelSegmentCount) + 0.5f);
		float direction = Random.value < 0.5f ? 1f : -1f;

		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;
		for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			TunnelItem item = Instantiate(
				itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =
				(start + i * direction) * 360f / pipe.tunnelSegmentCount;
			item.Setting(pipe, i * angleStep, pipeRotation);
		}
	}
}
