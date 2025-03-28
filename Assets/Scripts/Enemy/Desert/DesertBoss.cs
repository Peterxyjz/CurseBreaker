using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class DesertBoss : Enemy
{
    [Header("Boss Movement")]
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float patrolDistance = 3f;     // Khoảng cách di chuyển trái-phải
    [SerializeField] private float moveSpeed = 4f;         // Tốc độ di chuyển
    [SerializeField] private float jumpForce = 4f;         // Lực nhảy
    
    [Header("Boss Random Action")]
    private float actionCooldown = 4f;    // Thời gian giữa mỗi lần random hành động
    private float nextActionTime = 0f;

    [Header("Components")]
    private Animator animator;
    private Rigidbody2D rb;
    private Vector3 startPos;
    private bool movingRight = true;
    public GameObject circleMagic;
    public GameObject spaceGate;
    private AudioMonster audio;
    public GameObject artifact;

    [SerializeField] private GameObject normalAttackPrefab;
    [SerializeField] private GameObject exploreAttackPrefab;
    [SerializeField] private Transform firePoint; // Vị trí spawn đạn hoặc skill
    [SerializeField] private SkeletonController skeletonPrefab;
    [SerializeField] private Transform skeletonPoint; 
    // Các trạng thái
    private bool isIdle;
    private bool isRunning;
    private bool isJumping;
    private bool isFalling;
    private bool isAttacking;
    private bool isBreakerTime = false;
    private int loopAttack= 1;
    private float breakerTime = 10f;
    float direcJump = 1f;
    int flaltJump = 0;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    // Mảng tỉ lệ random (mỗi số đại diện cho 1 hành động)
    // 0 = Idle, 1 = Run, 2 = Jump, 3 = Attack
    private int[] weightedValues = { 0, 0, 1, 1, 1, 1, 3, 3, 3, 3, 2, 2 };
    private bool isInBreakerTime = false;
    // Tuỳ ý thay đổi độ lặp của từng hành động để tăng/giảm tần suất

    protected override void Start()
    {
        base.Start();
        spaceGate.SetActive(false);
        startPos = transform.position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        circleMagic.SetActive(isBreakerTime);
        audio = FindAnyObjectByType<AudioMonster>();
        artifact.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDying) return;  // Nếu boss chết thì bỏ qua

        // Cứ đến thời gian thì random hành động mới
        if (Time.time >= nextActionTime)
        {
            nextActionTime = Time.time + actionCooldown;
            RandomAction();
        }

        // Kiểm tra nếu đang chạy thì di chuyển tuần tra
        if (isRunning)
        {
            HandleMovement();
        }

     
       
    }

    /// <summary>
    /// Lấy 1 hành động ngẫu nhiên (có trọng số) từ mảng weightedActions
    /// </summary>
    private int GetRandomAction()
    {

        if (weightedValues.Length == 0) return 0; // Đề phòng mảng bị rỗng

        int index = Random.Range(0, weightedValues.Length); // Luôn chọn index hợp lệ
        int randomValue = weightedValues[index];

        if (randomValue == 3 && !isInBreakerTime)
        {
            StartCoroutine(MultipleAttacking());
        }

        return randomValue;
    }
    private IEnumerator MultipleAttacking()
    {
        isInBreakerTime = true;

        // Loại bỏ số 3 bằng danh sách mới
        weightedValues = new int[] { 0,  1, 1, 1,1,1,1, 2, 2, 2 };

        yield return new WaitForSeconds(breakerTime);

        // Khôi phục danh sách sau breaker
        weightedValues = new int[] { 0, 0, 1, 1, 1, 2,2, 3, 3, 3, 3, 3 };
        isInBreakerTime = false;
    }

    /// <summary>
    /// Random hành động: Idle, Run, Jump, Attack
    /// </summary>
    private void RandomAction()
    {
        // Nếu đang ở giữa một coroutine hành động (VD: Attack) thì thôi
        // (Tuỳ ý bạn, nếu muốn boss có thể ngắt hành động thì bỏ đoạn check này)
        if (isAttacking || isJumping || isDying) return;

        int action = GetRandomAction();
        //int action = 2;
        switch (action)
        {
            case 0:
                { // Run

                    Debug.Log("Run");
                    StartCoroutine(RunCoroutine());
                    break;
                }
            case 1:
                { // Attack
                    Debug.Log("Attack");
                    StartCoroutine(AttackCoroutine());
                    break;
                }
               
                
            case 2:
                {
                    Debug.Log("Jump");
                    StartCoroutine(JumpCoroutine());
                    break;
                }
              
            case 3:
                {
                    // Berser
                    Debug.Log("berser");
                    BerserkMode();
                    break;
                }
        }
    }

    // -------------------- CÁC COROUTINE HÀNH ĐỘNG -------------------- //
    private IEnumerator IdleCoroutine()
    {

        isRunning = false;
        isJumping = false;
        isFalling = false;
        isAttacking = false;
        UpdateAnimator();

        // Đứng yên 1-2 giây (tuỳ ý)
        yield return new WaitForSeconds(Random.Range(1f, 2f));

      
        UpdateAnimator();
    }

    private IEnumerator RunCoroutine()
    {
        isRunning = true;
      
        isJumping = false;
        isFalling = false;
        isAttacking = false;
        UpdateAnimator();

       



        // Chạy khoảng 2-3 giây (tuỳ ý)
        yield return new WaitForSeconds(Random.Range(2f, 3f));

        isRunning = false;
        UpdateAnimator();
    }

    private IEnumerator JumpCoroutine()
    {
        // Bắt đầu nhảy: đặt trạng thái và cập nhật animator
        isJumping = true;
        isRunning = false;
        isFalling = false;
        isAttacking = false;
        UpdateAnimator();

        // Thực hiện nhảy: tăng thêm vận tốc x (3f) và đặt vận tốc y bằng jumpForce
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + 7f, jumpForce);
        
        // Cho một khoảng delay ngắn để animation nhảy bắt đầu
        yield return new WaitForSeconds(0.2f);

        // Tính toán vị trí mục tiêu: ví dụ, dịch chuyển thêm 2 đơn vị theo trục x (bạn có thể điều chỉnh vector này)
        
        if(flaltJump >= 1)
        {
            direcJump *= -1f;
            flaltJump = 0;
        }
        flaltJump += 1;
        Vector3 targetPosition = transform.position + new Vector3(7f, 2f, 0f) * direcJump; 
        
       
          

        
        float moveSpeed = 3f;   // tốc độ di chuyển tới target khi đang bay
        float jumpDuration = 0.7f; // thời gian nhảy trong không trung là 1 giây
        float elapsed = 0f;

        // Di chuyển từ vị trí hiện tại tới targetPosition trong vòng 1 giây
        while (elapsed < jumpDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 4 * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
       
    

        isFalling = true; ;
        isJumping = false;
        UpdateAnimator();
        while (!IsGrounded())
        {
            
            yield return null; // đợi cho đến khi chạm đất
        }
        isAttacking = false;
        isJumping = false;
        isFalling = false;
        UpdateAnimator();
        StartCoroutine(AttackCoroutine());
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private IEnumerator AttackCoroutine()
    {
        FaceTarget();
        isAttacking = true;
      
        isRunning = false;
        isJumping = false;
        isFalling = false;
        UpdateAnimator();

        int count = 0;

       
        while (count < loopAttack)
        {
            count++;
            animator.Play("DesertBossAttack");
            yield return null;

            // Chờ animation "DesertBossAttack" bắt đầu
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("DesertBossAttack"));
            audio.PlayerBossAttack();
            // Chờ animation "DesertBossAttack" bắt đầu
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            int randomAttack = Random.Range(0, 2);
            Debug.Log(randomAttack);
            if (isBreakerTime)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (randomAttack == 0)
            {
                NormalAttack();
            }
            else
            {
                ExploreAttack();
            }
        }


        // Xử lý damage, spawn hitbox, ...
        Debug.Log("DesertBoss: Attack!");

        // Kết thúc
        isAttacking = false;
        UpdateAnimator();
    }

    private void NormalAttack()
    {
       
        

        Vector3 directionPlayer = (player.transform.position - firePoint.position).normalized;
        // Tạo đạn Normal
        GameObject bullet = Instantiate(normalAttackPrefab, firePoint.position, Quaternion.identity);
        
        
        NormalAttack bulletAttack = bullet.GetComponent<NormalAttack>();
        bulletAttack.SetMoveDirection(directionPlayer * 15f);
    }

    private void ExploreAttack()
    {
        Vector3 directionPlayer = (player.transform.position - firePoint.position).normalized;
        // Tạo đạn Explore
        GameObject bullet = Instantiate(exploreAttackPrefab, firePoint.position, Quaternion.identity);


        ExploreAttack bulletAttack = bullet.GetComponent<ExploreAttack>();
        bulletAttack.SetMoveDirection(directionPlayer * 15f);
    }

    private void BerserkMode()
    {
        Debug.Log("Breaker");
        if (!isBreakerTime)
        {
          
            nextActionTime -= actionCooldown;
            isBreakerTime = true;
            loopAttack += 2;
            circleMagic.SetActive(true);
            StartCoroutine(HealHpBoss());
            StartCoroutine(CallSkeleton());

            enemyMoveSpeed += 2f;
            actionCooldown -= 2.5f;
        }
        else
        {
            loopAttack -= 2;
            enemyMoveSpeed -= 2f;
            actionCooldown += 2.5f;
            isBreakerTime = false;
            circleMagic.SetActive(false);
        }


    }
    private IEnumerator HealHpBoss()
    {
        while (isBreakerTime)
        {
            if (currentHp < maxHp)
            {
                currentHp += 1.5f;
                UpdateHpBar();
            }

            yield return new WaitForSeconds(3f);
        }
    }
    // -------------------- HANDLE MOVEMENT & PATROL -------------------- //
    protected override void HandleMovement()
    {
        // Patrol quanh điểm startPos ± patrolDistance
        float leftBound = startPos.x - maxDistance;
        float rightBound = startPos.x + maxDistance;

        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

            if (transform.position.x >= rightBound)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

            if (transform.position.x <= leftBound)
            {
                movingRight = true;
            }
        }

        // Lật hướng theo di chuyển
        FlipBoss();
    }

    protected void FlipBoss()
    {
        if (movingRight)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    // -------------------- OVERRIDE DIE (từ Enemy) -------------------- //
    protected override void Die()
    {
        
        isDying = true;
        // Tắt tất cả các trạng thái khác
    
        isRunning = false;
        isJumping = false;
        isFalling = false;
        isAttacking = false;
        isBreakerTime = false;
        circleMagic.SetActive(false);
        UpdateAnimator();
        spaceGate.SetActive(true);
        artifact.gameObject.SetActive(true);
        StartCoroutine(DoAnimationDie());
        // Thêm logic rớt đồ, mở cổng, v.v.
    }
    private IEnumerator DoAnimationDie()
    {
        yield return null;

        // Chờ animation "DesertBossAttack" bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("DesertBossDie"));

       

        // Chờ animation "DesertBossAttack" bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        
        FadeOutAndDestroy();
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
    // -------------------- ANIMATOR -------------------- //
    private void UpdateAnimator()
    {
    
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDying", isDying);
    }

    // -------------------- DAMAGE PLAYER -------------------- //
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enemyDamage);
        }
    }

    private void FaceTarget()
    {
        Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
        if (directionToPlayer.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (directionToPlayer.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public void HealHpFromAttack()
    {
        if (currentHp < maxHp)
        {
            currentHp += 0.65f;
            UpdateHpBar();
        }
    }
    private IEnumerator CallSkeleton()
    {
        
        while(isBreakerTime)
        {
           
            SkeletonController skeletonIns = Instantiate(skeletonPrefab, skeletonPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }
       
    }
}
