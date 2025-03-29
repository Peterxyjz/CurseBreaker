using UnityEngine;
using System.Collections;

public class Lighting : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float pushUpForce = 3f; // Đẩy lên
    [SerializeField] private float pushBackForce = 5f; // Đẩy về trái
    [SerializeField] private float disableMovementTime = 1f; // Thời gian khóa di chuyển
    [SerializeField] private float cooldownTime = 5f; // Thời gian cooldown trước khi xuất hiện lại

    private Collider2D lightingCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject currentPlayer = null; // Lưu Player đang va chạm
    private AudioForest audioForest;
    private void Start()
    {
        animator = GetComponent<Animator>();
        lightingCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioForest = FindAnyObjectByType<AudioForest>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Bắt đầu Coroutine để delay 1s trước khi tấn công
            currentPlayer = collision.gameObject; // Lưu Player
            StartCoroutine(AttackAfterDelay(collision));
        }
    }

    private IEnumerator AttackAfterDelay(Collision2D collision)
    {
        if (lightingCollider != null) lightingCollider.enabled = false;
        yield return new WaitForSeconds(0.5f); // Delay 1 giây
        if (lightingCollider != null) lightingCollider.enabled = true;
        animator.SetBool("isAttacking", true);
        audioForest.PlayLighting();
        StartCoroutine(ResetToIdleAndCooldown());
        // Kiểm tra lại xem Player còn trong vùng va chạm không (tránh trường hợp delay xong nhưng player đã đi khỏi)
        if (collision == null || collision.gameObject == null)
        {
           
            yield break;
        }

        // Kích hoạt animation tấn công
        
        if (currentPlayer == null || !lightingCollider.bounds.Contains(currentPlayer.transform.position))
        {
            currentPlayer = null; // Reset player
           
            yield break;
        }
        // Lấy Rigidbody2D của Player
        Rigidbody2D playerRb = currentPlayer.GetComponent<Rigidbody2D>();
        PlayerController playerController = currentPlayer.GetComponent<PlayerController>();

        if (playerRb != null)
        {
            // Khóa di chuyển của nhân vật
            if (playerController != null)
            {
                playerController.enabled = false;
                playerController.TakeDamage(1f);
            }

            // Xác định hướng đẩy dựa vào vị trí của Player so với Barricade
            float pushDirectionX = collision.transform.position.x < transform.position.x ? -1 : 1; // Trái thì -1, phải thì 1

            // Tạo lực đẩy
            Vector2 pushDirection = new Vector2(pushDirectionX * pushBackForce, pushUpForce);
            playerRb.linearVelocity = Vector2.zero; // Reset vận tốc trước khi đẩy
            playerRb.AddForce(pushDirection, ForceMode2D.Impulse);

            // Sau 0.5s, mở khóa di chuyển
            StartCoroutine(EnableMovementAfterDelay(playerController));

            // Quay về trạng thái idle sau khi đánh xong, sau đó ẩn đi trong thời gian cooldown
           
        }
    }
    private IEnumerator EnableMovementAfterDelay(PlayerController playerController)
    {
        yield return new WaitForSeconds(disableMovementTime);
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private IEnumerator ResetToIdleAndCooldown()
    {
        // Đợi đến khi animation "LightingAttack" thực sự bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("LightingAttack"));

        // Lấy thời gian của animation "LightingAttack"
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animLength); // Chờ animation kết thúc
        animator.SetBool("isAttacking", false);

        // Ẩn đi và vô hiệu hóa va chạm
        if (lightingCollider != null) lightingCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // Đợi trong thời gian cooldown
        yield return new WaitForSeconds(cooldownTime);

        // Hiển thị lại và kích hoạt va chạm
        if (lightingCollider != null) lightingCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }
}
