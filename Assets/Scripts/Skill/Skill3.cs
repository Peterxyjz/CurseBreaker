using UnityEngine;

public class Skill3 : MonoBehaviour
{
    public float damageMultiplier = 4f;      // Hệ số nhân sát thương
    public float activeRadius = 10f;         // Phạm vi tác động của Skill3 (10f)
    public float activeDuration = 0.5f;        // Thời gian hiệu ứng hiển thị trên enemy
    public GameObject effectPrefab;          // Prefab hiệu ứng hiển thị trên enemy khi bị trúng

    private float playerDamage;              // Sát thương cơ bản của Player lưu lại khi kích hoạt Skill3

    /// <summary>
    /// Kích hoạt Skill3: Tìm tất cả đối tượng có tag "Enemy" trong phạm vi activeRadius và gây sát thương
    /// </summary>
    /// <param name="playerDamage">Sát thương cơ bản của Player</param>
    public void ActivateSkill(float playerDamage)
    {
        this.playerDamage = playerDamage;

        // Lấy tất cả đối tượng có Collider2D trong phạm vi activeRadius từ vị trí hiện tại
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, activeRadius);

        foreach (Collider2D enemy in enemies)
        {
            // Chỉ xử lý các đối tượng có tag "Enemy"
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(playerDamage * damageMultiplier);
                }

                // Hiển thị hiệu ứng tại vị trí enemy và tự hủy sau activeDuration
                if (effectPrefab != null)
                {
                    GameObject effect = Instantiate(effectPrefab, enemy.transform.position, Quaternion.identity);
                    Destroy(effect, activeDuration);
                }
            }
        }
    }

    // Hiển thị vùng phạm vi của Skill3 khi chọn đối tượng trong Scene (Gizmos)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeRadius);
    }
}
