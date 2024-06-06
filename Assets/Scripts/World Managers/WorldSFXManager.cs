using UnityEngine;

public class WorldSfxManager : MonoBehaviour
{
    public static WorldSfxManager instance;

    [Header("Weapon Sound Clips")]
    public AudioClip weaponSlashSound;
    public AudioClip weaponHitFleshSound;

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

        DontDestroyOnLoad(gameObject);
    }
}
