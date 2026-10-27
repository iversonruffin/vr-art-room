using System;
using UnityEngine;

public class MemoryObject : MonoBehaviour
{
    public String memoryName;

     // You can add logic here for when the memory is collected
    public void OnCollected()
    {
       // Debug.Log($"Memory Collected: {memoryName}");
        // Add your custom player/game progression logic here

    }
}