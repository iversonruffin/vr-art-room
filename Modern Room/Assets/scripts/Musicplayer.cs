using UnityEngine;

/// <summary>
/// Standalone music player for the music box - completely independent from
/// AudioManager. Holds a collection of songs and plays them through a single
/// dedicated AudioSource in the scene.
/// </summary>
public class MusicPlayer : MonoBehaviour
{
    [Tooltip("Drag the scene's dedicated Audio Source GameObject here directly. If left empty, it will be found automatically at runtime by the name below.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Only used if Audio Source above isn't assigned - must exactly match the GameObject's name in the Hierarchy.")]
    [SerializeField] private string audioSourceObjectName = "Audio Source";

    [Tooltip("The collection of songs this music box can play, in order.")]
    [SerializeField] private AudioClip[] songs;

    [Tooltip("Play the first song automatically when the scene starts.")]
    [SerializeField] private bool playOnStart = false;

    private int currentSongIndex = 0;

    private void Awake()
    {
        if (audioSource == null)
        {
            GameObject found = GameObject.Find(audioSourceObjectName);

            if (found != null)
            {
                audioSource = found.GetComponent<AudioSource>();
            }

            if (audioSource == null)
            {
                Debug.LogWarning("MusicBoxPlayer could not find an AudioSource named \"" + audioSourceObjectName + "\" in the scene. Drag one into the Audio Source field in the Inspector instead.");
            }
        }
    }

    private void Start()
    {
        if (playOnStart && songs.Length > 0)
        {
            PlaySong(0);
        }
    }

    /// <summary>
    /// Plays the song at the given index. Index wraps around if out of range
    /// (e.g. -1 wraps to the last song), so Next/Previous can call this freely.
    /// </summary>
    public void PlaySong(int index)
    {
        if (audioSource == null || songs.Length == 0) return;

        int wrappedIndex = ((index % songs.Length) + songs.Length) % songs.Length;
        currentSongIndex = wrappedIndex;

        audioSource.clip = songs[currentSongIndex];
        audioSource.Play();
    }

    /// <summary>
    /// Plays a specific clip by reference - useful for a menu where each
    /// button represents one specific song.
    /// </summary>
    public void PlaySongByClip(AudioClip clip)
    {
        int index = System.Array.IndexOf(songs, clip);

        if (index == -1)
        {
            Debug.LogWarning("MusicBoxPlayer: clip \"" + (clip != null ? clip.name : "null") + "\" not found in songs.");
            return;
        }

        PlaySong(index);
    }

    public void NextSong()
    {
        PlaySong(currentSongIndex + 1);
    }

    public void PreviousSong()
    {
        PlaySong(currentSongIndex - 1);
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Exposes the dedicated AudioSource, e.g. for a color-reactive script to
    /// read spectrum data from without needing its own reference wiring.
    /// </summary>
    public AudioSource GetAudioSource()
    {
        return audioSource;
    }

    public AudioClip GetCurrentSong()
    {
        return songs.Length > 0 ? songs[currentSongIndex] : null;
    }
}