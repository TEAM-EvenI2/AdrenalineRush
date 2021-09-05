using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapSystem : MonoBehaviour
{
	public MapMeshWrapper meshWrapperPrefab;

	public int mapCount; // 월드에 한 번에 생성될 수 있는 map의 개수
	public int emptyPipeCount;

	public List<StageInformation> stageInfo;
	public float minDistanceEachPreset = 0.1f;

	private int currentStage = -1;
	private int currentMap = 0;

	public MapMeshWrapper[] maps;
	private List<MapItemGenerateInfo> itemInfos;
	private int _infoIndex = 0;

	private void Awake()
	{
		maps = new MapMeshWrapper[mapCount];
		itemInfos = new List<MapItemGenerateInfo>();

		currentStage = 0;

		for(int i = 0; i< mapCount; i++)
		{
			MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
			MapMeshWrapper mw = maps[i] = Instantiate(meshWrapperPrefab);
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
				GenerateItem(mw);
			}

            mw.position = mw.transform.localPosition;
            mw.angle = mw.transform.localEulerAngles;
        }

		for (int i = 0; i < mapCount; i++)
		{
			MapMeshWrapper mw = maps[i];
			mw.GenerateMesh(false);

		}

        SetupNextPipe();

    }

    private void Update()
    {
		if (currentStage < stageInfo.Count - 1)
		{

			float score = Managers.Instance.GetScene<GameScene>().player.earnedScore;

			if(score >= stageInfo[currentStage + 1].enterPoint)
            {
				// TODO Update Stage
				ChangeStage();

				Managers.Instance.GetUIManager<GameUIManager>().cg.gameObject.SetActive(true);
				Managers.Instance.GetUIManager<GameUIManager>().cg.alpha = 1;

			}
		}

		
    }

	private void ChangeStage()
	{
		currentStage++;
		Managers.Instance.GetScene<GameScene>().postProcessVolume.profile = stageInfo[currentStage].volumeProfile;

		for (int i = 0; i < mapCount ; i++)
		{
			maps[i].DestoryChild();
			if (i > emptyPipeCount)
				GenerateItem(maps[i]);
		}
        SetupNextPipe();
    }


    private void GenerateItem(MapMeshWrapper mw)
	{

		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
		// Generate Item
		float finishedArc = 0;

		float curArc = mw.curveRadius * mw.curveAngle * Mathf.Deg2Rad;

		mw.ResetGenerateItem();

		while (true)
		{
			if (itemInfos.Count == _infoIndex)
			{
				EditableMap itemPlace = stageInfo[currentStage].GetRandomItemPlace();
				if (itemPlace == null)
					break;

				itemPlace.AddItemToInfos(itemInfos);
				_infoIndex = 0;

				if (itemInfos.Count == 0)
					break;

				finishedArc += minDistanceEachPreset;
			}

			MapItemGenerateInfo info = itemInfos[_infoIndex];
			if (finishedArc + info.curveArc > curArc)
			{
				info.curveArc -= curArc - finishedArc;
				break;
			}
			else
			{
				finishedArc += info.curveArc;

				mw.AddGenerateItem(info);
				_infoIndex++;
			}
		}

	}


	public MapMeshWrapper SetupFirstPipe()
	{
		transform.localPosition = new Vector3(0f, -maps[1].curveRadius);
		return maps[1];
	}

	public MapMeshWrapper SetupNextPipe(bool dontCreateItem = false)
	{
		ShiftPipes();
		AlignNextPipeWithOrigin();
		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[currentMap];
		MapMeshWrapper mw = maps[maps.Length - 1];

        mw.Init(mmdw.GetMesh(), mmdw.meshType);
        mw.AlignWith(maps[maps.Length - 2]);
        mw.cumulativeRelativeRotation = (maps[maps.Length - 2].cumulativeRelativeRotation + mw.relativeRotation) % 360;

		mw.position = maps[maps.Length - 2].position + (mw.transform.localPosition - maps[maps.Length - 2].transform.localPosition);
		mw.angle = maps[maps.Length - 2].angle + (mw.transform.localEulerAngles - maps[maps.Length - 2].transform.localEulerAngles);

		mw.GenerateMesh(true);
		if (!dontCreateItem) 
			GenerateItem(maps[maps.Length - 1]);

		transform.localPosition = new Vector3(0f, -maps[1].curveRadius);

		for (int i=0; i < maps[1].transform.childCount; i++){
            if (maps[1].transform.GetChild(i).GetComponent<MeshCollider>())
            {
				maps[1].transform.GetChild(i).GetComponent<MeshCollider>().enabled = true;

			}
        }

		maps[2].SpawnItem();

		return maps[1];
	}
	private void ShiftPipes()
	{
		MapMeshWrapper temp = maps[0];
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
