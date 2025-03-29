using UnityEngine;
using System.Collections;
public class NormalAttack : MonoBehaviour
{
    public float damage = 1f; // Giá trị sát thương của Arrow
    private Vector3 moveDirection;
    private Coroutine destroyCoroutine;
    private float destroyTime = 0.1f;
    private void Start()
    {
        destroyCoroutine = StartCoroutine(DestroyAfterDelay(destroyTime * 15));
    }
    void Update()
    {
        if (moveDirection == Vector3.zero) return;
        transform.position += moveDirection * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            // Lấy component của Player chứa hàm TakeDamage
            GameObject playerObj = collision.gameObject;
            DesertBoss boss = FindAnyObjectByType<DesertBoss>();
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
                boss.HealHpFromAttack();
            }

            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
            }
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
