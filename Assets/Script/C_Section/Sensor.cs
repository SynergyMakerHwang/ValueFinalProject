using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [SerializeField] GameObject CuttingPart;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] GameObject ĭ����;
    GameObject CreatedWall;
    bool IsSensor;
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Banana") || other.name.Contains("Apple") || other.name.Contains("Orange"))
        {
            Cutter.Instance.TurnOnOff();
            // ���������� �����̾ ���߾ ĭ���̷� �����־�ߵ�

            CreatedWall = Instantiate(ĭ����, transform);
            IsSensor = true;

            if (IsSensor)
            {
                StartCoroutine(GOGOSSING());


            }
        }
    }
    IEnumerator GOGOSSING()
    {
        yield return StartCoroutine(CuttingGo(StartPos.localPosition, EndPos.localPosition));
        yield return StartCoroutine(CuttingGo(EndPos.localPosition, StartPos.localPosition));

        yield return Cutter.Instance.TurnOnOff();
        Destroy(CreatedWall);

    }

    // ���ܱ�� �������ִ�
    IEnumerator CuttingGo(Vector3 From, Vector3 to)
    {
        float CurrentTime = 0;
        float duration = 2;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            CuttingPart.transform.localPosition = Vector3.Lerp(From, to, CurrentTime / duration);
            yield return null;
        }
    }
}
