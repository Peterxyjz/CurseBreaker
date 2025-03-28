using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    [SerializeField] private float healAmount = 1f; // Lượng máu hồi khi nhặt vật phẩm
    [SerializeField] private float IncreaseAmount = 1f; // Lượng máu hồi khi nhặt vật phẩm
    private GameManager gameManager;
    private AudioManager audioManager;

    PlayerController player;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        player = GetComponent<PlayerController>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hearth"))
        {

            audioManager.PlayCollectionItem();
            if (player != null)
            {
                player.HealHp(healAmount);
                Destroy(collision.gameObject); // Xóa vật phẩm sau khi nhặt
            }
        }
        else if (collision.CompareTag("HearthIncrease"))
        {

            audioManager.PlayCollectionItem();
            if (player != null)
            {
                player.IncreaseHp(IncreaseAmount);
                Destroy(collision.gameObject); // Xóa vật phẩm sau khi nhặt
            }
        }
        else if (collision.CompareTag("Trap"))
        {
            player.TakeDamage(1f);
            Debug.Log("tru mau = trap");
        }else if (collision.CompareTag("GroundDie"))
        {
            Debug.Log("out map");
            player.GoToGroundDie();
        }
        else if (collision.CompareTag("Artifact"))
        {
            Destroy(collision.gameObject);
        }
    }
}
