using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{

    public enum GoonSoundType
    {
        None,
        GettingHit,
        Hitting,
        Dying
    }

    // Correctly declare and initialize a variable of type GoonSoundType
    public GoonSoundType currentSound = GoonSoundType.None;

    public string name;

    public AudioClip clip;
    public AudioMixerGroup outputAudioMixerGroup;

    [Range(0f, 1f)]
    public float Volume;
    [Range(0.3f, 3f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
