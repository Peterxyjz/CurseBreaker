using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CooldownSkill : MonoBehaviour
{


    public Image cooldownMask;        // Kéo Image (Type = Filled) vào đây
    public TextMeshProUGUI cooldownText; // Kéo Text để hiển thị số giây (nếu cần)
    public Image icon;
    public TextMeshProUGUI guiUse;
    private float totalCooldown;      // Tổng thời gian hồi chiêu
    private float currentCooldown;    // Thời gian hồi chiêu còn lại
    private bool isCooldown = false;

    private void Start()
    {
        // Lúc đầu mask = 0, tức không che icon
        if (cooldownMask != null)
            cooldownMask.fillAmount = 0;
    }

    private void Update()
    {
        // Nếu đang hồi chiêu
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                currentCooldown = 0f;
                isCooldown = false;
            }

            // Cập nhật fillAmount (mask che dần hoặc mở dần tuỳ ý)
            float fillValue = currentCooldown / totalCooldown;
            cooldownMask.fillAmount = fillValue;

            // Cập nhật text
            if (cooldownText != null)
                cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
        }
        else
        {
            // Hết cooldown, xóa text
            if (cooldownText != null)
                cooldownText.text = "";
        }
    }

    // Hàm nhận thời gian hồi chiêu từ PlayerController
    public void UseSkill(float cooldownTime)
    {
        totalCooldown = cooldownTime;
        currentCooldown = totalCooldown;
        isCooldown = true;
    }
    public void setStatusSkill(bool status)
    {
        icon.gameObject.SetActive(status);
        guiUse.gameObject.SetActive(status);
    }
    
}
