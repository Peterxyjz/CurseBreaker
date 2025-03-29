using System.Collections;
using TMPro;
using UnityEngine;

public class SamuraiNPC : MonoBehaviour
{
    public GameObject NPCConversationUI; // GameObject chính chứa UI
    private GameObject dialogUI; // Panel hộp thoại
    private TextMeshProUGUI dialogText; // TextMeshPro hội thoại
    private TextMeshProUGUI dialogName;
    private string nameChar = "Samurai";
    private string[] dialogLines;
    private string[] dialogLinesRequest = {
        "🌀 Samurai: 'Ta nghe em gái ta an toàn trở về... là nhờ ngươi.'",
    "🌀 Samurai: 'Cảm ơn ngươi, thật lòng.'",
    "🌀 Samurai: 'Đáng tiếc, thân thể ta đã mục nát, chẳng thể bước thêm.'",
    "🌀 Samurai: 'Nhưng chí khí thì vẫn còn – và ta muốn truyền nó cho ngươi.'",
    "🌀 Samurai: 'Đây là kiếm kỹ ta trui rèn suốt đời – nhát chém phá tan mọi ràng buộc.'",
    "🌀 Samurai: 'Hãy dùng nó đúng lúc, đúng chỗ... và mang lấy ý chí của ta cùng ngươi.'",
        "🌀 Samurai: 'Nhưng hỡi người lữ khách, hãy giúp ta tiêu diệt toàn bộ hài cốt trong đây'",
            "🌀 Samurai: 'Em gái ta sẽ trao cho ngươi phần thưởng xứng đáng.'"
    };
    private Animator animator;
    private Transform playerTransform;

    private int currentLine = 0;
    private bool isTalking = false;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {

        dialogUI = NPCConversationUI.transform.Find("dialogUI")?.gameObject;
        dialogText = dialogUI?.transform.Find("dialogText")?.GetComponent<TextMeshProUGUI>();
        dialogName = dialogUI?.transform.Find("dialogName")?.GetComponent<TextMeshProUGUI>();

        dialogUI.SetActive(false);

       
    }

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.E))
        {
            ContinueConversation();
        }
    }
    

    void StartConversation()
    {
        isTalking = true;
        dialogUI.SetActive(true);
        currentLine = 0;
       
       

            dialogLines = dialogLinesRequest;
            dialogText.text = dialogLines[currentLine];
 
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
        if(currentLine == 4)
        {
            animator.SetBool("isAttacking", true);
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

        GiveSkill();
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDying", true);
        DisableColliders();
        StartCoroutine(Die());

    }

    private void GiveSkill()
    {
        playerController.UnlockSkill("Skill1");
    }
    private IEnumerator Die()
    {

        yield return null;

        // Chờ cho đến khi trạng thái chuyển sang "ArcherAssaAttack"
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SamuraiNPCDie"))
        {
            yield return null;
        }
        // Chờ cho đến khi animation hoàn thành
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("SamuraiNPCDie") &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        Destroy(gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("cham Player");
        if (collision.gameObject.CompareTag("Player"))
        {
            FacePlayer();
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
    private void FacePlayer()
    {
        if (playerTransform != null)
        {
            Vector3 scale = transform.localScale;
            if (playerTransform.position.x < transform.position.x)
            {
                // Player ở bên trái => NPC quay mặt sang trái
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                // Player ở bên phải => NPC quay mặt sang phải
                scale.x = -Mathf.Abs(scale.x);
            }
            transform.localScale = scale;
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
}
