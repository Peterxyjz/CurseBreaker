using UnityEngine;
using System.Collections;

public class Barricade : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float pushUpForce = 3f; // Đẩy lên
    [SerializeField] private float pushBackForce = 5f; // Đẩy về trái
    [SerializeField] private float disableMovementTime = 0.5f; // Thời gian khóa di chuyển

    private AudioForest audioForest;
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isAttacking", false);
        audioForest = FindAnyObjectByType<AudioForest>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kích hoạt animation tấn công
            animator.SetBool("isAttacking", true);

            // Lấy Rigidbody2D của Player
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerRb != null)
            {
                // Khóa di chuyển của nhân vật
                if (playerController != null)
                {
                    playerController.enabled = false;
                }

                // Xác định hướng đẩy dựa vào vị trí của Player so với Barricade
                float pushDirectionX = collision.transform.position.x < transform.position.x ? -1 : 1; // Trái thì -1, phải thì 1

                // Tạo lực đẩy
                Vector2 pushDirection = new Vector2(pushDirectionX * pushBackForce, pushUpForce);
                playerRb.linearVelocity = Vector2.zero; // Reset vận tốc trước khi đẩy
                playerRb.AddForce(pushDirection, ForceMode2D.Impulse);

                // Sau 0.5s, mở khóa di chuyển
                StartCoroutine(EnableMovementAfterDelay(playerController));

                // Quay về trạng thái idle sau khi đánh xong
                StartCoroutine(ResetToIdleAfterAnimation());
            }
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
    private IEnumerator ResetToIdleAfterAnimation()
    {
        // Đợi đến khi animation "attacking" thực sự bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("BarricadesAttack"));
        audioForest.PlayBarricade();
        // Lấy thời gian của animation "attacking"
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animLength); // Chờ animation kết thúc
        animator.SetBool("isAttacking", false);

    }

    private void DestroyAfterAnimation()
    {
        

        Destroy(gameObject); 
    }
}
