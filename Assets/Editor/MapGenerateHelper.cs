using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerateHelper : EditorWindow
{

    private Color borderColor = new Color(.2f, .2f, .2f);

    private const int headerHeight = 90;
    private const float keyWidth = 10;
    private const float keyHeight = 20;

    private Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    private Rect headerSection;
    private EditableTunnel targetTunnel;
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

    [MenuItem("Window/Map Generate Helper")]
    static void Init()
    {
        MapGenerateHelper window = (MapGenerateHelper)GetWindow(typeof(MapGenerateHelper));
        window.minSize = new Vector2(600, 400);
        window.Show();
    }



    private void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        if (targetTunnel)
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

    private void DrawHeader()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        GUILayout.BeginArea(headerSection);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Map Generate", style);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Main Map",GUILayout.MaxWidth(100));
        targetTunnel = EditorGUILayout.ObjectField(targetTunnel, typeof(EditableTunnel), true, GUILayout.MaxWidth(250)) as EditableTunnel;
        GUILayout.Label("Mat", GUILayout.MaxWidth(100));
        targetMaterial = EditorGUILayout.ObjectField(targetMaterial, typeof(Material), true, GUILayout.MaxWidth(250)) as Material;
        GUILayout.EndHorizontal();

        if(targetTunnel == null)
        {

            GUILayout.EndArea();
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Tunnel Radius", GUILayout.MaxWidth(100));
        float tunnelRadius = EditorGUILayout.FloatField(targetTunnel.tunnelRadius, GUILayout.MaxWidth(50));
        if (tunnelRadius != targetTunnel.tunnelRadius)
        {
            for (int i = 0; i < targetTunnel.prefabObjectEditInfos.Count; i++)
            {
                for (int j = 0; j < targetTunnel.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
                {
                    EditableTunnel.ObjectEditInfo etoei = targetTunnel.prefabObjectEditInfos[i].GetObject(j);
                    if (etoei.distanceFromCenter > tunnelRadius)
                        etoei.distanceFromCenter = tunnelRadius;
                    if (etoei.distanceFromCenter < 0)
                        etoei.distanceFromCenter = 0;
                }
            }
            targetTunnel.tunnelRadius = tunnelRadius;
        }
        GUILayout.Space(10);
        GUILayout.Label("Tunnel Length", GUILayout.MaxWidth(100));
        int tunnelLength = EditorGUILayout.IntField(targetTunnel.tunnelLength, GUILayout.MaxWidth(50));
        if(tunnelLength != targetTunnel.tunnelLength)
        {
            for (int i = 0; i < targetTunnel.prefabObjectEditInfos.Count; i++)
            {
                for (int j = 0; j < targetTunnel.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
                {
                    EditableTunnel.ObjectEditInfo etoei = targetTunnel.prefabObjectEditInfos[i].GetObject(j);
                    etoei.tunnelLength = tunnelLength;
                }
            }
            targetTunnel.tunnelLength = tunnelLength;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, headerHeight - 30, 100, 30));
        if (targetTunnel.tunnelRadius > 0 && targetTunnel.tunnelLength > 0 && targetMaterial != null)
        {
            if (targetTunnel)
            {
                if (GUILayout.Button("Regenerate", GUILayout.MaxWidth(250)))
                {
                    targetTunnel.Generate();
                }
            }
            else
            {

                if (GUILayout.Button("Generate", GUILayout.MaxWidth(250)))
                {
                    GameObject tunnel = new GameObject("Generated Tunnel");
                    tunnel.AddComponent<MeshFilter>();
                    tunnel.AddComponent<MeshRenderer>().material = targetMaterial;
                    targetTunnel = tunnel.AddComponent<EditableTunnel>();
                    targetTunnel.Generate();
                }
            }
        }
        GUILayout.EndArea();
        GUILayout.EndArea();

        targetTunnel.ValidateCheck();
    }
   
    private void DrawObjectSetting()
    {
        GUILayout.BeginArea(objectSettingSection);

        mapPreviewRect = new Rect(previewBorderSize, previewBorderSize, position.width - previewBorderSize * 2, 40);
        EditorGUI.DrawRect(mapPreviewRect, new Color(0.2f, 0.2f, 0.2f));


        if(selectedKeyIndex.x >= targetTunnel.prefabObjectEditInfos.Count)
        {
            selectedKeyIndex.x = targetTunnel.prefabObjectEditInfos.Count - 1;
        }
        if (targetTunnel.prefabObjectEditInfos.Count <= 0)
        {
            GUILayout.EndArea();
            return;
        }
        if (selectedKeyIndex.y >= targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos.Count)
        {
            selectedKeyIndex.y = targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos.Count - 1;
        }


        int keyCount = 0;
        for(int i = 0; i < targetTunnel.prefabObjectEditInfos.Count; i++)
        {
            keyCount += targetTunnel.prefabObjectEditInfos[i].spawnedObjectInfos.Count;
        }

        keyRects = new Rect[keyCount];
        int _i = 0;
        for (int i = 0; i < targetTunnel.prefabObjectEditInfos.Count; i++)
        {
            for (int j = 0; j < targetTunnel.prefabObjectEditInfos[i].spawnedObjectInfos.Count; j++)
            {
                Vector2Int vIndex = new Vector2Int(i, j);
                EditableTunnel.ObjectEditInfo edi = targetTunnel.prefabObjectEditInfos[i].GetObject(j);
                if (edi.percent < 0)
                    targetTunnel.UpdateObject(vIndex, 0, edi.angle, edi.distanceFromCenter);
                else if (edi.percent > 1)
                    targetTunnel.UpdateObject(vIndex, 1, edi.angle, edi.distanceFromCenter);

                Rect keyRect = new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 2f, mapPreviewRect.yMax + previewBorderSize, keyWidth, keyHeight);
                if (vIndex == selectedKeyIndex)
                {
                    EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), new Color(0.1f, 0.1f, 0.1f));
                }
                EditorGUI.DrawRect(keyRect, targetTunnel.prefabObjectEditInfos[i].c);
                EditorGUI.DrawRect(new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 4f, previewBorderSize, keyWidth / 2, 40), targetTunnel.prefabObjectEditInfos[i].c);
                keyRects[_i] = keyRect;
                _i++;
            }
        }

        EditableTunnel.ObjectEditInfo sedi = targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].GetObject(selectedKeyIndex.y);
        Rect infoRect = new Rect(previewBorderSize, mapPreviewRect.yMax + previewBorderSize * 2 + keyHeight, position.width - previewBorderSize * 2, objectSettingSection.height - (mapPreviewRect.height + previewBorderSize * 4 + keyHeight));
        GUILayout.BeginArea(infoRect);


        GUILayout.BeginHorizontal();
        GUILayout.Label("Position Percent", GUILayout.MaxWidth(100));
        float percent = EditorGUILayout.FloatField(sedi.percent, GUILayout.MaxWidth(100));
        if (Mathf.Abs(percent - sedi.percent) > Mathf.Epsilon)
        {
            targetTunnel.UpdateObject(selectedKeyIndex, percent, sedi.angle, sedi.distanceFromCenter);
        }
        GUILayout.Label("Distance From Center", GUILayout.MaxWidth(130));
        float distanceFromCenter = EditorGUILayout.FloatField(sedi.distanceFromCenter, GUILayout.MaxWidth(100));
        if (Mathf.Abs(distanceFromCenter - sedi.distanceFromCenter) > Mathf.Epsilon)
        {
            if (distanceFromCenter > targetTunnel.tunnelRadius)
                distanceFromCenter = targetTunnel.tunnelRadius;

            targetTunnel.UpdateObject(selectedKeyIndex, sedi.percent, sedi.angle, distanceFromCenter);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Space(40);
        GUILayout.Label("Object Rotation", GUILayout.MaxWidth(100));
        float angle = EditorGUILayout.FloatField(sedi.angle, GUILayout.MaxWidth(100));
        if (Mathf.Abs(angle - sedi.angle) > Mathf.Epsilon)
        {
            targetTunnel.UpdateObject(selectedKeyIndex, sedi.percent, angle, sedi.distanceFromCenter);
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
        EditorGUI.DrawRect(new Rect(point.x, point.y, size.x, size.y), targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].c);

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
        if (targetTunnel == null)
            return;

        Event guiEvent = Event.current;

        if (guiEvent.commandName.Equals("ObjectSelectorClosed") && currentPickerWindow == EditorGUIUtility.GetObjectPickerControlID())
        {
            TunnelItem tiPrefab = (EditorGUIUtility.GetObjectPickerObject() as GameObject)?.GetComponent<TunnelItem>();
            if (tiPrefab == null)
                return;

            selectedKeyIndex = targetTunnel.AddObject(tiPrefab, objectPosPercent);

            mouseIsDownOverKey = true;
            needsRepaint = true;
            currentPickerWindow = -1;
            objectPosPercent = 0;

        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            for (int i = 0; i < keyRects.Length; i++)
            {
                Rect wolrdKeyRect = new Rect(keyRects[i].x, keyRects[i].y + headerHeight, keyRects[i].width, keyRects[i].height);
                if (wolrdKeyRect.Contains(guiEvent.mousePosition))
                {
                    mouseIsDownOverKey = true;
                    selectedKeyIndex = targetTunnel.GetInfoIndex(i);
                    needsRepaint = true;
                    break;
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
                targetTunnel.UpdateObject(selectedKeyIndex, percent, targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].angle, targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].distanceFromCenter);
                needsRepaint = true;
            }
            RotateObjectEvent(guiEvent);


        }

        if (guiEvent.keyCode == KeyCode.Backspace  && guiEvent.type == EventType.KeyDown)
        {
            if (selectedKeyIndex.x >= 0 && selectedKeyIndex.y >= 0)
            {
                targetTunnel.RemoveObject(ref selectedKeyIndex);
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
            targetTunnel.UpdateObject(selectedKeyIndex, targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].percent, angle, targetTunnel.prefabObjectEditInfos[selectedKeyIndex.x].spawnedObjectInfos[selectedKeyIndex.y].distanceFromCenter);
            needsRepaint = true;
        }
    }



}



