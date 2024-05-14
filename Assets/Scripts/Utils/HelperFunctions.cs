using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperFunctions : MonoBehaviour
{
    public static HelperFunctions instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static T GetComponentFromTopParent<T>(Transform hitTransform) where T : Component
    {
        Transform parent = hitTransform;

        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        T scriptComponent = parent.GetComponent<T>();

        if (scriptComponent != null)
        {
            return scriptComponent;
        }
        else
        {
            Debug.LogWarning("Component not found in the topmost parent GameObject.");
            return null;
        }
    }

    public static T GetComponentFromTopParent<T>(Transform hitTransform, out T scriptComponent) where T : Component
    {
        Transform parent = hitTransform;

        while (parent.parent != null)
        {
            parent = parent.parent;
        }

        //Debug.Log("helper func " + parent.gameObject.name);
        scriptComponent = parent.GetComponent<T>();

        if (scriptComponent != null)
        {
            return scriptComponent;
        }
        else
        {
            Debug.LogWarning("Ccomponent not found in the topmost parent GameObject.");
            return null;
        }
    }
}
