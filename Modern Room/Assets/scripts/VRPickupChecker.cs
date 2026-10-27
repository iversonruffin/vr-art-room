using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRTagCheck : MonoBehaviour
{
    // Remove "public" so you don't have to drag anything in the Inspector
    private XRBaseInteractable interactable;

    void Awake()
    {
        // This finds the interactable script automatically
        interactable = GetComponent<XRBaseInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(CheckGrabbedTag);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(CheckGrabbedTag);
    }

    void CheckGrabbedTag(SelectEnterEventArgs args)
    {
        GameObject grabbedObject = args.interactableObject.transform.gameObject;

        if (grabbedObject.CompareTag("Memory"))
        {
            GameManager.Instance.OnMemoryFound();

            if (GameManager.Instance.itemsFound == GameManager.Instance.totalItems && GameManager.Instance.timeRemaining > 0)
            {
                GameManager.Instance.TriggerWin();
            }

            Debug.Log("The grabbed object has the correct tag!");
        }
        else
        {
            //Debug.Log("The grabbed object has the wrong tag!");

            GameManager.Instance.OnWrongObjectFound();
        }
    }
}
