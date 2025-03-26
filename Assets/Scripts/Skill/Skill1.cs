using UnityEngine;

public class Skill1 : MonoBehaviour
{

    private float speed = 10f;
    private Vector3 target;
    private bool hasTarget = false;
    private float damage = 0f;
    public void SetForwardDistance(float distance, bool isFacingRight)
    {
        Vector3 direction = isFacingRight ? Vector3.right : Vector3.left;
        target = transform.position + direction * distance;
        hasTarget = true;
        transform.localScale = new Vector3(isFacingRight ? -1 : 1, 1, 1);
    }

    private void Update()
    {
        if (!hasTarget) return;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("va cham skill 1: "+ collision.tag);
        if (collision.CompareTag("Enemy"))
        {
            // Lấy script Enemy từ collider
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            
        }
    }
    public void GetDamage(float playerDmg)
    {
        damage = playerDmg * 2.5f;
    }
}
