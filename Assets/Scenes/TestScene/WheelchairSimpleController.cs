using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelchairSimpleController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float turnSpeed = 60f;
    public float decelerationRate = 1.5f; // 新增：减速因子
    public float dragCoefficient = 3f;    // 新增：模拟摩擦（也可直接用Rigidbody的drag）
    public Transform cameraTransform;
    public float cameraHeight = 1.1f;
    public float cameraShakeIntensity = 0.05f; // 镜头抖动强度
    public float cameraShakeSpeed = 15f;       // 镜头抖动频率

    private Rigidbody rb;
    private float currentMoveSpeed;
    private Vector3 originalCamPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMoveSpeed = 0f;

        if (cameraTransform != null)
        {
            cameraTransform.localPosition = new Vector3(0, cameraHeight, 0);
            originalCamPos = cameraTransform.localPosition;
        }
    }

    void FixedUpdate()
    {
        float inputMove = Input.GetAxis("Vertical");
        float inputTurn = Input.GetAxis("Horizontal");

        // 模拟速度变慢（逐渐减速）
        if (inputMove != 0)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, moveSpeed * inputMove, Time.fixedDeltaTime * 5f);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, 0f, Time.fixedDeltaTime * decelerationRate);
        }

        // 摩擦力效果（手动施加反向力）
        Vector3 velocity = rb.velocity;
        Vector3 frictionForce = -velocity * dragCoefficient * Time.fixedDeltaTime;
        rb.AddForce(frictionForce, ForceMode.VelocityChange);

        // 移动
        Vector3 moveDirection = transform.forward * currentMoveSpeed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        // 旋转
        Quaternion turnOffset = Quaternion.Euler(0, inputTurn * turnSpeed * Time.fixedDeltaTime, 0);
        rb.MoveRotation(rb.rotation * turnOffset);

        // 镜头摇晃
        if (cameraTransform != null)
        {
            float shakeOffset = Mathf.PerlinNoise(Time.time * cameraShakeSpeed, 0f) - 0.5f;
            Vector3 shake = new Vector3(shakeOffset * cameraShakeIntensity, 0, 0);
            cameraTransform.localPosition = originalCamPos + shake;
        }
    }
}
