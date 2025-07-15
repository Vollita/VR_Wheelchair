using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelchairSimpleController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float turnSpeed = 60f;
    public float decelerationRate = 1.5f;
    public float dragCoefficient = 3f;
    public Transform cameraTransform;
    public float cameraHeight = 1.1f;

    [Header("Shake Settings")]
    public float cameraShakeIntensity = 0.05f;
    public float cameraShakeSpeed = 15f;
    public float cameraShakeVerticalRatio = 1f;

    [Header("Instability Settings")]
    public float tiltTorque = 5f; // 易摔模拟
    public float controlWobbleAmount = 10f; // 控制偏移幅度
    public float controlWobbleSpeed = 2f;
    public float terrainBumpiness = 1f; // 颠簸程度
    public float maxTiltAngle = 30f;

    private Rigidbody rb;
    private float currentMoveSpeed;
    private Vector3 originalCamPos;
    private float wobbleTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMoveSpeed = 0f;

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
            originalCamPos = cameraTransform.localPosition;
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; // 开始时冻结倾倒，后续可放开
    }

    void FixedUpdate()
    {
        float inputMove = Input.GetAxis("Vertical");
        float inputTurn = Input.GetAxis("Horizontal");

        // 模拟速度渐变
        if (inputMove != 0)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, moveSpeed * inputMove, Time.fixedDeltaTime * 5f);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, 0f, Time.fixedDeltaTime * decelerationRate);
        }

        // 摩擦阻力
        Vector3 velocity = rb.velocity;
        Vector3 frictionForce = -velocity * dragCoefficient * Time.fixedDeltaTime;
        rb.AddForce(frictionForce, ForceMode.VelocityChange);

        // 控制方向随机抖动（模拟难以操控方向）
        wobbleTimer += Time.fixedDeltaTime * controlWobbleSpeed;
        float wobble = Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f;
        float wobbleOffset = wobble * controlWobbleAmount * Mathf.Abs(inputMove);

        // 移动与旋转
        Vector3 moveDirection = transform.forward * currentMoveSpeed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);
        Quaternion turnOffset = Quaternion.Euler(0, (inputTurn + wobbleOffset) * turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * turnOffset);

        // 易摔效果：快速转弯时加入侧向扭矩
        if (Mathf.Abs(inputTurn) > 0.1f && Mathf.Abs(inputMove) > 0.1f)
        {
            rb.AddTorque(transform.right * -inputTurn * tiltTorque, ForceMode.Force);
        }

        // 倾倒检测：超过一定角度则提示失衡（可触发动画/失控）
        float tiltX = Mathf.Abs(transform.eulerAngles.x);
        float tiltZ = Mathf.Abs(transform.eulerAngles.z);
        if (tiltX > maxTiltAngle && tiltX < 180 || tiltZ > maxTiltAngle && tiltZ < 180)
        {
            Debug.LogWarning("Wheelchair is tilting! Risk of fall.");
            // 可播放摔倒动画 / 强制 reset
        }
        // 摄像机颠簸（仅在前/后/左右移动中触发）
        if (cameraTransform != null)
        {
            bool isMoving = Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.05f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.05f;

            if (isMoving)
            {
                float bumpX = Mathf.Sin(Time.time * cameraShakeSpeed * 1.1f) * 0.5f;
                float bumpY = Mathf.Cos(Time.time * cameraShakeSpeed * 1.7f) * 0.5f;
                Vector3 bumpOffset = new Vector3(bumpX, bumpY * cameraShakeVerticalRatio, 0) * cameraShakeIntensity * terrainBumpiness;

                cameraTransform.localPosition = originalCamPos + bumpOffset;
            }
            else
            {
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCamPos, Time.fixedDeltaTime * 5f);
            }
        }


    }
}
