using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class Belts : MonoBehaviour

{

    // ������ �ϳ��� ���� ;;
    Vector3 StartPos = new Vector3(0.1580003f, 0.05900013f, -0.01700032f);
    Vector3 MiddlePos1 = new Vector3(-1.99f, 0.05900013f, -0.01699984f);
    Vector3 MiddlePos2 = new Vector3(-2.351001f, 0.3090003f, -0.01699948f);
    Vector3 EndPos = new Vector3(-7.308f, 0.3090003f, -0.017f);

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.name.Contains("Banana") || other.name.Contains("Apple")) 
    //    {
    //        other.transform.parent = transform;
    //        Rigidbody rb = other.GetComponent<Rigidbody>();
    //        rb.useGravity = true;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.name.Contains("Banana")|| other.name.Contains("Apple"))
    //    {
    //        other.transform.parent = null;
    //    }
    //}
    void Start()
    {
        //�ʱ⿡  �� �ʱ�ȭ
        transform.localPosition = StartPos;
    }

    void Update()
    {
        Cutter.Instance.ReadConveyorOn();
        Cutter.Instance.ReadConveyorSpeed();

        if (Cutter.Instance.ReadConveyorOn() && transform.localPosition == StartPos)
        {
            StartCoroutine(seqeunce());

        }
    }
    IEnumerator seqeunce()
    {
        yield return ConveyorGo(Cutter.Instance.ReadConveyorSpeed(), StartPos, MiddlePos1);
        yield return ConveyorGo(Cutter.Instance.ReadConveyorSpeed(), MiddlePos1, MiddlePos2);
        yield return ConveyorGo(Cutter.Instance.ReadConveyorSpeed(), MiddlePos2, EndPos);

    }

    IEnumerator ConveyorGo(float Speed, Vector3 from, Vector3 to)
    {

        // �� ��ġ�� ���� float ������ �ٲٰ�
        // �� ������ ���ǵ尪���� ������ �Ȱ��� ��ġ�� ���ǵ尡 ���������� ���� ���ϼ��ִ�
        transform.localPosition = from;
        float Length = Vector3.Distance(from, to);
        float journeyTime = Length / Speed;
        float CurrentTime = 0;
        while (CurrentTime < journeyTime)
        {
            CurrentTime += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, to, CurrentTime / journeyTime);
            yield return null;
            //�������̴ٰ� Conveyor �� �ٲ�� �� �����ϰ� ���߱�
            while (!Cutter.Instance.ReadConveyorOn())
            {
                yield return null;
            }
        }
        // ���ѷ����� ���� ���ǽ�
        if (transform.localPosition == EndPos)
            transform.localPosition = StartPos;

    }
}
