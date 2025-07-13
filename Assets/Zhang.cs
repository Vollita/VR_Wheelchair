using UnityEngine;

public class WheelchairController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 100f;

    [Header("鼠标控制参数")]
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;

    [Header("摄像机")]
    public Transform cameraTransform;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 控制玩家左右转向（yaw）
        transform.Rotate(Vector3.up * mouseX);

        // 控制相机上下看（pitch）
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);

        cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");  // A/D
        float v = Input.GetAxis("Vertical");    // W/S

        // 移动方向（基于玩家正前方）
        Vector3 move = transform.forward * v * moveSpeed * Time.deltaTime;
        transform.position += move;

        // 左右转向（模拟轮椅转向）
        transform.Rotate(Vector3.up, h * turnSpeed * Time.deltaTime);
    }
}
