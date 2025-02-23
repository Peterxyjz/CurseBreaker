using UnityEditor.Tilemaps;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed = 1f;
    [SerializeField] protected float enemyDamage = 1f;
    [SerializeField] protected float maxHp = 5f; // Thêm HP tối đa
    [SerializeField] protected Image hpBar; 
    protected float currentHp;
    protected PlayerController player;
    private bool isDying = false; // Tránh gọi Die() nhiều lần
    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        currentHp = maxHp;
        UpdateHpBar();
    }
    public virtual void TakeDamage(float damage)
    {
        if (isDying) return;
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0)
        {                  
            Die();
        }
        
    }
    protected void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp/maxHp;
        }
    }
    protected virtual void Die()
    {
        if (isDying) return;
        isDying = true;
        Debug.Log(gameObject.name + " đang chết...");

        // 1. Dừng di chuyển
        enabled = false; // Vô hiệu hóa script điều khiển

        DisableColliders();

        StartCoroutine(FadeOutAndDestroy());

    }
    protected virtual void HandleMovement()
    {

    }
    protected virtual void FlipEnemy()
    {
        
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
