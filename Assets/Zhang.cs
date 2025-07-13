using UnityEngine;

public class WheelchairController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 100f;

    [Header("�����Ʋ���")]
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;

    [Header("�����")]
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

        // �����������ת��yaw��
        transform.Rotate(Vector3.up * mouseX);

        // ����������¿���pitch��
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -60f, 60f);

        cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");  // A/D
        float v = Input.GetAxis("Vertical");    // W/S

        // �ƶ����򣨻��������ǰ����
        Vector3 move = transform.forward * v * moveSpeed * Time.deltaTime;
        transform.position += move;

        // ����ת��ģ������ת��
        transform.Rotate(Vector3.up, h * turnSpeed * Time.deltaTime);
    }
}
