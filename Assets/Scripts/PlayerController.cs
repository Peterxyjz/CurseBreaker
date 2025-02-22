using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float maxHp = 10f;
    [SerializeField] private float hpBonus = 1f;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private float attackCooldown = 0.5f; // Thời gian hồi chiêu

    private float currentHp;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float lastAttackTime;
    private bool isAttacking = false;
    private GameManager gameManager;

    private Sword sword; // Tham chiếu đến kiếm
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
        gameManager = FindAnyObjectByType<GameManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHp += hpBonus;

        currentHp = maxHp;
        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAttack();
        UpdateAnimation();

    }

    private void HandleMovement()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero; // Giữ nhân vật đứng yên khi tấn công
            return;
        }
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
       
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleAttack()
    {
        if (!isGrounded) return;
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Skill 3");

            isAttacking = true;
            animator.SetBool("isAttacking", isAttacking);
            StartCoroutine(ResetAttack(0.5f));
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Skill 1");

            isAttacking = true;
            animator.SetBool("isAttacking", isAttacking);
            StartCoroutine(ResetAttack(0.5f));

        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("Skill 2");

            isAttacking = true;
            animator.SetBool("isAttacking", isAttacking);
            StartCoroutine(ResetAttack(0.5f));
        }
        else if (Input.GetKeyDown(KeyCode.J) && Time.time > lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isAttacking", isAttacking);

            if (sword != null)
            {
                sword.PerformAttack(damage);
                StartCoroutine(ResetAttack(0.5f));
            }
        }


    }

    private IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time); // Thời gian hoạt ảnh tấn công (có thể thay đổi)
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        bool isDie = currentHp <= 0.1f ? true : false;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isDie", isDie);
    }

    public void HealHp(float amount)
    {
        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp); // Giới hạn không vượt quá maxHp
        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        DisableColliders();
        FadeOutAndDestroy();
        Invoke(nameof(CallGameOver), 2f);
    }
    private void CallGameOver()
    {
        gameManager.GameOver();
    }
    private void UpdateHpBar()
    {
        if(hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp;
        }
    }
    protected IEnumerator FadeOutAndDestroy()
    {
        // Lấy SpriteRenderer của đối tượng
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Kiểm tra nếu có SpriteRenderer
        if (spriteRenderer != null)
        {
            // Đặt thời gian fade (2 giây)
            float fadeDuration = 2f;
            float timeElapsed = 0f;

            // Lấy màu hiện tại của Sprite
            Color startColor = spriteRenderer.color;

            // Mờ dần trong 2 giây
            while (timeElapsed < fadeDuration)
            {
                timeElapsed += Time.deltaTime;

                // Tính toán giá trị alpha dựa trên thời gian
                float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null; // Đợi 1 frame
            }
        }

        // Sau khi mờ dần, hủy đối tượng
        Destroy(gameObject);
    }
    protected void DisableColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
