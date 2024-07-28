using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Footstep Collection", menuName = "Create new footstep data")]
public class FootStepsData : ScriptableObject
{
    public List<AudioClip> footstepSounds = new List<AudioClip>();
    public AudioClip jumpSound;
    public AudioClip landSound;

    public List<string> layertags = new List<string>();
}
