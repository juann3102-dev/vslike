using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject gameOverUI;

    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        gameOverUI.SetActive(false);
    }
    
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over");

        if(gameOverUI != null ) gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }

}
