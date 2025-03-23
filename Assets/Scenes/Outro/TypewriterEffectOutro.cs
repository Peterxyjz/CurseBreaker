using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TypewriterEffectOutro : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI textUI;       // UI hiển thị nội dung
    public GameObject textBackground;    // Nền chữ
    public Button nextButton;            // Nút chuyển đoạn

    [Header("Typing Settings")]
    public float delay = 0.0000000005f; // Điều chỉnh tốc độ gõ chữ

    private string[] textLines = {
        "Đây là đoạn chữ đầu tiên...",
        "Đây là đoạn chữ thứ hai...",
        "Đây là đoạn chữ cuối cùng. Nhấn để tiếp tục!"
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
         UnityEngine.SceneManagement.SceneManager.LoadScene("Volcano"); 
    }
}
