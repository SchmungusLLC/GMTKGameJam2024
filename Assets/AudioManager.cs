using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _AudioManger;

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
        }
    }

    void Start()
    {
        Play("Music");
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



