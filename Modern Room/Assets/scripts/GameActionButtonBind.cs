using UnityEngine;
using UnityEngine.UI;

public class GameActionButtonBind : MonoBehaviour 
{
    [Header("0 = Start Game, 1 = Restart Game")]
    [Tooltip("Enter 0 for Start Game, or 1 for Restart Game")]
    [Range(0, 2)] 
    public int gameActionType = 0; 

    void Start() 
    {
        Button btn = GetComponent<Button>();
        if (btn != null) 
        {
            // Remove any existing listeners first to prevent double-firing
            btn.onClick.RemoveAllListeners();
            
            // Bind the code-based click action
            btn.onClick.AddListener(HandleButtonClick);
        }
    }

    private void HandleButtonClick() 
    {
        if (GameManager.Instance == null) return;

        // Switch statement determines action based on user input (0 or 1)
        switch (gameActionType)
        {
            case 0:
                GameManager.Instance.StartGame();
                break;
                
            case 1:
                GameManager.Instance.RestartGame();
                break;

            case 2:
                TextManager.Instance.OpenScreen("DirectionsScreen");
                break;
                
            default:
                Debug.LogWarning($"Invalid action type '{gameActionType}' assigned to button.");
                break;
        }
    }
}
