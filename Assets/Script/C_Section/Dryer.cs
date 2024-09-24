using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Dryer : MonoBehaviour
{
    // �Ǻ����� ��¦ ������
    [SerializeField] bool DoorValue;

    [SerializeField] Transform PivotAll;
    [SerializeField] Transform DoorPivot;
    [SerializeField] Transform Pivot1;
    [SerializeField] Transform LinkPivot1;
    [SerializeField] Transform LinkPivot2;





    bool DoorCheck = true;
    float duration = 1;

    //2�ʾȿ� ���ݱ� ����

    private void Update()
    {

        // ���ǿ� ���� ���� �޴´�.
        // PLC �������Ű�� ���ؼ� �־���Ѵ�.
        if (DoorCheck && DoorValue)
        {
            StartCoroutine(Open());
        }
        else if (!DoorCheck && !DoorValue)
        {
            StartCoroutine(Close());
        }

    }


    IEnumerator Open()
    {

        DoorCheck = !DoorCheck;

        // 1 Chapter
        Vector3 StartPos = Pivot1.localPosition;
        Vector3 TartgetPos = new Vector3(StartPos.x, StartPos.y, -0.08f);
        Quaternion StartPivot1 = LinkPivot1.localRotation;
        Quaternion EndPivot1 = Quaternion.Euler(80, -90, -90);
        Quaternion StartPivot2 = LinkPivot2.localRotation;
        Quaternion EndPivot2 = Quaternion.Euler(StartPivot2.x, -10, StartPivot2.z);


        // 2 Chapter
        Quaternion StartPivotAll = PivotAll.localRotation;
        Quaternion EndPiovtAll = Quaternion.Euler(StartPivotAll.x, -150, StartPivotAll.z);
        Quaternion StartDoorPivot = DoorPivot.localRotation;
        Quaternion EndDoorPivot = Quaternion.Euler(StartDoorPivot.x, -210, StartDoorPivot.z);
        Quaternion EndPivot1of2 = Quaternion.Euler(-70, -90, -90);


        // 1 ����
        float CurrentTIme1 = 0;
        while (CurrentTIme1 < duration)
        {
            CurrentTIme1 += Time.deltaTime;
            Pivot1.localPosition = Vector3.Lerp(StartPos, TartgetPos, CurrentTIme1 / duration);
            LinkPivot1.localRotation = Quaternion.Lerp(StartPivot1, EndPivot1, CurrentTIme1 / duration);
            LinkPivot2.localRotation = Quaternion.Lerp(StartPivot2, EndPivot2, CurrentTIme1 / duration);
            yield return null;
        }
        //// 2 ����
        CurrentTIme1 = 0;

        while (CurrentTIme1 < duration)
        {


            CurrentTIme1 += Time.deltaTime;
            PivotAll.localRotation = Quaternion.Slerp(StartPivotAll, EndPiovtAll, CurrentTIme1 / duration);
            DoorPivot.localRotation = Quaternion.Slerp(StartDoorPivot, EndDoorPivot, CurrentTIme1 / duration);
            LinkPivot1.localRotation = Quaternion.Slerp(EndPivot1, EndPivot1of2, CurrentTIme1 / duration);

            yield return null;


        }



    }
    //�����̵��� �ϸ��
    IEnumerator Close()
    {
        DoorCheck = !DoorCheck;

        float CurrentTime2 = 0;
        // 1 Chapter 
        Vector3 StartPos = Pivot1.localPosition;
        Vector3 TartgetPos = new Vector3(StartPos.x, StartPos.y, -0.04f);
        Quaternion StartPivot1 = LinkPivot1.localRotation;
        Quaternion EndPivot1 = Quaternion.Euler(80, -90, -90);
        Quaternion StartPivot2 = LinkPivot2.localRotation;
        Quaternion EndPivot2 = Quaternion.Euler(0, 0, 0);


        // 2 Chapter
        Quaternion StartPivotAll = PivotAll.localRotation;
        Quaternion EndPiovtAll = Quaternion.Euler(0, 0, 0);
        Quaternion StartDoorPivot = DoorPivot.localRotation;
        Quaternion EndDoorPivot = Quaternion.Euler(StartDoorPivot.x, 0, StartDoorPivot.z);
        Quaternion EndPivot1of2 = Quaternion.Euler(90, -90, -90);

        // 1 ����(Start 1 �� ����)
        while (CurrentTime2 < duration)
        {


            CurrentTime2 += Time.deltaTime;
            PivotAll.localRotation = Quaternion.Slerp(StartPivotAll, EndPiovtAll, CurrentTime2 / duration);
            DoorPivot.localRotation = Quaternion.Slerp(StartDoorPivot, EndDoorPivot, CurrentTime2 / duration);
            LinkPivot1.localRotation = Quaternion.Slerp(StartPivot1, EndPivot1, CurrentTime2 / duration);

            yield return null;
        }
        CurrentTime2 = 0;
        // 2 ����(start 2 �� ����)
        while (CurrentTime2 < duration)
        {
            CurrentTime2 += Time.deltaTime;
            Pivot1.localPosition = Vector3.Lerp(StartPos, TartgetPos, CurrentTime2 / duration);
            LinkPivot1.localRotation = Quaternion.Slerp(EndPivot1, EndPivot1of2, CurrentTime2 / duration);
            LinkPivot2.localRotation = Quaternion.Slerp(StartPivot2, EndPivot2, CurrentTime2 / duration);
            yield return null;
        }

    }

    public bool DryerOnOffPLC()
    {
        return DoorValue = !DoorValue;
    }
}