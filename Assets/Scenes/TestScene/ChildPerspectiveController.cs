using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChildPerspectiveController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 50f;
    public Transform cameraTransform;
    public float cameraHeight = 0.9f;

    [Header("Jump Settings")]
    public float jumpForce = 4f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Head Bobbing")]
    public float bobbingIntensity = 0.03f;
    public float bobbingSpeed = 6f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private Vector3 originalCameraLocalPos;
    private float bobbingTimer = 0f;
    private bool isGrounded;

    // ---- �ص����� ----
    private bool isRebounding = false;
    private float reboundAngle = 5f; // �ص��Ƕȣ��ɵ���
    private float reboundDuration = 0.2f; // �ص�ʱ�䣨�ɵ���
    private float reboundTimer = 0f;
    private int reboundDirection = 0; // -1 = ��ص���1 = �һص�

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
            originalCameraLocalPos = cameraTransform.localPosition;
        }
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // ������
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayer);

        // ��Ծ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // ������ζ�
        if (cameraTransform != null)
        {
            if (Mathf.Abs(moveInput) > 0.01f || Mathf.Abs(turnInput) > 0.01f)
            {
                bobbingTimer += Time.deltaTime * bobbingSpeed;

                float bobbingY = Mathf.Sin(bobbingTimer) * bobbingIntensity;
                float bobbingX = Mathf.Cos(bobbingTimer * 0.5f) * bobbingIntensity * 0.5f;
                float yawAngle = Mathf.Sin(bobbingTimer * 0.7f) * 2f; // ���������ƫ��

                cameraTransform.localPosition = originalCameraLocalPos + new Vector3(bobbingX, bobbingY, 0);
                cameraTransform.localRotation = Quaternion.Euler(0, yawAngle, 0);
            }
            else
            {
                bobbingTimer = 0f;
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraLocalPos, Time.deltaTime * 5f);
                cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
            }
        }

        // �������һص����ɿ������ʱ��¼������ص�
        if (!isRebounding)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                StartRebound(1); // �����ɿ����ص�����
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                StartRebound(-1); // �����ɿ����ص�����
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        if (isRebounding)
        {
            reboundTimer += Time.fixedDeltaTime;
            float t = reboundTimer / reboundDuration;
            float reboundAmount = Mathf.Lerp(reboundAngle, 0f, t); // �����ص��Ƕ�
            Quaternion reboundRot = Quaternion.Euler(0, reboundAmount * reboundDirection, 0);
            rb.MoveRotation(rb.rotation * reboundRot);

            if (t >= 1f)
            {
                isRebounding = false;
            }
        }
        else
        {
            // ������ת
            Quaternion turnOffset = Quaternion.Euler(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * turnOffset);
        }
    }

    void StartRebound(int direction)
    {
        isRebounding = true;
        reboundDirection = direction;
        reboundTimer = 0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;

        if (otherRb != null && !otherRb.isKinematic)
        {
            Vector3 pushDir = collision.contacts[0].normal * -1f;
            otherRb.AddForce(pushDir * moveSpeed * 50f);
        }
    }
}