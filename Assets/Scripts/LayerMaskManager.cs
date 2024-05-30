using UnityEngine;

public class LayerMaskManager : MonoBehaviour
{
    public static LayerMaskManager Instance;

    [Header("Layer Masks")]
    public LayerMask playerLayerMask;
    public LayerMask enemyLayerMask;
    public LayerMask damagableLayerMask;
    public LayerMask obstaclesLayerMask;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
