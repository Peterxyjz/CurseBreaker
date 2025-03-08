using UnityEngine;

public class Torch : MonoBehaviour
{
    public GameObject fireEffect; // Ngọn lửa của đuốc
    private bool isFire = false;
    void Start()
    {
        // Ẩn ngọn lửa khi game bắt đầu
        if (fireEffect != null)
        {
            fireEffect.SetActive(false);
        }
    }
    public bool GetStatusFire()
    {
        return isFire;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Đúng tên Tag "Player"
        {
            Debug.Log("Player chạm vào đuốc!");
            if (fireEffect != null)
            {
                fireEffect.SetActive(true); // Hiện ngọn lửa
                isFire = true;
            }
        }
    }
}
