using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _AudioManger;

    private bool MainMenuActive = true;

    public Sound[] sounds;

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
    }

    void Start()
    {
        Play("Rain");
        Play("MainMenuMusic");
        Play("CourtAmbience");
        StartCoroutine(PlayGavelRandomly());
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private System.Random random = new System.Random();

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene MainMenu, Scene Level1)
    {
        MainMenuActive = false; // Set the flag to false to stop the loop
        Stop("MainMenuMusic");
        Stop("CourtAmbience");
        Play("InGameMusic");
    }

    private IEnumerator PlayGavelRandomly()
    {
        while (MainMenuActive) // Infinite loop to keep repeating the action
        {
            // Wait for a random amount of time between 3 and 5 seconds
            float waitTime = random.Next(4, 8); // Random delay between 4s and 8s
            yield return new WaitForSeconds(waitTime);

            // Call the method to perform the action
            if (MainMenuActive)
            {
                Play("Gavel");
            }
        }
    }

    public void Play (string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound" + name + "no found!");
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



