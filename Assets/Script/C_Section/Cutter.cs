using System.Collections;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Cutter : MonoBehaviour

{
    // 조절하는 값들
    [SerializeField] bool ConveyorOn;
    [SerializeField] float ConveyorSpeed;
   

    //벨트 프레펩들
    [SerializeField] GameObject BeltPrefebs;
    [SerializeField] Transform parents;


    public static Cutter Instance;

    public void Start()
    {
        StartCoroutine(Spawn());
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    // 내보내는 함수들
    public bool TurnOnOff()
    {
        return ConveyorOn = !ConveyorOn;

    }
    public bool ReadConveyorOn()
    {

        return ConveyorOn;
    }
    public float ReadConveyorSpeed()
    {
        return ConveyorSpeed;
    }
 


    IEnumerator Spawn()
    {
        for (int i = 0; i < 14; i++)
        {
            // Conveyor가 꺼져 있을 경우, true가 될 때까지 대기
            while (!ConveyorOn)
            {
                yield return null; // 다음 프레임까지 대기
            }

            // Conveyor가 켜져 있을 경우 생성
            yield return new WaitForSeconds(0.55f); // 간격으로 생성
            Instantiate(BeltPrefebs, parents);
        }
    }

}
