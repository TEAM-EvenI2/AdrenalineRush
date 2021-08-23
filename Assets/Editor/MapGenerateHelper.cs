using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapGenerateHelper : EditorWindow
{
    public class ObjectEditInfo
    {
        public TunnelItem ti;
        public int tunnelLength;

        public float distanceFromCenter;

        public float percent
        {
            get
            {
                if (ti == null)
                    return 0;
                return ti.transform.localPosition.x / tunnelLength;
            }
        }

        public float angle
        {
            get
            {
                if (ti == null)
                    return 0;
                return ti.transform.GetChild(0).localEulerAngles.x;
            }
        }
        public Color c;

        public ObjectEditInfo(TunnelItem ti, int tunnelLength,  Color c, float distanceFromCenter)
        {
            this.ti = ti;
            this.tunnelLength = tunnelLength;
            this.c = c;
            this.distanceFromCenter = distanceFromCenter;
        }
    }

    private Color borderColor = new Color(.2f, .2f, .2f);

    private const int headerHeight = 90;
    private const float keyWidth = 10;
    private const float keyHeight = 20;

    private Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);
    private Rect headerSection;
    private EditableTunnel targetTunnel;
    private Material targetMaterial;
    private float _tunnelRadius;
    private int _tunnelLength;


    private Color objectSettingSectionColor = new Color(0.4f,0.4f,0.4f);
    private Rect objectSettingSection;
    private Rect mapPreviewRect;
    private const int previewBorderSize = 10;


    private List<ObjectEditInfo> objectEditInfos;

    Rect rotatePointRect;
    Vector2 rotatePointCenter;
    Rect[] keyRects;
    bool mouseIsDownOverKey;
    int selectedKeyIndex;
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

    private void OnEnable()
    {
        if(objectEditInfos == null)
        {
            objectEditInfos = new List<ObjectEditInfo>();
        }
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

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Tunnel Radius", GUILayout.MaxWidth(100));
        float tunnelRadius = EditorGUILayout.FloatField(_tunnelRadius, GUILayout.MaxWidth(50));
        if (tunnelRadius != _tunnelRadius)
        {
            for (int i = 0; i < objectEditInfos.Count; i++)
            {
                if(objectEditInfos[i].distanceFromCenter > tunnelRadius)
                    objectEditInfos[i].distanceFromCenter = tunnelRadius;
            }
            _tunnelRadius = tunnelRadius;
        }
        GUILayout.Space(10);
        GUILayout.Label("Tunnel Length", GUILayout.MaxWidth(100));
        int tunnelLength = EditorGUILayout.IntField(_tunnelLength, GUILayout.MaxWidth(50));
        if(tunnelLength != _tunnelLength)
        {
            for(int i =0; i< objectEditInfos.Count; i++)
            {
                objectEditInfos[i].tunnelLength = tunnelLength;
            }
            _tunnelLength = tunnelLength;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, headerHeight - 30, 100, 30));
        if (_tunnelRadius > 0 && _tunnelLength > 0 && targetMaterial != null)
        {
            if (targetTunnel)
            {
                if (GUILayout.Button("Regenerate", GUILayout.MaxWidth(250)))
                {
                    targetTunnel.tunnelRadius = _tunnelRadius;
                    targetTunnel.tunnelLength = _tunnelLength;
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
                    targetTunnel.tunnelRadius = _tunnelRadius;
                    targetTunnel.tunnelLength = _tunnelLength;
                    targetTunnel.Generate();
                }
            }
        }
        GUILayout.EndArea();
        GUILayout.EndArea();
    }
   
    private void DrawObjectSetting()
    {
        GUILayout.BeginArea(objectSettingSection);

        mapPreviewRect = new Rect(previewBorderSize, previewBorderSize, position.width - previewBorderSize * 2, 40);
        EditorGUI.DrawRect(mapPreviewRect, new Color(0.2f, 0.2f, 0.2f));

        if(objectEditInfos.Count != targetTunnel.transform.childCount)
        {
            for(int i = 0; i < targetTunnel.transform.childCount; i++)
            {
                TunnelItem ti = targetTunnel.transform.GetChild(i)?.GetComponent<TunnelItem>();
                AddObject(ti, ti.transform.localPosition.z / _tunnelLength, false);
            }
        }

        for(int i = objectEditInfos.Count - 1; i >= 0; i --)
        {
            if (objectEditInfos[i].ti == null)
            {
                objectEditInfos.RemoveAt(i);
            }
        }

        if(selectedKeyIndex >= objectEditInfos.Count)
        {
            selectedKeyIndex = objectEditInfos.Count - 1;
        }

        if (objectEditInfos.Count <= 0)
        {
            GUILayout.EndArea();
            return;
        }

        keyRects = new Rect[objectEditInfos.Count];
        for (int i = 0; i < objectEditInfos.Count; i++)
        {
            ObjectEditInfo edi = objectEditInfos[i];
            if (edi.percent < 0)
                UpdateObject(i, 0, edi.angle, edi.distanceFromCenter);
            else if (edi.percent > 1)
                UpdateObject(i, 1, edi.angle, edi.distanceFromCenter);

            Rect keyRect = new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 2f, mapPreviewRect.yMax + previewBorderSize, keyWidth, keyHeight);
            if (i == selectedKeyIndex)
            {
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), new Color(0.1f, 0.1f, 0.1f));
            }
            EditorGUI.DrawRect(keyRect, edi.c);
            EditorGUI.DrawRect(new Rect(mapPreviewRect.x + mapPreviewRect.width * (edi.percent) - keyWidth / 4f, previewBorderSize, keyWidth / 2, 40), edi.c);
            keyRects[i] = keyRect;
        }

        Rect infoRect = new Rect(previewBorderSize, mapPreviewRect.yMax + previewBorderSize * 2 + keyHeight, position.width - previewBorderSize * 2, objectSettingSection.height - (mapPreviewRect.height + previewBorderSize * 4 + keyHeight));

        ObjectEditInfo sedi = objectEditInfos[selectedKeyIndex];
        GUILayout.BeginArea(infoRect);


        GUILayout.BeginHorizontal();
        GUILayout.Label("Position Percent", GUILayout.MaxWidth(100));
        float percent = EditorGUILayout.FloatField(sedi.percent, GUILayout.MaxWidth(100));
        if (Mathf.Abs(percent - sedi.percent) > Mathf.Epsilon)
        {
            UpdateObject(selectedKeyIndex, percent, sedi.angle, sedi.distanceFromCenter);
        }
        GUILayout.Label("Position Percent", GUILayout.MaxWidth(100));
        float distanceFromCenter = EditorGUILayout.FloatField(sedi.distanceFromCenter, GUILayout.MaxWidth(100));
        if (Mathf.Abs(distanceFromCenter - sedi.distanceFromCenter) > Mathf.Epsilon)
        {
            UpdateObject(selectedKeyIndex, sedi.percent, sedi.angle, distanceFromCenter);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Space(40);
        GUILayout.Label("Object Rotation", GUILayout.MaxWidth(100));
        float angle = EditorGUILayout.FloatField(sedi.angle, GUILayout.MaxWidth(100));
        if (Mathf.Abs(angle - sedi.angle) > Mathf.Epsilon)
        {
            UpdateObject(selectedKeyIndex, sedi.percent, angle, sedi.distanceFromCenter);
        }
        GUILayout.EndVertical();

        float sediAngle = angle;

        Vector2 size = new Vector2(10, 50);
        Vector2 point = new Vector2(150, 50);// + Vector2.up * size.y / 2;
        rotatePointCenter = point + size / 2f;

        float max = Mathf.Max(size.x, size.y);
        rotatePointRect = new Rect(point.x - 5 - max / 2f + size.x / 2f, point.y - 5, max + 10, max + 10);
        EditorGUI.DrawRect(new Rect(point.x - 5 - max / 2f + size.x /2f, point.y - 5, max + 10, max + 10), new Color(0.25f, 0.25f, 0.25f));

        Matrix4x4 m = GUI.matrix;
        GUIUtility.RotateAroundPivot(sediAngle, point + size / 2);
        EditorGUI.DrawRect(new Rect(point.x, point.y, size.x, size.y),sedi.c);

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

            selectedKeyIndex = AddObject(tiPrefab, objectPosPercent);

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
                    selectedKeyIndex = i;
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
                UpdateObject(selectedKeyIndex, percent, objectEditInfos[selectedKeyIndex].angle, objectEditInfos[selectedKeyIndex].distanceFromCenter);
                needsRepaint = true;
            }
            RotateObjectEvent(guiEvent);


        }

        if (guiEvent.keyCode == KeyCode.Backspace  && guiEvent.type == EventType.KeyDown)
        {
            RemoveObject(selectedKeyIndex);
            if (selectedKeyIndex >= objectEditInfos.Count)
            {
                selectedKeyIndex--;
            }
            needsRepaint = true;
        }

    }

    private void RotateObjectEvent(Event guiEvent)
    {

        Vector2 wrpc = rotatePointCenter + new Vector2(previewBorderSize, headerHeight + previewBorderSize * 3 + keyHeight + mapPreviewRect.height);
        Rect worldRotatePointRect = new Rect(rotatePointRect.x + previewBorderSize, rotatePointRect.y + headerHeight + previewBorderSize * 3 + keyHeight + mapPreviewRect.height, rotatePointRect.width, rotatePointRect.height);
        if (worldRotatePointRect.Contains(guiEvent.mousePosition))
        {
            wrpc = rotatePointCenter + new Vector2(previewBorderSize, headerHeight + previewBorderSize * 3 + keyHeight + mapPreviewRect.height);
            float angle = Mathf.Atan2((wrpc.x - guiEvent.mousePosition.x), (wrpc.y - guiEvent.mousePosition.y)) * Mathf.Rad2Deg;
            UpdateObject(selectedKeyIndex, objectEditInfos[selectedKeyIndex].percent, angle, objectEditInfos[selectedKeyIndex].distanceFromCenter);
            needsRepaint = true;
        }
    }

    public int AddObject(TunnelItem obj, float percent, bool isPrefab=true)
    {

        TunnelItem ti = isPrefab? Instantiate(obj) : obj;
        float posZ = percent * _tunnelLength;

        ti.transform.SetParent(targetTunnel.transform);
        ti.transform.localPosition = targetTunnel.GetPointOnSurface(posZ, ti.transform.localEulerAngles.z * Mathf.Deg2Rad, _tunnelRadius);

        Color c = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        //if (!ColorSet.TryGetValue(tiPrefab.GetInstanceID(), out c))
        //{
        //	c = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        //	ColorSet.Add(tiPrefab.GetInstanceID(), c);
        //}


        for (int i = 0; i < objectEditInfos.Count; i++)
        {
            if (percent < objectEditInfos[i].percent)
            {
                objectEditInfos.Insert(i, new ObjectEditInfo(ti, _tunnelLength, c, _tunnelRadius));
                return i;
            }
        }

        objectEditInfos.Add(new ObjectEditInfo(ti, _tunnelLength, c, _tunnelRadius));
        return objectEditInfos.Count - 1;
    }

    public void UpdateObject(int index, float percent, float angle, float distanceFormCenter)
    {
        Transform t = objectEditInfos[index].ti.transform;
        t.localPosition = targetTunnel.GetPointOnSurface(_tunnelLength * percent, angle * Mathf.Deg2Rad, distanceFormCenter);
        t.GetChild(0).localEulerAngles = Vector3.right * angle;
        objectEditInfos[index].distanceFromCenter = distanceFormCenter;
    }

    public void RemoveObject(int index)
    {
        if (objectEditInfos[index].ti != null)
            DestroyImmediate(objectEditInfos[index].ti.gameObject);
        objectEditInfos.RemoveAt(index);
    }



}



