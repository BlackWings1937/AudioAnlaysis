using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformUtil
{
    public static void SetLocalZ(Transform t, float z)
    {
        Vector3 pos = t.localPosition;
        pos.z = z;
        t.localPosition = pos;
    }
    public static void SetLocalX(Transform t, float x)
    {
        Vector3 pos = t.localPosition;
        pos.x = x;
        t.localPosition = pos;
    }
    public static void SetLocalY(Transform t, float y)
    {
        Vector3 pos = t.localPosition;
        pos.y = y;
        t.localPosition = pos;
    }
    public static void SetLocalXY(Transform t, float x, float y)
    {
        Vector3 pos = t.localPosition;
        pos.x = x;
        pos.y = y;
        t.localPosition = pos;
    }
    public static void SetWidth(RectTransform t,float width) {
        Vector2 size = t.sizeDelta;
        size.x = width;
        t.sizeDelta = size;
    }
    public static void SetHeight(RectTransform t, float height) {
        Vector2 size = t.sizeDelta;
        size.y = height;
        t.sizeDelta = size;
    }
    public static float GetWidth(RectTransform t) {
        return t.sizeDelta.x;
    }
    public static float GetHeight(RectTransform t) {
        return t.sizeDelta.y;
    }
}
