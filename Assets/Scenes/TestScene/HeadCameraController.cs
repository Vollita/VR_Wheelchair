using UnityEngine;

public class HeadCameraController : MonoBehaviour
{
    [Header("旋转限制")]
    public float minXRotation = -60f; // 低头限制
    public float maxXRotation = 60f; // 抬头限制

    [Header("震动效果设置")]
    public float shakeIntensity = 0.02f; // 震动强度
    public float shakeFrequency = 15f; // 震动频率

    private Transform headTransform;
    private float xRotation = 0f;
    private Vector3 originalLocalPosition;
    private float shakeTimer = 0f;
    private bool isMoving = false;

    // 引用轮椅刚体组件
    public Rigidbody wheelchairRigidbody;
    public float moveThreshold = 0.1f; // 移动判断阈值

    private void Awake()
    {
        headTransform = GetComponent<Transform>();
        originalLocalPosition = headTransform.localPosition;
    }

    private void Update()
    {
        // 检查轮椅是否在移动
        isMoving = wheelchairRigidbody != null && wheelchairRigidbody.velocity.magnitude > moveThreshold;

        // 头部旋转限制
        ApplyHeadRotationLimits();

        // 应用视野震动效果
        ApplyCameraShake();
    }

    private void ApplyHeadRotationLimits()
    {
        // 获取当前头部旋转
        Vector3 currentRotation = headTransform.localEulerAngles;

        // 转换为-180到180度范围以便限制
        xRotation = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

        // 应用限制后的旋转
        headTransform.localEulerAngles = new Vector3(xRotation, currentRotation.y, currentRotation.z);
    }

    private void ApplyCameraShake()
    {
        if (isMoving)
        {
            shakeTimer += Time.deltaTime;

            // 使用正弦函数创建上下震动效果
            float shakeOffsetY = Mathf.Sin(shakeTimer * shakeFrequency) * shakeIntensity;

            // 应用震动偏移
            headTransform.localPosition = originalLocalPosition + new Vector3(0, shakeOffsetY, 0);
        }
        else
        {
            // 当轮椅停止移动时，平滑恢复到原始位置
            if (headTransform.localPosition != originalLocalPosition)
            {
                headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, originalLocalPosition, Time.deltaTime * 10f);
            }
            shakeTimer = 0f;
        }
    }
}