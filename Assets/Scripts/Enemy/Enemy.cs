using UnityEditor.Tilemaps;
using UnityEngine;
using System.Collections;
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed = 1f;
    [SerializeField] protected float enemyDamage = 1f;
    protected PlayerController player; 
    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    public virtual void TakeDamage(float damage)
    {
        Die();
    }
    protected virtual void Die()
    {
        DisableColliders();
        FadeOutAndDestroy();
    }
    protected virtual void HandleMovement()
    {

    }
    protected virtual void FlipEnemy()
    {
        
    }
    private IEnumerator FadeOutAndDestroy()
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
    private void DisableColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
