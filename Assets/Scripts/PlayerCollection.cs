using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f; // Lượng máu hồi khi nhặt vật phẩm
    private GameManager gameManager;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hearth"))
        {
            PlayerController player = GetComponent<PlayerController>();

            if (player != null)
            {
                player.HealHp(healAmount);
                Destroy(collision.gameObject); // Xóa vật phẩm sau khi nhặt
            }
        }
    }
}
