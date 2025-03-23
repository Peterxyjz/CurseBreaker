using UnityEngine;

public class BossAreaScript : MonoBehaviour
{

    [SerializeField] GameObject ColliderParent;

    private void Awake()
    {
        
    }

    void Start()
    {
        ColliderParent.GetComponent<Collider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            ColliderParent.GetComponent<Collider2D>().enabled = true;
            GameObject.FindGameObjectWithTag("Boss").GetComponent<WormBoss>().enabled = true;
            Destroy(gameObject);
        }
    }

}
