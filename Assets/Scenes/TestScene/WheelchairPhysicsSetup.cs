using UnityEngine;

public class WheelchairPhysicsSetup : MonoBehaviour
{
    void Start()
    {
        // 添加刚体组件
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 10f; // 轮椅质量
        rb.drag = 0.1f; // 移动阻力
        rb.angularDrag = 2f; // 旋转阻力，增大此值使转向更困难
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // 添加碰撞体
        CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 0.5f, 0);
        collider.radius = 0.4f;
        collider.height = 1f;

        // 创建自定义物理材质
        PhysicMaterial highFrictionMaterial = new PhysicMaterial("HighFrictionMaterial");
        highFrictionMaterial.dynamicFriction = 1.2f; // 高动态摩擦
        highFrictionMaterial.staticFriction = 1.5f; // 高静态摩擦
        highFrictionMaterial.frictionCombine = PhysicMaterialCombine.Maximum;
        collider.material = highFrictionMaterial;
    }
}