using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : ItemGenerator
{
	public MapItem[] itemPrefabs;

	public float spawnPercent = 0.5f;

	public override void GenerateItems(MeshWrapper mw)
	{
		//float angleStep = mw.curveAngle / mw.curveSegmentCount;
		//for (int i = 0; i < mw.curveSegmentCount; i++)
		//{
		//	if(Random.Range(0 ,1f) > spawnPercent)
  //          {
		//		continue;	
  //          }
		//	MapItem item = Instantiate(
		//		itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
		//	float pipeRotation =
		//		(Random.Range(0, mw.tunnelSegmentCount) + 0.5f) *
		//		360f / mw.tunnelSegmentCount;
		//	item.Setting(mw, i * angleStep, pipeRotation, mw.tunnelRadius);
		//}
	}
}
