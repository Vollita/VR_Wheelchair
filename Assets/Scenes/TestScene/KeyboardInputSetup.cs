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
        // 创建新的输入动作资源
        inputAsset = ScriptableObject.CreateInstance<InputActionAsset>();

        // 创建动作映射
        actionMap = inputAsset.AddActionMap("WheelchairControl");

        // 创建移动动作 (WASD)
        var moveAction = actionMap.AddAction("Move", InputActionType.Value, null, null, null, "<Keyboard>/w");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // 创建旋转动作 (Q/E)
        var rotateAction = actionMap.AddAction("Rotate", InputActionType.Value, null, null, null, "<Keyboard>/q");
        rotateAction.AddCompositeBinding("1DAxis")
            .With("Positive", "<Keyboard>/e")
            .With("Negative", "<Keyboard>/q");

        // 启用动作映射
        actionMap.Enable();

        // 将动作分配给轮椅控制器
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