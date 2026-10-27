using UnityEngine;
using System;

[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;
    [Tooltip("Check this for background music tracks (as opposed to sound effects), so GetCurrentMusicSource() can find them.")]
    public bool isMusic;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour 
{
    public static AudioManager Instance;
    public Sound[] sounds;

    void Awake() 
    {
         // Make sure there is only one GameManager 
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        } 
        Instance = this;

        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;

            // CRITICAL FOR VR BACKGROUND MUSIC: Force 2D Spatial Blend
            // (0 = 2D/non-positional, 1 = 3D/positional - was incorrectly set to 1)
            s.source.spatialBlend = 0f; 
        }

          // Keep this GameManager when changing scenes 
        DontDestroyOnLoad(gameObject); 
    }

    // Removed the automatic Play("Maintheme") call that used to live here -
    // track selection is now owned by MusicPlayer.cs, so AudioManager stays a
    // general-purpose sound player instead of also deciding what music starts.

    // Changed from IEnumerator to standard void so music functions normally
    public void Play(string soundName) 
    {
        Sound s = Array.Find(sounds, item => item.name == soundName);
        if (s == null) 
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        if (s.source != null && !s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    public void Stop(string soundName) 
    {
        Sound s = Array.Find(sounds, item => item.name == soundName);
        if (s == null) 
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        s.source.Stop();
    }

    /// <summary>
    /// Returns the specific AudioSource for a named sound, or null if not found.
    /// Use this instead of GameObject.Find(...).GetComponent&lt;AudioSource&gt;()
    /// when grabbing a source directly - AudioManager's GameObject has one
    /// AudioSource per Sound entry, and GetComponent&lt;AudioSource&gt;() only
    /// ever returns the first one it finds, not necessarily the one you want.
    /// </summary>
    public AudioSource GetAudioSource(string soundName)
    {
        Sound s = Array.Find(sounds, item => item.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return null;
        }

        return s.source;
    }

    /// <summary>
    /// Returns whichever Sound flagged as isMusic is currently playing, or null
    /// if none are. Lets other scripts (like a music-reactive visual) find the
    /// active music AudioSource without needing to know which specific track
    /// is playing or depend on MusicPlayer directly.
    /// </summary>
    public AudioSource GetCurrentMusicSource()
    {
        foreach (Sound s in sounds)
        {
            if (s.isMusic && s.source != null && s.source.isPlaying)
            {
                return s.source;
            }
        }

        return null;
    }
}