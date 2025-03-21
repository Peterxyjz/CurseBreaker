using UnityEngine;
using System.Collections;

public class ArcherAssa : Enemy
{
    [SerializeField] private float detectionRange = 5f;    // Vùng phát hiện
    [SerializeField] private GameObject arrowPrefab; // Prefab của mũi tên Arrow
    [SerializeField] private Transform firePoint;
    private float attackCooldownTimer = 0f;                  // Thời gian chờ giữa các đòn tấn công

    private Animator animator;
    private bool isAttacking = false;
    private AudioMonster audio;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        audio = FindAnyObjectByType<AudioMonster>();
    }

    private void Update()
    {
        if (player == null) return;

        // Flip hướng về player mỗi frame
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        FlipEnemy(directionToPlayer);

        // Tính khoảng cách từ ArcherAssa đến Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Nếu player trong vùng phát hiện và cooldown đã hết thì tấn công
        if (distanceToPlayer <= detectionRange && attackCooldownTimer <= 0f)
        {
            AttackPlayer();
            attackCooldownTimer = 1.5f; // Reset cooldown sau mỗi lần tấn công
        }

        // Giảm dần cooldown theo thời gian
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Tiến hành tấn công Player.
    /// </summary>
    private void AttackPlayer()
    {
        if (!isAttacking)
        {
            
            isAttacking = true;
            setAnimator();
            Debug.Log("ArcherAssa attacks the Player!");
            Vector2 targetPosition = player.transform.position;
            // Thêm logic tấn công (ví dụ: bắn tên, gọi hàm player.TakeDamage(enemyDamage), v.v.)
            StartCoroutine(WaitForAttackAnimation(targetPosition));
        }
    }

    /// <summary>
    /// Coroutine đợi cho đến khi animation "ArcherAssaAttack" được chạy và hoàn thành.
    /// </summary>
    private IEnumerator WaitForAttackAnimation(Vector2 targetPosition)
    {
        // Đợi một frame để đảm bảo trigger đã được xử lý
        yield return null;

        // Chờ cho đến khi trạng thái chuyển sang "ArcherAssaAttack"
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("ArcherAssaAttack"))
        {
            yield return null;
        }
        // Chờ cho đến khi animation hoàn thành
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ArcherAssaAttack") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }
        audio.PlayerArcherAttack();
        Vector3 directionPlayer = (player.transform.position - firePoint.position).normalized;
        GameObject arrowInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        Arrow arrow = arrowInstance.GetComponent<Arrow>();
        arrow.SetMoveDirection(directionPlayer * 15f);
        isAttacking = false;
        setAnimator();
    }

    /// <summary>
    /// Lật sprite của ArcherAssa dựa trên hướng đến Player.
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
    /// Cập nhật các tham số cho Animator.
    /// </summary>
    private void setAnimator()
    {
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDying", isDying);
    }

    protected override void Die()
    {
        if (isDying) return;
        isDying = true;
        
        animator.SetBool("isDying", true);
        Debug.Log("ArcherAssa is dying...");

        // Vô hiệu hóa script để ngưng các hành vi
        this.enabled = false;
        StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        audio.PlayerArcherDie();
        // Đợi một frame để đảm bảo trigger đã được xử lý
        yield return null;

        // Chờ cho đến khi trạng thái chuyển sang "ArcherAssaAttack"
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("ArcherAssaDie"))
        {
            yield return null;
        }
        // Chờ cho đến khi animation hoàn thành
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ArcherAssaDie") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    public float GetAttackDamage()
    {
        return enemyDamage;
    }
}
