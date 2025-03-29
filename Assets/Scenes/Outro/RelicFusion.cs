using UnityEngine;
using System.Collections;

public class RelicFusion : MonoBehaviour
{
    [Header("Relics")]
    public GameObject relic1;
    public GameObject relic2;
    public GameObject relic3;

    [Header("Final Item")]
    public GameObject finalItem;  // Thuốc giải hoặc vật phẩm cuối

    [Header("Fusion Settings")]
    public Vector3 fusionPosition = new Vector3(0, 0, 0); // Vị trí hợp nhất
    public float moveDuration = 2f;  // Thời gian di chuyển relic đến vị trí

    [Header("Glow Settings")]
    public float glowDuration = 1f;    // Thời gian hiệu ứng phát sáng
    public float glowMultiplier = 2f;  // Hệ số nhân độ sáng

    private void Start()
    {
        // Ẩn vật phẩm cuối lúc đầu
        finalItem.SetActive(false);
        StartCoroutine(MoveAndFusion());
    }

    private IEnumerator MoveAndFusion()
    {
        // Lưu lại vị trí ban đầu của 3 relic
        Vector3 startPos1 = relic1.transform.position;
        Vector3 startPos2 = relic2.transform.position;
        Vector3 startPos3 = relic3.transform.position;

        float elapsed = 0f;

        // Di chuyển relic đến fusionPosition trong moveDuration giây
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            relic1.transform.position = Vector3.Lerp(startPos1, fusionPosition, t);
            relic2.transform.position = Vector3.Lerp(startPos2, fusionPosition, t);
            relic3.transform.position = Vector3.Lerp(startPos3, fusionPosition, t);

            yield return null;
        }

        // Đảm bảo relic đã nằm đúng vị trí
        relic1.transform.position = fusionPosition;
        relic2.transform.position = fusionPosition;
        relic3.transform.position = fusionPosition;

        // Ẩn 3 relic sau khi hợp nhất
        relic1.SetActive(false);
        relic2.SetActive(false);
        relic3.SetActive(false);

        // Hiển thị vật phẩm cuối (thuốc giải)
        finalItem.SetActive(true);

        // Bắt đầu hiệu ứng phát sáng cho vật phẩm (dùng SpriteRenderer)
        StartCoroutine(GlowEffect());
    }

    private IEnumerator GlowEffect()
    {
        // Lấy SpriteRenderer của finalItem
        SpriteRenderer spriteRenderer = finalItem.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Không tìm thấy SpriteRenderer trên finalItem!");
            yield break;
        }

        // Lấy màu ban đầu của sprite và tính toán màu mục tiêu tăng độ sáng
        Color initialColor = spriteRenderer.color;
        Color targetColor = initialColor * glowMultiplier;

        float elapsed = 0f;
        while (elapsed < glowDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / glowDuration);
            spriteRenderer.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }
    }
}
