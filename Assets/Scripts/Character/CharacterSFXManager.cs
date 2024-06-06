using UnityEngine;

public class CharacterSfxManager : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayWeaponSlashSound()
    {
        audioSource.PlayOneShot(WorldSfxManager.instance.weaponSlashSound);
    }

    public void PlayWeaponHitFleshSound()
    {
        audioSource.PlayOneShot(WorldSfxManager.instance.weaponHitFleshSound);
    }
}
