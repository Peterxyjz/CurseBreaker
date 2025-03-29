using UnityEngine;

public class SwordFollowIng : MonoBehaviour
{
    public Transform player; // Gán Player vào đây
    public Animator animator;

    // Danh sách các vị trí của kiếm theo từng frame animation
    public Vector3[] swordPositions;
    public Quaternion[] swordRotations; // Nếu muốn xoay kiếm theo từng frame

    private void Update()
    {
        if (animator == null || swordPositions.Length == 0)
            return;

        // Lấy thông tin state animation hiện tại
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Tính frame hiện tại theo normalized time
        float normalizedTime = stateInfo.normalizedTime % 1; // Lấy phần thập phân của thời gian animation
        int frameIndex = Mathf.FloorToInt(normalizedTime * swordPositions.Length);

        // Cập nhật vị trí và góc quay của kiếm
        if (frameIndex >= 0 && frameIndex < swordPositions.Length)
        {
            transform.position = player.position + swordPositions[frameIndex];
            transform.rotation = swordRotations[frameIndex]; // Nếu có xoay
        }
    }
}
