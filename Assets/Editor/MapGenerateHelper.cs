using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using static Define;

public class MapGenerateHelper : EditorWindow
{

    private Color borderColor = new Color(.2f, .2f, .2f);

    private int headerHeight;
    private const float keyWidth = 10;
    private const float keyHeight = 20;

    private Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    private Rect headerSection;
    private EditableMap targetMap;
    private Material targetMaterial;


    private Color objectSettingSectionColor = new Color(0.4f,0.4f,0.4f);
    private Rect objectSettingSection;
    private Rect mapPreviewRect;
    private const int previewBorderSize = 10;

    Rect rotatePointRect;
    Vector2 rotatePointCenter;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    Vector2Int selectedKeyIndex;
    bool needsRepaint;

    int currentPickerWindow = -1;
    float objectPosPercent = 0;

    static TorusMesh torusMesh;

    MapMeshType meshType;

    [MenuItem("Window/Map Generate Helper")]
    static void Init()
    {
        MapGenerateHelper window = (MapGenerateHelper)GetWindow(typeof(MapGenerateHelper));
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnEnable()
    {
        torusMesh = new TorusMesh();
    }

    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        if (targetMap)
        {
            DrawObjectSetting();
            HandleInput();

            if (needsRepaint)
            {
                needsRepaint = false;
                Repaint();
            }

        }
    }

    private void DrawLayouts()
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            headerHeight = 120;
        else
            headerHeight = 90;
        switch (meshType)
        {
            case MapMeshType.Tunnel:
            case MapMeshType.ReverseTunnel:
                headerHeight += 36;
                break;
        }


        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = position.width;
        headerSection.height = headerHeight;

        objectSettingSection.x = 0;
        objectSettingSection.y = headerHeight;
        objectSettingSection.width = position.width;
        objectSettingSection.height = position.height - headerHeight;

        EditorGUI.DrawRect(headerSection, headerSectionColor);
        EditorGUI.DrawRect(objectSettingSection, objectSettingSectionColor);
        EditorGUI.DrawRect(new Rect(0, headerHeight-2, position.width, 4), borderColor);
    }

    #region Header

    private void DrawHeader()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        GUILayout.BeginArea(headerSection);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Map Generate" + (PrefabStageUtility.GetCurrentPrefabStage() != null ? " Prefab" : ""), style);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Main Map", GUILayout.MaxWidth(100));
        targetMap = EditorGUILayout.ObjectField(targetMap, typeof(EditableMap), true, GUILayout.MaxWidth(250)) as EditableMap;
        GUILayout.Label("Material", GUILayout.MaxWidth(100));
        targetMaterial = EditorGUILayout.ObjectField(targetMaterial, typeof(Material), true, GUILayout.MaxWidth(250)) as Material;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (targetMap == null)
            meshType = (MapMeshType)EditorGUILayout.EnumPopup(meshType, GUILayout.MaxWidth(100));
        else
        {
            targetMap.meshType = meshType = (MapMeshType)EditorGUILayout.EnumPopup(targetMap.meshType, GUILayout.MaxWidth(100));
        }
        GUILayout.EndHorizontal();


        switch (meshType) {
            case MapMeshType.Tunnel:
                DrawTunnelMesh();
                break;
            case MapMeshType.Plane:

                break;
            case MapMeshType.ReverseTunnel:

                break;

        }

        int _h = PrefabStageUtility.GetCurrentPrefabStage() != null ? 50 : 25;
        GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, headerHeight - _h, 100, _h));

        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            if (GUILayout.Button("Save Prefab", GUILayout.MaxWidth(250)))
            {
                string path = PrefabStageUtility.GetPrefabStage(targetMap.gameObject).assetPath; // Get the prefab path in disk
                PrefabUtility.SaveAsPrefabAsset(targetMap.gameObject, path); // Save the prefab to disk   
            }
        }

        if ( targetMaterial != null)
        {
            if (targetMap)
            {
                if (GUILayout.Button("Regenerate", GUILayout.MaxWidth(250)))
                {
                    targetMap.GetComponent<MeshRenderer>().material = targetMaterial;

                    targetMap.Generate();
                    targetMap.transform.localPosition = new Vector3(0, -targetMap.meshWrapper.curveRadius);
                    for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
                    {
                        for(int j = 0; j < targetMap.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
                        {
                            EditableMap.ObjectEditInfo emoei = targetMap.prefabObjectEditInfos[i].spawnedObjectInfos[j];
                            targetMap.UpdateObject(new Vector2Int(i, j), emoei.percent, emoei.angle);
                            emoei.curveAngle = targetMap.meshWrapper.curveAngle;
                            emoei.curveRadius = targetMap.meshWrapper.curveRadius;
                        }
                    }
                }
            }
            else
            {

                if (GUILayout.Button("Generate", GUILayout.MaxWidth(250)))
                {
                    GameObject tunnel = new GameObject("Generated Tunnel");
                    tunnel.AddComponent<MeshFilter>();
                    tunnel.AddComponent<MeshRenderer>().material = targetMaterial;       
                    tunnel.AddComponent<MeshWrapper>();
                    targetMap = tunnel.AddComponent<EditableMap>();
                    targetMap.meshWrapper = targetMap.GetComponent<MeshWrapper>();

                    GenerateDefaultMapMesh();
                    targetMap.transform.localPosition = new Vector3(0, -targetMap.meshWrapper.curveRadius);

                }
            }
        }
        GUILayout.EndArea();
        GUILayout.EndArea();

        if(targetMap != null)
            targetMap.ValidateCheck();
    }

    private void DrawTunnelMesh()
    {

        TorusMesh mesh = targetMap == null ? torusMesh : (TorusMesh)targetMap.GetMesh();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Tunnel Radius", GUILayout.MaxWidth(90));
        mesh.mapSize = EditorGUILayout.FloatField(mesh.mapSize, GUILayout.MaxWidth(40));
        EditorGUILayout.LabelField("Tunnel Segment Count", GUILayout.MaxWidth(140));
        mesh.roadSegmentCount = EditorGUILayout.IntField(mesh.roadSegmentCount, GUILayout.MaxWidth(40));

        EditorGUILayout.LabelField("Ring Distance", GUILayout.MaxWidth(90));
        mesh.ringDistance = EditorGUILayout.FloatField(mesh.ringDistance, GUILayout.MaxWidth(40));
        EditorGUILayout.EndHorizontal();


        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Curve Radius", GUILayout.MaxWidth(90));
        mesh.minCurveRadius = mesh.maxCurveRadius = EditorGUILayout.FloatField(mesh.maxCurveRadius, GUILayout.MaxWidth(40));

        EditorGUILayout.LabelField("Curve Segment Count", GUILayout.MaxWidth(140));
        mesh.minCurveSegmentCount = mesh.maxCurveSegmentCount = EditorGUILayout.IntField(mesh.maxCurveSegmentCount, GUILayout.MaxWidth(40));

        EditorGUILayout.LabelField("Noise Strength", GUILayout.MaxWidth(90));
        mesh.noiseStrength = EditorGUILayout.FloatField(mesh.noiseStrength, GUILayout.MaxWidth(40));
        EditorGUILayout.EndHorizontal();
      
    }

    private void GenerateDefaultMapMesh()
    {
        if (targetMap == null)
            return;

        switch (targetMap.meshType)
        {
            case MapMeshType.Tunnel:
                {
                    TorusMesh tm = (TorusMesh)targetMap.GetMesh();

                    tm.roadSegmentCount = torusMesh.roadSegmentCount;
                    tm.minCurveRadius = tm.maxCurveRadius = torusMesh.maxCurveRadius;
                    tm.minCurveSegmentCount = tm.maxCurveSegmentCount = torusMesh.maxCurveSegmentCount;
                    tm.ringDistance = torusMesh.ringDistance;
                    tm.mapSize = torusMesh.mapSize;
                    break;
                }
        }
        targetMap.Generate();
    }
#endregion


    private void DrawObjectSetting()
    {
        GUILayout.BeginArea(objectSettingSection);

        mapPreviewRect = new Rect(previewBorderSize, previewBorderSize, position.width - previewBorderSize * 2, 40);
        EditorGUI.DrawRect(mapPreviewRect, new Color(0.2f, 0.2f, 0.2f));


        if(selectedKeyIndex.x >= targetMap.prefabObjectEditInfos.Count)
        {
            selectedKeyIndex.x = targetMap.prefabObjectEditInfos.Count - 1;
        }
        if (selectedKeyIndex.x < 0)
            selectedKeyIndex.x = 0;
        if (targetMap.prefabObjectEditInfos.Count <= 0)
        {
            GUILayout.EndArea();
            return;
        }


        if (selectedKeyIndex.y >= targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos.Count)
        {
            selectedKeyIndex.y = targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos.Count - 1;
        }
        if (selectedKeyIndex.y < 0)
            selectedKeyIndex.y = 0;


        int keyCount = 0;
        for(int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
        {
            keyCount += targetMap.prefabObjectEditInfos[i].spawnedObjectInfos.Count;
        }

        keyRects = new Rect[keyCount];
        int _i = 0;
        for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
        {
            for (int j = 0; j < targetMap.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
            {
                Vector2Int vIndex = new Vector2Int(i, j);
                EditableMap.ObjectEditInfo edi = targetMap.prefabObjectEditInfos[i].GetObject(j);
                if (edi.percent < 0)
                    targetMap.UpdateObject(vIndex, 0, edi.angle);
                else if (edi.percent > 1)
                    targetMap.UpdateObject(vIndex, 1, edi.angle);

                Rect keyRect = new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 2f, mapPreviewRect.yMax + previewBorderSize, keyWidth, keyHeight);
                if (vIndex == selectedKeyIndex)
                {
                    EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), new Color(0.1f, 0.1f, 0.1f));
                }
                EditorGUI.DrawRect(keyRect, targetMap.prefabObjectEditInfos[i].c);
                EditorGUI.DrawRect(new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 4f, previewBorderSize, keyWidth / 2, 40), targetMap.prefabObjectEditInfos[i].c);
                keyRects[_i] = keyRect;
                _i++;
            }
        }
        EditableMap.ObjectEditInfo sedi = targetMap.prefabObjectEditInfos[selectedKeyIndex.x].GetObject(selectedKeyIndex.y);
        Rect infoRect = new Rect(previewBorderSize, mapPreviewRect.yMax + previewBorderSize * 2 + keyHeight, position.width - previewBorderSize * 2, objectSettingSection.height - (mapPreviewRect.height + previewBorderSize * 4 + keyHeight));
        GUILayout.BeginArea(infoRect);


        GUILayout.BeginHorizontal();
        GUILayout.Label("Position Percent", GUILayout.MaxWidth(100));
        float percent = EditorGUILayout.Slider(sedi.percent, 0, 1f, GUILayout.MaxWidth(200));
        if (Mathf.Abs(percent - sedi.percent) > Mathf.Epsilon)
        {
            targetMap.UpdateObject(selectedKeyIndex, percent, sedi.angle);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Space(40);
        GUILayout.Label("Object Rotation", GUILayout.MaxWidth(100));
        float angle = EditorGUILayout.FloatField(sedi.angle, GUILayout.MaxWidth(100));
        if (Mathf.Abs(angle - sedi.angle) > Mathf.Epsilon)
        {
            targetMap.UpdateObject(selectedKeyIndex, sedi.percent, angle);
        }
        GUILayout.EndVertical();


        float sediAngle = -(angle);

        Vector2 size = new Vector2(10, 50);
        Vector2 point = new Vector2(150, 50);// + Vector2.up * size.y / 2;
        rotatePointCenter = point + size / 2f;

        float max = Mathf.Max(size.x, size.y);
        rotatePointRect = new Rect(point.x - 5 - max / 2f + size.x / 2f, point.y - 5, max + 10, max + 10);
        EditorGUI.DrawRect(new Rect(point.x - 5 - max / 2f + size.x /2f, point.y - 5, max + 10, max + 10), new Color(0.25f, 0.25f, 0.25f));

        Matrix4x4 m = GUI.matrix;
        GUIUtility.RotateAroundPivot(sediAngle, point + size / 2);
        EditorGUI.DrawRect(new Rect(point.x, point.y, size.x, size.y), targetMap.prefabObjectEditInfos[selectedKeyIndex.x].c);

        GUI.matrix = m;
        GUILayout.EndHorizontal();

        //float rotatePointSize = 14;
        //rotatePointRect = new Rect(rotatePointCenter.x - rotatePointSize / 2 + (max / 2 + 15) * Mathf.Cos((sediAngle - 90) * Mathf.Deg2Rad),
        //    rotatePointCenter.y - rotatePointSize / 2 + (max / 2 + 15) * Mathf.Sin((sediAngle - 90) * Mathf.Deg2Rad),
        //    rotatePointSize, rotatePointSize);
        //EditorGUI.DrawRect(rotatePointRect, sedi.c);


        GUILayout.EndArea();
        GUILayout.EndArea();
    }

    void HandleInput()
    {
        if (targetMap == null)
            return;

        Event guiEvent = Event.current;

        if (guiEvent.commandName.Equals("ObjectSelectorClosed") && currentPickerWindow == EditorGUIUtility.GetObjectPickerControlID())
        {
            MapItem tiPrefab = (EditorGUIUtility.GetObjectPickerObject() as GameObject)?.GetComponent<MapItem>();
            if (tiPrefab == null)
                return;

            selectedKeyIndex = targetMap.AddObject(tiPrefab, objectPosPercent);

            mouseIsDownOverKey = true;
            needsRepaint = true;
            currentPickerWindow = -1;
            objectPosPercent = 0;

        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (keyRects != null)
            {
                for (int i = 0; i < keyRects.Length; i++)
                {
                    Rect wolrdKeyRect = new Rect(keyRects[i].x, keyRects[i].y + headerHeight, keyRects[i].width, keyRects[i].height);
                    if (wolrdKeyRect.Contains(guiEvent.mousePosition))
                    {
                        mouseIsDownOverKey = true;
                        selectedKeyIndex = targetMap.GetInfoIndex(i);
                        needsRepaint = true;
                        GUI.FocusControl(null);
                        break;
                    }
                }
            }

            Rect worldMapPreviewRect = new Rect(mapPreviewRect.x, mapPreviewRect.y + headerHeight, mapPreviewRect.width, mapPreviewRect.height);
            if (!mouseIsDownOverKey && worldMapPreviewRect.Contains(guiEvent.mousePosition))
            {
                objectPosPercent = Mathf.InverseLerp(mapPreviewRect.x, mapPreviewRect.xMax, guiEvent.mousePosition.x);
                currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", currentPickerWindow);
            }
            RotateObjectEvent(guiEvent);

        }
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            mouseIsDownOverKey = false;
        }

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            if (mouseIsDownOverKey)
            {
                float percent = Mathf.InverseLerp(mapPreviewRect.x, mapPreviewRect.xMax, guiEvent.mousePosition.x);
                //selectedKeyIndex = gradient.UpdateKeyTime(selectedKeyIndex, keyTime);
                // Update Object Position
                targetMap.UpdateObject(selectedKeyIndex, percent, targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].angle);
                needsRepaint = true;
            }
            RotateObjectEvent(guiEvent);


        }

        if (guiEvent.keyCode == KeyCode.Backspace  && guiEvent.type == EventType.KeyDown)
        {
            if (selectedKeyIndex.x >= 0 && selectedKeyIndex.y >= 0)
            {
                targetMap.RemoveObject(ref selectedKeyIndex);
                needsRepaint = true;
            }
        }

    }

    private void RotateObjectEvent(Event guiEvent)
    {

        Vector2 wrpc = rotatePointCenter + new Vector2(previewBorderSize, headerHeight + previewBorderSize * 3 + keyHeight + mapPreviewRect.height);
        Rect worldRotatePointRect = new Rect(rotatePointRect.x + previewBorderSize, rotatePointRect.y + headerHeight + previewBorderSize * 3 + keyHeight + mapPreviewRect.height, rotatePointRect.width, rotatePointRect.height);
        if (worldRotatePointRect.Contains(guiEvent.mousePosition))
        {
            float angle = Mathf.Atan2((wrpc.x - guiEvent.mousePosition.x), (wrpc.y - guiEvent.mousePosition.y)) * Mathf.Rad2Deg;
            targetMap.UpdateObject(selectedKeyIndex, targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].percent, angle);
            needsRepaint = true;
        }
    }



}



