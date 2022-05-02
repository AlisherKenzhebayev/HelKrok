using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public List<Sound> sounds;
    
    private List<GameObject> soundsGOs;
    
    private static AudioManager audioManager;

    public static AudioManager instance
    {
        get
        {
            if (!audioManager)
            {
                audioManager = FindObjectOfType(typeof(AudioManager)) as AudioManager;

                if (!audioManager)
                {
                    Debug.LogError("There needs to be one active AudioManager script on a GameObject in your scene.");
                }
                else
                {
                    audioManager.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(audioManager);
                }
            }
            return audioManager;
        }
    }

    private void Init()
    {
        soundsGOs = new List<GameObject>();

        for (int i = 0; i < instance.sounds.Count; i++)
        {
            var s = instance.sounds[i];
            var newGO = new GameObject(s.name);
            instance.soundsGOs.Add(newGO);
            newGO.transform.SetParent(this.transform);
            s.source = newGO.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.isLooping;
        }
    }

    void Start()
    {
        //Debug.LogError("Start");

        foreach (var item in instance.sounds)
        {
            if (item.playOnStart) {
                instance.Play(item.name);
            }
        }
    }

    public void Play(string name)
    {
        Sound s = instance.sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = instance.sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound " + name + " not found!");
        }
        s.source.Stop();
    }
}
