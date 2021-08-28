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
	private int currentMap = 0;

	public MeshWrapper[] maps;
	private List<MapItemGenerateInfo> itemInfos;

	private void Awake()
	{
		Random.InitState(0);

		maps = new MeshWrapper[mapCount];
		itemInfos = new List<MapItemGenerateInfo>();

		currentStage = 0;

		for(int i = 0; i< mapCount; i++)
		{
			MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
			MeshWrapper mw = maps[i] = Instantiate(meshWrapperPrefab);
			mw.gameObject.name = mw.gameObject.name + " " + i;
			mw.transform.SetParent(transform, false);
			mw.Init(mmdw.GetMesh(), mmdw.meshType);
			if (i > 0)
			{
				mw.AlignWith(maps[i - 1]);
				maps[i].cumulativeRelativeRotation = (maps[i - 1].cumulativeRelativeRotation + maps[i].relativeRotation) % 360;

			}
			if (i > emptyPipeCount)
			{
				GenerateItem(mw, i);
			}

            mw.position = mw.transform.localPosition;
            mw.angle = mw.transform.localEulerAngles;
        }

		for (int i = 0; i < mapCount; i++)
		{
			MeshWrapper mw = maps[i];
			mw.GenerateMesh(false);

		}

        SetupNextPipe();

    }


    private void GenerateItem(MeshWrapper mw, int i)
	{

		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
		// Generate Item
		float finishedArc = 0;

		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;

		while (true)
		{
			if (itemInfos.Count == 0)
			{
				EditableMap itemPlace = stageInfo[currentStage].GetRandomItemPlace();
				if (itemPlace == null)
					break;
				itemPlace.AddItemToInfos(itemInfos);

				if (itemInfos.Count == 0)
					break;

				finishedArc += minDistanceEachPreset;
			}

			MapItemGenerateInfo info = itemInfos[0];
			if (finishedArc + info.curveArc > curArc)
			{
				info.curveArc -= curArc - finishedArc;
				break;
			}
			else
			{
				finishedArc += info.curveArc;

				MapItem item = Instantiate(info.prefab);
				float angle = info.angle + (i > 0 ? -mw.cumulativeRelativeRotation : 0);
				if (angle < 0)
					angle += 360;

				float distance = mmdw.GetMesh().GetDistance(mw, finishedArc / curArc, angle / 360);
				if (item is ScoreItem)
					distance = mmdw.GetMesh().mapSize;

				item.Setting(mw, finishedArc / curArc, angle/ 360, distance);

				itemInfos.RemoveAt(0);
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
		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
		MeshWrapper mw = maps[maps.Length - 1];

        mw.Init(mmdw.GetMesh(), mmdw.meshType);
        mw.AlignWith(maps[maps.Length - 2]);
        mw.cumulativeRelativeRotation = (maps[maps.Length - 2].cumulativeRelativeRotation + mw.relativeRotation) % 360;

		mw.position = maps[maps.Length - 2].position + (mw.transform.localPosition - maps[maps.Length - 2].transform.localPosition);
		mw.angle = maps[maps.Length - 2].angle + (mw.transform.localEulerAngles - maps[maps.Length - 2].transform.localEulerAngles);

		mw.GenerateMesh(true);
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
