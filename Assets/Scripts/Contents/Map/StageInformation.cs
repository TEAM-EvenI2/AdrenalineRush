using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class StageInformation 
{
    [System.Serializable]
    public class ItemPlaceStruct
    {
        public int enterCount;
        public float[] eachPercents;
        public List<EditableMap> itemPlaces;
    }

    public int enterPoint;
    public Color color;
    public List<MapMeshDataWrapper> meshDataWrappers;
    public List<ItemPlaceStruct> itemPlaceStructs;

    public EditableMap GetRandomItemPlace(int curCount)
    {
        if (itemPlaceStructs.Count == 0)
            return null;

        int innerCount = curCount - enterPoint;
        int index = 0;
        for(int i =0; i < itemPlaceStructs.Count; i++)
        {
            if (itemPlaceStructs[i].enterCount < innerCount)
            {
                if(index <  itemPlaceStructs.Count - 1) 
                index++;
            }
            else
                break;
        }

        ItemPlaceStruct ips = itemPlaceStructs[index];
        float r = Random.Range(0, 1f);
        float totalP = 0;
        for(int i = 0; i < ips.eachPercents.Length; i++)
        {
            totalP += ips.eachPercents[i];
            if(r <= totalP)
            {
                return ips.itemPlaces[Random.Range(0, ips.itemPlaces.Count)];
            }
        }
        return null;
    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        foreach(ItemPlaceStruct ips in itemPlaceStructs)
        {
            float[] eachPercents = new float[itemPlaceStructs.Count];
            for(int i = 0; i < ips.eachPercents.Length && i < eachPercents.Length; i++)
            {
                eachPercents[i] = ips.eachPercents[i];
            }
            if (eachPercents.Length == 1)
                eachPercents[0] = 1;
            ips.eachPercents = eachPercents;
        }
    }
#endif
}
