using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        panel.SetActive(true);
        Time.timeScale = 0f; // freeze gameplay — enemies, spawning, and weapons all pause automatically
    }

    // Wire this up to the "Try Again" button's OnClick in the Inspector
    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f; // must reset before reloading, or the new scene loads already paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}