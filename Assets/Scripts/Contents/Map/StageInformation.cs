using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInformation 
{
    public List<MapMeshDataWrapper> meshDataWrappers;
    public List<EditableMap> itemPlaces;

    public EditableMap GetRandomItemPlace()
    {
        return itemPlaces[Random.Range(0, itemPlaces.Count)];
    }
}
