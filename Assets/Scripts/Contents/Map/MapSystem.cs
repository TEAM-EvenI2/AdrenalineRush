using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MapSystem : MonoBehaviour
{
	public MeshWrapper meshWrapperPrefab;

	public int mapCount; // ���忡 �� ���� ������ �� �ִ� map�� ����
	public int emptyPipeCount;

	public List<StageInformation> stageInfo;
	public float minDistanceEachPreset = 0.5f;

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
			GenerateMap(mw, i > emptyPipeCount);

			if (i > 0)
			{
				mw.AlignWith(maps[i - 1]);
			}
		}
		AlignNextPipeWithOrigin();
	}

	private void GenerateMap(MeshWrapper mw, bool withItem = true)
    {
		MapMeshDataWrapper mmdw = stageInfo[currentStage].meshDataWrappers[0];

		mw.Generate(mmdw.GetMesh(), mmdw.meshType);

        if (withItem)
        {
            // Generate Item
            float finishedAngle = 0;
            while (true)
            {
                if (itemInfos.Count == 0)
                {
                    stageInfo[currentStage].GetRandomItemPlace().AddItemToInfos(itemInfos);
                    //finishedAngle += minDistanceEachPreset;
                }

                MapItemGenerateInfo info = itemInfos.Peek();
                if (info.positionDelta + finishedAngle > mw.curveAngle)
                {
                    info.positionDelta -= mw.curveAngle - finishedAngle;
                    break;
                }
                else
                {
                    finishedAngle += info.positionDelta;
                    MapItem item = Instantiate(info.prefab);
                    float pipeRotation = info.angle;
                    item.Setting(mw, finishedAngle, pipeRotation , mmdw.GetMesh().mapSize);

                    itemInfos.Dequeue();
                }
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
