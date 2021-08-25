using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapSystem : MonoBehaviour
{
	public MeshWrapper meshWrapperPrefab;

	public int mapCount; // 월드에 한 번에 생성될 수 있는 map의 개수
	public int emptyPipeCount;

	public List<StageInformation> stageInfo;
	public float minDistanceEachPreset = 0.1f;

	private int currentStage = 0;

	private MeshWrapper[] maps;
	private Queue<MapItemGenerateInfo> itemInfos;

	private void Awake()
	{
		maps = new MeshWrapper[mapCount];
		itemInfos = new Queue<MapItemGenerateInfo>();

		currentStage = 0;

		for(int i = 0; i< mapCount; i++)
        {
			MeshWrapper mw = maps[i] = Instantiate(meshWrapperPrefab);
			mw.transform.SetParent(transform, false);
			GenerateMap(mw);

			if (i > 0)
			{
				mw.AlignWith(maps[i - 1]);
				maps[i].cumulativeRelativeRotation = (maps[i - 1].cumulativeRelativeRotation + maps[i].relativeRotation) % 360;
			}
			if(i > emptyPipeCount)
            {
				GenerateItem(mw, i);
            }

		}
		//AlignNextPipeWithOrigin();
		SetupNextPipe();
	}

	private void GenerateMap(MeshWrapper mw)
    {
		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[0];

		mw.Generate(mmdw.GetMesh(), mmdw.meshType);
    }

	private void GenerateItem(MeshWrapper mw, int i)
	{

		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[0];
		// Generate Item
		float finishedArc = 0;

		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;

		while (true)
		{
			if (itemInfos.Count == 0)
			{
				stageInfo[currentStage].GetRandomItemPlace().AddItemToInfos(itemInfos);
				finishedArc += minDistanceEachPreset;
			}

			MapItemGenerateInfo info = itemInfos.Peek();
			if (finishedArc + info.curveArc > curArc)
			{
				info.curveArc -= curArc - finishedArc;
				break;
			}
			else
			{
				finishedArc += info.curveArc;

				MapItem item = Instantiate(info.prefab);
				item.Setting(mw, (finishedArc / mw.curveRadius) * Mathf.Rad2Deg, info.angle + (i > 0? -mw.cumulativeRelativeRotation : 0), mmdw.GetMesh().mapSize);

				itemInfos.Dequeue();
			}
		}

	}


	public MeshWrapper SetupFirstPipe()
	{
		transform.localPosition = new Vector3(0f, -maps[1].curveRadius);
		return maps[1];
	}

	public MeshWrapper SetupNextPipe()
	{
		ShiftPipes();
		AlignNextPipeWithOrigin();
		GenerateMap(maps[maps.Length - 1]);
		maps[maps.Length - 1].AlignWith(maps[maps.Length - 2]);
		maps[maps.Length - 1].cumulativeRelativeRotation = (maps[maps.Length - 2].cumulativeRelativeRotation + maps[maps.Length - 1].relativeRotation) % 360;
		GenerateItem(maps[maps.Length - 1], maps.Length - 1);
		transform.localPosition = new Vector3(0f, -maps[1].curveRadius);


		return maps[1];
	}
	private void ShiftPipes()
	{
		MeshWrapper temp = maps[0];
		for (int i = 1; i < maps.Length; i++)
		{
			maps[i - 1] = maps[i];
		}
		maps[maps.Length - 1] = temp;
	}
	private void AlignNextPipeWithOrigin()
	{
		Transform transformToAlign = maps[1].transform;
		for (int i = 0; i < maps.Length; i++)
		{
			if (i != 1)
				maps[i].transform.SetParent(transformToAlign);
		}

		transformToAlign.localPosition = Vector3.zero;
		transformToAlign.localRotation = Quaternion.identity;

		for (int i = 0; i < maps.Length; i++)
		{
			if (i != 1)
				maps[i].transform.SetParent(transform);
		}
	}


}
