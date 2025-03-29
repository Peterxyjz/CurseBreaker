using TMPro;
using UnityEngine;
using System.Collections;
public class NPCDesert : MonoBehaviour
{
    public GameObject NPCConversationUI; // GameObject chính chứa UI
    private GameObject dialogUI; // Panel hộp thoại
    [SerializeField] private Transform skeletonList;
    public GameObject spaceGate;
    private TextMeshProUGUI dialogText; // TextMeshPro hội thoại
    private TextMeshProUGUI dialogName;
    private string nameChar = "Eleonore";
    private bool hasCompletedQuest = false;
    public GameObject gift;
    private string[] dialogLines;
    private string[] dialogLinesRequest = {
        "🌀 Linh Hồn: 'Người lữ khách… Ngươi đã không hoàn thành di nguyện của anh trai ta.'",
    "🌀 Linh Hồn: 'Hiện giờ vẫn còn quái vật hài cốt trong lòng đất.'",
    "🌀 Linh Hồn: 'Ta vẫn sẽ mở cánh cổng cho ngươi, nhưng không có phần thưởng xứng đáng.'"

    };
    private string[] dialogLinesAfterQuest = {
    "🌀 Linh Hồn: 'Cảm ơn người lữ khách, tâm nguyện đã hoàn thành'",
    "🌀 Linh Hồn: 'Đây là phần thưởng như đã hứa, ta sẽ mở cánh cổng cho ngươi.'",
    "🌀 Linh Hồn: 'Chúc ngươi may mắn.'"
};


    private int currentLine = 0;
    private bool isTalking = false;
    private PlayerController playerController;

    void Start()
    {

        dialogUI = NPCConversationUI.transform.Find("dialogUI")?.gameObject;
        dialogText = dialogUI?.transform.Find("dialogText")?.GetComponent<TextMeshProUGUI>();
        dialogName = dialogUI?.transform.Find("dialogName")?.GetComponent<TextMeshProUGUI>();

        dialogUI.SetActive(false);

        gift.SetActive(false);
        spaceGate.SetActive(false);
    }

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.E))
        {
            ContinueConversation();
        }
    }
    void CheckTorchesCompleted()
    {
        if (skeletonList != null)
        {
            // Nếu không còn Skeleton nào trong danh sách (tức là đã tiêu diệt hết)
            if (skeletonList.childCount == 0)
            {
                hasCompletedQuest = true;
            }
            else
            {
                hasCompletedQuest = false;
            }
            Debug.Log("Số Skeleton còn lại: " + skeletonList.childCount);
        }
    }

    void StartConversation()
    {
        isTalking = true;
        dialogUI.SetActive(true);
        currentLine = 0;
        CheckTorchesCompleted();
        if (hasCompletedQuest)
        {
            dialogLines = dialogLinesAfterQuest;
            dialogText.text = dialogLines[currentLine];
        }
        else
        {

            dialogLines = dialogLinesRequest;
            dialogText.text = dialogLines[currentLine];
        }
        dialogName.text = nameChar;
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.SetMovement(true);
            playerController.ResetVelocity();

        }
    }


    void ContinueConversation()
    {
        currentLine++;
        if (currentLine < dialogLines.Length)
        {
            dialogText.text = dialogLines[currentLine];
        }
        else
        {
            EndConversation();
        }
    }

    void EndConversation()
    {
        isTalking = false;
        dialogUI.SetActive(false);

        if (playerController != null)
        {
            playerController.SetMovement(false);
            playerController.enabled = true;
        }

       
        if (hasCompletedQuest)
        {
            GiveGift();
            
        }
        spaceGate.SetActive(true);
        DisableColliders();
    }

    private void GiveGift()
    {
        gift.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("cham Player");
        if (collision.gameObject.CompareTag("Player"))
        {

            Debug.Log("vao hop thaoi");
            playerController = collision.GetComponent<PlayerController>();
            StartConversation();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("het cham Player");
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isTalking)
            {
                if (dialogUI != null)
                {
                    dialogUI.SetActive(false);
                }

            }
        }
    }
    protected void DisableColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }
    protected IEnumerator FadeOutAndDestroy()
    {
        // Lấy SpriteRenderer của đối tượng
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Kiểm tra nếu có SpriteRenderer
        if (spriteRenderer != null)
        {
            // Đặt thời gian fade (2 giây)
            float fadeDuration = 2f;
            float timeElapsed = 0f;

            // Lấy màu hiện tại của Sprite
            Color startColor = spriteRenderer.color;

            // Mờ dần trong 2 giây
            while (timeElapsed < fadeDuration)
            {
                timeElapsed += Time.deltaTime;

                // Tính toán giá trị alpha dựa trên thời gian
                float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

                yield return null; // Đợi 1 frame
            }
        }

        // Sau khi mờ dần, hủy đối tượng
        Destroy(gameObject);
    }
}
