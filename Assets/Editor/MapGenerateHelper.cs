using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using static Define;

public class MapGenerateHelper : EditorWindow
{

    private Color borderColor = new Color(.7f, .7f, .7f);

    private int headerHeight;
    private const float keyWidth = 10;
    private const float keyHeight = 20;

    private Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    private Rect headerSection;
    private EditableMap targetMap;
    private Material targetMaterial;


    private Color objectPrefabSectionColor = new Color(0.2f, 0.2f, 0.2f);
    private Rect objectPrefabSection;
    private const float prefabFavoriteGap = 5;
    private const float prefabButtonSize = 30;

    private Color objectSettingSectionColor = new Color(0.4f,0.4f,0.4f);
    private Rect objectSettingSection;
    private Rect mapPreviewRect;
    private const int previewBorderSize = 10;

    Vector2 scrollPos;

    Rect rotatePointRect;
    Vector2 rotatePointCenter;
    List<KeyValuePair<Vector2Int, Rect>> keyRects;
    bool mouseIsDownOverKey;
    List<Vector2Int> selectedKeyIndexs;
    bool needsRepaint;

    int currentPickerWindow = -1;
    float objectPosPercent = 0;

    static TorusMesh torusMesh;

    MapMeshType meshType;


    private bool downShift = false;
    private bool downCtrl = false;
     Vector2 dragOn;
    bool drawBox = false;


    [MenuItem("Window/Map Generate Helper")]
    static void Init()
    {
        MapGenerateHelper window = (MapGenerateHelper)GetWindow(typeof(MapGenerateHelper));
        window.minSize = new Vector2(500, 350);
        window.Show();
    }

    private void OnEnable()
    {
        torusMesh = new TorusMesh();
        selectedKeyIndexs = new List<Vector2Int>();
    }

    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        if (targetMap)
        {
            DrawObjectPrefab();
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
            headerHeight = 120;
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

        float prefabSectionHeight = 0;
        if (targetMap != null)
        {
            float tWidth = 0;
            prefabSectionHeight = 30;
            for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
            {
                GUIContent label = new GUIContent(targetMap.prefabObjectEditInfos[i].itemPrefab.name);
                GUIStyle style = GUI.skin.box;
                style.alignment = TextAnchor.MiddleCenter;
                Vector2 size = style.CalcSize(label);
                tWidth += size.x + prefabButtonSize + 20 + prefabFavoriteGap;

                if(tWidth > Screen.width)
                {
                    tWidth = size.x + prefabButtonSize + 20 + prefabFavoriteGap;
                    prefabSectionHeight += 30;
                }
            }
        }
        objectPrefabSection.x = 0;
        objectPrefabSection.y = headerHeight;
        objectPrefabSection.width = position.width;
        objectPrefabSection.height = prefabSectionHeight;

        objectSettingSection.x = 0;
        objectSettingSection.y = headerHeight + prefabSectionHeight;
        objectSettingSection.width = position.width;
        objectSettingSection.height = position.height - (headerHeight - prefabSectionHeight);

        EditorGUI.DrawRect(headerSection, headerSectionColor);
        EditorGUI.DrawRect(objectPrefabSection, objectPrefabSectionColor);
        EditorGUI.DrawRect(objectSettingSection, objectSettingSectionColor);
        EditorGUI.DrawRect(new Rect(0, headerHeight-2, position.width, 2), borderColor);
    }

    #region Header

    private void DrawHeader()
    {

        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            if (targetMap == null)
            {
                targetMap = StageUtility.GetCurrentStageHandle().FindComponentOfType<EditableMap>();
            }

        }

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

        int _h = 50 ;
        GUILayout.BeginArea(new Rect(10, headerHeight - _h, 100, _h));

        if (targetMap != null)
        {
            if (GUILayout.Button("Refresh", GUILayout.MaxWidth(250)))
            {
                targetMap.Refresh();
            }
            if (GUILayout.Button("Find Selected", GUILayout.MaxWidth(250)))
            {
                bool finded = false;
                for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++) {

                    EditableMap.PrefabObjectEditInfo spoei = targetMap.prefabObjectEditInfos[i];
                    for (int j = 0; j < spoei.spawnedObjectInfos.Count; j++)
                    {
                        EditableMap.ObjectEditInfo oei = spoei.spawnedObjectInfos[j];

                        if(oei.ti.gameObject == Selection.activeGameObject)
                        {
                            selectedKeyIndexs.Clear();
                            selectedKeyIndexs.Add(new Vector2Int(i, j));
                            finded = true;
                            break;
                        }
                    }
                    if (finded)
                        break;
                }
            }
        }
            GUILayout.EndArea();
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
                    Regenerate();
                }
                if (targetMap.meshWrapper.mapMesh == null)
                    Regenerate();
            }
            else
            {

                if (GUILayout.Button("Generate", GUILayout.MaxWidth(250)))
                {
                    GameObject tunnel = new GameObject("Generated Tunnel");
                    tunnel.AddComponent<MeshFilter>();
                    tunnel.AddComponent<MeshRenderer>().material = targetMaterial;       
                    tunnel.AddComponent<MapMeshWrapper>();
                    targetMap = tunnel.AddComponent<EditableMap>();
                    targetMap.meshWrapper = targetMap.GetComponent<MapMeshWrapper>();

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

    private void Regenerate()
    {

        targetMap.GetComponent<MeshRenderer>().material = targetMaterial;

        targetMap.Generate();
        targetMap.transform.localPosition = new Vector3(0, -targetMap.meshWrapper.curveRadius);
        for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
        {
            for (int j = 0; j < targetMap.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
            {
                EditableMap.ObjectEditInfo emoei = targetMap.prefabObjectEditInfos[i].spawnedObjectInfos[j];
                targetMap.UpdateObject(new Vector2Int(i, j), emoei.percent, emoei.angle);
                emoei.curveAngle = targetMap.meshWrapper.curveAngle;
                emoei.curveRadius = targetMap.meshWrapper.curveRadius;
            }
        }
    }
#endregion

    private void DrawObjectPrefab()
    {
        GUILayout.BeginArea(objectPrefabSection);
        EditorGUILayout.BeginHorizontal();

        float x = prefabFavoriteGap;
        float y = 5;
        for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
        {
            EditableMap.PrefabObjectEditInfo prefabObjectEditInfo = targetMap.prefabObjectEditInfos[i];

            GUIContent label = new GUIContent(prefabObjectEditInfo.itemPrefab.name);
            GUIStyle style = GUI.skin.box;
            style.alignment = TextAnchor.MiddleCenter;

            Vector2 size = style.CalcSize(label);
            if (x + size.x + prefabButtonSize + 20 > Screen.width)
            {
                x = prefabFavoriteGap;
                y += 30;
            }

            EditorGUI.DrawRect(new Rect(x, y, size.x + prefabButtonSize + 20, size.y), new Color(0.1f, 0.1f, 0.1f));
            EditorGUI.DrawRect(new Rect(x + size.x + 15, y + 2, 5, size.y - 4), prefabObjectEditInfo.c);
             EditorGUI.LabelField(new Rect(x, y, size.x, size.y), label);

            prefabObjectEditInfo.toggle = EditorGUI.Toggle(new Rect(x + size.x, y, prefabButtonSize, size.y), prefabObjectEditInfo.toggle);

            if (GUI.Button(new Rect(x + size.x + 20, y, prefabButtonSize, size.y), "+"))
            {
                selectedKeyIndexs.Clear();

                selectedKeyIndexs.Add(targetMap.AddObject(prefabObjectEditInfo.itemPrefab, 0));

                mouseIsDownOverKey = true;
                needsRepaint = true;
            }

            x += size.x + prefabButtonSize + prefabFavoriteGap + 20;
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }


    private void DrawObjectSetting()
    {
        GUILayout.BeginArea(objectSettingSection);

        mapPreviewRect = new Rect(previewBorderSize, previewBorderSize, position.width - previewBorderSize * 2, 40);
        EditorGUI.DrawRect(mapPreviewRect, new Color(0.2f, 0.2f, 0.2f));

        if (selectedKeyIndexs.Count <= 0)
            selectedKeyIndexs.Add(new Vector2Int(0, 0));


            Vector2Int selectedKeyIndex = selectedKeyIndexs[0];
        
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

        keyRects = new List<KeyValuePair<Vector2Int, Rect>>();// new Rect[keyCount];
        for (int o = 0; o < 2; o++)
        {
            for (int i = 0; i < targetMap.prefabObjectEditInfos.Count; i++)
            {
                if ((o == 0 && targetMap.prefabObjectEditInfos[i].toggle) ||
                    (o == 1 && !targetMap.prefabObjectEditInfos[i].toggle))
                    continue;
                for (int j = 0; j < targetMap.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
                {
                    Vector2Int vIndex = new Vector2Int(i, j);
                    EditableMap.ObjectEditInfo edi = targetMap.prefabObjectEditInfos[i].GetObject(j);
                    if (edi.percent < 0)
                        targetMap.UpdateObject(vIndex, 0, edi.angle);
                    else if (edi.percent > 1)
                        targetMap.UpdateObject(vIndex, 1, edi.angle);

                    Rect keyRect = new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 2f, mapPreviewRect.yMax + previewBorderSize, keyWidth, keyHeight);
                    bool inSelected = false;
                    for(int k = 0; k < selectedKeyIndexs.Count; k++)
                    {
                        if(selectedKeyIndexs[k] == vIndex)
                        {
                            inSelected = true;
                            break;
                        }
                    }
                    if (inSelected)
                    {
                        EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), new Color(0.1f, 0.1f, 0.1f));
                    }


                    Color c = targetMap.prefabObjectEditInfos[i].c;
                    if (!targetMap.prefabObjectEditInfos[i].toggle)
                        c.a = 0.2f;

                    EditorGUI.DrawRect(keyRect, c);
                    EditorGUI.DrawRect(new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 4f, previewBorderSize, keyWidth / 2, 40), c);
                    keyRects.Add(new KeyValuePair<Vector2Int, Rect>(new Vector2Int(i, j), keyRect));
                }
            }
        }
        if(selectedKeyIndexs.Count > 1)
        {
            
            GUILayout.EndArea();
            return;
        }

        EditableMap.ObjectEditInfo sedi = targetMap.prefabObjectEditInfos[selectedKeyIndex.x].GetObject(selectedKeyIndex.y);
        Rect infoRect = new Rect(previewBorderSize, mapPreviewRect.yMax + previewBorderSize * 2 + keyHeight, position.width - previewBorderSize * 2, objectSettingSection.height - (mapPreviewRect.height + previewBorderSize * 4 + keyHeight));
        GUILayout.BeginArea(infoRect);

        GUILayout.BeginHorizontal();
        float percent = EditorGUILayout.Slider("Position Percent", sedi.percent, 0, 1f, GUILayout.MaxWidth(200));
        if (Mathf.Abs(percent - sedi.percent) > Mathf.Epsilon)
        {
            targetMap.UpdateObject(selectedKeyIndex, percent, sedi.angle);
        }

        if(sedi.ti is LongObstacle)
        {
            LongObstacle lo = (LongObstacle)sedi.ti;
            float lo_size = EditorGUILayout.FloatField("Size", lo.size, GUILayout.MaxWidth(200));
            
            if (Mathf.Abs(lo_size - lo.size) > Mathf.Epsilon)
            {
                lo.size = lo_size;
                targetMap.UpdateObject(selectedKeyIndex, percent, sedi.angle);
            }
        }
        else if(sedi.ti is SurfaceObstacle)
        {
            SurfaceObstacle so = (SurfaceObstacle)sedi.ti;
            float so_size_percent = EditorGUILayout.Slider("Size Percent", so.sizePercent, 0.1f, 0.7f);

            if (Mathf.Abs(so_size_percent - so.sizePercent) > Mathf.Epsilon)
            {
                so.sizePercent = so_size_percent;
                targetMap.UpdateObject(selectedKeyIndex, percent, sedi.angle);
            }

        }
        else if (sedi.ti is SurfacePartialObstacle)
        {
            SurfacePartialObstacle so = (SurfacePartialObstacle)sedi.ti;
            float so_size_percent = EditorGUILayout.Slider("Size Percent", so.sizePercent, 0.1f, 0.7f);

            if (Mathf.Abs(so_size_percent - so.sizePercent) > Mathf.Epsilon)
            {
                so.sizePercent = so_size_percent;
                targetMap.UpdateObject(selectedKeyIndex, percent, sedi.angle);
            }

        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.Label("Object Rotation1", GUILayout.MaxWidth(100));
        float angle = EditorGUILayout.FloatField(sedi.angle, GUILayout.MaxWidth(100));
        if (Mathf.Abs(angle - sedi.angle) > Mathf.Epsilon)
        {
            targetMap.UpdateObject(selectedKeyIndex, sedi.percent, angle);
        }
        GUILayout.EndVertical();
        float sediAngle = -(angle);

        Vector2 size = new Vector2(10, 50);
        Vector2 point = new Vector2(130, 30);// + Vector2.up * size.y / 2;
        rotatePointCenter = point + size / 2f;

        float max = Mathf.Max(size.x, size.y);
        rotatePointRect = new Rect(point.x - 5 - max / 2f + size.x / 2f, point.y - 5, max + 10, max + 10);
        EditorGUI.DrawRect(new Rect(point.x - 5 - max / 2f + size.x /2f, point.y - 5, max + 10, max + 10), new Color(0.25f, 0.25f, 0.25f));

        Matrix4x4 m = GUI.matrix;
        GUIUtility.RotateAroundPivot(sediAngle, point + size / 2);
        EditorGUI.DrawRect(new Rect(point.x, point.y, size.x, size.y), targetMap.prefabObjectEditInfos[selectedKeyIndex.x].c);

        GUI.matrix = m;

        if (sedi.ti is LongObstacle)
        {
            LongObstacle lo = (LongObstacle)sedi.ti;
            GUILayout.BeginVertical(GUILayout.MinWidth(300));
            lo.angleInTunnel = EditorGUILayout.Slider("Object Rotation2", lo.angleInTunnel, -60, 60, GUILayout.ExpandWidth(false));
            lo.middleSizePercent = EditorGUILayout.Slider("Middle Size Percent", lo.middleSizePercent, .06f, 1f, GUILayout.ExpandWidth(false));
            lo.curve = EditorGUILayout.CurveField("Curve", lo.curve, GUILayout.ExpandWidth(false));
            lo.noiseStrength = EditorGUILayout.FloatField("Noise", lo.noiseStrength, GUILayout.ExpandWidth(false));
            GUILayout.EndVertical();

            if (GUI.changed)
            {
                targetMap.UpdateObject(selectedKeyIndex, sedi.percent, sedi.angle);
            }
        }
        else if(sedi.ti is SurfaceObstacle)
        {
            SurfaceObstacle so = (SurfaceObstacle)sedi.ti;
            GUILayout.BeginVertical(GUILayout.MinWidth(300));
            so.curveLength = EditorGUILayout.FloatField("Curve Length", so.curveLength, GUILayout.ExpandWidth(false));
            so.roadWidth = EditorGUILayout.FloatField("Road Width", so.roadWidth, GUILayout.ExpandWidth(false));
            so.curve = EditorGUILayout.CurveField("Path", so.curve, GUILayout.ExpandWidth(false));
            GUILayout.BeginHorizontal();
            so.noiseStrength = EditorGUILayout.FloatField("Noise", so.noiseStrength, GUILayout.ExpandWidth(false));
            so.sideNoiseStrength = EditorGUILayout.FloatField("sideNoise", so.sideNoiseStrength, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                targetMap.UpdateObject(selectedKeyIndex, sedi.percent, sedi.angle);
            }

        }
        else if (sedi.ti is SurfacePartialObstacle)
        {
            SurfacePartialObstacle so = (SurfacePartialObstacle)sedi.ti;
            GUILayout.BeginVertical(GUILayout.MinWidth(300));
            so.curveLength = EditorGUILayout.FloatField("Curve Length", so.curveLength, GUILayout.ExpandWidth(false));
            so.anglePercent = EditorGUILayout.Slider("Angle Percent", so.anglePercent, 0.1f, 1f, GUILayout.ExpandWidth(false));
            so.noiseStrength = EditorGUILayout.FloatField("Noise", so.noiseStrength, GUILayout.ExpandWidth(false));
            so.sideNoiseStrength = EditorGUILayout.FloatField("sideNoise", so.sideNoiseStrength, GUILayout.ExpandWidth(false));
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                targetMap.UpdateObject(selectedKeyIndex, sedi.percent, sedi.angle);
            }

        }
        GUILayout.EndHorizontal();



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
            Debug.Log("Hello!");
            MapItem tiPrefab = (EditorGUIUtility.GetObjectPickerObject() as GameObject)?.GetComponent<MapItem>();
            if (tiPrefab == null)
                return;
            selectedKeyIndexs.Clear();

            selectedKeyIndexs.Add(targetMap.AddObject(tiPrefab, objectPosPercent));

            mouseIsDownOverKey = true;
            needsRepaint = true;
            currentPickerWindow = -1;
            objectPosPercent = 0;

        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (keyRects != null)
            {
                for (int i = 0; i < keyRects.Count; i++)
                {
                    if (keyRects[i].Key.x >= targetMap.prefabObjectEditInfos.Count)
                        continue;

                    if (!targetMap.prefabObjectEditInfos[keyRects[i].Key.x].toggle)
                        continue;

                    Rect wolrdKeyRect = new Rect(keyRects[i].Value.x, keyRects[i].Value.y + headerHeight + objectPrefabSection.height, keyRects[i].Value.width, keyRects[i].Value.height);
                    if (wolrdKeyRect.Contains(guiEvent.mousePosition))
                    {
                        mouseIsDownOverKey = true;

                        if (!downShift)
                        {
                            selectedKeyIndexs.Clear();
                            Selection.activeGameObject = targetMap.prefabObjectEditInfos[keyRects[i].Key.x].spawnedObjectInfos[keyRects[i].Key.y].ti.gameObject;
                        }
                        bool isIn = false;
                        for (int k = 0; k < selectedKeyIndexs.Count; k++)
                        {
                            if (selectedKeyIndexs[k] == keyRects[i].Key)
                            {
                                isIn = true;
                            }
                        }
                        if(!isIn)
                            selectedKeyIndexs.Add(keyRects[i].Key);

                        needsRepaint = true;
                        GUI.FocusControl(null);
                        break;
                    }
                }
            }

            Rect worldMapPreviewRect = new Rect(mapPreviewRect.x, mapPreviewRect.y + headerHeight + objectPrefabSection.height, mapPreviewRect.width, mapPreviewRect.height);
            if (!mouseIsDownOverKey && worldMapPreviewRect.Contains(guiEvent.mousePosition))
            {
                objectPosPercent = Mathf.InverseLerp(mapPreviewRect.x, mapPreviewRect.xMax, guiEvent.mousePosition.x);
                currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", currentPickerWindow);
            }
            RotateObjectEvent(guiEvent);

            dragOn = guiEvent.mousePosition;
            if(!mouseIsDownOverKey)
                drawBox = true;
        }

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
        {
            drawBox = false;
            if (mouseIsDownOverKey == false)
            {
                Vector2 st = dragOn;
                Vector2 ed = guiEvent.mousePosition;
                float width = ed.x - st.x;
                float height = ed.y - st.y;

                if (width < 0)
                {
                    st.x += width;
                    width *= -1;
                }
                if (height < 0)
                {
                    st.y += height;
                    height *= -1;
                }

                Rect boundBox = new Rect(st.x, st.y, width, height);

                bool clear = false;
                if (keyRects != null)
                {
                    for (int i = 0; i < keyRects.Count; i++)
                    {
                        if (keyRects[i].Key.x >= targetMap.prefabObjectEditInfos.Count)
                            continue;

                        if (!targetMap.prefabObjectEditInfos[keyRects[i].Key.x].toggle)
                            continue;

                        Rect wolrdKeyRect = new Rect(keyRects[i].Value.x, keyRects[i].Value.y + headerHeight + objectPrefabSection.height, keyRects[i].Value.width, keyRects[i].Value.height);
                        if (boundBox.Contains(wolrdKeyRect.center))
                        {
                            if (!clear)
                            {

                                selectedKeyIndexs.Clear();
                                clear = true;
                            }
                            selectedKeyIndexs.Add(keyRects[i].Key);
                        }
                    }
                }
                if(clear && selectedKeyIndexs.Count > 0)
                {
                    needsRepaint = true;
                    GUI.FocusControl(null);
                }
            }
            mouseIsDownOverKey = false;
        }

        if (guiEvent.type == EventType.MouseDrag)
        {
            if (guiEvent.button == 0)
            {
                if (mouseIsDownOverKey)
                {
                    if (selectedKeyIndexs.Count == 1)
                    {
                        Vector2Int selectedKeyIndex = selectedKeyIndexs[0];
                        float percent = Mathf.InverseLerp(mapPreviewRect.x, mapPreviewRect.xMax, guiEvent.mousePosition.x);
                        targetMap.UpdateObject(selectedKeyIndex, percent, targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].angle);
                        needsRepaint = true;
                    }
                }
                RotateObjectEvent(guiEvent);
            }
            else if (guiEvent.button == 1)
            {
                if (keyRects != null)
                {
                    Vector2Int mainkeyIndex = Vector2Int.zero;
                    float min = 1000;
                    for (int i = 0; i < keyRects.Count; i++)
                    {

                        bool isSelected = false;
                        for (int j = 0; j < selectedKeyIndexs.Count; j++)
                        {
                            if (selectedKeyIndexs[j] == keyRects[i].Key)
                            {
                                isSelected = true;
                                break;
                            }
                        }

                        if (!isSelected)
                            continue;

                        Rect wolrdKeyRect = new Rect(keyRects[i].Value.x, keyRects[i].Value.y + headerHeight + objectPrefabSection.height, keyRects[i].Value.width, keyRects[i].Value.height);
                        if (Vector2.Distance(wolrdKeyRect.center, guiEvent.mousePosition) < min)
                        {
                            mainkeyIndex = keyRects[i].Key;
                            min = Vector2.Distance(wolrdKeyRect.center, guiEvent.mousePosition);
                        }
                    }
                    float percent = Mathf.InverseLerp(mapPreviewRect.x, mapPreviewRect.xMax, guiEvent.mousePosition.x);
                    float added = percent - targetMap.prefabObjectEditInfos[mainkeyIndex.x].spawnedObjectInfos[mainkeyIndex.y].percent;

                    for (int i = 0; i < selectedKeyIndexs.Count; i++)
                    {
                        Vector2Int selectedKeyIndex = selectedKeyIndexs[i];
                        if (!targetMap.prefabObjectEditInfos[selectedKeyIndexs[i].x].toggle)
                            continue;
                        targetMap.UpdateObject(selectedKeyIndex,
                            targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].percent + added,
                            targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].angle);
                    }
                    needsRepaint = true;
                }
            }

        }

        if (drawBox)
        {
            Vector2 st = dragOn;
            Vector2 ed = guiEvent.mousePosition;
            float width = ed.x - st.x;
            float height = ed.y - st.y;

            if (width < 0)
            {
                st.x += width;
                width *= -1;
            }
            if (height < 0)
            {
                st.y += height;
                height *= -1;
            }

            EditorGUI.DrawRect(new Rect(st.x, st.y, width, height), new Color(0.1764705f, 0.5313722f, 0.6509804f, 0.5f));
            needsRepaint = true;
        }

        if (guiEvent.keyCode == KeyCode.Backspace && guiEvent.type == EventType.KeyDown)
        {
            selectedKeyIndexs.Sort(delegate (Vector2Int x1, Vector2Int x2)
            {
                if (x1.x < x2.x)
                    return 1;
                else if (x1.x > x2.x)
                    return -1;
                else
                {
                    if (x1.y < x2.y)
                        return 1;
                    else if (x1.y> x2.y)
                        return -1;
                }
                return 0;
            });

            for (int i = 0; i < selectedKeyIndexs.Count; i++)
            {
                if (selectedKeyIndexs[i].x >= 0 && selectedKeyIndexs[i].y >= 0)
                {
                    Vector2Int ski = selectedKeyIndexs[i];
                    targetMap.RemoveObject(ref ski);
                    selectedKeyIndexs[i] = ski;
                    needsRepaint = true;
                }
            }
        }

        if (guiEvent.type == EventType.KeyDown)
        {
            if (guiEvent.keyCode == KeyCode.LeftShift)
            {
                downShift = true;
            }
            if (guiEvent.keyCode == KeyCode.LeftControl)
            {
                downCtrl = true;
            }

            if (downCtrl && guiEvent.keyCode == KeyCode.D)
            {
                List<Vector2Int> tmp = new List<Vector2Int>();
                for(int i  = 0; i < selectedKeyIndexs.Count; i++)
                {
                    tmp.Add(selectedKeyIndexs[i]);
                }
                selectedKeyIndexs.Clear();

                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i].x >= targetMap.prefabObjectEditInfos.Count)
                        continue;
                    EditableMap.PrefabObjectEditInfo prefabObjectEditInfo = targetMap.prefabObjectEditInfos[tmp[i].x];
                    if (tmp[i].y >= prefabObjectEditInfo.spawnedObjectInfos.Count)
                        continue;
                    EditableMap.ObjectEditInfo oei = prefabObjectEditInfo.spawnedObjectInfos[tmp[i].y];
                    Vector2Int target = targetMap.AddObject(prefabObjectEditInfo.itemPrefab, 1);
                    selectedKeyIndexs.Add(target);

                    targetMap.DuplicateSetting(
                        prefabObjectEditInfo.spawnedObjectInfos[prefabObjectEditInfo.spawnedObjectInfos.Count - 1].ti,
                        prefabObjectEditInfo.spawnedObjectInfos[tmp[i].y].ti);


                    targetMap.UpdateObject(new Vector2Int(target.x, target.y),
                        oei.percent,
                        oei.angle);
                }
                needsRepaint = true;
            }
        }
        else if (guiEvent.type == EventType.KeyUp)
        {
            if (guiEvent.keyCode == KeyCode.LeftShift)
            {
                downShift = false;
            }
            if (guiEvent.keyCode == KeyCode.LeftControl)
            {
                downCtrl = false;
            }

        }
    }

    private void RotateObjectEvent(Event guiEvent)
    {
        if (selectedKeyIndexs.Count == 1)
        {
            Vector2Int selectedKeyIndex = selectedKeyIndexs[0];

            Vector2 wrpc = rotatePointCenter + new Vector2(
            previewBorderSize,
            headerHeight + objectPrefabSection.height + previewBorderSize * 3 + keyHeight + mapPreviewRect.height
            );
            Rect worldRotatePointRect = new Rect(
                rotatePointRect.x + previewBorderSize,
                rotatePointRect.y + headerHeight + objectPrefabSection.height + previewBorderSize * 3 + keyHeight + mapPreviewRect.height,
                rotatePointRect.width,
                rotatePointRect.height
                );

            if (worldRotatePointRect.Contains(guiEvent.mousePosition))
            {
                float angle = Mathf.Atan2((wrpc.x - guiEvent.mousePosition.x), (wrpc.y - guiEvent.mousePosition.y)) * Mathf.Rad2Deg;
                targetMap.UpdateObject(selectedKeyIndex, targetMap.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].percent, angle);
                needsRepaint = true;
            }
        }
    }



}



