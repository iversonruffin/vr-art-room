using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MemorySocketCollector : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;

    void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        // Subscribe to the event when an object is placed in the socket
        socketInteractor.selectEntered.AddListener(OnItemDroppedIntoSocket);
    }

    void OnDisable()
    {
        socketInteractor.selectEntered.RemoveListener(OnItemDroppedIntoSocket);
    }

    private void OnItemDroppedIntoSocket(SelectEnterEventArgs args)
    {
        // Get the interactable object that was dropped into the socket
        GameObject interactable = args.interactableObject.transform.gameObject;
        
        // Check if the object has the MemoryItem component
        if (interactable.CompareTag("Memory"))
        {
            // Trigger any custom collection logic (e.g., scoring, UI updates)
            GameManager.Instance.OnMemoryFound();

            // Optional: Play a sound using your AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play("foundsound");
            }

            if (GameManager.Instance.itemsFound == GameManager.Instance.totalItems && GameManager.Instance.timeRemaining > 0)
            {
                GameManager.Instance.TriggerWin();
            }

            // Completely remove the item from the scene
            //Destroy(interactable);
        }
        else
        {
            Debug.Log("Item dropped is not a memory. It will stay in the socket.");

            GameManager.Instance.OnWrongObjectFound();
            
        }

        interactable.SetActive(false);
    }
}
