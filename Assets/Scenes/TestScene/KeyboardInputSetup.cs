using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(WheelchairController))]
public class KeyboardInputSetup : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap actionMap;
    private WheelchairController wheelchairController;

    void Awake()
    {
        wheelchairController = GetComponent<WheelchairController>();
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        // �����µ����붯����Դ
        inputAsset = ScriptableObject.CreateInstance<InputActionAsset>();

        // ��������ӳ��
        actionMap = inputAsset.AddActionMap("WheelchairControl");

        // �����ƶ����� (WASD)
        var moveAction = actionMap.AddAction("Move", InputActionType.Value, null, null, null, "<Keyboard>/w");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // ������ת���� (Q/E)
        var rotateAction = actionMap.AddAction("Rotate", InputActionType.Value, null, null, null, "<Keyboard>/q");
        rotateAction.AddCompositeBinding("1DAxis")
            .With("Positive", "<Keyboard>/e")
            .With("Negative", "<Keyboard>/q");

        // ���ö���ӳ��
        actionMap.Enable();

        // ��������������ο�����
        wheelchairController.SetInputActions(moveAction, rotateAction);
    }

    void OnDestroy()
    {
        if (actionMap != null)
            actionMap.Disable();

        if (inputAsset != null)
            Destroy(inputAsset);
    }
}