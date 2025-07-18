using UnityEngine;
using UnityEngine.InputSystem;

public class WheelchairController : MonoBehaviour
{
    [Header("�ƶ�����")]
    public float moveForce = 500f;
    public float maxSpeed = 5f;
    public float rotationTorque = 150f;
    public float turnResistance = 5f; // ת��������ֵԽ��ת��Խ����

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 rotateInput;
    private InputAction moveAction;
    private InputAction rotateAction;

    public void SetInputActions(InputAction move, InputAction rotate)
    {
        // �Ƴ����а�
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

        // �����µ����붯��
        moveAction = move;
        rotateAction = rotate;

        // ��ӻص�
        moveAction.performed += OnMovePerformed;
        moveAction.canceled += OnMoveCanceled;
        rotateAction.performed += OnRotatePerformed;
        rotateAction.canceled += OnRotateCanceled;

        // ���ö���
        moveAction.Enable();
        rotateAction.Enable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // ��ȡ���붯��
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
        // ��ȡ���ǰ���򣬺���Y��
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // �����ƶ�����
        Vector3 moveDirection = (cameraForward * moveInput.y + Camera.main.transform.right * moveInput.x).normalized;

        // Ӧ���ƶ���
        if (moveDirection.magnitude > 0.1f && rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * moveForce * Time.fixedDeltaTime);
        }

        // Ӧ��ת��Ť�ز����ת������
        if (Mathf.Abs(rotateInput.x) > 0.1f)
        {
            // ����ת��Ť��
            float turnTorque = rotateInput.x * rotationTorque * Time.fixedDeltaTime;

            // ���ݵ�ǰ�ٶ����ת������
            float speedFactor = rb.velocity.magnitude / maxSpeed;
            float resistanceFactor = 1 + (turnResistance * speedFactor);

            // Ӧ���������ת��Ť��
            float appliedTorque = turnTorque / resistanceFactor;

            rb.AddTorque(Vector3.up * appliedTorque);
        }
        else
        {
            // ��û��ת������ʱ��������ת����ʹ���ξ���ֹͣת��
            rb.angularVelocity *= 0.9f;
        }

        // ��������ٶ�
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}