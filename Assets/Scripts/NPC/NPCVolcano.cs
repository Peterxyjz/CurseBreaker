using TMPro;
using UnityEngine;
using System.Collections;
public class NPCVolcano : MonoBehaviour
{
    public GameObject NPCConversationUI; // GameObject chính chứa UI
    private GameObject dialogUI; // Panel hộp thoại
    private TextMeshProUGUI dialogText; // TextMeshPro hội thoại
    private TextMeshProUGUI dialogName;
    private string nameChar = "Magic Master";
    private string[] dialogLines;
    private string[] dialogLinesRequest = {
        "🌀 Magic Master: 'Giỏi lắm người lữ khách, ngươi đã tiêu diệt Worm Boss'",
    "🌀 Magic Master: 'Ta sẽ ban cho ngươi chúc phúc của ta'",
    "🌀 Magic Master: 'Thứ sẽ giúp ngươi lên tầm cao mới, nào giờ hãy vào cánh cổng đó'",
    "🌀 Magic Master: 'Nơi sẽ có quái vật đáng sợ đang chờ ngươi, cố lên.'",
   
    };
    public SpaceGate spaceGate;

    private Transform playerTransform;

    private int currentLine = 0;
    private bool isTalking = false;
    private PlayerController playerController;


    void Start()
    {
        spaceGate.gameObject.SetActive(false);
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
        if (currentLine == 4)
        {
            spaceGate.gameObject.SetActive(true);
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

        DisableColliders();
        Destroy(gameObject);

    }

    private void GiveSkill()
    {
        playerController.UnlockSkill("Skill2");
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
