using UnityEngine;

public class WormBoss : Enemy
{
    const string ANIMATION_BOOL_WALKING = "isWalking";
    const string ANIMATION_TRIGGER_ATTACKING = "bossAttack";

    public float moveSpeed = 2f;   // Tốc độ di chuyển của boss
    public Transform attackPosition; // Vị trí boss sẽ đến trước khi tấn công
    public float attackRange = 2f; // Phạm vi tấn công người chơi
    public GameObject? weaponDrop;

    private Animator animator;
    private bool hasReachedPosition = false; 
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isStarting = false;
    private bool isPlayerInAttackRange = false;
    //private float distanceToPlayer = 0f;

    protected override void Start()
    {
        base.Start();
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        FlipTowardsPlayer();
        if (isDead) return;

        CheckPlayerDistance();

        if (isPlayerInAttackRange && !isAttacking)
        {
            AttackPlayer();
        }
        else
        {
            HandleMovement();

        }

    }

    private void CheckPlayerDistance()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > attackRange)
        {
            isPlayerInAttackRange = false;

        }
        else
        {
            isPlayerInAttackRange = true;

        }
    }

    protected override void HandleMovement()
    {
        var distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log(distanceToPlayer);
        if (distanceToPlayer > attackRange - 0.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);

            animator.SetBool(ANIMATION_BOOL_WALKING, true);

        }
        else
        {
            animator.SetBool(ANIMATION_BOOL_WALKING, false);
        }
    }

    void AttackPlayer()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange)
        {
            isAttacking = true;
            animator.SetTrigger(ANIMATION_TRIGGER_ATTACKING);

            // Gây sát thương lên người chơi (giả sử PlayerController có phương thức TakeDamage)
            player.GetComponent<PlayerController>().TakeDamage(1f);

            Invoke(nameof(EndAttack), 1.5f);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }



    void Die()
    {
        isDead = true;
        animator.SetTrigger("deathBoss");

        // Sau khi boss chết, rơi ra vũ khí cho người chơi
        if (weaponDrop != null)
        {
            Instantiate(weaponDrop, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 2f); // Xóa boss sau 2 giây
    }
    void FlipTowardsPlayer()
    {
        if (player == null) return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Nếu người chơi ở bên trái boss, lật hình theo trục X
        if (player.transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

}

