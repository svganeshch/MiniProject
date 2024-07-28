using UnityEngine;

public class WorldSfxManager : MonoBehaviour
{
    public static WorldSfxManager instance;

    [Header("Weapon Sound Clips")]
    public AudioClip weaponSlashSound;
    public AudioClip weaponHitFleshSound;
    public AudioClip weaponDrawSound;
    public AudioClip weaponHolsterSound;

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
