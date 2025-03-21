using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class ExploreAttack : MonoBehaviour
{
    public float damage = 1f; // Giá trị sát thương của Arrow
    private Vector3 moveDirection;
    private Coroutine destroyCoroutine;
    private float destroyTime = 0.1f;
    private float knockbackForce = 3f; // Lực đẩy lùi
    private float disableMoveDuration = 0.5f; // Thời gian Player không thể di chuyển
    Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(destroyTime * 15));
    }
    void Update()
    {
        if (moveDirection == Vector3.zero) return;
        transform.position += moveDirection * Time.deltaTime;
    }


    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
           
            animator.SetBool("isAttacking", true);
            // Lấy component của Player chứa hàm TakeDamage
            GameObject playerObj = collision.gameObject;
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            DesertBoss boss = FindAnyObjectByType<DesertBoss>();
            if (playerController != null)
            {
                boss.HealHpFromAttack();
                playerController.TakeDamage(damage);
            }

            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
            }
            moveDirection = Vector3.zero;
            Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();

            if (playerRb != null && playerController != null)
            {
                // Vô hiệu hóa di chuyển của Player
                StartCoroutine(playerController.DisableMovement(disableMoveDuration));

                // Xác định hướng đẩy ngược lại từ viên đạn
                Vector2 pushDirection = (playerObj.transform.position - transform.position).normalized * knockbackForce;

                // Reset vận tốc trước khi đẩy
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(pushDirection, ForceMode2D.Impulse);
            }
            yield return null;

            // Chờ animation "DesertBossAttack" bắt đầu
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("ExploreAttackBoom"));

            // Chờ animation "DesertBossAttack" bắt đầu
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            destroyCoroutine = StartCoroutine(DestroyAfterDelay(destroyTime));
        }
    }
    
    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
        // Tính góc quay dựa trên hướng di chuyển, chuyển từ radian sang độ
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

        // Gán góc quay cho Arrow, xoay quanh trục Z (cho 2D)
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
