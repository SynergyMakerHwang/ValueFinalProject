using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] MeshRenderer redLamp;
    [SerializeField] MeshRenderer yellowLamp;
    [SerializeField] MeshRenderer greenLamp;
    [SerializeField] bool[] isLampOns = { false, false, false };

    enum LampColor
    {
        RED,
        YELLOW,
        GREEN
    }

    private void Start()
    {
        redLamp.material.color = Color.grey;
        yellowLamp.material.color = Color.grey;
        greenLamp.material.color = Color.grey;
    }

    public void OnRedLampBtnClkEvent()
    {
        isLampOns[(int)LampColor.RED] = !isLampOns[(int)LampColor.RED];

        if (isLampOns[(int)LampColor.RED])
            redLamp.material.color = Color.red;
        else
            redLamp.material.color = Color.grey;
    }


    public void OnRedLampBtnClkEvent(bool isOn)
    {
        if (isOn)
            redLamp.material.color = Color.red;
        else
            redLamp.material.color = Color.grey;
    }

    public void OnYellowLampBtnClkEvent()
    {
        isLampOns[(int)LampColor.YELLOW] = !isLampOns[(int)LampColor.YELLOW];

        if (isLampOns[(int)LampColor.YELLOW])
            yellowLamp.material.color = Color.yellow;
        else
            yellowLamp.material.color = Color.grey;
    }

    public void OnYellowLampBtnClkEvent(bool isOn)
    {
        if (isOn)
            yellowLamp.material.color = Color.yellow;
        else
            yellowLamp.material.color = Color.grey;
    }

    public void OnGreenLampBtnClkEvent()
    {
        isLampOns[(int)LampColor.GREEN] = !isLampOns[(int)LampColor.GREEN];

        if (isLampOns[(int)LampColor.GREEN])
            greenLamp.material.color = Color.green;
        else
            greenLamp.material.color = Color.grey;
    }


    public void OnGreenLampBtnClkEvent(bool isOn)
    {
        if (isOn)
            greenLamp.material.color = Color.green;
        else
            greenLamp.material.color = Color.grey;
    }
}
