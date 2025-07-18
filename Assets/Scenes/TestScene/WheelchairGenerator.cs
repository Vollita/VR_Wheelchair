using UnityEngine;

public class WheelchairGenerator : MonoBehaviour
{
    void Start()
    {
        // 创建座位
        GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.name = "Seat";
        seat.transform.parent = transform;
        seat.transform.localPosition = new Vector3(0, 0.5f, 0);
        seat.transform.localScale = new Vector3(0.8f, 0.3f, 0.6f);

        // 创建靠背
        GameObject backrest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backrest.name = "Backrest";
        backrest.transform.parent = transform;
        backrest.transform.localPosition = new Vector3(0, 0.95f, -0.25f);
        backrest.transform.localScale = new Vector3(0.8f, 0.6f, 0.2f);

        // 创建轮子
        CreateWheel("FrontLeftWheel", new Vector3(-0.3f, 0.25f, 0.3f));
        CreateWheel("FrontRightWheel", new Vector3(0.3f, 0.25f, 0.3f));
        CreateWheel("RearLeftWheel", new Vector3(-0.3f, 0.25f, -0.3f));
        CreateWheel("RearRightWheel", new Vector3(0.3f, 0.25f, -0.3f));
    }

    void CreateWheel(string name, Vector3 localPosition)
    {
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = name;
        wheel.transform.parent = transform;
        wheel.transform.localPosition = localPosition;
        wheel.transform.localScale = new Vector3(0.2f, 0.25f, 0.2f);
        wheel.transform.localEulerAngles = new Vector3(90, 0, 0);
    }
}