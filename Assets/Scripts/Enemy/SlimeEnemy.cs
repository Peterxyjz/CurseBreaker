using UnityEngine;
using System.Collections;
public class SlimeEnemy : Enemy
{
    [SerializeField] private float distance = 5f;
    private Vector3 startPos;
    private bool movingRight = false;
    private bool isDie = false;
    private Animator animator;
    private AudioForest audioForest;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        audioForest = FindAnyObjectByType<AudioForest>();
    }

    // Update is called once per frame
    void Update()
    {

        HandleMovement();
        FlipEnemy(); 
    }
    protected override void HandleMovement()
    {
        if (isDie) return;
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
    public void PlayBoomb()
    {
        audioForest.PlaySlimeDie();
    }
    protected override void FlipEnemy()
    {
        transform.localScale = new Vector3(movingRight ? -1 : 1, 1, 1);
    }
    protected override void Die()
    {
        if (isDie) return;
        isDie = true;
        enemyMoveSpeed = 0;
        animator.SetBool("isDie", isDie);

        // Chờ animation kết thúc rồi mới phá hủy
        StartCoroutine(WaitForDeathAnimation());
    }
    private IEnumerator WaitForDeathAnimation()
    {
        // Chờ đến khi animation Die kết thúc
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // để lấy độ dài animation hiện tại và chờ trước khi Destroy(gameObject)

        Debug.Log(gameObject.name + " đã bị xóa!");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enemyDamage);
        }
    }
}
