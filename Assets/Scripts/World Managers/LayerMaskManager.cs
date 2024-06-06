using UnityEngine;

public class LayerMaskManager : MonoBehaviour
{
    public static LayerMaskManager Instance;

    [Header("Layer Masks")]
    public LayerMask damagableLayerMask;
    public LayerMask enemyLayerMask;
    public LayerMask groundLayerMask;
    public LayerMask obstaclesLayerMask;
    public LayerMask playerLayerMask;

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
