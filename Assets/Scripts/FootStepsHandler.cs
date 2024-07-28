using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepsHandler : MonoBehaviour
{
    public static FootStepsHandler instance;

    Character character;
    string currentLayer;
    string onLayer;

    public FootStepsData[] terrainFootStepsData;

    [Header("Step interval controls")]
    [Range(0, 5)] public float stepInterval = 1f;
    [Range(0, 1)] public float walkingStepInterval = 1f;
    [Range(0, 1)] public float runningStepInterval = 1f;
    [Range(0, 1)] public float combatStepInterval = 1f;
    [Range(0, 1)] public float sprintStepInterval = 1f;

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

        character = GetComponent<Character>();
    }

    public void CheckLayers()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3, LayerMaskManager.Instance.groundLayerMask))
        {
            if (character.characterAnimatorManager.IsGrounded)
            {
                hit.transform.TryGetComponent<Terrain>(out Terrain t);
                if (t == null) return;

                onLayer = HelperFunctions.instance.GetLayerName(transform.position, t);

                if (currentLayer != onLayer)
                {
                    currentLayer = onLayer;

                    foreach (FootStepsData footStepsData in terrainFootStepsData)
                    {
                        if (footStepsData.layertags.Contains(currentLayer))
                        {
                            character.characterSfxManager.SetFootStepsSound(footStepsData);
                        }
                    }
                }
            }
        }
    }
}
