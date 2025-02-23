using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUi;
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverUi.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameOver()
    {
        Time.timeScale = 0;// ko cho nhấn gì hết 
        gameOverUi.SetActive(true);
    }
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }
}
