using UnityEngine;
using TMPro;

public class NPCForest : MonoBehaviour
{
    public GameObject NPCConversationUI; // GameObject chính chứa UI
    private GameObject dialogUI; // Panel hộp thoại
    [SerializeField] private GameObject[] torches; // Mảng chứa các đuốc
    public GameObject spaceGate;
    private TextMeshProUGUI dialogText; // TextMeshPro hội thoại
    private TextMeshProUGUI dialogName;
    private string nameChar = "Eleonore";
    private bool hasCompletedQuest = false;
    private int countTorch = 0;
    private string[] dialogLines;
    private string[] dialogLinesRequest = {
        "🌀 Linh Hồn: 'Người lữ khách… Ngươi không thể đi tiếp.'",
        "🌀 Linh Hồn: 'Cánh cổng này chỉ có thể mở bởi ta.'",
        "🌀 Linh Hồn: 'Nhưng sinh mệnh lực của ta hiện không đủ.'",
        "🌀 Linh Hồn: 'Hãy giúp ta thắp sáng các ngọn đuốc sinh mệnh.'",
        "🌀 Linh Hồn: 'Nơi thứ nhất ở trước mặt ta*.'",
        "🌀 Linh Hồn: 'Nơi thứ hai ở trên không, sấm chớp vang trời*.'",
        "🌀 Linh Hồn: 'Nơi cuối cùng ở ngọn núi, phía sau ba cây vàng*.'",
        "🌀 Linh Hồn: 'Chỉ khi ngươi hoàn thành nhiệm vụ, cánh cổng mới mở.'",
        "🌀 Linh Hồn: 'Liệu ngươi có sẵn sàng đối mặt với những gì phía trước?'"
    };
    private string[] dialogLinesAfterQuest = {
    "🌀 Linh Hồn: 'Người lữ khách… Ta cảm nhận được sinh mệnh quay trở lại.'",
    "🌀 Linh Hồn: 'Ba ngọn đuốc đã thắp sáng. Ta đã chấp nhận sự gan dạ của ngươi.'",
    "🌀 Linh Hồn: 'Ngươi đã chứng tỏ bản thân… Cũng như giúp ta thêm sinh lực.'",
    "🌀 Linh Hồn: 'Ta sẽ trao cho ngươi sự bảo hộ của ta, đó là tấm khiên bất tử'",
    "🌀 Linh Hồn: 'Nó sẽ giúp ngươi tránh khỏi tổn thương trong một thời gian'",
    "🌀 Linh Hồn: 'Và cần hồi phục sau mỗi lần sử dụng. Thi triển nó bằng khả năng của ngươi (Press K)'",
    "🌀 Linh Hồn: 'Bây giờ ta sẽ tích năng lượng mở cánh cổng này giờ sẽ mở ra.'",
    "🌀 Linh Hồn: 'Bên kia cánh cổng là thử thách cuối cùng. Một khi bước qua, không thể quay lại.'",
    "🌀 Linh Hồn: 'Hãy chuẩn bị tinh thần… và bước tiếp, Chiến Binh.'",
    "*Cánh cổng cổ xưa rung chuyển, ánh sáng bí ẩn bùng lên, mở ra con đường phía trước...*"
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

        if (torches != null && torches.Length > 0)
        {
            foreach (GameObject torch in torches)
            {
                if (torch != null)
                {
                    torch.SetActive(false);
                }
            }
        }
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
        if (torches != null && torches.Length > 0)
        {
            

            foreach (GameObject torch in torches)
            {
                if (torch != null)
                {
                    Torch torchScript = torch.GetComponent<Torch>();

                    hasCompletedQuest = torchScript.GetStatusFire();
                    Debug.Log(torch);
                }
            }          
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

    public void CompletedFireTorch()
    {
        countTorch++;
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

        //Hiển thị tất cả các đuốc
        if (torches != null && torches.Length > 0)
        {
            foreach (GameObject torch in torches)
            {
                if (torch != null)
                {
                    torch.SetActive(true);
                }
                
            }
        }
        if (hasCompletedQuest)
        {
            GiveSkill();
            spaceGate.SetActive(hasCompletedQuest);
        }


    }

    private void GiveSkill()
    {
        playerController.UnlockSkill("Skill0");
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
}
