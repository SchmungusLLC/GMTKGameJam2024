using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer MasterMixer;
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider FXSlider;


    public void SetMasterVol(float MasterVol)
    {
        MasterMixer.SetFloat("MasterVol", MasterVol);
    }
    public void SetMusicVol(float MusicVol)
    {
        MasterMixer.SetFloat("MusicVol", MusicVol);
    }
    public void SetFXVol(float FXVol)
    {
        MasterMixer.SetFloat("FXVol", FXVol);
    }

    void OnEnable()
    {
        if (MasterMixer.GetFloat("MasterVol", out float mastervol))
        {
            MasterSlider.value = mastervol;
        }

        if (MasterMixer.GetFloat("MusicVol", out float musicvol))
        {
            MusicSlider.value = musicvol;
        }

        if (MasterMixer.GetFloat("FXVol", out float fxvol))
        {
            FXSlider.value = fxvol;
        }

    }

}
