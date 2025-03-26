using UnityEngine;

public class FireballScript : MonoBehaviour
{
    CircleCollider2D FireballCollider;
    Rigidbody2D FireballRigid2D;

    Vector3 FireballMovementVector;
    float FireballSpeed = 2.5f;
    private float FireballLivingTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        FireballCollider = GetComponent<CircleCollider2D>();
        FireballRigid2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        var playerObject = GameObject.FindWithTag("Player");

        Vector3 FireballMovementVector = playerObject.transform.position - transform.position;
        FireballRigid2D.linearVelocity = new Vector2(FireballMovementVector.x, FireballMovementVector.y).normalized * FireballSpeed;

        float rot = Mathf.Atan2(-FireballMovementVector.y, -FireballMovementVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot-90);

        FireballRigid2D.linearVelocity = FireballMovementVector;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        FireballLivingTime -= Time.fixedDeltaTime;

        if (FireballLivingTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
