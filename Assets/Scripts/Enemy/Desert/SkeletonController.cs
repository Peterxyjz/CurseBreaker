using UnityEngine;
using System.Collections;

public class SkeletonController : Enemy
{
    [SerializeField] private float detectionRange = 5f;    // Vùng phát hiện
    [SerializeField] private float attackRange = 1f;         // Khoảng cách để tấn công
    [SerializeField] private float chaseSpeed = 2f;          // Tốc độ di chuyển khi đuổi
    [SerializeField] private float returnSpeed = 2f;         // Tốc độ di chuyển quay về vị trí ban đầu
    [SerializeField] private float forcedReturnDuration = 2f; // Thời gian buộc quay về (2 giây)
    [SerializeField] private Collider2D attackRangeCollider;
    private bool isChasing = false;
    private Vector3 initialPosition; // Vị trí ban đầu của Skeleton

    // Biến quản lý trạng thái forced return
    private bool isForcedReturn = false;
    private float forcedReturnTimer = 0f;

    private Animator animator;
    private bool isRunning = false;
    private bool isAttacking = false;
    private AudioMonster audio;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        audio = FindAnyObjectByType<AudioMonster>();
        // Lưu lại vị trí ban đầu
        initialPosition = transform.position;

        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
    }

    private void Update()
    {
        if (player == null) return; // Phòng trường hợp không tìm thấy Player

        // Nếu đang forced return, cập nhật forced timer và quay về vị trí ban đầu
        if (isForcedReturn)
        {
            forcedReturnTimer -= Time.deltaTime;
            ReturnToInitialPosition();
            if (forcedReturnTimer <= 0f)
            {
                isForcedReturn = false;
            }
            return;
        }
        // Nếu đang tấn công, không cho di chuyển hoặc cập nhật hành vi khác
        if (isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        float distanceFromInitial = Vector2.Distance(transform.position, initialPosition);

        // Nếu vượt quá giới hạn (vị trí ban đầu + detectionRange * 2), bật chế độ forced return
        if (distanceFromInitial > detectionRange * 1)
        {
            isForcedReturn = true;
            forcedReturnTimer = forcedReturnDuration;
            ReturnToInitialPosition();
            return;
        }

        // Kiểm tra vùng phát hiện của Player
        isChasing = (distanceToPlayer <= detectionRange);

        if (isChasing)
        {
            isRunning = true;
            MoveTowardsPlayer();
            setAnimator();
            // Nếu đủ gần, tấn công Player
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
        else
        {
            // Nếu không đang đuổi theo, thì quay về vị trí ban đầu
            ReturnToInitialPosition();
        }
    }

    /// <summary>
    /// Di chuyển về phía Player và bật animation chạy
    /// </summary>
    private void MoveTowardsPlayer()
    {
      
        isRunning = true;
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y); // giữ nguyên Y
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );
        FlipEnemy(direction);
    }


    /// <summary>
    /// Di chuyển trở về vị trí ban đầu.
    /// Nếu đã về vị trí ban đầu (distance <= 0.1f), isRunning sẽ được đặt là false.
    /// </summary>
    private void ReturnToInitialPosition()
    {
        Vector2 targetPosition = new Vector2(initialPosition.x, transform.position.y); // giữ nguyên Y
        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance > 0.1f)
        {
            
            isRunning = true;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                returnSpeed * Time.deltaTime
            );
            FlipEnemy(direction);
        }
        else
        {
          
            isRunning = false;
        }
        setAnimator();
    }


    /// <summary>
    /// Xử lý hành vi tấn công Player.
    /// Khi tấn công, Skeleton sẽ gọi trigger "Attack", và không cho di chuyển cho đến khi animation "SkeletonAttack" hoàn thành.
    /// </summary>
    private void AttackPlayer()
    {
        if (!isAttacking)
        {
            isRunning = false;
            isAttacking = true;
            setAnimator();
            if (attackRangeCollider != null)
                attackRangeCollider.enabled = true;
            Debug.Log($"{attackRangeCollider != null}");
            // Thêm logic tấn công ở đây, ví dụ: player.TakeDamage(enemyDamage)
            StartCoroutine(WaitForAttackAnimation());
        }
    }

    /// <summary>
    /// Coroutine đợi cho đến khi animation "SkeletonAttack" được chạy và hoàn thành.
    /// </summary>
    private IEnumerator WaitForAttackAnimation()
    {
        // Đợi một frame để đảm bảo trigger đã được xử lý
        yield return null;
        // Chờ cho đến khi trạng thái chuyển sang "SkeletonAttack"
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAttack"))
        {
            yield return null;
        }
        // Đợi cho đến khi animation "SkeletonAttack" hoàn thành
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAttack") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
        isAttacking = false;
        setAnimator();
    }

    /// <summary>
    /// Lật sprite của Skeleton dựa trên hướng di chuyển
    /// </summary>
    private void FlipEnemy(Vector2 direction)
    {
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    /// <summary>
    /// Cập nhật các tham số của Animator
    /// </summary>
    private void setAnimator()
    {
        if (isRunning)
        {
            
            audio.StartSkeletonRunLoop();
        }
        else
        {
            audio.StopSkeletonRunLoop();
        }
            animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDying", isDying);
    }

    protected override void Die()
    {
        // Kiểm tra nếu đã bắt đầu quá trình chết rồi thì không thực hiện lại
        if (isDying) return;
        isDying = true;
        audio.PlayerSkeletonDie();
        // Cập nhật trạng thái cho Animator (nếu có animation chết)
        animator.SetBool("isDying", true);
        Debug.Log("Skeleton is dying...");

        // Vô hiệu hóa collider để tránh tương tác sau khi chết
        //Collider2D[] colliders = GetComponents<Collider2D>();
        //foreach (var collider in colliders)
        //{
        //    collider.enabled = false;
        //}

        // Bắt đầu coroutine để đợi 1 giây rồi xóa đối tượng
        this.enabled = false;
        StartCoroutine(DisappearAfterDelay());
    }
    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(1.25f);
        Debug.Log("Skeleton đã biến mất sau 1 giây.");
        Destroy(gameObject);
    }
    public float GetAttackDamage()
    {
        return enemyDamage;
    }
}
