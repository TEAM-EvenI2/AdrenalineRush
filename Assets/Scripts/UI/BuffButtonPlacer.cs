using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;

public class BuffButtonPlacer : MonoBehaviour
{
    public List<RectTransform> rectTransforms;

    private Vector2[] initPos;

    private void Awake()
    {

        initPos = new Vector2[rectTransforms.Count];
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            initPos[i] = rectTransforms[i].anchoredPosition;
        }
    }

    public void ResetPos()
    {
        SetButtonsPositions(initPos);

    }

    public void SetButtonsPositions(Vector2[] pos)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            rectTransforms[i].anchoredPosition = pos[i];
        }
    }
    public void SetButtonsPositions(SerVector2[] pos)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            rectTransforms[i].anchoredPosition = Utils.Ser2Vector(pos[i]);
        }
    }

    public Vector2[] GetButtonPositions()
    {
        Vector2[] buttons = new Vector2[rectTransforms.Count];
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            buttons[i] = rectTransforms[i].anchoredPosition;
        }

        return buttons;
    }
}
