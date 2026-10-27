using UnityEngine;

/// <summary>
/// Reacts to the music playing through an assigned AudioSource. Choose a
/// ReactionType per object: ColorChanging tints the material based on the
/// spectrum (bass/mid/high -> RGB), or Moving pulses the object's scale along
/// either its vertical or horizontal axis. Purely visual - no physics/forces
/// involved, so this works safely on static or non-Rigidbody objects too.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class MusicColorReactive : MonoBehaviour
{
    public enum ReactionType { Moving, ColorChanging }
    public enum ScaleAxis { Vertical, Horizontal }

    [Header("Audio")]
    [SerializeField] public AudioSource audioSource;

    [Header("Reaction")]
    [Tooltip("Moving pulses the object's scale with the music. ColorChanging tints its material instead.")]
    public ReactionType reactionType = ReactionType.ColorChanging;

    [Tooltip("Only used when Reaction Type is Moving. Vertical scales the Y axis; Horizontal scales X and Z.")]
    public ScaleAxis scaleAxis = ScaleAxis.Vertical;

    [Header("Intensity Multipliers")]
    public float bassMultiplier = 20f;
    public float midMultiplier = 15f;
    public float highMultiplier = 10f;

    [Header("Blend Feel")]
    [Tooltip("How quickly the reaction snaps UP when the music hits a peak.")]
    public float attackSpeed = 12f;
    [Tooltip("How slowly the reaction fades back DOWN after a peak - lower = longer glow/pulse.")]
    public float releaseSpeed = 2.5f;

    [Header("Scale Settings (Moving only)")]
    [Tooltip("How much extra scale gets added at full intensity, on top of the object's starting scale.")]
    public float scaleAmount = 1f;

    private Renderer rend;
    private Vector3 baseScale;

    // Spectrum array
    private float[] spectrum = new float[512];

    // Smoothed (attack/release) band values, carried across frames for the blend effect.
    private float smoothedBass;
    private float smoothedMid;
    private float smoothedHigh;

    void Start()
    {
        rend = GetComponent<Renderer>();
        baseScale = transform.localScale; // captured once, so scaling is additive rather than compounding every frame
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        // Get spectrum data
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float bass = GetFrequencyRange(0, 20);     // Low frequencies
        float mid = GetFrequencyRange(20, 100);    // Mid frequencies
        float high = GetFrequencyRange(100, 512);  // High frequencies

        // Apply multipliers
        bass *= bassMultiplier;
        mid *= midMultiplier;
        high *= highMultiplier;

        // Clamp values (keep results valid)
        bass = Mathf.Clamp01(bass);
        mid = Mathf.Clamp01(mid);
        high = Mathf.Clamp01(high);

        // Attack/release smoothing: rise quickly toward a louder value, fall
        // slowly toward a quieter one - this is what creates the blend/pulse
        // feel instead of everything jumping around every frame.
        smoothedBass = SmoothTowards(smoothedBass, bass);
        smoothedMid = SmoothTowards(smoothedMid, mid);
        smoothedHigh = SmoothTowards(smoothedHigh, high);

        switch (reactionType)
        {
            case ReactionType.ColorChanging:
                ApplyColorReaction();
                break;
            case ReactionType.Moving:
                ApplyScaleReaction();
                break;
        }
    }

    private void ApplyColorReaction()
    {
        // Create color (bass = red, mid = green, high = blue)
        Color newColor = new Color(smoothedBass, smoothedMid, smoothedHigh);
        rend.material.color = newColor;
    }

    private void ApplyScaleReaction()
    {
        // Bass drives the pulse strength - swap this for
        // (smoothedBass + smoothedMid + smoothedHigh) / 3f if you'd rather it
        // react to the whole spectrum instead of just the low end.
        float intensity = smoothedBass;

        Vector3 newScale = baseScale;

        switch (scaleAxis)
        {
            case ScaleAxis.Vertical:
                newScale.y = baseScale.y + intensity * scaleAmount;
                break;
            case ScaleAxis.Horizontal:
                newScale.x = baseScale.x + intensity * scaleAmount;
                newScale.z = baseScale.z + intensity * scaleAmount;
                break;
        }

        transform.localScale = newScale;
    }

    private float SmoothTowards(float current, float target)
    {
        float speed = target > current ? attackSpeed : releaseSpeed;
        return Mathf.Lerp(current, target, Time.deltaTime * speed);
    }

    float GetFrequencyRange(int start, int end)
    {
        float sum = 0;

        for (int i = start; i < end; i++)
        {
            sum += spectrum[i];
        }

        return sum / (end - start);
    }
}