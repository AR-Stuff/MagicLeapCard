using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Static Instance
    /// <summary>
    /// Get Audiomanager Instance
    /// </summary>
    public static AudioManager _instance;
    #endregion

    #region Private Variable
    /// <summary>
    /// The Sounds Track of the Audio
    /// </summary>
    [SerializeField]
    private Sound[] sounds;
    #endregion

    #region Unity Function
    /// <summary>
    /// When Game Start
    /// Only one instance should be in the map
    /// Set all sound tracks to Audio Source
    /// </summary>
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    /// <summary>
    /// When Game Start Play background music
    /// </summary>
    void Start()
    {
        Play("Theme");
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Start playing an audio
    /// </summary>
    /// <param name="name">audio name</param>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    /// <summary>
    /// Stop playing an audio
    /// </summary>
    /// <param name="name">audio name</param>
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    /// <summary>
    /// Stop playing all audio
    /// </summary>
    public void StopAll()
    {
        foreach(var itr in sounds)
        {
            itr.source.Stop();
        }
    }
    #endregion
}
