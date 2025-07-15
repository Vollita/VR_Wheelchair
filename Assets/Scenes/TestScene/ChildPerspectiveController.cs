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

    // ---- 回弹设置 ----
    private bool isRebounding = false;
    private float reboundAngle = 5f; // 回弹角度（可调）
    private float reboundDuration = 0.2f; // 回弹时间（可调）
    private float reboundTimer = 0f;
    private int reboundDirection = 0; // -1 = 左回弹，1 = 右回弹

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

        // 地面检测
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.5f, groundLayer);

        // 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 摄像机晃动
        if (cameraTransform != null)
        {
            if (Mathf.Abs(moveInput) > 0.01f || Mathf.Abs(turnInput) > 0.01f)
            {
                bobbingTimer += Time.deltaTime * bobbingSpeed;

                float bobbingY = Mathf.Sin(bobbingTimer) * bobbingIntensity;
                float bobbingX = Mathf.Cos(bobbingTimer * 0.5f) * bobbingIntensity * 0.5f;
                float yawAngle = Mathf.Sin(bobbingTimer * 0.7f) * 2f; // 摄像机左右偏摆

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

        // 触发左右回弹：松开方向键时记录反方向回弹
        if (!isRebounding)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                StartRebound(1); // 向左松开，回弹向右
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                StartRebound(-1); // 向右松开，回弹向左
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
            float reboundAmount = Mathf.Lerp(reboundAngle, 0f, t); // 渐弱回弹角度
            Quaternion reboundRot = Quaternion.Euler(0, reboundAmount * reboundDirection, 0);
            rb.MoveRotation(rb.rotation * reboundRot);

            if (t >= 1f)
            {
                isRebounding = false;
            }
        }
        else
        {
            // 正常旋转
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