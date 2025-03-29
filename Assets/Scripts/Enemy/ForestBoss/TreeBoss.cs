using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class TreeBoss : Enemy
{
    [SerializeField] private float distance = 5f;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject attackNormalPrefabs;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float skillCooldown = 4f;
    public GameObject spaceGate;
    private Vector3 startPos;
    private float speed = 5f;
    private bool movingRight = true;
    private float nextAttackTime;
    private Animator animator;
    private bool isRunning;
    private bool isAttackNormal;
    private bool isAttackSkill;
    public GameObject circleMagic;
    private bool isBreakerTime = false;
    private int loopSkill1 = 5;
    private int loopNormal = 3;
    private float breakerTime = 10f;
    private AudioBoss audioBoss;
    public GameObject artifact;
    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
        circleMagic.SetActive(isBreakerTime);
        animator = GetComponent<Animator>();
        audioBoss = FindAnyObjectByType<AudioBoss>();
        spaceGate.SetActive(false);
        artifact.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        FlipEnemy();
        if (Time.time >= nextAttackTime)
        {
            
            UseSkillTime();
        }

    }
    private IEnumerator AttackBySkillNo1()
    {
        this.enabled = false;
        isRunning = false;
        isAttackSkill = true;
        UpdateAnimator();
        yield return null;

        // Đợi 1 frame để Unity cập nhật animator
        yield return null;

        // Chờ animation "TreeSkill1" bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("TreeSkill1"));

        yield return null;

        // Chờ animation "TreeSkill1" bắt đầu
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("TreeSkill1"));



        int loopCount = 0;
        while (loopCount < loopSkill1)
        {
            // Chờ cho animation kết thúc 1 lần
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            loopCount++;


            // Reset animation để loop lại
            animator.Play("TreeSkill1", 0, 0f);
        }




        Vector3 directionPlayer = (player.transform.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefabs, firePoint.position, Quaternion.identity);

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        enemyBullet.SetMoveDirection(directionPlayer * speed);
        audioBoss.PlaySkillNo1Clip();


        this.enabled = true;
        isAttackSkill = false;
        isRunning = true;
        UpdateAnimator();
    }
    private IEnumerator AttackNormal()
    {
        this.enabled = false;
        isRunning = false;
        isAttackNormal = true;
        UpdateAnimator();

        // Đợi 1 frame để Unity cập nhật animation
        yield return null;

        Vector3 originalPosition = attackPoint.position; // Lưu vị trí gốc
        float attackOffset = 1.5f;

        // Xác định hướng dựa trên transform.localScale.x
        int direction = transform.localScale.x > 0 ? 1 : -1; // Nếu đang nhìn phải → 1, nếu đang nhìn trái → -1
        
        int i = 0;
        while (i < loopNormal)
        {
            i++;
            // Chờ cho animation kết thúc mỗi lần
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            // Tạo attack tại vị trí mới
            GameObject attack = Instantiate(attackNormalPrefabs, attackPoint.position, Quaternion.identity);

            // Dịch chuyển attackPoint theo hướng enemy đang quay mặt
            attackPoint.position += new Vector3(direction * attackOffset, 0, 0);
        }

        // Khôi phục vị trí attackPoint sau khi hoàn tất
        attackPoint.position = originalPosition;
        audioBoss.PlayNormalSkill();
        this.enabled = true;
        isAttackNormal = false;
        isRunning = true;
        UpdateAnimator();
    }




    private IEnumerator StopMovement()
    {
        audioBoss.PlayIdlerClip();
        isRunning = false;
        UpdateAnimator();
        yield return new WaitForSeconds(2f);
      
        isRunning = true;
        UpdateAnimator();
        HandleMovement();
    }
    private void BerserkMode()
    {
        if (!isBreakerTime)
        {
            audioBoss.PlayBreakerClip();
            nextAttackTime -= skillCooldown;
            isBreakerTime = true;
            StartCoroutine(HealHpBoss());
            circleMagic.SetActive(true);

            loopSkill1 -= 3;
            loopNormal += 3;
            enemyMoveSpeed += 2f;
            skillCooldown -= 2.5f;
        }else 
        {

            loopSkill1 += 3;
            loopNormal -= 3;
            enemyMoveSpeed -= 2f;
            skillCooldown += 2.5f;
            isBreakerTime = false;
            circleMagic.SetActive(false);
        }

        
    }

    // Hàm hồi máu, đảm bảo không vượt quá maxHp
    private IEnumerator HealHpBoss()
    {
        while (isBreakerTime) 
        {
            if (currentHp < maxHp)
            {
                currentHp += 2.5f;
                UpdateHpBar();
            }

            yield return new WaitForSeconds(3f);
        }
    }
    //random
    private int[] weightedValues = { 0, 0, 0, 0, 1, 1, 1, 1, 3, 3, 3, 3, 2 };
    private bool isInBreakerTime = false;

    private int GetWeightedRandom()
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
        weightedValues = new int[] { 0, 0, 0, 1, 1, 1, 2 };

        yield return new WaitForSeconds(breakerTime);

        // Khôi phục danh sách sau breaker
        weightedValues = new int[] { 0, 0, 0, 1, 1, 1, 2, 3, 3,3,3,3 };
        isInBreakerTime = false;
    }

    private void RandomAttack()
    {
        FacePlayer();
        //int random = 1;
        int random = GetWeightedRandom();


        switch (random)
        {
            case 0:
                {
                    Debug.Log("danh thuong");
                    StartCoroutine(AttackNormal());
                    
                    break;
                }
            case 1:
                {
                    Debug.Log("skill 1");
                    StartCoroutine(AttackBySkillNo1());
                    
                    break;
                }
            case 2:
                {
                    Debug.Log("dung yen");
                    StartCoroutine(StopMovement());
                    break;
                }
            case 3:
                {
                    Debug.Log("bkreaer");
                    BerserkMode();
                    break;
                }
           

        }
    }
    private void UseSkillTime()
    {
        nextAttackTime = Time.time + skillCooldown ;
        RandomAttack();
    }
    private void FacePlayer()
    {
        if (player == null) return;

        float direction = player.transform.position.x - transform.position.x;

        if (direction > 0)
        {
            // Player ở bên phải -> Quay sang phải
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (direction < 0)
        {
            // Player ở bên trái -> Quay sang trái
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enemyDamage);
        }
    }
    protected override void HandleMovement()
    {
        if (!isRunning) return;
        isRunning = true;
        UpdateAnimator();
        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;

        if (movingRight)
        {
            transform.Translate(Vector2.right * enemyMoveSpeed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false; // Đổi hướng khi chạm biên phải
            }
        }
        else
        {
            transform.Translate(Vector2.left * enemyMoveSpeed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true; // Đổi hướng khi chạm biên trái
            }
        }
    }
    protected override void FlipEnemy()
    {
        transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);
    }
    protected override void Die()
    {
        base.Die();
        audioBoss.PlayDieClip();
        spaceGate.SetActive(true);
        artifact.gameObject.SetActive(true);
    }
    private void UpdateAnimator()
    {
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isAttackNormal", isAttackNormal);
        animator.SetBool("isAttackSkill", isAttackSkill);
    }
}
