using UnityEngine;
using System.Collections;
public class WormBoss : Enemy
{
    private float detectionRange = 15f;
    private float attackRange = 2f;
    private float chaseSpeed = 2f;
    [SerializeField] private Collider2D attackRangeCollider;
    private bool isChasing = false;

    private Animator animator;
    private bool isRunning = false;
    private bool isAttacking = false;
    private bool isDie = false;
    public GameObject gift;
    public NPCVolcano npc;
    protected override void Start()
    {
        base.Start();
        npc.gameObject.SetActive(false);
        animator = GetComponent<Animator>();

        gift.SetActive(false);

        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
    }

    private void Update()
    {
        if (player == null || isDie) return;

        if (isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        isChasing = (distanceToPlayer <= detectionRange);

        if (isChasing)
        {
            isRunning = true;
            MoveTowardsPlayer();
            setAnimator();

            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        isRunning = true;
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            chaseSpeed * Time.deltaTime
        );
        FlipEnemy(direction);
    }

    private void AttackPlayer()
    {
        if (!isAttacking)
        {
            isRunning = false;
            isAttacking = true;
            setAnimator();
            if (attackRangeCollider != null)
                attackRangeCollider.enabled = true;
            StartCoroutine(WaitForAttackAnimation());
        }
    }

    private IEnumerator WaitForAttackAnimation()
    {
        yield return null;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("attackBoss"))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("attackBoss") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        if (attackRangeCollider != null)
            attackRangeCollider.enabled = false;
        isAttacking = false;
        setAnimator();
    }

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

    private void setAnimator()
    {
        
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isDie", isDie);
    }

    protected override void Die()
    {
        if (isDie) return;
        isDie = true;
        
        

        this.enabled = false;
        StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        animator.SetBool("isDie", true);
       
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("deathBoss"))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("deathBoss") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        gift.SetActive(true);
        npc.gameObject.SetActive(true);
        Destroy(gameObject);
    }

    public float GetAttackDamage()
    {
        return enemyDamage;
    }

}

