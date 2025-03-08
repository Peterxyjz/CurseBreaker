using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform PosA;
    [SerializeField] private Transform PosB;
    [SerializeField] private float movingSpeed = 2f;
    private Vector3 target;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = PosA.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, movingSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.position, target) < 0.1f)
            if(target == PosA.position)
            {
                target = PosB.position;
            }
            else
            {
                target = PosA.position;
            }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

}
