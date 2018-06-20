using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class GameObjectUtil
{
    public static void SetParentToChild(GameObject parent, GameObject child, bool stay = true)
    {
        child.transform.SetParent(parent.transform, stay);
    }
    public static void SetZ(GameObject obj, float z)
    {
        TransformUtil.SetLocalZ(obj.transform, z);
    }
    public static void SetX(GameObject obj, float x)
    {
        TransformUtil.SetLocalX(obj.transform, x);
    }
    public static void SetY(GameObject obj, float y)
    {
        TransformUtil.SetLocalY(obj.transform, y);
    }
    public static void SetXY(GameObject obj, float x, float y)
    {
        TransformUtil.SetLocalXY(obj.transform, x, y);
    }
    public static void SetWidth(GameObject obj, float width) {
        TransformUtil.SetWidth((RectTransform)obj.transform,width);
    }
    public static void SetHeight(GameObject obj, float height) {
        TransformUtil.SetHeight((RectTransform)obj.transform,height);
    }
    public static void SetColor(GameObject obj, Color c) {
        obj.GetComponent<Image>().color = c;
    }
    public static float GetWidth(GameObject obj) { return TransformUtil.GetWidth((RectTransform)obj.transform); }
    public static float GetHeight(GameObject obj) { return TransformUtil.GetHeight((RectTransform)obj.transform); }

}
