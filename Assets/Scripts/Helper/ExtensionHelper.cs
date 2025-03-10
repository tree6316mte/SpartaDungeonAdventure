using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionHelper
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : MonoBehaviour
    {
        var component = gameObject.GetComponent<T>();
        if (component == null) gameObject.AddComponent<T>();
        return component;
    }
}
