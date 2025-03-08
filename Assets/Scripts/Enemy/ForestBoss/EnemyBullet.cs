using UnityEngine;
using System.Collections;

public class EnemyBullet : MonoBehaviour
{
    private Vector3 moveDirection;
    private float bulletDamage = 1f;
    private float knockbackForce = 7f; // Lực đẩy lùi
    private float disableMoveDuration = 0.75f; // Thời gian Player không thể di chuyển
    private Coroutine destroyCoroutine;

    void Start()
    {
        // Bắt đầu coroutine hủy sau 3 giây
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(3f));
    }

    void Update()
    {
        if (moveDirection == Vector3.zero) return;
        transform.position += moveDirection * Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Cham roi");
            AttackPlayer(collision);
        }
    }

    private void AttackPlayer(Collider2D collision)
    {
        GameObject playerObj = collision.gameObject;
        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
        PlayerController playerController = playerObj.GetComponent<PlayerController>();

        if (playerRb != null && playerController != null)
        {
            // Vô hiệu hóa di chuyển của Player
            StartCoroutine(playerController.DisableMovement(disableMoveDuration));
  
            playerController.TakeDamage(bulletDamage);

            // Xác định hướng đẩy ngược lại từ viên đạn
            Vector2 pushDirection = (playerObj.transform.position - transform.position).normalized * knockbackForce;

            // Reset vận tốc trước khi đẩy
            playerRb.linearVelocity = Vector2.zero;
            playerRb.AddForce(pushDirection, ForceMode2D.Impulse);
        }

       

        // Hủy coroutine trước đó để tránh hủy sai thời gian
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
        }

        // Bắt đầu coroutine mới để hủy sau 0.75 giây
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(disableMoveDuration));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
