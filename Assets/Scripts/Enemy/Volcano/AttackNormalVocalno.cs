using UnityEngine;

public class AttackNormalVocalno : MonoBehaviour
{
    [SerializeField] private VolcanoBossController boss; // Kéo thả Skeleton vào

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("attack check");
        // Kiểm tra nếu là Player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("attack player by normal");
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
