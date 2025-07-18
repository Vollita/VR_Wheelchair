using UnityEngine;
using UnityEngine.InputSystem;

public class WheelchairController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveForce = 500f;
    public float maxSpeed = 5f;
    public float rotationTorque = 150f;
    public float turnResistance = 5f; // 转向阻力，值越大转向越困难

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 rotateInput;
    private InputAction moveAction;
    private InputAction rotateAction;

    public void SetInputActions(InputAction move, InputAction rotate)
    {
        // 移除现有绑定
        if (moveAction != null)
        {
            moveAction.performed -= OnMovePerformed;
            moveAction.canceled -= OnMoveCanceled;
            moveAction.Disable();
        }

        if (rotateAction != null)
        {
            rotateAction.performed -= OnRotatePerformed;
            rotateAction.canceled -= OnRotateCanceled;
            rotateAction.Disable();
        }

        // 设置新的输入动作
        moveAction = move;
        rotateAction = rotate;

        // 添加回调
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;

        // 启用动作
        moveAction.Enable();
        rotateAction.Enable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 获取输入动作
        InputActionAsset inputAsset = Resources.Load<InputActionAsset>("XRInputActions");
        var actionMap = inputAsset.FindActionMap("WheelchairControl");
        moveAction = actionMap.FindAction("Move");
        rotateAction = actionMap.FindAction("Rotate");
    }

    private void OnEnable()
    {
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;

        moveAction.Enable();
        rotateAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        moveAction.canceled -= OnMoveCanceled;
        rotateAction.performed -= OnRotatePerformed;
        rotateAction.canceled -= OnRotateCanceled;

        moveAction.Disable();
        rotateAction.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        rotateInput = context.ReadValue<Vector2>();
    }

    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        rotateInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // 获取相机前方向，忽略Y轴
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // 计算移动方向
        Vector3 moveDirection = (cameraForward * moveInput.y + Camera.main.transform.right * moveInput.x).normalized;

        // 应用移动力
        if (moveDirection.magnitude > 0.1f && rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * moveForce * Time.fixedDeltaTime);
        }

        // 应用转向扭矩并添加转向阻力
        if (Mathf.Abs(rotateInput.x) > 0.1f)
        {
            // 基础转向扭矩
            float turnTorque = rotateInput.x * rotationTorque * Time.fixedDeltaTime;

            // 根据当前速度添加转向阻力
            float speedFactor = rb.velocity.magnitude / maxSpeed;
            float resistanceFactor = 1 + (turnResistance * speedFactor);

            // 应用阻力后的转向扭矩
            float appliedTorque = turnTorque / resistanceFactor;

            rb.AddTorque(Vector3.up * appliedTorque);
        }
        else
        {
            // 当没有转向输入时，增加旋转阻力使轮椅尽快停止转向
            rb.angularVelocity *= 0.9f;
        }

        // 限制最大速度
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}