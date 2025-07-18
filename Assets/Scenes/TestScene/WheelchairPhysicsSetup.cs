using UnityEngine;

public class WheelchairPhysicsSetup : MonoBehaviour
{
    void Start()
    {
        // ��Ӹ������
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 10f; // ��������
        rb.drag = 0.1f; // �ƶ�����
        rb.angularDrag = 2f; // ��ת�����������ֵʹת�������
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // �����ײ��
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 0.5f, 0);
        collider.radius = 0.4f;
        collider.height = 1f;

        // �����Զ����������
        PhysicMaterial highFrictionMaterial = new PhysicMaterial("HighFrictionMaterial");
        highFrictionMaterial.dynamicFriction = 1.2f; // �߶�̬Ħ��
        highFrictionMaterial.staticFriction = 1.5f; // �߾�̬Ħ��
        highFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        collider.material = highFrictionMaterial;
    }
}