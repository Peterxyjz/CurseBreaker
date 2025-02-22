using UnityEngine;

public class Sword : MonoBehaviour
{
    private BoxCollider2D attackCollider;
    private LineRenderer lineRenderer;

    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private Vector2 attackSize = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject attackEffect;

    public Transform player;
    public Vector3 offsetRight = new Vector3(0.5f, 0, 0);
    public Vector3 offsetLeft = new Vector3(-0.5f, 0, 0);

    private void Start()
    {
        // Khởi tạo BoxCollider2D (nếu chưa có)
        attackCollider = GetComponent<BoxCollider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        attackCollider.isTrigger = true;
        attackCollider.size = attackSize;
        attackCollider.enabled = false;
        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }

        // Khởi tạo LineRenderer để hiển thị vùng tấn công
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0f); // Màu trắng nhưng Alpha = 0 (trong suốt)
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0f);

        lineRenderer.positionCount = 5; // 4 góc + 1 điểm đóng vùng
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (player != null)
        {
            // Cập nhật vị trí theo hướng nhân vật
            transform.position = player.position + (player.localScale.x > 0 ? offsetRight : offsetLeft);

            // Đảm bảo BoxCollider luôn cập nhật theo hướng nhân vật
            attackCollider.offset = new Vector2((player.localScale.x > 0 ? offsetRight.x : offsetLeft.x), 0);
        }
    }

    public void PerformAttack(float damage)
    {
        attackCollider.enabled = true; // Bật Collider để phát hiện va chạm

        if (attackEffect != null)
        {
            attackEffect.SetActive(true);
        }

        DrawAttackBox(); // Hiển thị vùng tấn công bằng LineRenderer

        // Kiểm tra kẻ địch trúng đòn
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(transform.position, attackSize, 0, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy enemyController = enemy.GetComponent<Enemy>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(damage);
                    Debug.Log("🗡 Gây sát thương cho " + enemy.gameObject.name);
                }
            }
        }

        Invoke(nameof(DisableAttack), attackDuration);
    }

    private void DrawAttackBox()
    {
        Vector3 attackPosition = transform.position;
        float direction = player.localScale.x > 0 ? 1f : -1f;

        // Tính toán vị trí 4 góc vùng tấn công
        Vector3 topLeft = attackPosition + new Vector3(-attackSize.x / 2 * direction, attackSize.y / 2);
        Vector3 topRight = attackPosition + new Vector3(attackSize.x / 2 * direction, attackSize.y / 2);
        Vector3 bottomRight = attackPosition + new Vector3(attackSize.x / 2 * direction, -attackSize.y / 2);
        Vector3 bottomLeft = attackPosition + new Vector3(-attackSize.x / 2 * direction, -attackSize.y / 2);

        // Cập nhật vị trí LineRenderer
        lineRenderer.SetPosition(0, topLeft);
        lineRenderer.SetPosition(1, topRight);
        lineRenderer.SetPosition(2, bottomRight);
        lineRenderer.SetPosition(3, bottomLeft);
        lineRenderer.SetPosition(4, topLeft); // Đóng hình chữ nhật

        lineRenderer.enabled = true;
    }

    private void DisableAttack()
    {
        attackCollider.enabled = false; // Tắt va chạm
        lineRenderer.enabled = false;   // Ẩn vùng hiển thị
        if (attackEffect != null)
        {
            attackEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemyController = collision.GetComponent<Enemy>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(1f);
                Debug.Log("🗡 Gây sát thương cho " + collision.gameObject.name);
            }
        }
    }
}
