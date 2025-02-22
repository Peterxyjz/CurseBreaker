using UnityEngine;

public class BasicEnemy : Enemy
{
    [SerializeField] private float distance = 5f;
    private Vector3 startPos;
    private bool movingRight = true;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    void Update()
    {
        HandleMovement();
        FlipEnemy();
    }

    protected override void HandleMovement()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(enemyDamage);
        }
    }
}
