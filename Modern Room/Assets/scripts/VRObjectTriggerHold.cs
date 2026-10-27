using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRObjectTriggerHold : MonoBehaviour
{
    [Header("UI / Visual Elements")]
    [SerializeField] private GameObject displayObject; 

    private XRGrabInteractable grabInteractable;
    private VideoPlayer videoPlayer;
    public VideoClip videoClip;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        displayObject = GameObject.Find("TV_Screen");
        
        if (displayObject != null)
        {
            videoPlayer = displayObject.GetComponent<VideoPlayer>();
            //displayObject.SetActive(false); // Hide on startup
        }
    }

    private void OnEnable()
    {
        // Bind both trigger squeeze and trigger release events
        grabInteractable.activated.AddListener(OnTriggerPressed);
        grabInteractable.deactivated.AddListener(OnTriggerReleased);
    }

    private void OnDisable()
    {
        // Always clean up listeners to avoid errors
        grabInteractable.activated.RemoveListener(OnTriggerPressed);
        grabInteractable.deactivated.RemoveListener(OnTriggerReleased);
    }

    // Runs the exact frame the trigger passes the threshold down
    public void OnTriggerPressed(ActivateEventArgs args)
    {
        SetMediaState(true);
    }

    // Runs the exact frame the trigger is let go
    public void OnTriggerReleased(DeactivateEventArgs args)
    {
        SetMediaState(false);
    }

    private void SetMediaState(bool isActive)
    {
        if (displayObject == null) return;

        //displayObject.SetActive(isActive);

        if (this.videoPlayer != null)
        {
            if (isActive)
            {
                this.videoPlayer.clip = this.videoClip;
                this.videoPlayer.Play();
            }
            else
            {
                this.videoPlayer.Stop(); 
                // Alternative: Use videoPlayer.Stop() if you want it to reset to the beginning every time
            }
        }
    }
}
