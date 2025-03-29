using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 5f;
     private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
     private float damage = 1f;
    [SerializeField] private float maxHp = 5f;
    [SerializeField] private float hpBonus = 0f;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private float attackCooldown = 1f; // Thời gian hồi chiêu


    private AudioManager audioManager;
    private float currentHp;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private float lastAttackTime;
    private bool isAttacking = false;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private bool isInvincible = false;
    private bool isStopping = false;
    private bool isShield = false;

    private Dictionary<string, bool> unlockedSkills = new Dictionary<string, bool>
{
    { "Skill0", false},
    { "Skill1", false },
    { "Skill2", false },
    { "Skill3", false },
};
    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>
{
    {"Skill0", 0f},
    {"Skill1", 0f},
    {"Skill2", 0f},
    {"Skill3", 0f}
};
     private float skill0Cooldown = 5f;
     private float skill1Cooldown = 5f;
     private float skill2Cooldown = 9f;
     private float skill3Cooldown = 5f;

    [SerializeField] private CooldownSkill skill0UI;
    [SerializeField] private CooldownSkill skill1UI;
    [SerializeField] private CooldownSkill skill2UI;
    [SerializeField] private CooldownSkill skill3UI;
    private Dictionary<string, CooldownSkill> skillUIMap;

    private Sword sword; // Tham chiếu đến kiếm
    public GameObject shieldEffect;
    [SerializeField] private Transform positionAttackSkill1;
    [SerializeField] private GameObject skill1EffectPrefab;
    public GameObject skill2Effect;
    public float skill2Duration = 3f;
    public Skill3 skill3;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
        gameManager = FindAnyObjectByType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = FindAnyObjectByType<AudioManager>();
        skill2Effect.SetActive(false);

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPlayerData();
        shieldEffect.SetActive(isShield);
        skill2Effect.SetActive(false);
        UnlockIconSkill();
        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
        skillUIMap = new Dictionary<string, CooldownSkill>()
    {
        { "Skill0", skill0UI },
        { "Skill1", skill1UI },
        { "Skill2", skill2UI },
        { "Skill3", skill3UI }
    };
    }
    public void LoadPlayerData()
    {
        PlayerData data = SaveSystem.LoadPlayerData();
        if (data != null)
        {
            maxHp = data.maxHp;
            currentHp = data.currentHp == 0f ? 5f : data.currentHp;
            damage = data.damage;

            // 🔄 Chuyển từ List<SkillData> -> Dictionary<string, bool>
            unlockedSkills = SaveSystem.ConvertListToDictionary(data.unlockedSkills);
            foreach (var skill in unlockedSkills)
            {
                Debug.Log($"🔍 Kỹ năng: {skill.Key}, Đã mở khóa: {skill.Value}");
            }
        }
        else
        {
            currentHp = maxHp;
        }
    }


    private void OnApplicationQuit()
    {
        SaveSystem.SavePlayerData(this); // Tự động lưu khi thoát game
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
        if (isAttacking || isStopping)
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
    public void ResetVelocity()
    {
        animator.SetBool("isRunning", false);
        rb.linearVelocity = Vector2.zero;

    }
    public void SetMovement(bool status)
    {
        isStopping = status;
    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            audioManager.PlayPlayerJump();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleAttack()
    {
        if (!isGrounded) return;

        float currentTime = Time.time;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            if (unlockedSkills["Skill3"] && currentTime >= cooldownTimers["Skill3"])
            {
                Debug.Log("Skill 3");

                isAttacking = true;
                animator.SetBool("isAttacking", isAttacking);
                skill3.ActivateSkill(damage);
                cooldownTimers["Skill3"] = currentTime + skill3Cooldown; // Đặt thời gian hồi chiêu
                skill3UI.UseSkill(skill3Cooldown);
                StartCoroutine(ResetAttack(0.5f));
            }
        }
        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            if (unlockedSkills["Skill2"] && currentTime >= cooldownTimers["Skill2"])
            {
                Debug.Log("Skill 2");

                
               
                cooldownTimers["Skill2"] = currentTime + skill2Cooldown; // Đặt thời gian hồi chiêu
                UseSkill2();
                skill2UI.UseSkill(skill2Cooldown);
                StartCoroutine(ResetAttack(0.5f));
            }
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.J))
        {
            if (unlockedSkills["Skill1"] && currentTime >= cooldownTimers["Skill1"])
            {
                Debug.Log("Skill 1");

                isAttacking = true;
                animator.SetBool("isAttacking", isAttacking);
                cooldownTimers["Skill1"] = currentTime + skill1Cooldown; // Đặt thời gian hồi chiêu
                skill1UI.UseSkill(skill1Cooldown);

                GameObject skill1Ins = Instantiate(skill1EffectPrefab, positionAttackSkill1.position, Quaternion.identity);
                Skill1 skill1 = skill1Ins.GetComponent<Skill1>();
                skill1.GetDamage(damage);
                bool isFacingRight = transform.localScale.x > 0;
                skill1.SetForwardDistance(7f, isFacingRight);
                StartCoroutine(ResetAttack(0.5f));
            }
        }
        else if (Input.GetKeyDown(KeyCode.K)) // Thay đổi phím kích hoạt nếu cần
        {
            Debug.Log($"dang dung skill 0: {unlockedSkills["Skill0"]} : {cooldownTimers["Skill0"]} / {currentTime}");
            if (unlockedSkills["Skill0"] && currentTime >= cooldownTimers["Skill0"])
            {
               
                // bật khiên bảo vệ 
                isShield = true;
                shieldEffect.SetActive(isShield);
                // reset time
                cooldownTimers["Skill0"] = currentTime + skill0Cooldown;
                //sau 3s thì khiên bảo vệ mất tác dụng
                skill0UI.UseSkill(skill0Cooldown);
                StartCoroutine(DisableShieldAfterDelay(1.5f));

            }
        }
        else if (Input.GetKeyDown(KeyCode.J) && currentTime > lastAttackTime + attackCooldown)
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

    private void PlayAudioAttack()
    {
        audioManager.PlaySwordAttack();
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
    public void IncreaseHp(float amount)
    {
        hpBonus += amount;
        maxHp += amount;
        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp); // Tăng currentHp nhưng không vượt quá maxHp

        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
    }
    public void TakeDamage(float damage)
    {
        if (isInvincible || isShield) return; // Nếu đang miễn nhiễm, bỏ qua sát thương
        
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        textHp.text = currentHp.ToString() + "/" + maxHp.ToString();
        UpdateHpBar();
        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrame()); // Kích hoạt miễn nhiễm và hiệu ứng nhấp nháy
        }
    }
    private IEnumerator InvincibilityFrame()
    {
        isInvincible = true;

        // Chớp đỏ trong 0.5s
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;

        // Nhấp nháy liên tục trong 1.5s
        float blinkDuration = 1.5f;
        float blinkInterval = 0.1f; // Mỗi lần nhấp nháy mất 0.1s
        for (float i = 0; i < blinkDuration; i += blinkInterval)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled; // Bật/tắt sprite để nhấp nháy
            yield return new WaitForSeconds(blinkInterval);
        }
        spriteRenderer.enabled = true; // Đảm bảo sprite được bật lại

        isInvincible = false;
    }
    private void Die()
    {

        //DisableColliders();
        DisableMovement(5f);
        FadeOutAndDestroy();
        CallGameOver();

    }
    private void CallGameOver()
    {
        gameManager.GameOver();
        Debug.Log("done call Game OVer");
    }
    private void UpdateHpBar()
    {
        if (hpBar != null)
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
    public float GetMaxHp()
    {
        return maxHp;
    }
    public float GetDamagePlayer()
    {
        return damage;
    }
    public float GetCurrentHp()
    {
        return currentHp;
    }
    public Dictionary<string, bool> GetAllSkill()
    {
        return unlockedSkills;
    }
    public void UnlockSkill(string skillName)
    {
        if (unlockedSkills.ContainsKey(skillName))
        {
            unlockedSkills[skillName] = true;
            Debug.Log($"{skillName} has been unlocked!");
            if (skillUIMap.ContainsKey(skillName))
            {
                skillUIMap[skillName].setStatusSkill(true);
            }
            else
            {
                Debug.LogWarning($"No UI mapped for skill: {skillName}");
            }
        }
    }
    public Dictionary<string, bool> GetUnlockedSkills()
    {
        return new Dictionary<string, bool>(unlockedSkills); 
    }

    private IEnumerator DisableShieldAfterDelay(float time)
    {
       
        yield return new WaitForSeconds(time);
        isShield = false;
        shieldEffect.SetActive(isShield);

    }
    public IEnumerator DisableMovement(float time)
    {
        // khóa di chuyển và mở lại sau time
        this.enabled = false;
        isStopping = true;
        rb.linearVelocity = Vector2.zero; // reset van toc
        Debug.Log(" khoa di chuyen");
        yield return new WaitForSeconds(time);
        isStopping = false;
        this.enabled = true;
        Debug.Log("mo khoa di chuyen");

    }
    private void UnlockIconSkill()
    {
        // Ẩn icon nếu skill chưa mở khóa
        if (!unlockedSkills["Skill0"] && skill0UI != null)
            skill0UI.setStatusSkill(false);

        if (!unlockedSkills["Skill1"] && skill1UI != null)
            skill1UI.setStatusSkill(false);

        if (!unlockedSkills["Skill2"] && skill2UI != null)
            skill2UI.setStatusSkill(false);

        if (!unlockedSkills["Skill3"] && skill3UI != null)
            skill3UI.setStatusSkill(false);
    }
    public void GoToGroundDie()
    {
        Die();
    }

    public void UseSkill2()
    {
        StartCoroutine(Skill2Coroutine());
    }

    private IEnumerator Skill2Coroutine()
    {
        skill2Effect.SetActive(true);
        float oriDamage = damage;
        float oriSpeed = moveSpeed;
        float oriJump = jumpForce;
        HealHp(1f);
        damage = oriDamage * 2f;
     
        moveSpeed = oriSpeed + 2f;
      
        jumpForce = oriJump * 1.25f;
        yield return new WaitForSeconds(skill2Duration);
        damage = oriDamage;
        moveSpeed = oriSpeed;
        jumpForce = oriJump;
        skill2Effect.SetActive(false);
    }
}
