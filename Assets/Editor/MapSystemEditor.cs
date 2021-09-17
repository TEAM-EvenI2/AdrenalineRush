using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapSystem))]
public class MapSystemEditor : Editor
{
    MapSystem system;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(20);

        if (system.stageInfo == null)
            return;
        HashSet<MapMeshDataWrapper> mmdwSet = new HashSet<MapMeshDataWrapper>();
        for (int i = 0; i < system.stageInfo.Count; i++)
        {
            if (system.stageInfo[i].meshDataWrappers == null)
                continue;
            for (int j = 0; j < system.stageInfo[i].meshDataWrappers.Count; j++)
            {
                MapMeshDataWrapper mmdw = system.stageInfo[i].meshDataWrappers[j];
                if (mmdw == null)
                    continue;
                if (!mmdwSet.Contains(mmdw))
                    mmdwSet.Add(mmdw);
            }
        }

        foreach(MapMeshDataWrapper mmdw  in mmdwSet)
        {
            DrawSettingEditor(mmdw, ref mmdw.foldout);
        }
    }

    private void DrawSettingEditor(Object setting, ref bool foldout)
    {
        foldout = EditorGUILayout.InspectorTitlebar(foldout, setting);
        if (foldout)
        {
            Editor editor = CreateEditor(setting);
            editor.OnInspectorGUI();
        }

    }

    private void OnEnable()
    {
        system = (MapSystem)target;
    }
}
