using UnityEngine;
using UnityEngine.UI;

public class StartButtonBind : MonoBehaviour
{
    int num = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       Button btn = GetComponent<Button>();
        if (btn != null)
        {
            // Remove any existing listeners first to prevent double-firing
            btn.onClick.RemoveAllListeners();
            
            // Bind the code-based click action
            btn.onClick.AddListener(HandleStartClick);
        }
        
    }

    private void HandleStartClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
