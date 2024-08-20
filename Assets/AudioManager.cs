using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _AudioManger;

    private bool MainMenuActive = true;

    public Sound[] sounds;
    public Sound[] GoonsHit; //getting hit, hitting, dying
    public Sound[] GoonsHitting;
    public Sound[] GoonsDying;
    public Sound[] GoonsIntro;
    public Sound[] BossHit;
    public Sound[] BossHitting;
    public Sound[] GoonCelebrate;
    public Sound[] LJHit;
    public Sound[] LJKill;
    public Sound[] LJLightAttack;
    public Sound[] LJHeavyAttack;




    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_AudioManger == null)
        {
            _AudioManger = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in GoonsHit)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in GoonsHitting)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in GoonsDying)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in GoonsIntro)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in BossHit)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in BossHitting)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in GoonCelebrate)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in LJHit)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in LJKill)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in LJLightAttack)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }

        foreach (Sound s in LJHeavyAttack)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.Volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.outputAudioMixerGroup;
        }


        GameObject playbutton = GameObject.Find("PlayButton");
        if (playbutton && playbutton.TryGetComponent(out Button button))
            {
            button.onClick.AddListener(PlayButtonPressed);
        }
        else
        {
            GameObject btmenubutton = GameObject.Find("BackToMenuButton");
        if (btmenubutton && btmenubutton.TryGetComponent(out Button button2))
            {
                button2.onClick.AddListener(BackToMenuButtonPressed);
        }
        }
    }

    void Start()
    {
        Debug.Log("Start");
        Play("Rain");
        Play("MainMenuMusic");
        Play("CourtAmbience");
        StartCoroutine(PlayGavelRandomly());
    }

    public Button PlayButton;
    public Button BackToMenuButton;

    public void PlayButtonPressed()// When you ext the main menu the music changes and the court sounds stop
    {
        Debug.Log("Play");
        MainMenuActive = false; // Set the flag to false to stop the loop
        Stop("MainMenuMusic");
        Stop("CourtAmbience");
        Play("InGameMusic");
    }

    public void BackToMenuButtonPressed()// When you ext the main menu the music changes and the court sounds stop
    {
        Debug.Log("Back");
        MainMenuActive = true; // Set the flag to false to stop the loop
        Debug.Log("got here");
        Play("MainMenuMusic");
        Play("CourtAmbience");
        Stop("InGameMusic");
    }

    private System.Random randomGavelTime = new System.Random();

    private IEnumerator PlayGavelRandomly() // play gavel sounds at random intervals durnig main menu
    {
        while (MainMenuActive) 
        {
            // Wait for a random amount of time between 3 and 5 seconds
            float waitTime = randomGavelTime.Next(4, 8); // Random delay between 4s and 8s
            yield return new WaitForSeconds(waitTime);

            // Call the method to perform the action
            if (MainMenuActive) // Only play if player did not leave main menu during delay
            {
                Play("Gavel");
            }
        }
    }

    public void PlayRandomSoundFromArray(Sound[] sounds)
    {
        int randomIndex = UnityEngine.Random.Range(0, sounds.Length);
        sounds[randomIndex].source.Play();
    }


    public void Play (string name)
        {

        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound" + name + "no found!");
            return;
        }
        s.source.Stop();
    }
}

/*
class House : MonoBehaviour
{
    static bool DestoyedByEarthEndingMetor = false;
    int numberOfRooms;
    public void SetNumberOfRooms(int count)
    {
        this.numberOfRooms = count;
    }

    public static void DestroyAll()
    {
        DestoyedByEarthEndingMetor = true;
    }
}

class Josh
{
    void BuyHouse()
    {
        //josh goes to the market and buys a 3 bedroom appartment
        House firstHouse = new House();
        firstHouse.SetNumberOfRooms(3);
        //another buy of house with 9 rooms
        House SecondHouse = new House();
        SecondHouse.SetNumberOfRooms(9)

            House.DestroyAll();
    }
}*/



