using UnityEngine;

public class MiniBossAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private WormBoss boss; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("attack check");
        // Kiểm tra nếu là Player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("attack player");
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Lấy sát thương từ SkeletonController hoặc gán cứng
                float damage = boss.GetAttackDamage();
                player.TakeDamage(damage);
            }
        }
    }
}
