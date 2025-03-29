using UnityEngine;
using UnityEngine.SceneManagement; // Import SceneManager để chuyển cảnh

public class SpaceGate : MonoBehaviour
{
    [SerializeField] private string nextSceneName; // Tên cảnh tiếp theo
    private GameManager gameManager;
    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Kiểm tra nếu Player chạm vào cổng
        {
            Debug.Log("🌌 Người chơi bước vào cổng dịch chuyển!");
            gameManager.LoadNextScene(collision.gameObject);
        }
    }

   
}
;