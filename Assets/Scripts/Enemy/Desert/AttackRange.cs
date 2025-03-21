using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [SerializeField] private SkeletonController skeleton; // Kéo thả Skeleton vào

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
                float damage = skeleton.GetAttackDamage();
                player.TakeDamage(damage);
            }
        }
    }
}
