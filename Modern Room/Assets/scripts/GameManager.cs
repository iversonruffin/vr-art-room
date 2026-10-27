using TMPro; 
using UnityEngine; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour 
{ 
    public static GameManager Instance { get; private set; } 

    [Header("Game Settings")] 
    [SerializeField] private float startingTime = 120f; // 5 minutes 

    [Header("Timer")] 
    public float timeRemaining; 
    public bool isGameOver; 
    public bool isGameRunning; 

    [Header("Items")] 
    public int totalItems; 
    public int itemsFound; 
    public int itemsPlaced; 

    void Awake() 
    { 
        // Make sure there is only one GameManager 
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        } 
        Instance = this; 
        isGameRunning = false; 
        isGameOver = false; // Explicitly ensure this starts false

        // Check if the container exists to prevent a null reference exception
        GameObject itemsParent = GameObject.Find("ItemsToFind");
        if (itemsParent != null)
        {
            totalItems = itemsParent.transform.childCount; 
        }

        // Keep this GameManager when changing scenes 
        DontDestroyOnLoad(gameObject); 
    } 

    void Start() { } 

    void Update() 
    { 
        // If the game isn't actively running or is already over, do nothing
        if (!isGameRunning || isGameOver) return;

        UpdateTimer(); 

        // CRITICAL FIX: Only trigger lose if the game isn't already handled
        if (timeRemaining <= 0 && itemsFound != totalItems) 
        { 
            TriggerLose(); 
        } 
    } 

    public void UpdateTimer() 
    { 
        // Count down 
        timeRemaining -= Time.deltaTime; 

        // Prevent timer from going below zero 
        timeRemaining = Mathf.Max(timeRemaining, 0f); 

        // Convert seconds into minutes and seconds 
        int minutes = Mathf.FloorToInt(timeRemaining / 60f); 
        int seconds = Mathf.FloorToInt(timeRemaining % 60f); 

        // Format as 5:00, 4:59, 4:58, etc. 
        string formattedTime = string.Format("{0}:{1:00}", minutes, seconds); 

        // Update the HUD 
        if (TextManager.Instance != null && TextManager.Instance.timerText != null) 
        { 
            TextManager.Instance.timerText.text = formattedTime; 
        } 
    } 

    public void OnMemoryFound() 
    { 
        if (isGameOver) return; // Prevent collecting items after game ends

        itemsFound++; 
        if (AudioManager.Instance != null) AudioManager.Instance.Play("foundsound"); 

        // Add 15 seconds 
        timeRemaining += 15f; 

        // Check if finding this item triggers a win immediately
        if (itemsFound == totalItems)
        {
            TriggerWin();
        }
    } 

    public void OnWrongObjectFound() 
    { 
        if (isGameOver) return;

        RemoveTime(15f); 
        if (AudioManager.Instance != null) AudioManager.Instance.Play("wrongsound"); 
    } 

    public void RemoveTime(float amount) 
    { 
        timeRemaining -= amount; 
        timeRemaining = Mathf.Max(timeRemaining, 0f); 
        Debug.Log("Incorrect item! Removed " + amount + " seconds. Time remaining: " + timeRemaining); 
    } 

    private void GameOver() 
    { 
        // CRITICAL FIX: Turn off the gameplay loops immediately
        isGameOver = true; 
        isGameRunning = false; 

        if (TextManager.Instance != null)
        {
            TextManager.Instance.OpenScreen("EndScreen"); 
        }
        Debug.Log("Game Over status applied."); 
    } 

    public void TriggerWin() 
    { 
        GameOver(); 
        if (AudioManager.Instance != null) AudioManager.Instance.Play("victorysound"); 
        if (TextManager.Instance != null && TextManager.Instance.endingText != null)
        {
            TextManager.Instance.endingText.text = "You Win!"; 
        }
    } 

    public void TriggerLose() 
    { 
        GameOver(); 
        if (AudioManager.Instance != null) AudioManager.Instance.Play("losesound"); 
        if (TextManager.Instance != null && TextManager.Instance.endingText != null)
        {
            TextManager.Instance.endingText.text = "You Lose!"; 
        }
    } 

    public void StartGame() 
    { 
        // Reset game state completely
        timeRemaining = startingTime; 
        isGameRunning = true; 
        isGameOver = false; // Reset game over check
        itemsFound = 0; 
        itemsPlaced = 0; 

        // Recalculate total items in case a new level scene was loaded
        GameObject itemsParent = GameObject.Find("ItemsToFind");
        if (itemsParent != null)
        {
            totalItems = itemsParent.transform.childCount; 
        }

        // Show timer 
        if (TextManager.Instance != null && TextManager.Instance.timerText != null) 
        { 
            TextManager.Instance.timerText.gameObject.SetActive(true); 
            TextManager.Instance.timerText.text = "5:00"; 
        } 

        // Show items text 
        if (TextManager.Instance != null && TextManager.Instance.formattedItemsText != null) 
        { 
            TextManager.Instance.formattedItemsText.gameObject.SetActive(true); 
        } 

        TextManager.Instance.OpenScreen("GameScreen");
    } 

    public void RestartGame() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    } 
}
