using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject gameOverUi;

    void Start()
    {
       
        if (gameOverUi != null)
        {
            gameOverUi.SetActive(false);
        }
    }
    
    public void GameOver()
    {

        Time.timeScale = 0; // Ngừng game
        if (gameOverUi != null)
        {
            gameOverUi.SetActive(true);
        }
    }

    public void PlayAgainGame()
    {
        Debug.Log("Restarting Scene");
      
        Time.timeScale = 1;

        // Reset lại UI game over
        if (gameOverUi != null)
        {
            gameOverUi.SetActive(false);
        }
        
        // Lấy tên màn chơi hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Nếu scene có chữ "Boss", thì restart lại màn trước đó
        SaveSystem.DeleteSaveFile();
        if (currentSceneName.EndsWith("Boss"))
        {
            string previousSceneName = currentSceneName.Substring(0, currentSceneName.Length - 4); // Cắt bỏ "Boss"
            Debug.Log($"Restarting previous scene: {previousSceneName}");
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            // Nếu không phải Boss, restart lại màn hiện tại
            Debug.Log($"Restarting current scene: {currentSceneName}");
            SceneManager.LoadScene(currentSceneName);
        }
    }



    public void LoadNextScene(GameObject player)
    {
        if (player != null)
        {
            Debug.Log("Tìm thấy Player");
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Lưu dữ liệu Player...");
                SaveSystem.SavePlayerData(playerController);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy PlayerController trên Player!");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy đối tượng Player!");
        }

        // Lấy index của scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Kiểm tra nếu còn scene tiếp theo
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Chuyển đến scene {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Đã ở scene cuối cùng!");
        }
    }




}
