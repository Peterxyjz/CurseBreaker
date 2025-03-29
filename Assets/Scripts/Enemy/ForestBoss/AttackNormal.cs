using UnityEngine;
using System.Collections;
public class AttackNormal : MonoBehaviour
{
    private float attackDamage = 1f;
    private Animator animator;
    private bool hasAttacked = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasAttacked && collision.CompareTag("Player")) // Đảm bảo chỉ tấn công 1 lần
        {
            hasAttacked = true;
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }

    // Gọi hàm này khi animation kết thúc
    public void SetDestroyAfterAnimation()
    {
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
