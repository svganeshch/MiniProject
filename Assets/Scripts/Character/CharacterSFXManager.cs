using System.Collections.Generic;
using UnityEngine;

public class CharacterSfxManager : MonoBehaviour
{
    AudioSource audioSource;

    FootStepsHandler footstepsHandler;
    public List<AudioClip> footStepsClipsList = new List<AudioClip>();
    public AudioClip jumpSound;
    public AudioClip landSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        footstepsHandler = GetComponent<FootStepsHandler>();
    }

    public void PlayWeaponSlashSound()
    {
        audioSource.PlayOneShot(WorldSfxManager.instance.weaponSlashSound);
    }

    public void PlayWeaponHitFleshSound()
    {
        audioSource.PlayOneShot(WorldSfxManager.instance.weaponHitFleshSound);
    }

    public void PlayFootStepsSound()
    {
        footstepsHandler.CheckLayers();

        int n = Random.Range(1, footStepsClipsList.Count);
        audioSource.clip = footStepsClipsList[n];
        audioSource.PlayOneShot(audioSource.clip);

        footStepsClipsList[n] = footStepsClipsList[0];
        footStepsClipsList[0] = audioSource.clip;
    }

    public void PlayJumpSound()
    {
        footstepsHandler.CheckLayers();

        audioSource.clip = jumpSound;
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlayLandSound()
    {
        footstepsHandler.CheckLayers();

        audioSource.clip = landSound;
        audioSource.PlayOneShot(landSound);
    }

    public void SetFootStepsSound(FootStepsData footStepsData)
    {
        footStepsClipsList = footStepsData.footstepSounds;
        jumpSound = footStepsData.jumpSound;
        landSound = footStepsData.landSound;
    }
}
