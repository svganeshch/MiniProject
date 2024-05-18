using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioFXClip", order = 1)]
public class AudioFXClip : ScriptableObject
{
    public AudioClip clip;
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource audioSource;

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

    public void PlayWeaponSound(AudioFXClip audioFXClip)
    {
        audioSource.PlayOneShot(audioFXClip.clip);
    }
}
