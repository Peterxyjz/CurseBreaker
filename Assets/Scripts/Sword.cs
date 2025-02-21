using UnityEngine;

public class Sword : MonoBehaviour
{
    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        void Start()
        {
            // Tìm Player trong scene theo tag (đảm bảo Player có tag "Player")
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // Theo vị trí Player
            transform.position = playerTransform.position;

            // Theo hướng Player
            float direction = playerTransform.localScale.x;
            transform.localScale = new Vector3(direction, 1, 1);
        }
    }
}
