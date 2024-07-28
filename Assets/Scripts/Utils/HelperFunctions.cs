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

    public float[] GetTextureMix(Vector3 playerPos, Terrain t)
    {
        Vector3 tpos = t.transform.position;
        TerrainData tData = t.terrainData;

        int mapX = Mathf.RoundToInt((playerPos.x - tpos.x) / tData.size.x * tData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((playerPos.z - tpos.z) / tData.size.z * tData.alphamapHeight);

        float[,,] splatMapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellmix = new float[splatMapData.GetUpperBound(2) + 1];

        for (int i = 0; i < cellmix.Length; i++)
        {
            cellmix[i] = splatMapData[0, 0, i];
        }
        return cellmix;
    }

    public string GetLayerName(Vector3 playerPos, Terrain t)
    {
        float[] cellMix = GetTextureMix(playerPos, t);
        float strongest = 0;
        int maxIndex = 0;

        for (int i = 0; i < cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }

        return t.terrainData.terrainLayers[maxIndex].name;
    }
}
