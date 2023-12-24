using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMain : MonoBehaviour
{
    public Transform target1; // Transform của nhân vật thứ nhất
    public Transform target2; // Transform của nhân vật thứ hai
    public float smoothTime = 0.05f; // Thời gian di chuyển mượt của camera
    public float minHeight = 15f; // Chiều cao tối thiểu của camera
    public float minWidth = 10f; // Chiều rộng tối thiểu của camera
    public float maxZoom = 10f; // Độ zoom tối đa
    public float minZoom = 2f; // Độ zoom tối thiểu

    private Vector3 velocity; // Biến lưu trữ vận tốc hiện tại của camera

    private void Update()
    {
        if (target1 == null || target2 == null)
        {
            target1 = GameObject.Find("Player").transform;
            target2 = GameObject.Find("Enemy").transform;
        }

        // Hoán đổi target1 và target2 nếu điều kiện xảy ra
        if (target1.position.x > target2.position.x)
        {
            Transform temp = target1;
            target1 = target2;
            target2 = temp;
        }

        // Tính toán khoảng cách giữa hai target
        float distance = Vector3.Distance(target1.position, target2.position);

        // Tính toán kích thước khung chứa
        float width = Mathf.Max(minWidth, distance);
        float height = Mathf.Max(minHeight, distance);

        // Tính toán điểm giữa của khung chứa
        Vector3 middlePoint = Vector3.Lerp(target1.position, target2.position, 0.5f);

        // Tạo một điểm mục tiêu mới cho camera dựa trên điểm giữa và kích thước khung chứa
        Vector3 targetPosition = new Vector3(middlePoint.x, transform.position.y, middlePoint.z) - transform.forward * height / (2f * Mathf.Tan(Mathf.Deg2Rad * GetComponent<Camera>().fieldOfView / 1.5f));

        // Dùng SmoothDamp để di chuyển camera một cách mượt mà đến vị trí mới
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Tính toán góc quay Y mục tiêu của camera
        Vector3 cameraDirection = target2.position - target1.position;
        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection, Vector3.up);

        // Xoay camera từ từ theo góc quay mới
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotationQuaternion = Quaternion.Euler(0f, targetRotation.eulerAngles.y - 90f, 0f);
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotationQuaternion, smoothTime);


        // Điều chỉnh góc nhìn của camera
        float targetFieldOfView = Mathf.Clamp(Mathf.Max(width / 2f, height / 2f), minZoom, maxZoom);
        GetComponent<Camera>().fieldOfView = targetFieldOfView;
    }
}
