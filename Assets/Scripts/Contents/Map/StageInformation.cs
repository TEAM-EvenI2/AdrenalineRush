using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class StageInformation 
{
    public float enterPoint;
    public VolumeProfile volumeProfile;
    public List<MapMeshDataWrapper> meshDataWrappers;
    public List<EditableMap> itemPlaces;

    public EditableMap GetRandomItemPlace()
    {
        if (itemPlaces.Count == 0)
            return null;

        return itemPlaces[Random.Range(0, itemPlaces.Count)];
    }
}
