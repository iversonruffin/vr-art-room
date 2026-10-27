using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class TextManager : MonoBehaviour 
{
    public static TextManager Instance { get; private set; }

    [Header("UI Text References")]
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI formattedItemsText;
    [SerializeField] public TextMeshProUGUI endingText;

    [Header("UI Screens")]
    [SerializeField] private GameObject[] screens;

    void Awake() 
    {
        // Make sure there is only one GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep this GameManager when changing scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start() 
    {
        // REMOVED: GameObject.Find calls are gone. 
        // Assign timerText, formattedItemsText, and endingText in the Inspector instead!
        Debug.Log("Game started");
        
    }

    void Update() 
    {
        FormatItemsFoundText();
    }

    public void FormatItemsFoundText() 
    {
        // Safety check: Prevent errors if GameManager is missing during scene reloads
        if (GameManager.Instance == null) return;

        // Safety check: Prevent errors if the text component isn't assigned yet
        if (formattedItemsText == null) return;

        string newString = "Items: " + GameManager.Instance.itemsFound.ToString() + " / " + GameManager.Instance.totalItems.ToString();
        formattedItemsText.text = newString;
    }

    public void OpenScreen(string screenName) 
    {
        CloseAllMenus();

        // FIX: Search the screens array instead of using GameObject.Find
        // This allows Unity to find screens even if they are deactivated/turned off
        foreach (GameObject screenObject in screens)
        {
            if (screenObject != null && screenObject.name == screenName)
            {
                screenObject.SetActive(true);
                return; // Stop searching once we find and activate the right screen
            }
        }

        //Debug.LogWarning("Screen named '" + screenName + "' was not found in the screens array!");
    }

    private void CloseAllMenus() 
    {
        foreach (GameObject screenObject in screens) 
        {
            if (screenObject != null)
            {
                screenObject.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe to the sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

     // This method runs AUTOMATICALLY every time any scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name} using mode: {mode}");

        // Example 2: Find objects or initialize level parameters here
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        formattedItemsText = GameObject.Find("ItemsFoundMenuText").GetComponent<TextMeshProUGUI>();
        endingText = GameObject.Find("EndingText").GetComponent<TextMeshProUGUI>();

        // 2. Find UI Screens. 
        // Because deactivated objects cannot be found with GameObject.Find, 
        // we find a persistent parent canvas or look for objects tagged as "UIScreen".
        // For this to work, ensure all your screen GameObjects have the Tag "UIScreen" in Unity!
        GameObject[] foundScreens = GameObject.FindGameObjectsWithTag("UIScreen");
        
        // If screens are found, allocate them to our array
        if (foundScreens.Length > 0)
        {
            screens = foundScreens;
        }

        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }

        OpenScreen("WelcomeScreen");
    }
}
