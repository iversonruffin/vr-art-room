using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MemoryItem", menuName = "MemoryQuest/MemoryItem")]
public class MemoryItemData : ScriptableObject
{
    public string itemName;
    public enum MemoryType { Image, Video, Text }
    public MemoryType type;

    public Sprite image;          // used if type == Image
    public VideoClip video;       // used if type == Video
    [TextArea] public string text; // used if type == Text

    public AudioClip narration;   // optional, plays alongside any type
}