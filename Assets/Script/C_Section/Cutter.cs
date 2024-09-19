using System.Collections;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Cutter : MonoBehaviour

{
    // �����ϴ� ����
    [SerializeField] bool ConveyorOn;
    [SerializeField] float ConveyorSpeed;
   

    //��Ʈ �������
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
    // �������� �Լ���
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
            // Conveyor�� ���� ���� ���, true�� �� ������ ���
            while (!ConveyorOn)
            {
                yield return null; // ���� �����ӱ��� ���
            }

            // Conveyor�� ���� ���� ��� ����
            yield return new WaitForSeconds(0.55f); // �������� ����
            Instantiate(BeltPrefebs, parents);
        }
    }

}
