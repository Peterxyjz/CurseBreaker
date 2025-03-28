using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI textUI;       // UI hiển thị nội dung
    public GameObject textBackground;    // Nền chữ
    public Button nextButton;            // Nút chuyển đoạn

    [Header("Typing Settings")]
    public float delay = 0.0000000005f; // Điều chỉnh tốc độ gõ chữ

    private string[] textLines = {
         "Vương quốc chìm trong bóng tối, lời nguyền của mụ phù thủy đã biến nơi đây thành một vùng đất chết. Dịch bệnh lan tràn, người dân gục ngã, chỉ còn lại những tiếng than khóc vang vọng trong đêm. Nhưng giữa đống tro tàn ấy, ta tìm thấy manh mối về bốn thánh di vật – những báu vật cuối cùng có thể hóa giải lời nguyền. Không ai biết chúng đang ở đâu, chỉ biết rằng con đường dẫn đến chúng đầy rẫy hiểm nguy. Ta không có lựa chọn. Nếu đây là hy vọng cuối cùng, ta sẽ cầm kiếm lên và bước vào bóng tối.",
    "Tương truyền, có một thánh di vật có thể hóa giải mọi lời nguyền. Nhưng theo thời gian, nó đã bị chia cắt và cất giấu tại những nơi nguy hiểm bậc nhất vương quốc. Một mảnh gợi ý cổ xưa đã dẫn ta đến điểm khởi đầu của hành trình.\r\n\r\nĐường đi phía trước không hề dễ dàng.Trên đường, những trái tim rải rác giúp ta phục hồi sức mạnh, nhưng ta cần tìm được những rương báu chứa vật phẩm để chuẩn bị cho thử thách lớn nhất—đánh bại kẻ bảo vệ di vật.",
    "Hãy cùng ta chiến đấu"
    };
    private int currentLine = 0;
    private bool isTyping = false; // Kiểm tra trạng thái gõ chữ

    private void Start()
    {
        textBackground.SetActive(false);  // Ẩn nền lúc đầu
        nextButton.gameObject.SetActive(false); // Ẩn nút Next ban đầu
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true; // Đánh dấu đang gõ chữ
        textBackground.SetActive(true); // Hiện nền chữ khi bắt đầu
        nextButton.gameObject.SetActive(false); // Ẩn nút khi chữ đang chạy

        textUI.text = ""; // Xóa nội dung cũ
        foreach (char letter in textLines[currentLine].ToCharArray()) // Gõ từng ký tự
        {
            textUI.text += letter;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false; // Đã hoàn thành gõ chữ
        nextButton.gameObject.SetActive(true); // Hiện nút Next sau khi chữ gõ xong
    }

    public void NextText()
    {
        if (isTyping) return; // Nếu chữ đang gõ thì không làm gì

        nextButton.gameObject.SetActive(false); // Ẩn nút trước khi chạy dòng mới

        if (currentLine < textLines.Length - 1)
        {
            currentLine++;
            StartCoroutine(TypeText());
        }
        else
        {
            textBackground.SetActive(false); // Ẩn nền khi hết chữ
            nextButton.gameObject.SetActive(false); // Ẩn nút Next
            LoadNextScene(); // Chuyển scene nếu cần
        }
    }

    private void LoadNextScene()
    {
        Debug.Log("Chuyển sang Scene mới!");
         UnityEngine.SceneManagement.SceneManager.LoadScene("Forest"); 
    }
}
