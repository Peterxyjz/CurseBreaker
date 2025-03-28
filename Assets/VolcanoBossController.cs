using System.Collections;
using UnityEngine;

public class VolcanoBossController : Enemy
{
    private float distance = 30f; // phạm vi phát hiện player
    public SpaceGate spaceGate;

    [SerializeField] private GameObject FireballPrefab;
    [SerializeField] private Transform FireballSpawnPoint;
    private float FireBallCooldown = 4f; // cooldown
    private int FireBallCount = 3;
    public GameObject circleMagic;
    private Vector3 startPos;

    private Rigidbody2D BossRigidBody;
    private Animator animator;
    private AudioBoss audioBoss;

    private bool IsFacingLeft = true;
    private bool isRunning = false;
    private bool IsSprinting = false;
    private float MovementSpeed = 5f;
    private float JumpStrength = 2f;
    private float SprintMultiplier = 1.75f;
    private bool isBerserMode = false;

    private bool isAttacking = false;
    private bool isHit = false;
    private bool isEnrage = false;
    private float AttackDamage = 1f;

    // Thêm biến cho cooldown tấn công (2s)
    private float attackCooldownTimer = 0f;
    private float attackCooldonwTimerReset = 3f;

    Vector2 MovementVector;

    [SerializeField] private Collider2D attackRangeCollider;

    void Start()
    {
        base.Start();
        spaceGate.gameObject.SetActive(false);
        circleMagic.SetActive(false);
        startPos = transform.position;
        BossRigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioBoss = FindAnyObjectByType<AudioBoss>();
        StartCoroutine(FireballAttackRoutine());
        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
    }

    void Update()
    {
        // Tính khoảng cách giữa player và boss theo trục X
        float playerDistance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (isEnrage || isDying)
        {
            BossRigidBody.linearVelocity = Vector2.zero;
            return;
        }
            
        // Nếu player nằm trong vùng phát hiện
        if (!isAttacking && playerDistance <= distance)
        {
            FacePlayer();

            // Nếu player chưa quá gần (khoảng cách > 3f) thì di chuyển tới player
            if (playerDistance > 3f)
            {
                isRunning = true;
                HandleMovement();
            }
            // Khi player quá gần (<= 3f) thì tấn công nếu cooldown cho phép
            else
            {
                
                if (attackCooldownTimer <= 0f && !isAttacking)
                {
                    // Khởi chạy coroutine attack
                    StartCoroutine(WaitForAttackAnimation());
                    attackCooldownTimer = attackCooldonwTimerReset;
                }
            }
        }
        else
        {
            // Có thể thêm logic boss trở về vị trí ban đầu hoặc dừng di chuyển
            isRunning = false;
        }

        // Giảm cooldown theo thời gian
        if (attackCooldownTimer > 0)
            attackCooldownTimer -= Time.deltaTime;

        
    }

    /// <summary>
    /// Quay đầu về phía player
    /// </summary>
    private void FacePlayer()
    {
        if (player.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            IsFacingLeft = false;
        }
        else if (player.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            IsFacingLeft = true;
        }
    }

    /// <summary>
    /// Di chuyển boss theo hướng player
    /// </summary>
    private void HandleMovement()
    {
        float xVelocity = MovementSpeed * 10 * Time.fixedDeltaTime;

        // Nếu boss đang chạy thì có thể nhân tốc độ sprint
        if (IsSprinting)
        {
            xVelocity *= SprintMultiplier;
        }

        // Điều chỉnh hướng di chuyển dựa trên hướng quay mặt
        if (IsFacingLeft)
        {
            xVelocity *= -1;
        }

        float yVelocity = BossRigidBody.linearVelocity.y;
        MovementVector = new Vector2(xVelocity, yVelocity);
        BossRigidBody.linearVelocity = MovementVector;
        UpdateAnimator();
    }

    /// <summary>
    /// Coroutine đợi animation attack và xử lý collider
    /// </summary>
    private IEnumerator WaitForAttackAnimation()
    {
        BossRigidBody.linearVelocity = Vector2.zero;
        isAttacking = true;
        isRunning = false;
        UpdateAnimator();

        // Đợi một frame để đảm bảo trigger được xử lý
        yield return null;

        // Đợi cho đến khi animation attack (ví dụ "cleave") bắt đầu
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("cleave"))
        {
            yield return null;
        }
        // Đợi cho đến khi animation attack đạt một thời điểm thích hợp (ví dụ normalizedTime < 0.1f)
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("cleave") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }

        // Kích hoạt collider để xác định trúng đòn
        if (attackRangeCollider != null)
            attackRangeCollider.enabled = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            yield return null;
        }

        // Sau khi animation cleave gần kết thúc, chờ cho đến khi animator chuyển sang state khác (đang trong transition)
        while (animator.IsInTransition(0))
        {
            yield return null;
        }
        // Gọi coroutine hoàn tất tấn công
        FinishAttack();
    }

    /// <summary>
    /// Sau khi attack xong: tắt collider và reset trạng thái tấn công
    /// </summary>
    public void FinishAttack()
    {
        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
        isAttacking = false;
        Debug.Log("tat collider");
        UpdateAnimator();
        
    }

    /// <summary>
    /// Xử lý tấn công Fireball (nếu cần)
    /// </summary>
    private IEnumerator FireballAttackRoutine()
    {
        while (!isDying)  // Chạy liên tục cho đến khi boss chết
        {
            // Chờ FireBallCooldown giây
            yield return new WaitForSeconds(FireBallCooldown);

            // Thực hiện tấn công Fireball
            HandleFireballAttack();
        }
    }

    private void HandleFireballAttack()
    {
        for (int i = 0; i < FireBallCount; i++)
        {
            Vector3 spawnPos = FireballSpawnPoint.position;
            spawnPos.y += Random.Range(-2f, 2f);
            Instantiate(FireballPrefab, spawnPos, Quaternion.identity);
        }

       
    }

    /// <summary>
    /// Cập nhật trạng thái animation dựa trên trạng thái của boss
    /// </summary>
    private void UpdateAnimator()
    {
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isDying", isDying);
        animator.SetBool("isHit", isHit);
        animator.SetBool("isEnrage", isEnrage);
    }

    /// <summary>
    /// Khi boss nhận dame, chuyển sang trạng thái Berser mode nếu hp dưới 50%
    /// </summary>
    public override void TakeDamage(float damage)
    {
        if (isDying) return;
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();

        if (!isBerserMode && currentHp <= maxHp * 0.5f)
        {

            isBerserMode = true;
            StartCoroutine(StartBerserMode());
        }

        if (currentHp <= 0f)
        {
            StartCoroutine(Die());
        }
        StartCoroutine(StartIsHit());
        
    }
    private IEnumerator StartIsHit()
    {
        isHit = true;
        UpdateAnimator();

        // Chờ cho đến khi animation hit bắt đầu
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("hit"))
        {
            yield return null;
        }

        // Chờ cho đến khi animation hit hoàn toàn kết thúc (normalizedTime đạt 1)
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("hit") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        isHit = false;
        UpdateAnimator();
    }
    private IEnumerator StartBerserMode()
    {
      
        MovementSpeed *= 2f;
        AttackDamage += 0.25f;
        FireBallCooldown -= 2f;
        attackCooldonwTimerReset -= 2.5f;
        FireBallCount += 7;
        circleMagic.SetActive(true);
        isEnrage = true;
        isAttacking = false;
        isRunning = false;
        animator.SetFloat("cleaveSpeed", 2f);
        UpdateAnimator();
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("enrage"))
        {
            yield return null;
        }

        // Chờ cho đến khi animation "enrage" hoàn thành (normalizedTime đạt 1)
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("enrage") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        Debug.Log("kethuc enrage"); 
        // Kết thúc trạng thái enrage
        isEnrage = false;
        UpdateAnimator();
    }



    public float GetAttackDamage()
    {
        return AttackDamage; // trả về dame của boss cho bên gây dame
    }
    private  IEnumerator Die()
    {
        circleMagic.gameObject.SetActive(false);
        isDying = true;
        UpdateAnimator();
        yield return null;


        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("death"))
        {
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("death") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        spaceGate.gameObject.SetActive(true);
        StartCoroutine(FadeOutAndDestroy());
        DisableColliders();
    }
    private IEnumerator FadeOutAndDestroy()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            float fadeDuration = 1f;
            float timeElapsed = 0f;
            Color startColor = spriteRenderer.color;

            while (timeElapsed < fadeDuration)
            {
                timeElapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null;
            }
        }

        Debug.Log(gameObject.name + " đã bị xóa!");
        Destroy(gameObject);
    }
    private void DisableColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

}
