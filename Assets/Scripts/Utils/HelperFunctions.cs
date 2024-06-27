using UnityEngine;
using UnityEngine.InputSystem;

public class HelperFunctions : MonoBehaviour
{
    public static HelperFunctions instance;

    PlayerInput input;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        input = FindObjectOfType<PlayerInput>();
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

    public float SnapInput(float input)
    {
        if (input > 0 && input <= 0.5f)
        {
            return 0.5f;
        }
        if (input > 0.5f && input <= 1)
        {
            return 1;
        }
        if (input < 0 && input >= -0.5f)
        {
            return -0.5f;
        }
        if (input < -0.5f && input >= -1)
        {
            return -1;
        }
        return 0;
    }

    public InputDevice GetCurrenInputDevice()
    {
        InputDevice currentDevice = input.devices[0];

        if (currentDevice != null)
        {
            return currentDevice;
        }

        return null;
    }
}
