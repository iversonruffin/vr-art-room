using UnityEngine;
using UnityEngine.UI;

public class RestartButtonBind : MonoBehaviour
{
    void Start()
    {

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            // Remove any existing listeners first to prevent double-firing
            btn.onClick.RemoveAllListeners();
            
            // Bind the code-based click action
            btn.onClick.AddListener(HandleRestartClick);
        }
        
    }

    private void HandleRestartClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
