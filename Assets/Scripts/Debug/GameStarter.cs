using UnityEngine;

/// <summary>
/// Temporary script to auto-start the game for testing.
/// Attach to any GameObject in the scene.
/// </summary>
public class GameStarter : MonoBehaviour
{
    [SerializeField] private bool autoStartOnPlay = true;

    private void Start()
    {
        if (autoStartOnPlay && GameManager.Instance != null)
        {
            Debug.Log("Auto-starting game...");
            GameManager.Instance.StartGame();
        }
    }
}
