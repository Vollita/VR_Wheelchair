using UnityEngine;

public class HeadCameraController : MonoBehaviour
{
    [Header("��ת����")]
    public float minXRotation = -60f; // ��ͷ����
    public float maxXRotation = 60f; // ̧ͷ����

    [Header("��Ч������")]
    public float shakeIntensity = 0.02f; // ��ǿ��
    public float shakeFrequency = 15f; // ��Ƶ��

    private Transform headTransform;
    private float xRotation = 0f;
    private Vector3 originalLocalPosition;
    private float shakeTimer = 0f;
    private bool isMoving = false;

    // �������θ������
    public Rigidbody wheelchairRigidbody;
    public float moveThreshold = 0.1f; // �ƶ��ж���ֵ

    private void Awake()
    {
        headTransform = GetComponent<Transform>();
        originalLocalPosition = headTransform.localPosition;
    }

    private void Update()
    {
        // ��������Ƿ����ƶ�
        isMoving = wheelchairRigidbody != null && wheelchairRigidbody.velocity.magnitude > moveThreshold;

        // ͷ����ת����
        ApplyHeadRotationLimits();

        // Ӧ����Ұ��Ч��
        ApplyCameraShake();
    }

    private void ApplyHeadRotationLimits()
    {
        // ��ȡ��ǰͷ����ת
        Vector3 currentRotation = headTransform.localEulerAngles;

        // ת��Ϊ-180��180�ȷ�Χ�Ա�����
        xRotation = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

        // Ӧ�����ƺ����ת
        headTransform.localEulerAngles = new Vector3(xRotation, currentRotation.y, currentRotation.z);
    }

    private void ApplyCameraShake()
    {
        if (isMoving)
        {
            shakeTimer += Time.deltaTime;

            // ʹ�����Һ�������������Ч��
            float shakeOffsetY = Mathf.Sin(shakeTimer * shakeFrequency) * shakeIntensity;

            // Ӧ����ƫ��
            headTransform.localPosition = originalLocalPosition + new Vector3(0, shakeOffsetY, 0);
        }
        else
        {
            // ������ֹͣ�ƶ�ʱ��ƽ���ָ���ԭʼλ��
            if (headTransform.localPosition != originalLocalPosition)
            {
                headTransform.localPosition = Vector3.Lerp(headTransform.localPosition, originalLocalPosition, Time.deltaTime * 10f);
            }
            shakeTimer = 0f;
        }
    }
}